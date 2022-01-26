using libicma.events;
using System.Windows.Media.Imaging;

namespace icma
{
    public class ListCellViewModel : ViewModel
    {
        private ActionInvoker eventDispatcher;
        protected override void InitUI()
        {
            eventDispatcher = ActionInvoker.Instance();
            var view = GetView<ListCell>();
            view.execute.Click += OnExecuteItem;
            view.del.Click += OnDeleteItem;
            view.open.Click += OnOpenItem;
        }
        private void OnOpenItem(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = GetModel<Media>();
            if (model != null)
            {
                eventDispatcher?.Invoke(this, Action.Open, model);
            }
        }

        private void OnDeleteItem(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = GetModel<Media>();
            if (model != null)
            {
                eventDispatcher?.Invoke(this, Action.Delete, model);
            }
        }

        private void OnExecuteItem(object sender, System.Windows.RoutedEventArgs e)
        {
            var model = GetModel<Media>();
            if (model != null)
            {
                eventDispatcher?.Invoke(this, Action.Execute, model);
            }

        }

        protected override void OnModelUpdate(Model model)
        {
            if (model is not Media item)
            {
                return;
            }
          
            var view = GetView<ListCell>();
            view.titleLabel.Content = item.Name;

            view.flag.Visibility = item.Handled ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            BitmapImage image;
            if (item.Status == MediaStatus.ProccedSuccess)
            {
                image = ImageFetch.GetImage("success");
            }
            else
            {
                image = ImageFetch.GetImage("error");
            }
            view.flag.Source = image;
            view.progressBar.Visibility = item.Status == MediaStatus.Processing ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            view.image.Source = ImageFetch.GetImage(item.Image);
            view.open.IsEnabled = item.Status != MediaStatus.Processing;
            view.execute.IsEnabled = item.Status != MediaStatus.Processing;
            view.del.IsEnabled = item.Status != MediaStatus.Processing;
        }
    }
}
