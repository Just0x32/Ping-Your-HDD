using Microsoft.Win32;
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
    public partial class MainWindow : Window
    {
        private WindowState previewState;

        ViewModel viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
            Closing += CloseApp;
            StateChanged += Window_StateChanged;
            viewModel.PropertyChanged += ViewModelNotify;
            viewModel.Start();

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
            else if (viewModel.IsPingingOnAppStart)
                Hide();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
            else
                previewState = WindowState;
        }

        #region [   Click Handlers  ]
        private void ToPingFilePathChooseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Settings files (*.*)|*.*";
            saveFileDialog.FileName = viewModel.ToPingFilePath;

            if (saveFileDialog.ShowDialog() == true)
                viewModel.SetToPingFilePath(saveFileDialog.FileName);
        }

        private void RunScriptButton_Click(object sender, RoutedEventArgs e) => viewModel.RunScript();

        private void ToScriptFilePathChooseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Script files (*.bat, *.exe)|*.bat;*.exe";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
                viewModel.SetToScriptFilePath(openFileDialog.FileName);
        }

        private void ToScriptFilePathClearButton_Click(object sender, RoutedEventArgs e) => viewModel.ClearToScriptFilePath();

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e) => viewModel.SetFromViewSettings();

        private void PingToogleButton_Click(object sender, RoutedEventArgs e) => viewModel.TooglePingState();

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e) => ExpandWindow();

        private void ExpandMenuItem_Click(object sender, RoutedEventArgs e) => ExpandWindow();

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Close();
        #endregion

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
                    sb.Append(propertyName[startIndex..(i + 1)] + "!");
                else if (char.IsUpper(propertyName[i]) && !char.IsUpper(propertyName[i - 1]))
                {
                    sb.Append(propertyName[startIndex..i] + " ");
                    startIndex = i;
                }
            }

            return sb.ToString();
        }
    }
}
