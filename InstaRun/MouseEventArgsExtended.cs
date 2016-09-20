using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstaRun
{
    public class MouseEventArgsExtended : MouseEventArgs
    {
        public bool Handled = false;

        public MouseEventArgsExtended(MouseButtons button, int clicks, int x, int y, int delta) : base(button, clicks, x, y, delta)
        {
        }
    }
}
