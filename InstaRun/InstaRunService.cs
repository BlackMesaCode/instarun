using InstaRun.ConfigManagement;
using InstaRun.ContextMenuManagement;
using InstaRun.HotkeyManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace InstaRun
{
    public class InstaRunService
    {
        private ConfigService _configService;
        private ContextMenuService _contextMenuService;
        private TaskbarService _taskbarService;
        private HotkeyService _hotkeyService;

        public InstaRunService(
            ConfigService configService, 
            ContextMenuService contextMenuService, 
            TaskbarService taskbarService, 
            HotkeyService hotkeyService)
        {
            // Update current working environment, because it is wrongly set, when the app is started from the registry run
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _configService = configService;
            _contextMenuService = contextMenuService;
            _taskbarService = taskbarService;
            _hotkeyService = hotkeyService;

            // manually triggering the event chain: ConfigChanged -> Create ContextMenu -> ContextMenuChanged -> UpdateTaskbar
            _configService.UpdateConfigFromXml();

        }

    }
}
