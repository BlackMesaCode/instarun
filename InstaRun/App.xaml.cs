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
        public MouseHook MouseHook;
        public ContextMenu ContextMenu;
        public TrayManager TrayManager;

        public static readonly string ExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static readonly string ExeDir = Path.GetDirectoryName(ExePath);
        public static readonly string ConfigFileName = "Config.xml";
        public static readonly string SampleConfigFileName = "Config.Sample.xml";
        public static readonly string PathToConfig = Path.Combine(ExeDir, ConfigFileName);
        public static readonly string PathToSampleConfig = Path.Combine(ExeDir, SampleConfigFileName);

        bool _reinitialize = false;

        public App()
        {
            // Check if Config.xml exists
            if (!File.Exists(PathToConfig))
            {
                MessageBox.Show($"Couldn't find: {PathToConfig}\n\nProgram will be closed.");
                App.Current.Shutdown();
            }

            // Create invisible window to catch clicks
            CreateInvisibleWindow();

            // Create NotifyIcon in the tray menu
            TrayManager = new TrayManager();

            // Add Exit Handler to dispose tray menu
            Exit += App_Exit;

            // Deserialize Config.xml to a config object
            // (just in case someone accidently deleted the config.xml and doesnt remember the xml schema)
            ConfigManager.CreateSampleConfigXml();

            // Initialize the application based on the config.xml
            Initialize();

            // Intercept MouseButtonDown event to open ContextMenu
            //MouseHook = new MouseHook();
            //MouseHook.ButtonDown += MouseHook_ButtonDown;

            // Watching Config.xml for changes
            CreateFileWatcher(ExeDir);
        }

        private void CreateInvisibleWindow()
        {
            var window = new Window();
            window.WindowStyle = WindowStyle.None;
            window.AllowsTransparency = true;
            window.Opacity = 0.01;
            window.MouseLeftButtonUp += Window_MouseLeftButtonUp; // up event is more reliable when used on top of chrome Oo
            window.Topmost = true;
            window.Left = 0;
            window.Top = -17;
            window.Height = 20;
            window.Width = 10000;
            window.ShowInTaskbar = false;
            window.ResizeMode = ResizeMode.NoResize;
            window.Cursor = Cursors.ScrollS;
            window.Show();
        }


        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_reinitialize)
            {
                Initialize();
                _reinitialize = false;
            }

            ContextMenu.IsOpen = true;
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            TrayManager.NotifyIcon.Icon = null; // Dispose NotifyIcon in the tray
        }

        public void Initialize()
        {
            // Deserialize config.xml
            var config = ConfigManager.GetConfig();

            // Generate the ContextMenu out of the config object
            ContextMenu = ContextMenuManager.CreateContextMenu(config.Items);

            // Update application settings
            UpdateSettings(config.Settings);
        }

        private void MouseHook_ButtonDown(object sender, MouseEventArgsExtended e)
        {
            if (_reinitialize)
            {
                Initialize();
                _reinitialize = false;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Y == 0 && !ContextMenu.IsOpen)
            {
                ContextMenu.IsOpen = true;
                e.Handled = true;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left && Mouse.DirectlyOver == ContextMenu) //&& MyContextMenu.IsOpen) // !MouseOverItem
            {
                ContextMenu.IsOpen = false;
                e.Handled = false;
            }
        }


        public void UpdateSettings(Settings settings)
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
            _reinitialize = true;
        }


    }
}
