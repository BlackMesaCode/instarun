using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaRun
{
    public class Settings
    {
        public DockingArea DockingArea { get; set; }
        public double MaxWidth { get; set; }
        public bool StartWithWindows { get; set; }

        public Settings()
        {

        }

        public Settings(DockingArea dockingArea = DockingArea.Top, double maxWidth = 200.0, bool startWithWindows = true)
        {
            DockingArea = dockingArea;
            MaxWidth = maxWidth;
            StartWithWindows = startWithWindows;
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
