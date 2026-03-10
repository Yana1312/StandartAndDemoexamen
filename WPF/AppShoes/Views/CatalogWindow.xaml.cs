using AppShoes.Models;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace AppShoes.Views
{
    /// <summary>
    /// Логика взаимодействия для CatalogWindow.xaml
    /// </summary>
    public partial class CatalogWindow : Window
    {
        private User User;
        List<Tovar> AllTovars;
        List<Supplier> AllSuppliers;

        private string CurrentSort = "";
        private int? currentSupplierId = null;
        private string currentSearch = "";

        public CatalogWindow()
        {
            InitializeComponent();
            User = App.curUser;
            DataContext = User;
            CheckRoles();
        }

        private void CheckRoles()
        {
            if (User == null || User.UserRole == "Авторизированный клиент")
            {
                EditMenu.Visibility = Visibility.Hidden;
                TovarsLv.ContextMenu = null;
                FilterPanel.Visibility = Visibility.Hidden;
            }
            else if (User.UserRole == "Менеджер")
            {
                AddOrderBtn.Visibility = Visibility.Visible;
            }
            else if (User.UserRole == "Администратор")
            {
                AddOrderBtn.Visibility = Visibility.Visible;
                AddBtn.Visibility = Visibility.Visible;
                EditMenu.Visibility = Visibility.Visible;
            }

            LoadData();
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            App.curUser = null;
            new MainWindow().Show();
            this.Close();
        }

        public void LoadData()
        {
            AllTovars = ShoesContext.context.Tovars.Include(t => t.TovarManufactorNavigation)
                                                   .Include(t => t.TovarCategoryNavigation)
                                                   .Include(t => t.TovarSupplierNavigation).ToList();
            foreach (Tovar tovar in AllTovars)
            {
                TovarsLv.Items.Add(new TovarControl(tovar));
            }

            AllSuppliers = ShoesContext.context.Suppliers.ToList();
            SupplierCb.Items.Add("Все поставщики");
            foreach (Supplier supplier in AllSuppliers)
            {
                SupplierCb.Items.Add(supplier.SupplierTitle);
            }
            FastFilter();

        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            new SettingTovarWindow(null).Show();
            this.Close();
        }


        private void SortCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortCb.SelectedItem != null)
            {
                ComboBoxItem item = (ComboBoxItem)SortCb.SelectedItem;
                CurrentSort = item.Content.ToString();
                FastFilter();
            }
        }
        private void SupplierCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SupplierCb.SelectedIndex == 0) currentSupplierId = null;

            string itemTitle = SupplierCb.SelectedItem.ToString();
            switch (itemTitle)
            {
                case "Kari":
                    currentSupplierId = 1;
                    break;
                case "Обувь для вас":
                    currentSupplierId = 2;
                    break;
                default:
                    currentSupplierId = null;
                    break;
            }
            FastFilter();
        }

        private void SearchTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = SearchTbox.Text;
            FastFilter();
        }

        private void FastFilter()
        {
            if (AllTovars == null || TovarsLv == null) return;

            TovarsLv.Items.Clear();
            var filteredTovars = AllTovars.AsEnumerable();

            if (currentSupplierId != null)
            {
                filteredTovars = filteredTovars.Where(t => t.TovarSupplier == currentSupplierId);
            }

            if (!string.IsNullOrEmpty(currentSearch))
            {
                filteredTovars = filteredTovars.Where(t => t.TovarTitle.ToLower().Contains(currentSearch.ToLower()) ||
                                                     t.TovarArticul.ToLower().Contains(currentSearch.ToLower()) ||
                                                     t.TovarDescription.ToLower().Contains(currentSearch.ToLower()));
            }

            switch (CurrentSort)
            {
                case "Количество - по возрастанию":
                    filteredTovars = filteredTovars.OrderBy(t => t.TovarCount);
                    break;
                case "Количество - по убыванию":
                    filteredTovars = filteredTovars.OrderByDescending(t => t.TovarCount);
                    break;
                default:
                    break;
            }
            if (filteredTovars.Count() == 0)
            {
                NotTovarsTb.Visibility = Visibility.Visible;
                TovarsLv.Visibility = Visibility.Hidden;
            }
            else
            {
                NotTovarsTb.Visibility = Visibility.Hidden;
                TovarsLv.Visibility = Visibility.Visible;
            }

            foreach (Tovar tovar in filteredTovars)
            {
                TovarsLv.Items.Add(new TovarControl(tovar));
            }
        }

        private void EditTovarMi_Click(object sender, RoutedEventArgs e)
        {
            if (TovarsLv.SelectedItem is TovarControl tovar)
            {
                new SettingTovarWindow(tovar.Tovar).Show();
                this.Close();
            }
        }

        private void DeleteTovarMi_Click(object sender, RoutedEventArgs e)
        {
            if (TovarsLv.SelectedItem is TovarControl tovar)
            {
                bool existOrder = ShoesContext.context.Items.Any(op => op.ItemArticul == tovar.Tovar.TovarArticul);
                if (existOrder)
                {
                    MessageBox.Show("Нельзя удалить товар, так как он присутствует в заказе",
                        "Запрещено", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (MessageBox.Show($"Вы уверены, что хотите удалить {tovar.Tovar.TovarTitle}?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ShoesContext.context.Tovars.Remove(tovar.Tovar);
                    ShoesContext.context.SaveChanges();
                    LoadData();
                }
            }
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            this.Close();
        }
    }
}