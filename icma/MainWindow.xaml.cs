using System.Collections.Generic;
using System.Windows;

namespace icma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new MainWindowViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel?.SetView(this);

        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel?.Dispose();

        }

        private void TableView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop, true);
                var vn = viewModel as MainWindowViewModel;
                if (files is IEnumerable<string> enu)
                {
                    vn.AddFiles(enu);
                }
                e.Handled = true;

            }
        }
    }
}
