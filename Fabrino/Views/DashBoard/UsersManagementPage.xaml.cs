using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Fabrino.Models;
using Fabrino.Services;
using Microsoft.VisualBasic;

namespace Fabrino.Views.DashBoard
{
    public partial class UsersManagementPage : Page
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public UsersManagementPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            _userService = new UserService(_context);
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersGrid.ItemsSource = _context.Users
                .Where(u => u.is_active)
                .ToList();
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is UserModel user)
            {
                string newName = Interaction.InputBox("نام جدید:", "ویرایش نام", user.full_name);
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    user.full_name = newName;
                    _context.SaveChanges();
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک کاربر را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersGrid.SelectedItem is UserModel user)
            {
                var result = MessageBox.Show(
                    $"آیا از حذف کاربر {user.username} اطمینان دارید؟",
                    "تایید حذف",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    user.is_active = false;
                    _context.SaveChanges();
                    LoadUsers();
                    MessageBox.Show("کاربر با موفقیت حذف شد", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("لطفاً یک کاربر را انتخاب کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
} 