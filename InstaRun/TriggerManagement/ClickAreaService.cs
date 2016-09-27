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
            new TriggerArea(_contextMenuService);
            
        }




    }
}
