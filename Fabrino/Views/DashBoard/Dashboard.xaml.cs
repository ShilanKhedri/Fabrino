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

namespace Fabrino.Views.DashBoard
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        private readonly AppDbContext _dbContext;
        private UserModel _currentUser;
        IUserRepository _userRepository;

        public Dashboard(UserModel user)
        {
            InitializeComponent();
            _dbContext = new AppDbContext();
            _userRepository = new SqlUserRepository(_dbContext);
            _currentUser = user;
            LoadUserData(user);
            this.Closed += Dashboard_Closed;
        }


        private void Dashboard_Closed(object sender, EventArgs e)
        {
            _userRepository.UpdateLastLogin(_currentUser.username);

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
            SetActiveButton(DashboardButton);
            MainFrame.Navigate(new DashBoardPage());
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashBoardPage());
            SetActiveButton(DashboardButton);


        }

        private void PurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(PurchaseButton); 
            MainFrame.Navigate(new Purchase2Way());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow login = new MainWindow();
            _userRepository.UpdateLastLogin(_currentUser.username);
            login.Show();
            this.Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(SettingsButton);
            MainFrame.Navigate(new SettingsPage(_currentUser));
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

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}
