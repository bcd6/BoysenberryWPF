using System;
using System.Collections.Generic;
using System.Text;
using Boysenberry.Models;
using Boysenberry.Repos;
using Boysenberry.Services;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using System.Configuration;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.IO;
using ToastNotifications;
using ToastNotifications.Position;
using System.Windows;

namespace Boysenberry.ViewModels
{
    class WeiboViewModel : BindableBase
    {
        private string _base;
        private string _input;
        private ObservableCollection<Record> _list;
        private Record _selected;
        private DataAccess _dataAccess;
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
            _service = service;
            _dataAccess = new DataAccess(SettingViewModel.WeiboDB);

            if (!ConfigurationManager.AppSettings["WeiboBase"].Equals(SettingViewModel.SELECT_BASE_HINT) &&
                !ConfigurationManager.AppSettings["WeiboBase"].Equals(""))
            {
                _base = ConfigurationManager.AppSettings["WeiboBase"];
                _list = _dataAccess.QueryAll();
            }
            else
            {
                _base = SettingViewModel.SELECT_BASE_HINT;
                _list = new ObservableCollection<Record>();
            }
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
                record.IsFuncEnable = true;
                RaisePropertyChanged(string.Empty);
                var imgUrls = new List<string>();
                await Task.Run(() =>
                {
                    imgUrls = _service.GetAllImgURL(record.UserId);
                });
                record.Count = imgUrls.Count;
                record.UpdateTime = DateTime.Now;
                _dataAccess.Update(record);
                record.IsFuncEnable = false;
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
                record.IsFuncEnable = true;
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
                record.IsFuncEnable = false;
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
                if (record.IsFuncEnable == true)
                {
                    record.CancellationTokenSource.Cancel();
                    record.CancellationTokenSource = null;
                    record.IsFuncEnable = false;
                    RaisePropertyChanged(string.Empty);
                    ToastNotificationUtil.Show("Task cancellation");
                }
            }
            catch (Exception)
            {
            }

        }
        public async void Open()
        {
            var record = Selected;
            var path = $"{Base}\\{record.UserId}";
            Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path);
        }

        public async void Delete()
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
