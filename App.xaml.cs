using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MetroRadiance.UI;
using Newtonsoft.Json;
using Ruminoid.Common.Helpers;
using Ruminoid.Common.Net;
using Ruminoid.Common.UI.Windows;
using Ruminoid.PluginManager.Models;
using Ruminoid.PluginManager.Windows;

namespace Ruminoid.PluginManager
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (sender, args) =>
            {
                args.Handled = true;
                MessageBox.Show(
                    args.Exception.Message,
                    "灾难性故障",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                MessageBox.Show(
                    ((Exception)args.ExceptionObject)?.Message ?? "Exception",
                    "灾难性故障",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
            };

            PluginManager.Properties.Resources.Culture = CultureInfo.CurrentUICulture;

            MainWindow mainWindow = new MainWindow();
            MainWindow = mainWindow;

            // Get Update Source

            Dashboard.Config dashboardConfig = ConfigHelper<Dashboard.Config>.OpenConfig();
            mainWindow.DataSource = $"{dashboardConfig.UpdateServer}{dashboardConfig.UpdateChannel}";
            mainWindow.OnPropertyChanged(nameof(mainWindow.DisplayDataSource));

            // Create Downloader

            Downloader pluginsDataDownloader = new Downloader();
            ProgressWindow progressWindow = ProgressWindow.CreateAndShowDialog(pluginsDataDownloader);

            pluginsDataDownloader.Progress.Title = "正在准备";
            pluginsDataDownloader.Progress.Description = "正在获取插件列表";

            // Set UI Style

            Current.Dispatcher?.Invoke(() => ThemeService.Current.ChangeTheme(Theme.Dark));
            Current.Dispatcher?.Invoke(() => ThemeService.Current.ChangeAccent(Accent.Blue));

            // Start Download

            Task<byte[]> downloadTask = pluginsDataDownloader.DownloadByteArray($"{mainWindow.DataSource}/plugins.json", 1);

            downloadTask.Wait();

            progressWindow.Hide();

            if (!downloadTask.IsCompleted || downloadTask.IsFaulted)
            {
                MessageBox.Show(
                    "获取插件源时出现错误。请检查更新通道设置和网络连接。",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
                Current.Shutdown(1);
            }

            // Parse Plugin Source

            try
            {
                PluginSource.Current =
                    JsonConvert.DeserializeObject<PluginSource>(Encoding.UTF8.GetString(downloadTask.Result));
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "解析插件源时出现错误：" + e.Message,
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
                Current.Shutdown(1);
            }

            mainWindow.RootView.DataContext = PluginSource.Current;
            MainWindow.Show();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ThemeService.Current.Register(this, Theme.Windows, Accent.Windows);
        }
    }
}
