using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для DobavitNewSotrudnika.xaml
    /// </summary>
    public partial class DobavitNewSotrudnika : Window
    {
       public static List<Rol_> rols = MainWindow.entities.Rol_.ToList();
        public static List<Polzovatel_> listpol = MainWindow.entities.Polzovatel_.ToList();
        public DobavitNewSotrudnika()
        {
            InitializeComponent();

            Dolgnost.ItemsSource = MainWindow.entities.Rol_.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            familia.Clear();
            name.Clear();
            login.Clear();
            porol.Clear();
            NumberTelefon.Clear();
            Email.Clear();
            Dolgnost.ItemsSource = null ;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Ваша логика для загрузки изображения
            if (Dolgnost.Text == "лоборант")
            {
                string put = Environment.CurrentDirectory;
                Img.Source = new BitmapImage(new Uri(put + "/Sourse/laborant_1.jpeg"));
            }
            else if (Dolgnost.Text == "бухгалтер")
            {
                string put = Environment.CurrentDirectory;
                Img.Source = new BitmapImage(new Uri(put + "/Sourse/Бухгалтер.jpeg"));
            }
            else if (Dolgnost.Text == "администратор")
            {
                string put = Environment.CurrentDirectory;
                Img.Source = new BitmapImage(new Uri(put + "/Sourse/Администратор.jpeg"));
            }
            else if (Dolgnost.Text == "лоборант-исследователь")
            {
                string put = Environment.CurrentDirectory;
                Img.Source = new BitmapImage(new Uri(put + "/Sourse/laborant_2.jpeg"));
            }
            else
            {
               string put = Environment.CurrentDirectory;
               Img.Source = new BitmapImage(new Uri(put + "/Sourse/Polzovatel.jpeg"));
            }
            Polzovatel_ vod = new Polzovatel_
            {
                familia = familia.Text,
                name = name.Text,
                login = login.Text,
                telefon = NumberTelefon.Text,
                Email = Email.Text,
                IDRol = rols.FirstOrDefault(a => a.Rol == Dolgnost.Text).IDRol,
                kartinka = Img.Source.ToString() 
            };

            Boolean check_Mail = Mail_LIB.Class1.check_Mail(Email.Text);
            Boolean check_password = Mail_LIB.Class1.check_password(porol.Text);
            Boolean check_login = Mail_LIB.Class1.check_login(login.Text);
            if (!check_Mail)
            {
                MessageBox.Show("Поменяйте почту!");
            }
            else if (!check_login)
            {
                MessageBox.Show("Поменяйте логин!");
            }
            else if (!check_password)
            {
                MessageBox.Show("Поменяйте пароль!");
            }
           
            else if (!NumberTelefon.Text.StartsWith("+") || NumberTelefon.Text.Length != 12 || !NumberTelefon.Text.Skip(1).All(char.IsDigit))
            {
                MessageBox.Show("Номер телефона должен быть в формате +99999999999.");
            }
          
            else
            {
                MainWindow.entities.Polzovatel_.Add(vod);
                MainWindow.entities.SaveChanges();
                string Items = $"\n Фамилия: {familia.Text}, \n Имя: {name.Text}, \n Логин: {login.Text}, \n Пароль: {porol.Text}, \n Номер телефонаl: {NumberTelefon.Text}, \n E-mail: {Email.Text}, \n Роль: {2}";
                MessageBox.Show($"Ввод данных:" + Items);
            }
        }
    }
}
