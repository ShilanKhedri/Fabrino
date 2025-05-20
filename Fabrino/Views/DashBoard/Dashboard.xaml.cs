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
using System.Windows.Shapes;

namespace Fabrino.Views.DashBoard
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
            //MainFrame.Navigate(new Uri("DashBoardPage.xaml", UriKind.Relative));
        }

        private void SetActiveButton(Button activeButton)
        {
            
            DashboardButton.Style = (Style)FindResource("MenuButtonStyle");
            ReportButton.Style = (Style)FindResource("MenuButtonStyle");
            InventoryButton.Style = (Style)FindResource("MenuButtonStyle");
            PurchaseButton.Style = (Style)FindResource("MenuButtonStyle");
            SettingsButton.Style = (Style)FindResource("MenuButtonStyle");

            
            activeButton.Style = (Style)FindResource("ActiveMenuButtonStyle");
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashBoardPage());
            SetActiveButton(DashboardButton);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashBoardPage());
        }
        
        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(PurchaseButton); // اگه تابع SetActiveButton داری
            MainFrame.Navigate(new Purchase2Way());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton); // اگه تابع SetActiveButton داری
            MainFrame.Navigate(new SettingsPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow login = new MainWindow();
            login.Show();
        }
    }
}
