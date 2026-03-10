using AppShoes.Models;
using Microsoft.Win32;
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

namespace AppShoes.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingTovarWindow.xaml
    /// </summary>
    public partial class SettingTovarWindow : Window
    {
        private bool IsInCreationMode;
        private bool IsSaved = false;
        public Tovar Tovar { get; set; }

        public SettingTovarWindow(Tovar tovar)
        {
            InitializeComponent();
            IsInCreationMode = tovar == null;
            if (!IsInCreationMode)
            {
                Tovar = tovar;
                Title = "Редактирование товара";
            }
            else
            {
                Tovar = new Tovar { TovarArticul = GenerateNewArticul() };
                Title = "Добавление товара";
            }
            DataContext = Tovar;
            LoadData();

            if (!string.IsNullOrEmpty(Tovar.TovarPhoto))
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", Tovar.TovarPhoto);

                if (System.IO.File.Exists(path))
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(path);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    TovarImg.Source = img;
                }
                else
                {
                    TovarImg.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/picture.png"));
                }
            }
        }

        private void LoadData()
        {
            SupplierCb.ItemsSource = ShoesContext.context.Suppliers.ToList();
            SupplierCb.DisplayMemberPath = "SupplierTitle"; 
            SupplierCb.SelectedValuePath = "SupplierId";   

            ManufacturCb.ItemsSource = ShoesContext.context.Manufacturs.ToList();
            ManufacturCb.DisplayMemberPath = "ManufacturName";
            ManufacturCb.SelectedValuePath = "ManufacturId";

            CategoryCb.ItemsSource = ShoesContext.context.Categories.ToList();
            CategoryCb.DisplayMemberPath = "CategoryTitle";
            CategoryCb.SelectedValuePath = "CategoryId";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveBtn.Focus();

            var errors = GetErrors(Tovar);
            if (errors.Count > 0)
            {
                MessageBox.Show($"Некорректные данные:\n{string.Join("\n", errors)}", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                //if (IsInCreationMode)
                //    ShoesContext.context.Tovars.Add(Tovar);

                //IsSaved = true;
                //ShoesContext.context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<string> GetErrors(Tovar tovar)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(tovar.TovarTitle))
                errors.Add("Название не может быть пустым");
            if (string.IsNullOrEmpty(tovar.TovarUnit))
                tovar.TovarUnit = "шт.";

            else if (tovar.TovarUnit != "шт.")
                errors.Add("Единица измерения должна быть в штуках (шт.)");

            if (string.IsNullOrEmpty(tovar.TovarSupplier.ToString()))
                errors.Add("Поставщик не может быть пустой");
            if (string.IsNullOrEmpty(tovar.TovarCost.ToString()))
                errors.Add("Цена не может быть пустой");
            if (string.IsNullOrEmpty(tovar.TovarManufactor.ToString()))
                errors.Add("Производитель не может быть пустым");
            if (string.IsNullOrEmpty(tovar.TovarCategory.ToString()))
                errors.Add("Категория не может быть пустой");

            if (string.IsNullOrEmpty(tovar.TovarCount.ToString()))
                errors.Add("Количество не может быть пустым");
            if (string.IsNullOrEmpty(tovar.TovarDescription))
                errors.Add("Описание не может быть пустым");

            if (string.IsNullOrEmpty(tovar.TovarPhoto))
                tovar.TovarPhoto = "picture.png";

            if (tovar.TovarSale < 0 || tovar.TovarSale > 99) 
                errors.Add("Некорректная скидка");
            if (tovar.TovarCost <= 1)
                errors.Add("Цена не может быть отрицательным или мегьшу 1!");
            if (tovar.TovarCount <= 1)
                errors.Add("Количество не может быть отрицательным или меньше 1!");

            return errors;
        }
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved && !IsInCreationMode)
                ShoesContext.context.Entry(Tovar).Reload();
            new CatalogWindow().Show();
            this.Close();
        }

        private void PriceTbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!char.IsDigit(e.Text, 0) && e.Text != ".")
            {
                e.Handled = true;
                return;
            }

            if (e.Text == ".")
            {
                if (textBox.Text.Length == 0 || textBox.CaretIndex == 0)
                {
                    e.Handled = true;
                    MessageBox.Show("Цена не может начинаться с точки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (textBox.Text.Contains("."))
                {
                    e.Handled = true;
                    MessageBox.Show("Можно ввести только одну точку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        private string GenerateNewArticul()
        {
            var lastArticul = ShoesContext.context.Tovars
                .OrderByDescending(t => t.TovarArticul)
                .Select(t => t.TovarArticul)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(lastArticul)) return "A100T1"; 

            var match = System.Text.RegularExpressions.Regex.Match(lastArticul, @"([A-ZА-Я])(\d+)([A-ZА-Я])(\d+)");

            if (match.Success)
            {
                string p1 = match.Groups[1].Value;
                int num = int.Parse(match.Groups[2].Value); 
                string p2 = match.Groups[3].Value;
                string p3 = match.Groups[4].Value;

                return $"{p1}{num + 1}{p2}{p3}";
            }
            return "A" + (ShoesContext.context.Tovars.Count() + 1) + "T1";
        }
        private void SelectPhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "Images |*.jpg;*.png;*.jpeg";

            if (op.ShowDialog() == true)
            {
                try
                {
                    var imageInfo = new BitmapImage();
                    imageInfo.BeginInit();
                    imageInfo.UriSource = new Uri(op.FileName);
                    imageInfo.CacheOption = BitmapCacheOption.OnLoad;
                    imageInfo.EndInit();

                    if (imageInfo.PixelWidth > 300 || imageInfo.PixelHeight > 200)
                    {
                        MessageBox.Show("Размер изображения не должен превышать 300x200!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    string targetPath = "";

                    try
                    {
                        string extension = Path.GetExtension(op.FileName);
                        string uniqueName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + Path.GetFileName(op.FileName);
                        targetPath = Path.Combine(folderPath, uniqueName);

                        if (!string.IsNullOrEmpty(Tovar.TovarPhoto) && Tovar.TovarPhoto != "picture.png")
                        {
                            string oldFileName = Tovar.TovarPhoto;

                            bool isUsedByOthers = ShoesContext.context.Tovars
                                .Any(t => t.TovarPhoto == oldFileName && t.TovarArticul != Tovar.TovarArticul);

                            if (!isUsedByOthers)
                            {
                                string oldPath = Path.Combine(folderPath, oldFileName);
                                if (File.Exists(oldPath))
                                {
                                    try
                                    {
                                        TovarImg.Source = null;
                                        GC.Collect();
                                        GC.WaitForPendingFinalizers();
                                        File.Delete(oldPath);
                                    }
                                    catch { }
                                }
                            }
                        }

                        File.Copy(op.FileName, targetPath, true);
                        Tovar.TovarPhoto = uniqueName;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при работе с файлом: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    BitmapImage finalImage = new BitmapImage();
                    finalImage.BeginInit();
                    finalImage.UriSource = new Uri(targetPath);
                    finalImage.CacheOption = BitmapCacheOption.OnLoad;
                    finalImage.EndInit();

                    TovarImg.Source = finalImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при работе с файлом: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaleTbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }
        }
    }
}
