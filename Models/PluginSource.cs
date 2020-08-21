using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace Ruminoid.PluginManager.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PluginSource : INotifyPropertyChanged
    {
        #region Current

        public static PluginSource Current { get; internal set; }

        #endregion

        #region Data Context

        [JsonProperty]
        private Collection<PluginPlatform> platforms;

        public Collection<PluginPlatform> Platforms => platforms;

        [JsonProperty]
        private string path = "plugins";

        public string Path => path;

        #endregion

        //#region Scan Utilities

        //private bool _scaned;

        //public bool Scaned
        //{
        //    get => _scaned;
        //    set
        //    {
        //        _scaned = value;
        //        OnPropertyChanged();
        //    }
        //}

        //#endregion

        #region Display Context

        private PluginPlatform _selectedPlatform;

        public PluginPlatform SelectedPlatform
        {
            get => _selectedPlatform;
            set
            {
                _selectedPlatform = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PluginPlatform : INotifyPropertyChanged
    {
        #region Json Data

        [JsonProperty]
        private string id = "Unknown";

        public string Id => id;

        [JsonProperty]
        private string name = "（未知平台）";

        public string Name => name;

        [JsonProperty]
        private string suggestPath = "";

        public string SuggestPath => suggestPath;

        private string actualPath = "";

        public string ActualPath
        {
            get => actualPath;
            set
            {
                actualPath = value;
                OnPropertyChanged();
            }
        }

        [JsonProperty]
        private Collection<Plugin> plugins = new Collection<Plugin>();

        public Collection<Plugin> Plugins => plugins;

        #endregion

        #region Local Data

        private ObservableCollection<PluginFile> localPlugins = new ObservableCollection<PluginFile>();

        public ObservableCollection<PluginFile> LocalPlugins => localPlugins;

        #endregion

        #region Scan Utilities

        public void ChoosePluginsFolder()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog()
            {
                Title = "选择文件夹",
                IsFolderPicker = true,
                DefaultDirectory = Environment.CurrentDirectory,
                EnsurePathExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;

            ActualPath = dialog.FileName;
        }

        public void Scan()
        {
            if (ActualPath is null || ActualPath == string.Empty)
            {
                MessageBox.Show(
                    "请先选择管理路径。",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
                return;
            }

            IEnumerable<string> files = Directory.EnumerateFiles(actualPath);

            foreach (string file in files)
            {
                
            }
        }

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class Plugin : INotifyPropertyChanged
    {
        #region Data Context

        [JsonProperty]
        private string id = "Unknown";

        public string Id => id;

        [JsonProperty]
        private string name = "（未知插件）";

        public string Name => name;

        [JsonProperty]
        private string description = "（未知插件）";

        public string Description => description;

        [JsonProperty]
        private Collection<PluginFile> files = new Collection<PluginFile>();

        public Collection<PluginFile> Files => files;

        private PluginFile _selectedFile;

        public PluginFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    [JsonObject(MemberSerialization.OptIn)]
    public sealed class PluginFile
    {
        #region Data Context

        [JsonProperty]
        private string id = "Unknown";

        public string Id => id;

        [JsonProperty]
        private string path = "unknown";

        public string Path => path;

        [JsonProperty]
        private string name = "（未知文件）";

        public string Name => name;

        private string _pluginName = "（未知插件）";

        public string PluginName => _pluginName;

        private string _pluginDescription = "";

        public string PluginDescription => _pluginDescription;

        [JsonProperty]
        private uint hash;

        public uint Hash => hash;

        #endregion

        #region Methods

        public void Install()
        {

        }

        public void Uninstall()
        {

        }

        #endregion
    }
}
