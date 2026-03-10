using System.Data.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AppShoes.Models;
using AppShoes.Views;

namespace AppShoes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AuthBtn_Click(object sender, RoutedEventArgs e)
        {
            List<User> users = ShoesContext.context.Users.ToList();
            foreach (User user in users) 
            {
                if (user.UserPassword == PasswordTbox.Text && user.UserLogin == LoginTbox.Text)
                {
                    App.curUser = user; 
                    new CatalogWindow().Show();
                    this.Close();
                    return;
                }
            }
            MessageBox.Show("Данный пользователь не найден! Проверьте введенные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void GuestAuthBtn_Click(object sender, RoutedEventArgs e)
        {
            App.curUser = null;
            new CatalogWindow().Show();
            this.Close();
            return;
        }
    }
}