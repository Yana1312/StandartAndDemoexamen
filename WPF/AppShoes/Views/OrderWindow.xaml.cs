using AppShoes.Models;
using Microsoft.EntityFrameworkCore;
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

namespace AppShoes.Views
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
            LoadData();
            if (App.curUser.UserRole == "Менеджер")
            {
                OrdersLv.ContextMenu = null;
                AddOrderBtn.Visibility = Visibility.Hidden;
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            new CatalogWindow().Show();
            this.Close();
        }

        private void LoadData()
        {
            AppUserTb.Text = App.curUser.UserFullName;

            OrdersLv.Items.Clear();
            var orders = ShoesContext.context.Orders
                        .Include(t => t.OrderAdressNavigation)
                        .Include(o => o.Items)
                        .ThenInclude(t => t.ItemArticulNavigation)
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();
            foreach (var order in orders)
            {
                {
                    OrdersLv.Items.Add(new OrderControl(order));
                }
            }
            int newOrderCount = orders.Count(o => o.OrderStatus == "Новый");
            int oldOrderCount = orders.Count(o => o.OrderStatus == "Завершен");
            OrdersNewTb.Text = $"Новые заказы: {newOrderCount}\nЗавершенные заказы: {oldOrderCount}";

        }

        private void EditOrderMi_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersLv.SelectedItem is OrderControl order)
            {
                if (order.Order.OrderStatus == "Завершен")
                {
                    MessageBox.Show("Завершенные заказы нельзя редактировать!",
                                   "Доступ запрещен",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                    return;
                }

                new SettingOrderWindow(order.Order).Show();
                this.Close();
            }
        }

        private void DeleteOrderMi_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersLv.SelectedItem is OrderControl order)
            {
                if (MessageBox.Show($"Вы уверены, что хотите удалить заказ номер {order.Order.OrderId}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var orderToDelete = ShoesContext.context.Orders
                            .Include(o => o.Items)  
                            .FirstOrDefault(o => o.OrderId == order.Order.OrderId);

                        if (orderToDelete != null)
                        {
                            ShoesContext.context.Items.RemoveRange(orderToDelete.Items);

                            ShoesContext.context.Orders.Remove(orderToDelete);

                            ShoesContext.context.SaveChanges();

                            LoadData();

                            MessageBox.Show("Заказ успешно удален!", "Успех",
                                           MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            new SettingOrderWindow(null).Show();
            this.Close();
        }
    }
}
