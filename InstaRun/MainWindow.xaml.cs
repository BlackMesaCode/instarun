using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();

            var configManager = new ConfigManager();
            //configManager.CreateSampleConfigXml();
            var config = configManager.GetConfig();
            var contextMenu = CreateContextMenu(config.Items);
            contextMenu.IsOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button != null)
            {
                button.ContextMenu.IsOpen = true;
            }
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

    }
}
