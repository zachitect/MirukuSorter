using Microsoft.Win32;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MirukuSorter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            // Subscribe to unhandled exception for this window's dispatcher
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }
        private void ButtonSortVideos_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.CopySortedVideo();
            }
        }
        private void ButtonSortPhotos_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.CopySortedPhoto();
            }
        }
        private void ButtonScanDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.LoadMediaFiles();
            }
        }
        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = $"An error occurred: {e.Exception.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Information);
            // Prevent the exception from crashing the application
            e.Handled = true;
        }
        protected override void OnClosed(EventArgs e)
        {
            // Unsubscribe to avoid memory leaks
            this.Dispatcher.UnhandledException -= OnDispatcherUnhandledException;
            base.OnClosed(e);
        }
    }
}