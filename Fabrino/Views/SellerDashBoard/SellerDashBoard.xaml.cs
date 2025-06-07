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

namespace Fabrino.Views.SellerDashBoard
{
    /// <summary>
    /// Interaction logic for SellerDashBoard.xaml
    /// </summary>
    public partial class SellerDashBoard : Window
    {
        private readonly AppDbContext _dbContext;
        private UserModel _currentUser;
        IUserRepository _userRepository;
        public SellerDashBoard(UserModel user)
        {
            InitializeComponent();
            _currentUser = user;
            CheckAndPromptSecurityQuestion(_currentUser);
            _dbContext = new AppDbContext();
            _userRepository = new SqlUserRepository(_dbContext);
            LoadUserData(user);
            this.Closed += Dashboard_Closed;

        }
        private void CheckAndPromptSecurityQuestion(UserModel user)
        {
            if (user.role == "Seller" && string.IsNullOrWhiteSpace(user.security_answer_hash))
            {
                SecuritySetupWindow securityQuestion = new SecuritySetupWindow(user);
                securityQuestion.Show();
            }
        }

        private void Dashboard_Closed(object sender, EventArgs e)
        {
            _userRepository.UpdateLastLogin(_currentUser.username);

        }
        
        private void SetActiveButton(Button activeButton)
        {

            DashboardButton.Style = (Style)FindResource("MenuButtonStyle");
            InventoryButton.Style = (Style)FindResource("MenuButtonStyle");
            OrderButton.Style = (Style)FindResource("MenuButtonStyle");
            SettingsButton.Style = (Style)FindResource("MenuButtonStyle");


            activeButton.Style = (Style)FindResource("ActiveMenuButtonStyle");
        }

        private void LoadUserData(UserModel user)
        {
            if (user != null)
            {
                Dispatcher.Invoke(() => {
                    // تنظیم اطلاعات در Status Bar
                    UsernameTextBlock.Text = user.full_name;
                    UserRoleTextBlock.Text = user.role;
                    LastLoginTextBlock.Text = user.last_login.HasValue
                        ? $"آخرین ورود: {user.last_login.Value.ToString("yyyy/MM/dd HH:mm")}"
                        : "اولین ورود";

                    // تنظیم اطلاعات در نوار کناری
                    SidebarUsernameText.Text = user.full_name;
                    SidebarUserRoleText.Text = user.role;
                });
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new DashBoardPage());

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new DashBoardPage());

        }


        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            _userRepository.UpdateLastLogin(_currentUser.username);
            login.Show();
            this.Close();
        }

       
        private void InventoryButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InventoryButton);
            MainFrame.Navigate(new InventoryPage());
        }

        

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(OrderButton);
            MainFrame.Navigate(new SOrderPage());
        }

        private void LogoutButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            _userRepository.UpdateLastLogin(_currentUser.username);
            login.Show();
            this.Close();
        }

        private void SettingsButton_Click_1(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton);
            MainFrame.Navigate(new SellerSettingPage());
        }

        private void InventoryButton_Click_1(object sender, RoutedEventArgs e)
        {
            SetActiveButton(InventoryButton);
            MainFrame.Navigate(new InventoryPage());
        }
    }
}
