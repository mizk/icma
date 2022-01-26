using HandyControl.Controls;
using libicma.codecs;
using libicma.events;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace icma
{
    public class MainWindowViewModel : ViewModel, ICodecToolkitHandler
    {
        private readonly List<ListCell> cells = new();
        private readonly ConcurrentDictionary<string, ListCell> cellMap = new();
        private ActionInvoker eventDispatcher;
        private MediaList storage;
        private Executor executor;
        protected override void InitUI()
        {
            executor = Executor.Instance();
            storage = MediaList.Instance();
            eventDispatcher = ActionInvoker.Instance();
            eventDispatcher.Event += OnEventDispatched;
            var view = GetView<MainWindow>();
            
            view.AddButton.Click += OnAddItem;

            view.SettingsButton.Click += OnSettings;
            view.pagenum.PageUpdated += OnPageUpdated;
            LoadCells(view);
            ReloadData(true);
        }
        private void LoadCells(MainWindow view)
        {
            var viewModel1 = view.CellRow1.ViewModel as ListRowViewModel;
            cells.AddRange(viewModel1.Cells);
            var viewModel2 = view.CellRow2.ViewModel as ListRowViewModel;
            cells.AddRange(viewModel2.Cells);
            var viewModel3 = view.CellRow3.ViewModel as ListRowViewModel;
            cells.AddRange(viewModel3.Cells);
        }



        private void OnPageUpdated(object sender, HandyControl.Data.FunctionEventArgs<int> e)
        {

            storage.MoveTo(e.Info - 1);
            ReloadData(false);
        }

        private void OnEventDispatched(object sender, libicma.events.Action action, object state)
        {
            if (state is not Media item)
            {
                return;
            }
            switch (action)
            {
                case libicma.events.Action.Delete:
                    DeleteFileItem(item);
                    break;
                case libicma.events.Action.Execute:
                    ExecuteFileItem(item);
                    break;
                case libicma.events.Action.Open:
                    OpenFileItem(item);
                    break;
            }
        }

        private static void OpenFileItem(Media item)
        {
            var file = "";
            if (File.Exists(item.Output))
            {
                file = item.Output;
            }
            else if (File.Exists(item.Input))
            {
                file = item.Input;
            }
            try
            {
                Process.Start("explorer.exe", $"/e,/select,\"{file}\"");
            }
            catch
            {

            }
        }

        private void DeleteFileItem(Media item)
        {
            var result = MessageBox.Show($"确定删除文件[{item.Name}]?", "删除确认", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (result == System.Windows.MessageBoxResult.Yes)
            {

                storage.Remove(item.ID);
                ReloadData(true);

            }

        }
        private void ExecuteFileItem(Media item)
        {
            var settings = AppSettings.Instance();
            var option = settings.AppOption;
            if (string.IsNullOrEmpty(option.Password))
            {
                var r = MessageBox.Show("密码为空,请先设置密码!", "设置密码", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                if (r == System.Windows.MessageBoxResult.OK)
                {

                    OptionDialog dialog = new();
                    dialog.Owner=GetView<MainWindow>();
                    dialog.ShowDialog();
                    
                }
                return;
            }
            
            Execute(item, settings);
           

        }

        private void Execute(Media item, AppSettings settings)
        {
            if (item.IsEncoded)
            {
                executor.Execute(item, ExecutorAction.Decode, settings.AppOption, this);
            }
            else
            {
                executor.Execute(item, ExecutorAction.Encode, settings.AppOption, this);
            }
        }

        private void OnSettings(object sender, EventArgs e)
        {
            OptionDialog dialog = new();
            dialog.Owner = GetView<MainWindow>();
            dialog.ShowDialog();
        }
        private void OnAddItem(object sender, EventArgs e)
        {
            var button = (System.Windows.Controls.Button)sender;
            button.IsEnabled = false;
            OpenFileDialog dialog = new()
            {
                Multiselect = true
            };


            var r = dialog.ShowDialog();
            if (r == true)
            {
                var files = dialog.FileNames;
                AddFiles(files);
            }
            button.IsEnabled = true;
        }

        public void AddFiles(IEnumerable<string> files)
        {
            storage.AddRange(files);
            ReloadData(true);
        }
        /// <summary>
        /// 刷新数据显示
        /// </summary>
        /// <param name="updateUI">是否更新分页UI</param>
        private void ReloadData(bool updateUI)
        {

            if (updateUI)
            {
                var view = GetView<MainWindow>();
                view.pagenum.MaxPageCount = storage.Page <= 0 ? 1 : storage.Page;
                view.pagenum.DataCountPerPage = MediaList.PageSize;
                view.pagenum.PageIndex = storage.CurrentPage + 1;
            }

            ReloadData();
        }
        private void BeginUpdate(string id)
        {
            if (cellMap.TryGetValue(id, out var cell))
            {
                cell.Dispatcher.BeginInvoke(() =>
                {
                    cell.BeginUpdate();
                });
            }
        }

        private void Update(string id, double value)
        {
            if (cellMap.TryGetValue(id, out var cell))
            {
                cell.Dispatcher.BeginInvoke(() =>
                {
                    cell.UpdateProgress(value);
                });
            }
        }

        private void EndUpdate(string id)
        {
            if (cellMap.TryGetValue(id, out var cell))
            {
                cell.Dispatcher.BeginInvoke(() =>
                {
                    cell.EndUpdate();
                });
            }
        }

        public void UpdateStatus(string id, bool success)
        {
            if (cellMap.TryGetValue(id, out var cell))
            {
                cell.Dispatcher.BeginInvoke(() =>
                {
                    cell.UpdateStatus(success);
                });
            }
        }
        protected override void OnModelUpdate(Model model)
        {

            var m = storage.CurrentItems();
            cellMap.Clear();
            for (var index = 0; index < cells.Count; index++)
            {
                var cell = cells[index];
                var data = m.Cells.At(index);
                cell.Visibility = data == null ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
                if (data != null)
                {
                    cell.ViewModel.UpdateModel(data);
                    cellMap.TryAdd(data.ID, cell);
                }
            }

        }

        public void OnOpen(string id, long frameCount)
        {
            var item = storage.Get(id);
            if (item != null)
            {
                item.Status = MediaStatus.Processing;
            }
            BeginUpdate(id);
        }

        public void OnProgress(string id, double progress)
        {
            Update(id, progress);

        }

        public void OnNext(object data)
        {

        }

        public void OnError(string id, string err)
        {
            var item = storage.Get(id);
            if (item != null)
            {
                item.Status = MediaStatus.ProccedFailure;
                item.Output = "";
            }
            executor.Remove(id);
            EndUpdate(id);
            UpdateStatus(id, false);

        }

        public void OnComplete(string id,string output)
        {
            var item = storage.Get(id);
            if (item != null)
            {
                item.Status = MediaStatus.ProccedSuccess;
                item.Output = output;
            }
            executor.Remove(id);
            EndUpdate(id);
            UpdateStatus(id, true);
        }

        public void OnOpen(string id, Header header)
        {
            var item = storage.Get(id);
            if (item != null)
            {
                item.Status = MediaStatus.Processing;
            }
            BeginUpdate(id);
        }
    }
}
