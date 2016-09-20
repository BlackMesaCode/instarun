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
            ContextMenu = ContextMenuManager.CreateContextMenu(config.Items);

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


    }
}
