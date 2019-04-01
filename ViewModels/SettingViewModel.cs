using Boysenberry.Repos;
using Boysenberry.Services;
using Prism.Mvvm;
using System.Configuration;

namespace Boysenberry.ViewModels
{
    class SettingViewModel : BindableBase
    {
        public static string SELECT_BASE_HINT = "Please select the base";

        private DataAccess _dataAccess;
        private string _weibobase = SELECT_BASE_HINT;
        private WeiboService _weiboService;


        public string WeiboBase
        {
            get { return _weibobase; }
            set { SetProperty(ref _weibobase, value); }
        }

        public SettingViewModel(WeiboService weiboService)
        {
            _weiboService = weiboService;
            _weibobase = !ConfigurationManager.AppSettings[_weiboService.BASE].Equals("") ? 
                ConfigurationManager.AppSettings[_weiboService.BASE] : SELECT_BASE_HINT;
        }

        public void InitBase(string mode)
        {
            using (var baseDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                baseDialog.ShowDialog();
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var selectPath = !baseDialog.SelectedPath.Equals("") ? baseDialog.SelectedPath : SELECT_BASE_HINT;
                if (mode == _weiboService.TAG)
                {
                    WeiboBase = !WeiboBase.Equals(selectPath) && !selectPath.Equals(SELECT_BASE_HINT) ? selectPath : WeiboBase;
                    config.AppSettings.Settings[_weiboService.BASE].Value = WeiboBase;
                    _dataAccess = new DataAccess(_weiboService.DB);
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
