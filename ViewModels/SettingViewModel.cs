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
using System.Diagnostics;

namespace Boysenberry.ViewModels
{
    class SettingViewModel : BindableBase
    {
        public static string SELECT_BASE_HINT = "Please select the base";
        public static string WeiboDB = @"Data Source=weibo.db;";

        private string _weibobase = SELECT_BASE_HINT;
        //private string _input;
        //private ObservableCollection<Record> _list;
        //private Record _selected;
        private DataAccess _dataAccess;
        //private WeiboService _service;


        public string WeiboBase
        {
            get { return _weibobase; }
            set { SetProperty(ref _weibobase, value); }
        }

        public SettingViewModel()
        {
            _weibobase = !ConfigurationManager.AppSettings["WeiboBase"].Equals("") ? ConfigurationManager.AppSettings["WeiboBase"] : "Please select the base";
        }

        public void InitBase(string mode)
        {
            using (var baseDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                baseDialog.ShowDialog();
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var selectPath = !baseDialog.SelectedPath.Equals("") ? baseDialog.SelectedPath : SELECT_BASE_HINT;
                if (mode == "weibo")
                {
                    WeiboBase = !WeiboBase.Equals(selectPath) && !selectPath.Equals(SELECT_BASE_HINT) ? selectPath : WeiboBase;
                    config.AppSettings.Settings["WeiboBase"].Value = WeiboBase;
                    _dataAccess = new DataAccess(WeiboDB);
                    _dataAccess.CreateTable();
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                //if (mode == BaseBilibili)
                //{
                //    filePath = @"\sqliteBilibili.db";
                //}
                //if (mode == BaseFlickr)
                //{
                //    filePath = @"\sqliteFlickr.db";
                //}
                //if (mode == BasePixiv)
                //{
                //    filePath = @"\sqlitePixiv.db";
                //}
                //if (!string.IsNullOrEmpty(DirectoryPath))
                //{
                //    myDialog.SelectedPath = DirectoryPath;
                //}
                //myDialog.ShowDialog();
                //DirectoryPath = myDialog.SelectedPath;
            }
        }
    }
}
