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

namespace icma
{
    /// <summary>
    /// ListCell.xaml 的交互逻辑
    /// </summary>
    public partial class ListCell : UserControl
    {
        private readonly IViewModel viewModel;
        public ListCell()
        {
            InitializeComponent();
            viewModel = new ListCellViewModel();
            viewModel?.SetView(this);
        }

        public IViewModel ViewModel => viewModel;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            progressBar.Value = 0;

            progressBar.Visibility = Visibility.Hidden;

            flag.Visibility = Visibility.Hidden;

           

            titleLabel.Content = "";

            




        }
        public void BeginUpdate()
        {
            flag.Visibility = Visibility.Hidden;
           
            
            execute.IsEnabled = false;
            open.IsEnabled = false;
            del.IsEnabled = false;
            progressBar.Visibility = Visibility.Visible;
            progressBar.Value = 0;

        }

        public void EndUpdate()
        {
            
            del.IsEnabled = true;
            execute.IsEnabled = true;
            open.IsEnabled = true;
            progressBar.Visibility = Visibility.Hidden;
            progressBar.Value = 0;
        }

        public void UpdateStatus(bool success)
        {
            
            flag.Visibility = Visibility.Visible;
            if (success)
            {
                flag.Source = ImageFetch.Success();
            }
            else
            {
                flag.Source = ImageFetch.Error();
            }

        }
        public void UpdateProgress(double progress)
        {
            progressBar.Value = progress;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel?.Dispose();
        }
    }
}
