using AppShoes.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace AppShoes.Views
{
    /// <summary>
    /// Логика взаимодействия для TovarControl.xaml
    /// </summary>
    public partial class TovarControl : UserControl
    {
        public Tovar Tovar;
        public TovarControl(Tovar tovar)
        {
            InitializeComponent();
            Tovar = tovar;
            DataContext = Tovar;
            StyleViewTovar();
        }

        public void StyleViewTovar()
        {
            if (Tovar.TovarSale > 15)
                BackBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57"));
            if (Tovar.TovarCount == 0)
                CountTovarTb.Background = new SolidColorBrush(Colors.LightBlue);

            if (Tovar.TovarSale > 0)
            {
                FirstCostTb.TextDecorations = TextDecorations.Strikethrough;
                FirstCostTb.Foreground = Brushes.Red;
                decimal? cost = Tovar.FinalCost();
                FinalCostTb.Text = $" {cost}";
                FinalCostTb.Visibility = Visibility.Visible;
                FinalCostTb.ToolTip = $"Цена со скидкой {Tovar.TovarSale}";
            }
            else
            {
                FinalCostTb.Visibility = Visibility.Hidden;
            }
        }
    }
}