using InstaRun.ContextMenuManagement;
using InstaRun.SearchManagement;
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
        private SearchBox _searchBoxService;

        public HotkeyService(ContextMenuService contextMenuService, KeyboardHook keyboardHook, SearchBox searchBoxService)
        {
            _contextMenuService = contextMenuService;
            _searchBoxService = searchBoxService;
            _searchBoxService.Show();
            _searchBoxService.Hide();

            // Register Hotkey to Open Context Menu
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
                _searchBoxService.SearchTextBox.Text = "";
                _searchBoxService.Show();
                _searchBoxService.Activate();
                e.Handled = true;
            }
            else
                e.Handled = false;
        }



    }
}
