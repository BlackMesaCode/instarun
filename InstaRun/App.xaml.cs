using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InstaRun
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public static ContextMenu ContextMenu;
        public static TrayManager TrayManager;
        public static Config Config;
        public static KeyboardHook KeyboardHook;

        public static readonly string ExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string ExeDir = Path.GetDirectoryName(ExePath);
        public static readonly string ConfigFileName = "Config.xml";
        public static readonly string SampleConfigFileName = "Config.Sample.xml";
        public static readonly string IconCacheFolderName = "IconCache";
        public static readonly string PathToConfig = Path.Combine(ExeDir, ConfigFileName);
        public static readonly string PathToSampleConfig = Path.Combine(ExeDir, SampleConfigFileName);
        public static readonly string PathToIconCache = Path.Combine(ExeDir, IconCacheFolderName);

        public static bool Reinitialize = false;

        public App()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Update current working environment, because it is wrongly set, when the app is started from the registry run
            Environment.CurrentDirectory = ExeDir;

            // Check if Config.xml exists
            if (!File.Exists(PathToConfig))
            {
                MessageBox.Show($"Couldn't find: {PathToConfig}\n\nProgram will be closed.");
                App.Current.Shutdown();
            }
            
            // Add Exit Handler to dispose tray menu
            Exit += App_Exit;

            // Deserialize Config.xml to a config object
            // (just in case someone accidently deleted the config.xml and doesnt remember the xml schema)
            ConfigManager.CreateSampleConfigXml();

            // Deserialize config.xml and build context menu
            Initialize();

            // Create NotifyIcon in the tray menu
            TrayManager = new TrayManager();

            // Watching Config.xml for changes
            CreateFileWatcher(ExeDir);

            // Register Hotkey to Open Context Menu
            KeyboardHook = new KeyboardHook();
            KeyboardHook.KeyDown += KeyboardHook_KeyDown;

        }

        private void KeyboardHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.W && KeyboardHook.IsKeyPressed(System.Windows.Forms.Keys.LWin))
            {
                if (Reinitialize)
                {
                    Initialize();
                    Reinitialize = false;
                }

                ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                ContextMenu.HorizontalOffset = 0;
                ContextMenu.VerticalOffset = 0;
                ContextMenu.IsOpen = !ContextMenu.IsOpen;
            }
        }


        public static void Initialize()
        {
            // Deserialize config.xml
            Config = ConfigManager.GetConfig();

            // Generate the ContextMenu out of the config object
            ContextMenu = ContextMenuManager.CreateContextMenu(Config.Items);

            // If TrayManager has already been created: reassign updated context menu
            if (TrayManager != null)
                TrayManager.TaskbarIcon.ContextMenu = ContextMenu;

            // Update application settings
            UpdateSettings(Config.Settings);
        }


        private void App_Exit(object sender, ExitEventArgs e)
        {
            TrayManager.TaskbarIcon.Icon = null; // Dispose NotifyIcon in the tray
            TrayManager.TaskbarIcon.Dispose();
        }



        public static void UpdateSettings(Settings settings)
        {
            // Warning: MaxWidth must not be 0 otherwise, we wont see shit
            // Setting the MaxWidth only on the Root ContextMenu wont be enough - we would have to set it on each menuitem
            //ContextMenu.MaxWidth = settings.MaxWidth;
        }

        public void CreateFileWatcher(string path)
        {
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // Only watch text files.
            watcher.Filter = ConfigFileName;

            // Add event handlers.
            watcher.Changed += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }


        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Reinitialize if config.xml has changed
            Reinitialize = true;
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleUncaughtException((Exception)e.ExceptionObject);
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleUncaughtException(e.Exception);
        }

        public void HandleUncaughtException(Exception ex)
        {
            string errorMessage = string.Empty;

            if (!string.IsNullOrWhiteSpace(ex.InnerException?.ToString()))
                errorMessage += $"{ex.InnerException}";

            errorMessage += $"\n\n{ ex.Message}";

            errorMessage += "\n\nSee error log file for further information.";

            var fileName = "Error[" + DateTime.Now.ToString("dd.MM.yyyy-HH_mm_ss") + "].txt";


            TextWriter writer = new StreamWriter(Path.Combine(App.ExeDir, fileName));
            writer.WriteLine("-------------- Exception --------------\n\n");
            writer.WriteLine(ex.Message);
            writer.WriteLine("\n\n-------------- Inner Exception --------------\n\n");
            writer.WriteLine(ex.InnerException);
            writer.WriteLine("\n\n-------------- Stack Trace --------------\n\n");
            writer.WriteLine(ex.StackTrace);
            writer.Close();

            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }

    }
}
