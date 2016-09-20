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
        public bool RunOnStartup { get; set; }

        public Settings()
        {

        }

        public Settings(DockingArea dockingArea = DockingArea.Top, double maxWidth = 200.0, bool runOnStartup = true)
        {
            DockingArea = dockingArea;
            MaxWidth = maxWidth;
            RunOnStartup = runOnStartup;
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
