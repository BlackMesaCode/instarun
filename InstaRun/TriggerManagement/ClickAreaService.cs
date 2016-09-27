using InstaRun.ContextMenuManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InstaRun.TriggerManagement
{
    public class ClickAreaService
    {
        private ContextMenuService _contextMenuService;

        public ClickAreaService(ContextMenuService contextMenuService)
        {
            _contextMenuService = contextMenuService;
            CreateFormsWindow();
        }

        private void CreateFormsWindow()
        {
            var window = new Form();
            window.FormBorderStyle = FormBorderStyle.None;
            window.StartPosition = FormStartPosition.Manual;
            window.MinimumSize = new Size(1,1);
            window.AllowTransparency = true;
            window.TransparencyKey = Color.AliceBlue;
            window.BackColor = Color.AliceBlue;
            window.TopMost = true;
            window.ShowInTaskbar = false;
            window.MouseClick += Window_MouseClick;
            window.TopLevel = true;
            window.Bounds = new Rectangle(0, -17, 2*1920, 1);
            window.Cursor = Cursors.UpArrow;
            window.Show();
        }

        private void Window_MouseClick(object sender, MouseEventArgs e)
        {
            _contextMenuService.ToggleContextMenuAtMousePoint();
        }

    }
}
