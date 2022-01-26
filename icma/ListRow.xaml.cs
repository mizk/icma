

using System.Windows;
using System.Windows.Controls;

namespace icma
{
    /// <summary>
    /// ListRow.xaml 的交互逻辑
    /// </summary>
    public partial class ListRow : UserControl
    {
        
        
        private readonly IViewModel viewModel;
        public ListRow()
        {
            InitializeComponent();
            viewModel=new ListRowViewModel();
            viewModel?.SetView(this);
        }

        public IViewModel ViewModel => viewModel;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            
           
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            viewModel?.Dispose();
        }
    }
}
