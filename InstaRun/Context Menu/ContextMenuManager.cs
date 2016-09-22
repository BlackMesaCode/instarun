using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace InstaRun
{
    public static class ContextMenuManager
    {

        public static ContextMenu CreateContextMenu(List<Item> items)
        {
            var contextMenu = new ContextMenu();
            CreateContextMenuHelper(contextMenu, null, items);

            AddSettingsMenu(contextMenu);

            return contextMenu;
        }


        public static void CreateContextMenuHelper(ContextMenu contextMenu, MenuItem parent, List<Item> items)
        {
            foreach (var item in items)
            {
                if (item.GetType() == typeof(Executable))
                {
                    var newMenuItem = new MenuItem();
                    var executable = (item as Executable);

                    newMenuItem.Header = executable.Name;
                    newMenuItem.ToolTip = executable.Path;
                    newMenuItem.DataContext = executable;
                    newMenuItem.Click += NewMenuItem_Click;

                    if (!executable.IsInGlobalPath) // No icons for global path calls possible - we would have to search all the directories in the PATH variable
                    {
                        if (File.Exists(executable.Path))
                        {
                            var icon = Icon.ExtractAssociatedIcon(executable.Path);
                            var bmp = icon.ToBitmap();

                            newMenuItem.Icon = new System.Windows.Controls.Image
                            {
                                Source = icon.ToImageSource(),
                            };
                        }
                        else if (Directory.Exists(executable.Path))
                        {
                            newMenuItem.Icon = new System.Windows.Controls.Image
                            {
                                Source = IconReceiver.ReceiveIcon(executable.Path, false).ToImageSource()
                            };
                        }
                    }
                    if (parent == null)
                        contextMenu.Items.Add(newMenuItem);
                    else
                        parent.Items.Add(newMenuItem);

                }
                else if (item.GetType() == typeof(InstaRun.Separator))
                {
                    var newSeparator = new System.Windows.Controls.Separator();

                    if (parent == null)
                        contextMenu.Items.Add(newSeparator);
                    else
                        parent.Items.Add(newSeparator);
                }
                else if (item.GetType() == typeof(Container))
                {
                    var newMenuItem = new MenuItem();
                    var container = (item as Container);

                    newMenuItem.Header = container.Name;
                    //newMenuItem.Icon = new System.Windows.Controls.Image
                    //{
                    //    Source = new BitmapImage(new Uri("Container.ico", UriKind.Relative))
                    //};

                    if (parent == null)
                        contextMenu.Items.Add(newMenuItem);
                    else
                        parent.Items.Add(newMenuItem);

                    CreateContextMenuHelper(contextMenu, newMenuItem, container.Items);
                }
            }
        }

        private static void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {

            var dataContext = (sender as MenuItem).DataContext as Executable;
            try
            {
                Process.Start(dataContext.Path, dataContext.Arguments);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }



        private static void AddSettingsMenu(ContextMenu contextMenu)
        {
            //add separator
            var separator = new System.Windows.Controls.Separator();
            contextMenu.Items.Add(separator);

            //add settings container
            var settingsContainer = new MenuItem();
            settingsContainer.Header = "Settings";
            contextMenu.Items.Add(settingsContainer);

            // add start with windows
            var startWithWindowsMenuItem = new MenuItem();
            startWithWindowsMenuItem.Header = "Autorun";
            startWithWindowsMenuItem.IsCheckable = true;
            startWithWindowsMenuItem.IsChecked = IsStartingWithWindows();
            startWithWindowsMenuItem.DataContext = startWithWindowsMenuItem.IsChecked;
            startWithWindowsMenuItem.Click += StartWithWindowsMenuItem_Click;
            settingsContainer.Items.Add(startWithWindowsMenuItem);

            // Open config folder
            var openConfigFolderMenuItem = new MenuItem();
            openConfigFolderMenuItem.Header = "Open config folder";
            openConfigFolderMenuItem.Click += OpenConfigFolderMenuItem_Click;
            settingsContainer.Items.Add(openConfigFolderMenuItem);

            // Reload config
            var reloadConfigMenuItem = new MenuItem();
            reloadConfigMenuItem.Header = "Reload config";
            reloadConfigMenuItem.Click += ReloadConfigMenuItem_Click;
            settingsContainer.Items.Add(reloadConfigMenuItem);

            // Restart
            var restartMenuItem = new MenuItem();
            restartMenuItem.Header = "Restart";
            restartMenuItem.Click += RestartMenuItem_Click;
            settingsContainer.Items.Add(restartMenuItem);

            // Close ContextMenu - not really neccessary anymore
            //var closeMenuItem = new MenuItem();
            //closeMenuItem.DataContext = contextMenu;
            //closeMenuItem.Header = "Close";
            //closeMenuItem.Click += MenuItemToClose_Click;
            //settingsContainer.Items.Add(closeMenuItem);

            // Close application
            var closeApplicationMenuItem = new MenuItem();
            closeApplicationMenuItem.Header = "Close";
            closeApplicationMenuItem.Click += CloseApplicationMenuItem_Click;
            settingsContainer.Items.Add(closeApplicationMenuItem);
        }


        private static void StartWithWindowsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (e.Source as MenuItem);
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (menuItem.IsChecked)
            {
                registryKey.SetValue("InstaRun", "\"" + App.ExePath + "\"");
            }
            else
            {
                registryKey.DeleteValue("InstaRun", false);
            }
        }

        private static void MenuItemToClose_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = (sender as MenuItem).DataContext as ContextMenu;
            contextMenu.IsOpen = false;
        }

        public static bool IsStartingWithWindows()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return (registryKey.GetValue("InstaRun") != null);
        }

        private static void ReloadConfigMenuItem_Click(object sender, RoutedEventArgs e)
        {
            App.Initialize();
        }

        private static void OpenConfigFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", App.ExeDir);
        }

        private static void RestartMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(App.ExePath);
            App.Current.Shutdown();
        }

        private static void CloseApplicationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

















    }

    internal static class IconUtilities
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }
    }

}
