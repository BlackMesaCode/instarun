﻿using System;
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

            // add close option
            var menuItemToClose = new MenuItem();
            menuItemToClose.DataContext = contextMenu;
            menuItemToClose.Header = "Close";
            menuItemToClose.Click += MenuItemToClose_Click;
            contextMenu.Items.Add(menuItemToClose);
            return contextMenu;
        }

        private static void MenuItemToClose_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = (sender as MenuItem).DataContext as ContextMenu;
            contextMenu.IsOpen = false;
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
                        if (File.Exists(executable.Path) || Directory.Exists(executable.Path))
                        {
                            var pathToIcon = Path.Combine(App.PathToIconCache, executable.Name + ".ico");

                            if (File.Exists(pathToIcon))
                            {
                                var bitmap = new BitmapImage();
                                var stream = File.OpenRead(pathToIcon);
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                                stream.Close();
                                stream.Dispose();

                                newMenuItem.Icon = new System.Windows.Controls.Image
                                {
                                    Source = bitmap
                                };
                            }
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
