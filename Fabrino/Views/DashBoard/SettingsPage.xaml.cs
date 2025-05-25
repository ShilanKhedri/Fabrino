using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.DashBoard
{
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            
        }

        private void PersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new PersonalInfoControl();
        }

        private void Security_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new SecurityControl();
        }

        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new BackupControl();
        }

    }
}
