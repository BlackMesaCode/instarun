using InstaRun.ConfigManagement;
using InstaRun.ContextMenuManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
            Executables = GetAllExecutables(_configService.Config.Items);
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

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        ///     This function sets the keyboard focus to the specified window. All subsequent keyboard input is directed to this window. The window, if any, that previously had the keyboard focus loses it.
        /// </summary>
        /// <param name="hWnd">Handle to the window that will receive the keyboard input. If this parameter is NULL, keystrokes are ignored.</param>
        /// <returns>The handle to the window that previously had the keyboard focus indicates success.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);


        /// <summary>
        ///     The SetForegroundWindow function puts the thread that created the specified window into the foreground and activates the window. Keyboard input is directed to the window, and various visual cues are changed for the user.
        /// </summary>
        /// <param name="hWnd">Handle to the window that should be activated and brought to the foreground. </param>
        /// <returns>If the window was brought to the foreground, the return value is nonzero. </returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        private void SearchTextBox_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var windowHandle = new WindowInteropHelper(this).Handle;
            SetForegroundWindow(windowHandle);
            Activate();
            SetFocus(windowHandle);
            SearchTextBox.Focus();
        }
    }
}
