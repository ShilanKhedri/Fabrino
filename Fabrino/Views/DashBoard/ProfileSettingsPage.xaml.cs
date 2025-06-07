using System.Windows;
using System.Windows.Controls;
using Fabrino.Models;
using Fabrino.Services;

namespace Fabrino.Views.DashBoard
{
    public partial class ProfileSettingsPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;
        private UserModel _currentUser;

        public ProfileSettingsPage(UserModel user)
        {
            InitializeComponent();
            _context = new AppDbContext();
            _userService = new UserService(_context);
            _currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            FullNameBox.Text = _currentUser.full_name;
            EmailBox.Text = _currentUser.Email;
            PhoneBox.Text = _currentUser.Phone;
        }

        private void SaveProfile_Click(object sender, RoutedEventArgs e)
        {
            _userService.UpdateUserInfo(_currentUser, FullNameBox.Text, EmailBox.Text, PhoneBox.Text);
            MessageBox.Show("تغییرات با موفقیت ذخیره شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
} 