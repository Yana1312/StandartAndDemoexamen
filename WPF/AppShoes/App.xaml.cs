using System.Configuration;
using System.Data;
using System.Windows;
using AppShoes.Models;

namespace AppShoes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User curUser { get; set; }

    }
}
