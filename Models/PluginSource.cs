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
using Ruminoid.Common.Net;
using Ruminoid.Common.UI.Windows;
using Ruminoid.Common.Utilities;
using Ruminoid.PluginManager.Windows;

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
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
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

            if (!Directory.Exists(ActualPath))
            {
                MessageBox.Show(
                    "指定的路径不存在。",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
                return;
            }

            try
            {
                ProgressWindow progressWindow = ProgressWindow.CreateAndShowDialog();
                progressWindow.Progress.Title = "正在扫描";
                progressWindow.Progress.Description = ActualPath;

                LocalPlugins.Clear();

                List<string> files = Directory.EnumerateFiles(actualPath).ToList();
                int count = files.Count;

                for (int i = 0; i < count; i++)
                {
                    string file = files[i];
                    progressWindow.Progress.Detail = file;
                    var hash = Algorithm.CalcFileCrc32(file);

                    foreach (Plugin plugin in Plugins)
                    {
                        bool finded = false;
                        foreach (PluginFile pluginFile in plugin.Files)
                        {
                            if (pluginFile.Hash == hash)
                            {
                                finded = true;

                                // Add Item
                                LocalPlugins.Add(new PluginFile
                                {
                                    Name = pluginFile.Name,
                                    Hash = pluginFile.Hash,
                                    Path = file,
                                    PluginName = plugin.Name,
                                    PluginDescription = plugin.Description,
                                    Id = pluginFile.Id
                                });

                                break;
                            }
                        }

                        if (finded) break;
                    }

                    progressWindow.Progress.Percentage = (double) i / count;
                }

                progressWindow.Hide();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"扫描文件夹时发生错误：{e.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
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

        public string Id
        {
            get => id;
            set => id = value;
        }

        [JsonProperty]
        private string path = "unknown";

        public string Path
        {
            get => path;
            set => path = value;
        }

        [JsonProperty]
        private string name = "（未知）";

        public string Name
        {
            get => name;
            set => name = value;
        }

        private string _pluginName = "（未知插件）";

        public string PluginName
        {
            get => _pluginName;
            set => _pluginName = value;
        }

        private string _pluginDescription = "";

        public string PluginDescription
        {
            get => _pluginDescription;
            set => _pluginDescription = value;
        }

        [JsonProperty]
        private uint hash;

        public uint Hash
        {
            get => hash;
            set => hash = value;
        }

        #endregion

        #region Equals

        public override int GetHashCode() => id.GetHashCode() + name.GetHashCode();

        public override bool Equals(object obj) => obj != null && obj.GetHashCode() == GetHashCode();

        #endregion

        #region Methods

        public async void Install()
        {
            if (PluginSource.Current.SelectedPlatform.ActualPath is null ||
                PluginSource.Current.SelectedPlatform.ActualPath == string.Empty)
            {
                MessageBox.Show(
                    "请先选择管理路径。",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
                return;
            }

            if (!Directory.Exists(PluginSource.Current.SelectedPlatform.ActualPath))
            {
                MessageBox.Show(
                    "指定的路径不存在。",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
                return;
            }

            List<PluginFile> installedFiles = PluginSource.Current.SelectedPlatform.LocalPlugins.Where(file => file.Id == Id).ToList();

            if (installedFiles.Any())
            {
                MessageBoxResult result = MessageBox.Show(
                    "本地已经安装了此插件，继续操作将会升级或重新安装此插件。继续吗?",
                    "继续",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information,
                    MessageBoxResult.Yes);
                if (result != MessageBoxResult.Yes) return;
            }

            try
            {
                foreach (PluginFile installedFile in installedFiles) installedFile.Uninstall();

                string url = $"{(Application.Current.MainWindow as MainWindow)?.DataSource}/{PluginSource.Current.Path}/{Path}";
                string target = System.IO.Path.Combine(PluginSource.Current.SelectedPlatform.ActualPath, Path);

                Downloader downloader = new Downloader();
                ProgressWindow progressWindow = ProgressWindow.CreateAndShowDialog(downloader);

                await downloader.DownloadFile(url, 1, target);

                progressWindow.Hide();
                PluginSource.Current.SelectedPlatform.Scan();
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"安装插件时出现错误：{e.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
            }
        }

        public void Uninstall()
        {
            if (Path is null || Path == string.Empty || !File.Exists(path)) return;
            try
            {
                File.Delete(path);

                if (PluginSource.Current.SelectedPlatform == null) return;
                PluginSource.Current.SelectedPlatform.LocalPlugins.Remove(this);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"卸载时出现错误：{e.Message}",
                    "错误",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error,
                    MessageBoxResult.OK);
            }
        }

        #endregion
    }
}
