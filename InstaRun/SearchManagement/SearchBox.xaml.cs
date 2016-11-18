using InstaRun.ConfigManagement;
using InstaRun.ContextMenuManagement;
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
using System.Windows.Shapes;

namespace InstaRun.SearchManagement
{
    /// <summary>
    /// Interaktionslogik für SearchBox.xaml
    /// </summary>
    public partial class SearchBox : Window
    {
        private ConfigService _configService;

        private List<Executable> Executables;

        public SearchBox(ConfigService configService)
        {
            InitializeComponent();
            _configService = configService;
            _configService.OnConfigChanged += _configService_OnConfigChanged;

            SearchTextBox.Focus();
        }

        private void _configService_OnConfigChanged(Config newConfig)
        {
            Executables = GetAllExecutables(newConfig.Items);
        }


        private List<Executable> GetAllExecutables(List<Item> items, List<Executable> executables = null)
        {
            if (executables == null) executables = new List<Executable>();

            foreach (var exe in items.OfType<Executable>())
            {
                executables.Add(exe);
            }

            foreach (var container in items.OfType<Container>())
            {
                GetAllExecutables(container.Items, executables);
            }

            return executables;
        }

        
        private Tuple<Executable, string> GetMatchingExecutable()
        {
            foreach (var exe in Executables)
            {
                foreach (var magicWord in exe.MagicWordsSplitted)
                {
                    if (magicWord.StartsWith(SearchTextBox.Text))
                    {
                        return Tuple.Create(exe, magicWord);
                    }
                }
            }

            return null;
        }


        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text.Length >= 1) {

                var matchingExe = GetMatchingExecutable();

                if (matchingExe != null)
                {
                    SearchTextBox.Background = new VisualBrush(new TextBox() {
                        Text = matchingExe.Item2,
                        BorderThickness = new Thickness(0.0),
                        Padding = new Thickness(4.0)})
                    {
                        Opacity = 0.5,
                        AlignmentX = AlignmentX.Left,
                        Stretch = Stretch.None,
                    };
                }
                else
                    SearchTextBox.Background = null;
            }
            else
            {
                SearchTextBox.Background = null;
            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                var matchingExe = GetMatchingExecutable();
                if (matchingExe != null)
                {
                    SearchTextBox.Text = string.Empty;
                    Process.Start(matchingExe.Item1.Path, matchingExe.Item1.Arguments);
                }
                this.Hide();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.Hide();
                e.Handled = true;
            }
        }
    }
}
