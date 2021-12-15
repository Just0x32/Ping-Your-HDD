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

namespace Cyclic_Ping_Your_HDD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowState previewState;

        ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ViewModel();
            DataContext = viewModel;

            this.Closing += CloseApp;
            StateChanged += Window_StateChanged;

            //Hide();

            SetSettingsValuesFromModel();

            DebugMessage();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            else
            {
                previewState = WindowState;
            }
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = previewState;
        }

        private void TaskbarIcon_TrayRightMouseDown(object sender, RoutedEventArgs e)
        {
            
        }

        private void SetSettingsValuesFromModel()
        {
            PathTextBox.Text = viewModel.ToPingFilePath;
            DelayTextBox.Text = viewModel.PingDelay;

            PingOnStartAppCheckBox.IsChecked = viewModel.IsPingingOnAppStart;

            if (viewModel.IsPinging)
            {
                PingToogleButton.Content = "Stop ping";
            }
            else
            {
                PingToogleButton.Content = "Start ping";
            }
            SettingsGroupBox.IsEnabled = !viewModel.IsPinging;
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TransferFromViewSettingsValues(PathTextBox.Text, DelayTextBox.Text, PingOnStartAppCheckBox.IsChecked.ToString());
            SetSettingsValuesFromModel();

            DebugMessage();                         // Debug
        }

        private void PingToogleButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TooglePingState();
            SetSettingsValuesFromModel();
        }

        private void DebugMessage() => MessageBox.Show(viewModel.DebugMessage);                 // Debug

        private void MenuItem_Click(object sender, RoutedEventArgs e) => Close();

        private void CloseApp(object sender, System.ComponentModel.CancelEventArgs e) => viewModel.CloseApp();
    }
}
