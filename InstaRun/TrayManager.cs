using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstaRun
{
    public class TrayManager
    {
        public TaskbarIcon TaskbarIcon;

        public TrayManager()
        {
            TaskbarIcon = new TaskbarIcon();
            TaskbarIcon.ContextMenu = App.ContextMenu;
            TaskbarIcon.Visibility = System.Windows.Visibility.Visible;
            TaskbarIcon.Icon = new System.Drawing.Icon(Path.Combine(App.ExeDir, "InstaRun.ico"));
            TaskbarIcon.TrayRightMouseDown += TaskbarIcon_TrayRightMouseDown;

            App.ContextMenuChanged += App_ContextMenuChanged;
        }

        private void App_ContextMenuChanged(System.Windows.Controls.ContextMenu newConfig)
        {
            TaskbarIcon.ContextMenu = App.ContextMenu;
        }

        private void TaskbarIcon_TrayRightMouseDown(object sender, System.Windows.RoutedEventArgs e)
        {
            if (App.Reinitialize)
            {
                App.Initialize();
                App.Reinitialize = false;
            }
        }
    }
}
