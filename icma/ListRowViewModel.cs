using System.Collections.Generic;
using System.Windows;

namespace icma
{
    public class ListRowViewModel : ViewModel
    {
        private readonly List<ListCell> cells = new(capacity: 5);

        public List<ListCell> Cells => cells;
        protected override void InitUI()
        {
           ListRow view=GetView<ListRow>();
            view.Cell1.Visibility = Visibility.Hidden;
            view.Cell2.Visibility = Visibility.Hidden;
            view.Cell3.Visibility = Visibility.Hidden;
            view.Cell4.Visibility = Visibility.Hidden;
            view.Cell5.Visibility = Visibility.Hidden;
            cells.Add(view.Cell1);
            cells.Add(view.Cell2);
            cells.Add(view.Cell3);
            cells.Add(view.Cell4);
            cells.Add(view.Cell5);
        }

        protected override void OnModelUpdate(Model model)
        {
            
        }
    }
}
