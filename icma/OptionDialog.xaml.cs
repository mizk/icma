using libicma.utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace icma
{
    /// <summary>
    /// OptionDialog.xaml 的交互逻辑
    /// </summary>
    public partial class OptionDialog : Window
    {
        private readonly AppSettings settings = AppSettings.Instance();
        public OptionDialog()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Check())
            {
                return;
            }
            var option = settings.AppOption;
            option.Password = password.Text;
            option.Storage = storage.Text;
           
            
            option.RandomFileName=randomName.IsChecked ?? false;
            settings.Save();
            DialogResult = true;
        }

        private bool Check()
        {
            string passwd = password.Text;
            if (string.IsNullOrEmpty(passwd))
            {
                MessageBox.Show("密码不能为空", "保存设置", MessageBoxButton.OK, MessageBoxImage.Error);
                password.Focus();
                return false;
            }
            var d = storage.Text;
            if (!Directory.Exists(d))
            {
                MessageBox.Show("密码不能为空", "保存设置", MessageBoxButton.OK, MessageBoxImage.Error);
                storage.Focus();
                return false;
            }
            return true;
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            settings.ResetDefault();
            DialogResult = true;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true,
                RootFolder = Environment.SpecialFolder.MyDocuments
            };
            var r=dialog.ShowDialog();
            if (r == true)
            {
                storage.Text = dialog.SelectedPath;
            }

        }

        private void BtnRandom_Click(object sender, RoutedEventArgs e)
        {
            password.Text=Utils.RandomString(32);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            storage.Text = settings.AppOption.Storage;
            password.Text = settings.AppOption.Password;
           
            randomName.IsChecked = settings.AppOption.RandomFileName;


        }
    }
}
