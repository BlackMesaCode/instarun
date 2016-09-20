using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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

        public App()
        {
            // Create NotifyIcon in the tray menu
            TrayManager = new TrayManager();

            // Add Exit Handler to dispose tray menu
            Exit += App_Exit;

            // Deserialize Config.xml to a config object
            var configManager = new ConfigManager();
            // Just in case someone accidently deleted the config.xml and doesnt remember the xml schema :)
            configManager.CreateSampleConfigXml();
            var config = configManager.GetConfig();

            // Generate the ContextMenu out of the config object
            ContextMenu = CreateContextMenu(config.Items);

            // ToDo subscript to file changed event -> then recreate ContextMenu

            // Intercept MouseButtonDown event to open ContextMenu
            MouseHook = new MouseHook();
            MouseHook.ButtonDown += MouseHook_ButtonDown;

            
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            TrayManager.NotifyIcon.Icon = null; // Dispose NotifyIcon in the tray
        }

        private void MouseHook_ButtonDown(object sender, MouseEventArgsExtended e)
        {
            //if (e.Y == 0)
            //    Mouse.SetCursor(Cursors.Hand);
            //else
            //    Mouse.SetCursor(Cursors.Arrow);

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


        public void UpdateSetting(Settings settings)
        {
            ContextMenu.MaxWidth = settings.MaxWidth;
        }

        public ContextMenu CreateContextMenu(List<Item> items)
        {
            var contextMenu = new ContextMenu();
            CreateContextMenuHelper(contextMenu, null, items);
            return contextMenu;
        }

        public void CreateContextMenuHelper(ContextMenu contextMenu, MenuItem parent, List<Item> items)
        {
            foreach (var item in items.OrderBy(i => i.Position)) // TODO Add Icons
            {
                if (item.GetType() == typeof(Executable))
                {
                    var newMenuItem = new MenuItem();
                    var executable = (item as Executable);

                    newMenuItem.Header = executable.Name;
                    newMenuItem.ToolTip = executable.Path;
                    newMenuItem.DataContext = executable;
                    newMenuItem.Click += NewMenuItem_Click;

                    if (parent == null)
                        contextMenu.Items.Add(newMenuItem);
                    else
                        parent.Items.Add(newMenuItem);
                }
                else if (item.GetType() == typeof(Separator))
                {
                    var newSeparator = new Separator();

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

                    if (parent == null)
                        contextMenu.Items.Add(newMenuItem);
                    else
                        parent.Items.Add(newMenuItem);

                    CreateContextMenuHelper(contextMenu, newMenuItem, container.Items);
                }
            }

        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
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
}
