using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
