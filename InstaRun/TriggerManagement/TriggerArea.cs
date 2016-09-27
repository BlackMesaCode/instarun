using InstaRun.ContextMenuManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstaRun.TriggerManagement
{
    public class TriggerArea : Form
    {
        private ContextMenuService _contextMenuService;

        public TriggerArea(ContextMenuService contextMenuService)
        {
            _contextMenuService = contextMenuService;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            MinimumSize = new Size(1, 1);
            AllowTransparency = true;
            TransparencyKey = Color.AliceBlue;
            BackColor = Color.AliceBlue;
            TopMost = true;
            ShowInTaskbar = false;
            MouseClick += Window_MouseClick;
            TopLevel = true;
            Bounds = new Rectangle(0, -17, 2 * 1920, 1);
            Cursor = Cursors.UpArrow;
            //ClientSizeChanged += TriggerArea_Changed;
            LocationChanged += TriggerArea_Changed;
            //SizeChanged += TriggerArea_Changed;
            //RegionChanged += TriggerArea_Changed;
            Show();
        }

        private void TriggerArea_Changed(object sender, EventArgs e)
        {
            //Bounds = new Rectangle(0, -17, 2 * 1920, 1);
            //Size = new Size(2 * 1920, 1);
            Location = new Point(0, -17);
        }

        private void Window_MouseClick(object sender, MouseEventArgs e)
        {
            _contextMenuService.ToggleContextMenuAtMousePoint();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }
            base.WndProc(ref m);
        }

    }
}
