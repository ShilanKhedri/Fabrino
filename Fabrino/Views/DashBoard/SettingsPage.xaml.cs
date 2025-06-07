using Fabrino.Helpers;
using Fabrino.Models;
using Fabrino.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class SettingsPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private readonly SignUpController _signUpController;
        private UserModel _currentUser;

        public SettingsPage(UserModel user)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _userService = new UserService(_context);
            _signUpController = new SignUpController();
            _currentUser = user;

            
        }

       

        private void NavigateToProfile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ProfileSettingsPage(_currentUser));
        }

        private void NavigateToPassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PasswordSettingsPage(_currentUser));
        }

        private void NavigateToUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UsersManagementPage());
        }

        private void NavigateToAddUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUserPage());
        }

        private void NavigateToBackup_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new BackupPage());
        }
    }
}
