using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Ruminoid.PluginManager.Models;
using Path = System.IO.Path;

namespace Ruminoid.PluginManager.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Closed += (sender, args) => Application.Current.Shutdown(0);
        }

        #region Event Processors

        private void InstallVCButtonBase_OnClick(object sender, RoutedEventArgs e) =>
            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries/VC_redist.x64.exe"));

        #endregion

        private void ChoosePluginsFolderButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (PluginSource.Current.SelectedPlatform != null)
            {
                PluginSource.Current.SelectedPlatform.ChoosePluginsFolder();
                PluginSource.Current.SelectedPlatform.Scan();
            }
        }

        private void ScanButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (PluginSource.Current.SelectedPlatform != null) PluginSource.Current.SelectedPlatform.Scan();
        }
    }
}
