using System;
using System.Windows;
using System.Windows.Controls;

namespace icma
{
    public abstract class ViewModel : IViewModel
    {
        private Model model;
        private UIElement view;

        public void SetView(UIElement element)
        {
            view = element;
            InitUI();
        }
        protected abstract void InitUI();
        
        
        public T GetModel<T>() where T : Model
        {
            if (model is T t)
            {
                return t;
            }
            return default;
        }
        public T GetView<T>() where T : ContentControl
        {
            if (view is T t)
            {
                return t;
            }
            return default;
        }


        public void UpdateModel(Model model)
        {
            this.model = model;
            OnModelUpdate(model);
        }
        protected abstract void OnModelUpdate(Model model);
        public void ReloadData()
        {
            OnModelUpdate(null);
        }

        public virtual void Dispose()
        {
           GC.SuppressFinalize(this);
        }
    }
}
