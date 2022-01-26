using System;
using System.Windows;
using System.Windows.Controls;

namespace icma
{
    public interface IViewModel : IDisposable
    {
        T GetModel<T>() where T : Model;
        T GetView<T>() where T : ContentControl;
        void SetView(UIElement view);
        void UpdateModel(Model model);
        void ReloadData();
    }
}
