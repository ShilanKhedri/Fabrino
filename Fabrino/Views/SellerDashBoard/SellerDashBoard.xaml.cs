using Fabrino.Views.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Fabrino.Views.SellerDashBoard
{
    /// <summary>
    /// Interaction logic for SellerDashBoard.xaml
    /// </summary>
    public partial class SellerDashBoard : Window
    {
        public SellerDashBoard()
        {
            InitializeComponent();
        }
        private void SetActiveButton(Button activeButton)
        {

            DashboardButton.Style = (Style)FindResource("MenuButtonStyle");
            InventoryButton.Style = (Style)FindResource("MenuButtonStyle");
            OrderButton.Style = (Style)FindResource("MenuButtonStyle");
            SettingsButton.Style = (Style)FindResource("MenuButtonStyle");


            activeButton.Style = (Style)FindResource("ActiveMenuButtonStyle");
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new SDashboardPage());

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SDashboardPage());
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            login.Show();
            this.Close();
        }

       
        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InventoryButton);
            MainFrame.Navigate(new InventoryPage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton);
            MainFrame.Navigate(new SellerSettingPage());
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(OrderButton);
            MainFrame.Navigate(new SOrderPage());
        }
    }
}
