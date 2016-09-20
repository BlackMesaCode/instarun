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
        public NotifyIcon NotifyIcon;

        public TrayManager()
        {
            NotifyIcon = new NotifyIcon();
            NotifyIcon.ContextMenu = CreateContextMenu();
            NotifyIcon.Icon = new System.Drawing.Icon(Path.Combine(App.ExeDir, "InstaRun.ico"));
            NotifyIcon.Visible = true;
        }

        private ContextMenu CreateContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(new MenuItem("Start with Windows", StartWithWindows) {
                Checked = IsStartingWithWindows(),
            });
            contextMenu.MenuItems.Add(new MenuItem("Build Icon Cache", BuildIconCache));
            contextMenu.MenuItems.Add(new MenuItem("Open Folder", OpenFolder));
            contextMenu.MenuItems.Add(new MenuItem("Restart", Restart));
            contextMenu.MenuItems.Add(new MenuItem("Close", Close));
            return contextMenu;
        }


        private void StartWithWindows(object sender, EventArgs e)
        {
            var menuItem = (sender as MenuItem);
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (menuItem.Checked)
            {
                registryKey.DeleteValue("InstaRun", false);
                menuItem.Checked = false;
            }
            else
            {
                registryKey.SetValue("InstaRun", "\"" + Application.ExecutablePath.ToString() + "\"");
                menuItem.Checked = true;
            }
        }

        private void BuildIconCache(object sender, EventArgs e)
        {
            IconCache.BuildCache(App.Config.Items);
            App.Reinitialize = true;
        }

        private void OpenFolder(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", App.ExeDir);
        }

        private void Restart(object sender, EventArgs e)
        {
            Process.Start(App.ExePath);
            App.Current.Shutdown();
        }

        public bool IsStartingWithWindows()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return (registryKey.GetValue("InstaRun") != null);
        }

        private void Close(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
