
using Fabrino.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fabrino.Views.AdminDashBoard
{
    public partial class SupportPage : Page
    {
        private readonly AppDbContext _context;

        public SupportPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadTickets();
        }

        private void LoadTickets()
        {
            try
            {
                var tickets = _context.SupportTickets.Include(t => t.User)
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new
                    {
                        t.Id,
                        Username = t.User.full_name,
                        t.Title,
                        t.Status,
                        t.Response,
                        CreatedAt = t.CreatedAt.ToString("yyyy/MM/dd HH:mm")
                    }).ToList();

                TicketsDataGrid.ItemsSource = tickets;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در بارگذاری تیکت‌ها: {ex.Message}");
            }
        }

        private void SubmitResponse_Click(object sender, RoutedEventArgs e)
        {
            if (TicketsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("لطفاً یک تیکت را انتخاب کنید.");
                return;
            }

            dynamic selected = TicketsDataGrid.SelectedItem;
            int ticketId = selected.Id;

            var ticket = _context.SupportTickets.Find(ticketId);
            if (ticket != null)
            {
                ticket.Response = ResponseTextBox.Text.Trim();
                ticket.Status = "پاسخ داده شد";
                _context.SaveChanges();
                MessageBox.Show("پاسخ ثبت شد.");
                ResponseTextBox.Clear();
                LoadTickets();
            }
        }
    }
}
