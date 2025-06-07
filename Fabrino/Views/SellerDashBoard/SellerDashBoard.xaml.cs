using Fabrino.Models;
using Fabrino.Views.DashBoard;
using Microsoft.EntityFrameworkCore;
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
using System.Globalization;

namespace Fabrino.Views.SellerDashBoard
{
    /// <summary>
    /// Interaction logic for SellerDashBoard.xaml
    /// </summary>
    public partial class SellerDashBoard : Window
    {
        private readonly AppDbContext _dbContext;
        private readonly UserModel _currentUser;
        private readonly IUserRepository _userRepository;

        public SellerDashBoard(UserModel user)
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            _userRepository = new SqlUserRepository(_dbContext);
            _currentUser = user;
            DataContext = this;
            LoadUserInfo();
            this.Closed += SellerDashBoard_Closed;
        }

        public string UserFullName => _currentUser.full_name;
        public string UserRole => _currentUser.role;

        private void SellerDashBoard_Closed(object sender, EventArgs e)
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
            InventoryButton.Style = (Style)FindResource("MenuButtonStyle");
            OrderButton.Style = (Style)FindResource("MenuButtonStyle");
            SettingsButton.Style = (Style)FindResource("MenuButtonStyle");

            activeButton.Style = (Style)FindResource("ActiveMenuButtonStyle");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new SDashboardPage());
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new SDashboardPage());
        }

        private void InventoryButton_Click_1(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InventoryButton);
            MainFrame.Navigate(new InventoryPage());
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(OrderButton);
            MainFrame.Navigate(new SOrderPage());
        }

        private void SettingsButton_Click_1(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton);
            MainFrame.Navigate(new SellerSettingPage(_currentUser));
        }

        private void LogoutButton_Click_1(object sender, RoutedEventArgs e)
        {
            _userRepository.UpdateLastLogin(_currentUser.username);
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
