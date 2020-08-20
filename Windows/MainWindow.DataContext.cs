using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
    }
}
