using InstaRun.ContextMenuManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaRun.TriggerManagement
{
    public class HotkeyService
    {
        private ContextMenuService _contextMenuService;
        private KeyboardHook _keyboardHook;

        public HotkeyService(ContextMenuService contextMenuService, KeyboardHook keyboardHook)
        {
            _contextMenuService = contextMenuService;

            // Register Hotkey to Open Context Menu
            _keyboardHook = keyboardHook;
            _keyboardHook.KeyDown += KeyboardHook_KeyDown;
        }

        private void KeyboardHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.W && _keyboardHook.IsKeyPressed(System.Windows.Forms.Keys.LWin))
            {
                _contextMenuService.ToggleContextMenuAtMousePoint();
                e.Handled = true;
            }
            else
                e.Handled = false;
        }



    }
}
