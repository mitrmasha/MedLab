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

namespace WpfApp1
{
    /// <summary>
    /// listhis - Лист для просмотра истории
    /// listpolz - Лист для пользователей
    /// </summary>
    public partial class Admin : Window
    {
        public static List<histor_> listhis;
        public static List<Polzovatel_> listpolz;
        public Admin()
        {
            InitializeComponent();

            listpolz = MainWindow.entities.Polzovatel_.ToList();
            Sortirovka.ItemsSource = listpolz;
            listhis = MainWindow.entities.histor_.ToList();
            Result.ItemsSource = listhis;

            string put = Environment.CurrentDirectory;
            Img.Source = new BitmapImage(new Uri(put + "/Sourse/Администратор.jpeg"));
            NameName.Content = MainWindow.namef;
            familiName.Content = MainWindow.fam;
            RolPolzov.Content = MainWindow.rol;
        }

        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void Sortirovka_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int select = Sortirovka.SelectedIndex;
            if (select >= -1)
            {
                int qwer = listpolz[select].IDPolzovatel;
                listhis = MainWindow.entities.histor_.Where(a => a.IDPolzovatel == qwer).ToList();
                Result.ItemsSource = listhis;
            }
        }

        private void SortirovkaDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //int select = SortirovkaDate.SelectedIndex;
            //if (select >= -1) 
            //{
            //   DateTime qwer = (DateTime)listhis[select].DataNach;
            //   listhis = MainWindow.entities.histor_.Where(a => a.DataNach == qwer).ToList();
            //   Result.ItemsSource = listhis;
            //}
        }

        private void RegisNewSotrud_Click(object sender, RoutedEventArgs e)
        {
            DobavitNewSotrudnika dobavitNewSotrudnika = new DobavitNewSotrudnika();
            dobavitNewSotrudnika.Show();
            this.Hide();
        }

        private void DatePik_KeyUp(object sender, KeyEventArgs e)
        {
            TimeSpan timeSpan = new TimeSpan(12, 59, 59);

            if (!DatePik.SelectedDate.HasValue)
            {
                MessageBox.Show("Пожалуйста, выберите дату!");
                
            }
            else if (e.Key == Key.Enter)
            {
                DateTime dateTime = (DatePik.SelectedDate.Value) + timeSpan;
                listhis = MainWindow.entities.histor_.Where(a => a.DataNach > DatePik.SelectedDate && a.DataNach < dateTime).ToList();
                Result.ItemsSource = listhis;
            }                                 
        }
    }
}
