using InstaRun.ConfigManagement;
using InstaRun.ContextMenuManagement;
using InstaRun.SearchManagement;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstaRun.TriggerManagement
{
    public class HotkeyService
    {
        private KeyboardHook _keyboardHook;

        public SearchBox SearchBox { get; set; }

        public HotkeyService(ContextMenuService contextMenuService, KeyboardHook keyboardHook)
        {
            // Register Hotkey to open SearchBox
            _keyboardHook = keyboardHook;
            _keyboardHook.KeyDown += KeyboardHook_KeyDown;
        }

        private void KeyboardHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (e.KeyCode == System.Windows.Forms.Keys.W && _keyboardHook.IsKeyPressed(System.Windows.Forms.Keys.LWin))
            //{
            //    _contextMenuService.ToggleContextMenuAtMousePoint();
            //    e.Handled = true;
            //}
            
            if (e.KeyCode == System.Windows.Forms.Keys.R && System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Alt)  // && _keyboardHook.IsKeyPressed(System.Windows.Forms.Keys.LWin)) //
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() => ShowAndActivateSearchBox()));
                e.Handled = true;
            }
            else
                e.Handled = false;
        }


        private void ShowAndActivateSearchBox()
        {
            var configService = App.Kernel.Get<ConfigService>();
            SearchBox = new SearchBox(configService);
            SearchBox.Show();
        }

    }
}
