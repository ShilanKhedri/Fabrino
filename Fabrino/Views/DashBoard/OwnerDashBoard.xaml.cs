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
    /// Interaction logic for OwnerDashBoard.xaml
    /// </summary>
    public partial class OwnerDashBoard : Window
    {
        public OwnerDashBoard()
        {
            //InitializeComponent();
        }

        private void RemoveText(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && tb.Text == "جستجو...")
            {
                tb.Text = "";
                tb.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddText(object sender, RoutedEventArgs e)
        {
            var tb = sender as System.Windows.Controls.TextBox;
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "جستجو...";
                tb.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
       
        private void TextBox_TextChanged(TextChangedEventArgs e)
        {

        }
        private Button currentActiveButton;

        private void SetActiveButton(Button clickedButton)
        {
            if (currentActiveButton != null)
                currentActiveButton.Style = (Style)this.Resources["MenuButtonStyle"];

            clickedButton.Style = (Style)this.Resources["ActiveMenuButtonStyle"];
            currentActiveButton = clickedButton;
        }
        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = sender as Button;
            SetActiveButton(clickedButton);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PurchaseOrder purchaseOrderWindow = new PurchaseOrder();
            purchaseOrderWindow.Show();
            this.Close(); 

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Purchase purchase = new Purchase();
            purchase.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow exit = new MainWindow();
            exit.Show();
            this.Close();
        }
    }
}
