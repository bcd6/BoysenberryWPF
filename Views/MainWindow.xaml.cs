using System;
using System.Windows;
using System.Windows.Controls;

namespace Boysenberry.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Uri("Views/Welcome.xaml", UriKind.Relative));
            MainTitle.Content = "Welcome";
        }

        private void Navigate(object sender, RoutedEventArgs e)
        {
            Button menu = sender as Button;
            MainFrame.Navigate(new Uri("Views/"+menu.Tag+".xaml", UriKind.Relative));
            MainTitle.Content = menu.Tag;
        }
    }
}
