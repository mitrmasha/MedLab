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
using System.Xml.Linq;
using System.Xml;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для SozdanZakaz.xaml
    /// </summary>
    public partial class SozdanZakaz : Window
    {
        public static List<Zakaz_> Zakazik = MainWindow.entities.Zakaz_.ToList();
        public static List<Polzovatel_> polzoV = MainWindow.entities.Polzovatel_.ToList();

        public static int ZakaID;
        public SozdanZakaz()
        {
            InitializeComponent();
            NameFIO.ItemsSource = MainWindow.entities.Polzovatel_.ToList();

            Date.SelectedDate = DateTime.Now;

            if (NameFIO.Text == "")
            {
                var ZakazID = MainWindow.entities.Zakaz_.OrderByDescending(z => z.IDZakaz).FirstOrDefault();
                int NumberZak = ZakazID.IDZakaz + 1;
                NumberZakaz.Text = NumberZak.ToString();
            }
          
        }
        public void SetOrderData(int orderId, string family, DateTime orderDate)
        {
            ZakaID = orderId; // Сохраняем ID заказа
            NumberZakaz.Text = orderId.ToString();
            NameFIO.Text = family;
            Date.SelectedDate = orderDate.Date; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Laborant.IsCreatingNewOrder)
            {
                var selectedUser = NameFIO.SelectedItem as Polzovatel_;
                Zakaz_ zakaz = new Zakaz_
                {
                    IDZakaz = int.Parse(NumberZakaz.Text),
                    DateSozdania = DateTime.Now,
                    IDStatus = 3,
                    VapolnenieZakaza = 0,
                    IDPolzovatel = selectedUser.IDPolzovatel,
                };

                MainWindow.entities.Zakaz_.Add(zakaz);
                MainWindow.entities.SaveChanges();
            }
            else
            {
                var qwer = MainWindow.entities.Zakaz_.FirstOrDefault(z => z.IDZakaz == ZakaID);
                qwer.Polzovatel_.familia = NameFIO.Text; // Обновляем фамилию
                qwer.DateSozdania = Date.SelectedDate.Value; // Обновляем дату
                // Сохраняем изменения в базе данных
                MainWindow.entities.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены.");              
            }

            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DobavitPachienta dobavitPachienta = new DobavitPachienta();
            dobavitPachienta.Show();
            this.Hide();
        }
    }
}
