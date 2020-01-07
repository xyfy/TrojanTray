using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Trojan.Tray;

namespace Trojan.Tran.Wpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        TrojanHelper helper = new TrojanHelper();
        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            helper.DataReceivedEvent += Helper_DataReceivedEvent;
            if (File.Exists(helper.ExePath))
            {
                btnStart.IsEnabled = true;
                lblCursorPosition.Text = $"运行文件路径：{helper.ExePath}";
            }
            else
            {
                btnStart.IsEnabled = false;
                lblCursorPosition.Text = $"运行文件不存在：{helper.ExePath}"; ;
            }
            btnEnd.IsEnabled = false;
        }

        private void Helper_DataReceivedEvent(string text, DataReceivedType dataReceivedType)
        {
            UpdateListBox(text);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            helper.Close();
            base.OnClosing(e);
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                helper.Start();
                btnEnd.IsEnabled = true;
                btnStart.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "发生错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void UpdateListBox(string text)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (lbInfos.Items.Count > 5000)
                {
                    lbInfos.Items.Clear();
                }
                lbInfos.Items.Add(text);
            }));
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            helper.Close();
            btnEnd.IsEnabled = false;
            btnStart.IsEnabled = true;
        }
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;

            if (disposing)
            {
                helper.Dispose();
            }

            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}