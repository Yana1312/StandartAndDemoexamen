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
using AppShoes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AppShoes.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingOrderWindow.xaml
    /// </summary>
    public partial class SettingOrderWindow : Window
    {
        private Order Order;
        private bool IsInCreationMode;
        private bool IsSaved = false;
        int MaxId;
        public SettingOrderWindow(Order order)
        {
            InitializeComponent();
            LoadData();
            IsInCreationMode = order == null;
            Order = order ?? new Order { OrderId = MaxId + 1, OrderDate = DateTime.Now, Items = new List<Item>(), OrderStatus  = "Новый"};
            if (IsInCreationMode)
                StatusCb.IsEnabled = false;
            DataContext = Order;
        }

        private void LoadData()
        {
            TovarsCb.ItemsSource = ShoesContext.context.Tovars.Include(t => t.Items).ToList();
            TovarsCb.DisplayMemberPath = "TovarTitle";
            TovarsCb.SelectedValuePath = "TovarArticul";
            StatusCb.ItemsSource = new List<string> { "Новый", "Завершен" };
            PickPointCb.ItemsSource = ShoesContext.context.PickPoints.ToList();
            MaxId = ShoesContext.context.Orders.Max(o => o.OrderId);
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved && !IsInCreationMode)
                ShoesContext.context.Entry(Order).Reload();
            new OrderWindow().Show();
            this.Close();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var errors = GetErrors(Order);
            if (errors.Count > 0)
            {
                MessageBox.Show($"Некорректные данные:\n{string.Join("\n", errors)}", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (IsInCreationMode)
                    ShoesContext.context.Orders.Add(Order);

                IsSaved = true;
                ShoesContext.context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<string> GetErrors(Order order)
        {
            var errors = new List<string>();

            if (order.OrderDeliveryDate == null)
                errors.Add("Дата доставки заказа не может быть пустой");

            if (string.IsNullOrEmpty(order.OrderStatus))
                errors.Add("Статус заказа не может быть пустым");

            if (order.OrderAdress == null)
                errors.Add("Адрес пункта выдачи не может быть пустым");

            if (!order.Items.Any())
                errors.Add("Заказ должен содержать хотя бы один товар");

            return errors;
        }

        private void AddTovarBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TovarsCb.SelectedItem is Tovar selectedTovar)
            {
                if (selectedTovar.TovarCount <= 0)
                {
                    MessageBox.Show("К сожалению, этот товар на складе закончился!","Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                

                var existingItem = Order.Items.FirstOrDefault(i => i.ItemArticul == selectedTovar.TovarArticul);

                if (existingItem != null && selectedTovar.TovarCount == existingItem.ItemCount)
                {
                    MessageBox.Show("К сожалению, вы уже добавили возможное количество товара в корзину((((!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                    if (existingItem != null)
                {
                    existingItem.ItemCount++;
                }
                else
                {
                    Item newItem = new Item
                    {
                        OrderId = Order.OrderId,
                        ItemArticul = selectedTovar.TovarArticul,
                        ItemCount = 1,
                        ItemArticulNavigation = selectedTovar 
                    };
                    Order.Items.Add(newItem);
                }

                ItemsDg.ItemsSource = null;
                ItemsDg.ItemsSource = Order.Items;

                TovarsCb.SelectedItem = null;
            }
            else MessageBox.Show("Сначала выберите товар из списка!");
        }

      
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Item itemToRemove)
            {
                Order.Items.Remove(itemToRemove);
                ItemsDg.Items.Refresh();
            }
        }
    }
}
