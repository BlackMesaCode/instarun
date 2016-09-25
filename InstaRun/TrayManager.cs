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
        private TaskbarIcon _taskbarIcon;
        private App _app;

        public TrayManager(App app)
        {
            _app = app;

            app.Exit += App_Exit;
            app.ContextMenuChanged += App_ContextMenuChanged;

            _taskbarIcon = new TaskbarIcon();
            _taskbarIcon.Visibility = System.Windows.Visibility.Visible;
            _taskbarIcon.Icon = new System.Drawing.Icon(Path.Combine(App.ExeDir, "InstaRun.ico"));
            _taskbarIcon.TrayRightMouseDown += TaskbarIcon_TrayRightMouseDown;
        }

        private void App_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            _taskbarIcon.Icon = null; // Dispose NotifyIcon in the tray
            _taskbarIcon.Dispose();
        }

        private void App_ContextMenuChanged(System.Windows.Controls.ContextMenu newContextMenu)
        {
            _taskbarIcon.ContextMenu = newContextMenu;
        }

        private void TaskbarIcon_TrayRightMouseDown(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_app.Reinitialize)
            {
                _app.Initialize();
                _app.Reinitialize = false;
            }
        }
    }
}
