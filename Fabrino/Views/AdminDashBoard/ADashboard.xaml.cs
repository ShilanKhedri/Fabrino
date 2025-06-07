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

namespace Fabrino.Views.AdminDashBoard
{
    /// <summary>
    /// Interaction logic for ADashboard.xaml
    /// </summary>
    public partial class ADashboard : Window
    {
         private readonly AppDbContext _dbContext;
        private UserModel _currentUser;
        private IUserRepository _userRepository;

        public ADashboard(UserModel user)
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            _userRepository = new SqlUserRepository(_dbContext);
            _currentUser = user;
            LoadUserData(user);
            this.Closed += ADashboard_Closed;
        }

        private void ADashboard_Closed(object sender, EventArgs e)
        {
            _userRepository.UpdateLastLogin(_currentUser.username);
        }

        private void LoadUserData(UserModel user)
        {
            if (user != null)
            {
                Dispatcher.Invoke(() =>
                {
                    UsernameTextBlock.Text = user.full_name;
                    UserRoleTextBlock.Text = user.role;
                    LastLoginTextBlock.Text = user.last_login.HasValue
                        ? $"آخرین ورود: {user.last_login.Value:yyyy/MM/dd HH:mm}"
                        : "اولین ورود";

                    SidebarUsernameText.Text = user.full_name;
                    SidebarUserRoleText.Text = user.role;
                });
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            DashboardButton.Style = (Style)FindResource("MenuButtonStyle");
            SupportButton.Style = (Style)FindResource("MenuButtonStyle");
            SettingsButton.Style = (Style)FindResource("MenuButtonStyle");

            activeButton.Style = (Style)FindResource("ActiveMenuButtonStyle");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new ADashBoardPage());
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new ADashBoardPage());
        }

        private void SupportButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SupportButton);
            MainFrame.Navigate(new SupportPage());
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton);
            MainFrame.Navigate(new AdminSettingsPage(_currentUser));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _userRepository.UpdateLastLogin(_currentUser.username);
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
