
using Boysenberry.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace Boysenberry.Views
{
    public partial class Setting : Page

    {
        private SettingViewModel ViewModel => DataContext as SettingViewModel;

        public Setting()
        {
            InitializeComponent();
        }

        private void SetWeiboBase(object sender, RoutedEventArgs e)
        {
            ViewModel.InitBase("weibo");
        }
    }
}
