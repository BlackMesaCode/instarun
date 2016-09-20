using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InstaRun
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MouseHook MouseHook;
        public ContextMenu MyContextMenu;

        public MainWindow()
        {
            InitializeComponent();

            var configManager = new ConfigManager();
            //configManager.CreateSampleConfigXml();
            var config = configManager.GetConfig();
            MyContextMenu = CreateContextMenu(config.Items);
            

            MouseHook = new MouseHook();
            MouseHook.ButtonDown += MouseHook_ButtonDown;
        }

        private void MouseHook_ButtonDown(object sender, MouseEventArgsExtended e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && e.Y == 0 && !MyContextMenu.IsOpen)
            {
                MyContextMenu.IsOpen = true;
                e.Handled = true;
            }
            //else if (e.Button == System.Windows.Forms.MouseButtons.Left && MyContextMenu.IsOpen) // !MouseOverItem
            //{
            //    MyContextMenu.IsOpen = false;
            //    e.Handled = false;
            //}
        }


        public void UpdateSetting(Settings settings)
        {
            MaxWidth = settings.MaxWidth;
        }

        public ContextMenu CreateContextMenu(List<Item> items)
        {
            var contextMenu = new ContextMenu();
            CreateContextMenuHelper(contextMenu, null, items);
            return contextMenu;
        }

        public void CreateContextMenuHelper(ContextMenu contextMenu, MenuItem parent, List<Item> items)
        {
            foreach (var item in items.OrderBy(i => i.Position)) // TODO Add Icons, Add Execution of Path command
            {
                if (item.GetType() == typeof(Executable)) 
                {
                    var newMenuItem = new MenuItem();
                    var executable = (item as Executable);

                    newMenuItem.Header = executable.Name;
                    newMenuItem.ToolTip = executable.Path;
                    newMenuItem.DataContext = executable;
                    newMenuItem.Click += NewMenuItem_Click;
                    
                    if (parent == null)
                        contextMenu.Items.Add(newMenuItem);
                    else
                        parent.Items.Add(newMenuItem);
                }
                else if (item.GetType() == typeof(Separator))
                {
                    var newSeparator = new Separator();

                    if (parent == null)
                        contextMenu.Items.Add(newSeparator);
                    else
                        parent.Items.Add(newSeparator);
                }
                else if (item.GetType() == typeof(Container))
                {
                    var newMenuItem = new MenuItem();
                    var container = (item as Container);

                    newMenuItem.Header = container.Name;

                    if (parent == null)
                        contextMenu.Items.Add(newMenuItem);
                    else
                        parent.Items.Add(newMenuItem);

                    CreateContextMenuHelper(contextMenu, newMenuItem, container.Items);
                }
            }

        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (sender as MenuItem).DataContext as Executable;
            Process.Start(dataContext.Path);
        }
    }
}
