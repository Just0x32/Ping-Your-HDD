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

            StateChanged += Window_StateChanged;
            FillPathTextBox();

            //Hide();

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

        private void FillPathTextBox() => PathTextBox.Text = viewModel.ToPingFilePath;

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TransferToPingFilePath(PathTextBox.Text);
            FillPathTextBox();

            DebugMessage();
        }

        private void PingToogleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DebugMessage() => MessageBox.Show(viewModel.DebugMessage);                 // Debug
    }
}
