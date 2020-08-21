using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ruminoid.PluginManager.Windows
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Global Data

        private string _dataSource = "";

        public string DataSource
        {
            get => _dataSource;
            set => _dataSource = value;
        }

        public string DisplayDataSource => $"当前更新通道：{_dataSource}";

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
