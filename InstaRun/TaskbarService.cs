using Hardcodet.Wpf.TaskbarNotification;
using InstaRun.ContextMenuManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace InstaRun
{
    public class TaskbarService
    {
        private TaskbarIcon _taskbarIcon;
        private ContextMenuService _contextMenuService;

        public TaskbarService(ContextMenuService contextMenuService)
        {
            _contextMenuService = contextMenuService;
            _contextMenuService.ContextMenuChanged += _contextMenuService_ContextMenuChanged;

            _taskbarIcon = new TaskbarIcon();
            _taskbarIcon.Visibility = System.Windows.Visibility.Visible;
            _taskbarIcon.Icon = new System.Drawing.Icon(Path.Combine(App.ExeDir, "InstaRun.ico"));
        }

        private void _contextMenuService_ContextMenuChanged(System.Windows.Controls.ContextMenu newContextMenu)
        {
            App.Current.Dispatcher.Invoke(() => _taskbarIcon.ContextMenu = newContextMenu);
        }
    }
}
