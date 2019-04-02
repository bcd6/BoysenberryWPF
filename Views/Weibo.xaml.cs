
using Boysenberry.ViewModels;
using System.Windows;
using System.Windows.Controls;


namespace Boysenberry.Views
{
    public partial class Weibo : Page
    {

        private WeiboViewModel ViewModel => DataContext as WeiboViewModel;

        public Weibo()
        {
            InitializeComponent();
        }
        private void UnloadedHandler(object sender, RoutedEventArgs e)
        {
            ViewModel.UnloadedHandler();
        }
        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectionChanged();
        }
       
        private void Add(object sender, RoutedEventArgs e)
        {
            ViewModel.Add();
        }
        private void Analyse(object sender, RoutedEventArgs e)
        {
            ViewModel.Analyse();
        }
        private void Start(object sender, RoutedEventArgs e)
        {
            ViewModel.Start();
        }
        private void Stop(object sender, RoutedEventArgs e)
        {
            ViewModel.Stop();
        }
        private void Open(object sender, RoutedEventArgs e)
        {
            ViewModel.Open();
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            ViewModel.Delete();
        }

        private void InputChange(object sender, RoutedEventArgs e)
        {
            ViewModel.InputChange();
        }
    }
}
