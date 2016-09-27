using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaRun.ConfigManagement
{
    public class Settings
    {
        public DockingArea DockingArea { get; set; }
        public double MaxWidth { get; set; }

        public Settings()
        {

        }

        public Settings(DockingArea dockingArea = DockingArea.Top, double maxWidth = 200.0)
        {
            DockingArea = dockingArea;
            MaxWidth = maxWidth;
        }

    }

    public enum DockingArea
    {
        Top,
        Bottom,
        Left,
        Right,
    }
}
