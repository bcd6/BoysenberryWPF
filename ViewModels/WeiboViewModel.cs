using System;
using System.Collections.Generic;
using Boysenberry.Models;
using Boysenberry.Utils;
using Boysenberry.Repos;
using Boysenberry.Services;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using System.Configuration;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows;
using System.Runtime.Caching;

namespace Boysenberry.ViewModels
{
    class WeiboViewModel : BindableBase
    {
        private Style _enableStyle = Application.Current.FindResource("ToolbarButtonIcon") as Style;
        private Style _disableStyle = Application.Current.FindResource("ToolbarButtonIconDisable") as Style;
        private string _base;
        private string _input;
        private ObservableCollection<Record> _list;
        private Record _selected;
        private DataAccess _dataAccess;
        private ObjectCache _cache;
        private WeiboService _service;



        public string Base
        {
            get { return _base; }
            set { SetProperty(ref _base, value); }
        }
        public string Input
        {
            get { return _input; }
            set { SetProperty(ref _input, value); }
        }
        public ObservableCollection<Record> List
        {
            get { return _list; }
            set { SetProperty(ref _list, value); }

        }
        public Record Selected
        {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        public WeiboViewModel(WeiboService service)
        {
            _cache = MemoryCache.Default;
            _selected = new Record();
            _selected.IsFuncEnable = false;
            _selected.IsOpenEnable = false;
            _selected.FuncIconStyle = _disableStyle;
            _selected.OpenIconStyle = _disableStyle;
            _selected.StopIconStyle = _disableStyle;
            _service = service;
            _dataAccess = new DataAccess(_service.DB);

            if (!ConfigurationManager.AppSettings[_service.BASE].Equals(SettingViewModel.SELECT_BASE_HINT) &&
                !ConfigurationManager.AppSettings[_service.BASE].Equals(""))
            {
                _base = ConfigurationManager.AppSettings[_service.BASE];
                ObservableCollection<Record> listCache = _cache[_service.TAG] as ObservableCollection<Record>;
                if (_cache[_service.TAG] != null)
                {
                    _list = listCache;
                }
                else
                {
                    _list = _dataAccess.QueryAll();
                }
            }
            else
            {
                _base = SettingViewModel.SELECT_BASE_HINT;
                _list = new ObservableCollection<Record>();
            }
        }

        public void UnloadedHandler()
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddYears(999);
            _cache.Set(_service.TAG, List, policy);
            Debug.WriteLine("page unloaded");
        }

        public void SelectionChanged()
        {
            RaisePropertyChanged(string.Empty);
        }

        public bool EnableButton(string mode)
        {
            try
            {
                var record = Selected;
                if (record == null)
                {
                    return false;
                }
                switch (mode)
                {
                    case "Analyse":
                        return record.IsFuncEnable ? false : true;
                    case "Start":
                        return record.IsFuncEnable ? false : true;
                    case "Stop":
                        return record.IsFuncEnable ? true : false;
                    case "Open":
                        return true;
                    case "Delete":
                        return record.IsFuncEnable ? false : true;
                    default:
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }


        }

        public void Add()
        {

            try
            {
                var nickname = Input.Trim();
                var userId = _service.NicknameToUserId(nickname);
                if (userId == null) return;
                Record record = new Record(userId, nickname, 0, DateTime.Now);
                var row = _dataAccess.Insert(record);
                if (row == 0)
                {
                    Input = "";
                    ToastNotificationUtil.Show("Save failed", "error");

                }
                else
                {
                    List.Add(record);
                    Input = "";
                    ToastNotificationUtil.Show("Save success: " + nickname);
                }
            }
            catch (Exception)
            {
                ToastNotificationUtil.Show("Save failed", "error");
            }
        }

        public async void Analyse()
        {
            try
            {
                var record = Selected;
                record.IsFuncEnable = false;
                record.IsStopEnable = true;
                record.FuncIconStyle = _disableStyle;
                record.StopIconStyle = _enableStyle;
                record.IsProgressVisiable = Visibility.Visible;
                RaisePropertyChanged(string.Empty);
                var imgUrls = new List<string>();
                await Task.Run(() =>
                {
                    imgUrls = _service.GetAllImgURL(record.UserId);
                });
                record.Count = imgUrls.Count;
                record.UpdateTime = DateTime.Now;
                _dataAccess.Update(record);
                record.IsFuncEnable = true;
                record.IsStopEnable = false;
                record.FuncIconStyle = _enableStyle;
                record.StopIconStyle = _disableStyle;
                record.IsProgressVisiable = Visibility.Hidden;
                RaisePropertyChanged(string.Empty);
                ToastNotificationUtil.Show("Analyse complete: " + record.Nickname);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

        public async void Start()
        {
            try
            {
                Selected.CancellationTokenSource = new CancellationTokenSource();
                var record = Selected;
                var path = $"{Base}\\{record.UserId}";
                record.IsFuncEnable = false;
                record.IsStopEnable = true;
                record.FuncIconStyle = _disableStyle;
                record.StopIconStyle = _enableStyle;
                record.IsProgressVisiable = Visibility.Visible;
                RaisePropertyChanged(string.Empty);
                var imgUrls = new List<string>();
                await Task.Run(async () =>
                {
                    imgUrls = _service.GetAllImgURL(record.UserId);
                    //DirectoryInfo dstFolder = Directory.CreateDirectory(path);
                    foreach (var imgUrl in imgUrls)
                    {
                        if (record.CancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                        await _service.DownlaodImg(imgUrl, path);
                    }
                }, record.CancellationTokenSource.Token);
                record.Count = imgUrls.Count;
                record.UpdateTime = DateTime.Now;
                _dataAccess.Update(record);
                record.IsFuncEnable = true;
                record.IsStopEnable = false;
                record.FuncIconStyle = _enableStyle;
                record.StopIconStyle = _disableStyle;
                record.IsProgressVisiable = Visibility.Hidden;
                RaisePropertyChanged(string.Empty);
                ToastNotificationUtil.Show("Downlaod complete: " + record.Nickname);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Stop()
        {
            try
            {
                var record = Selected;
                if (record.IsFuncEnable == false)
                {
                    record.CancellationTokenSource.Cancel();
                    record.CancellationTokenSource = null;
                    record.IsFuncEnable = true;
                    record.IsStopEnable = false;
                    record.FuncIconStyle = _enableStyle;
                    record.StopIconStyle = _disableStyle;
                    record.IsProgressVisiable = Visibility.Hidden;
                    RaisePropertyChanged(string.Empty);
                    ToastNotificationUtil.Show("Task cancellation");
                }
            }
            catch (Exception)
            {
                ToastNotificationUtil.Show("Task cancellation failed");
            }

        }

        public void Open()
        {
            var record = Selected;
            var path = $"{Base}\\{record.UserId}";
            Directory.CreateDirectory(path);
            Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path);
        }

        public void Delete()
        {
            try
            {
                var record = Selected;
                _dataAccess.Delete(record);
                var path = $"{Base}\\{record.UserId}";
                DirectoryInfo dstFolder = Directory.CreateDirectory(path);
                dstFolder.Delete();
                List.Remove(record);
                ToastNotificationUtil.Show("Delete complete: " + record.Nickname);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }
    }
}
