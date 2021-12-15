using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Ping_Your_HDD
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
            viewModel.PropertyChanged += ViewModelNotify;

            if (viewModel.IsIOError)
            {
                if (viewModel.IsFileCreatingError)
                    ErrorNotify("IsFileCreatingError");

                if (viewModel.IsFileReadingError)
                    ErrorNotify("IsFileReadingError");

                if (viewModel.IsFileWritingError)
                    ErrorNotify("IsFileWritingError");

                Close();
            }
            else
            {
                SetSettingsValuesFromModel();

                if (viewModel.IsPingingOnAppStart)
                    Hide();
            }
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
        }

        private void PingToogleButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TooglePingState();
            SetSettingsValuesFromModel();
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e) => ExpandWindow();

        private void ExpandMenuItem_Click(object sender, RoutedEventArgs e) => ExpandWindow();

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Close();

        private void ExpandWindow()
        {
            Show();
            WindowState = previewState;
        }

        private void CloseApp(object sender, System.ComponentModel.CancelEventArgs e) => viewModel.CloseApp();

        private void ViewModelNotify(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.EndsWith("Error"))
                ErrorNotify(e.PropertyName);
        }

        private void ErrorNotify(string propertyName) => MessageBox.Show(CreateNotifyMessage(propertyName));

        private string CreateNotifyMessage(string propertyName)
        {
            StringBuilder sb = new StringBuilder();
            int startIndex = 0;

            for (int i = 1; i < propertyName.Length; i++)
            {
                if (i + 1 == propertyName.Length)
                {
                    sb.Append(propertyName[startIndex..(i + 1)] + "!");
                }
                else if (Char.IsUpper(propertyName[i]) && !Char.IsUpper(propertyName[i - 1]))
                {
                    sb.Append(propertyName[startIndex..i] + " ");
                    startIndex = i;
                }
            }

            return sb.ToString();
        }
    }
}
