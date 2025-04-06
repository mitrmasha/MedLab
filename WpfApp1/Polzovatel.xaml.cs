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
    /// Логика взаимодействия для Polzovatel.xaml
    /// </summary>
    public partial class Polzovatel : Window
    {
        public Polzovatel()
        {
            InitializeComponent();

            string put = Environment.CurrentDirectory;
            Img.Source = new BitmapImage(new Uri(put + "/Sourse/Polzovatel.jpeg"));
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
    }
}
