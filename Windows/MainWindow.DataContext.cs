﻿using System;
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
            set
            {
                _dataSource = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void InstallVCButtonBase_OnClick(object sender, RoutedEventArgs e) =>
            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Libraries/VC_redist.x64.exe"));
    }
}
