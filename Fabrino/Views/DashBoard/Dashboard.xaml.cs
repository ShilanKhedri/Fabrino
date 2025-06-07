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
using Fabrino.Models;
using System.Windows;
using System.Globalization;

namespace Fabrino.Views.DashBoard
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private readonly AppDbContext _dbContext;
        private readonly UserModel _currentUser;
        private readonly IUserRepository _userRepository;

        public Dashboard(UserModel user)
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            _userRepository = new SqlUserRepository(_dbContext);
            _currentUser = user;
            DataContext = this;
            LoadUserInfo();
            this.Closed += Dashboard_Closed;
        }

        public string UserFullName => _currentUser.full_name;
        public string UserRole => _currentUser.role;

        private void Dashboard_Closed(object sender, EventArgs e)
        {
            _userRepository.UpdateLastLogin(_currentUser.username);
        }

        private void LoadUserInfo()
        {
            if (_currentUser.last_login.HasValue)
            {
                var pc = new PersianCalendar();
                var lastLogin = _currentUser.last_login.Value;
                LastLoginTextBlock.Text = $"آخرین ورود: {pc.GetYear(lastLogin)}/{pc.GetMonth(lastLogin):00}/{pc.GetDayOfMonth(lastLogin):00} {lastLogin:HH:mm}";
            }
            else
            {
                LastLoginTextBlock.Text = "اولین ورود";
            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new DashBoardPage());
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new DashBoardPage());
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(ReportButton);
            MainFrame.Navigate(new ReportPage());
        }

        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InventoryButton);
            MainFrame.Navigate(new InventoryPage());
        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(PurchaseButton);
            MainFrame.Navigate(new PurchasePage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton);
            MainFrame.Navigate(new SettingsPage(_currentUser));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _userRepository.UpdateLastLogin(_currentUser.username);
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}
