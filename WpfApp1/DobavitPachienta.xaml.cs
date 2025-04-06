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
    /// Логика взаимодействия для DobavitPachienta.xaml
    /// </summary>
    public partial class DobavitPachienta : Window
    {
        public static List<TypeStrahPolic_> Type = MainWindow.entities.TypeStrahPolic_.ToList();
        public static List<StrahovaiKompania_> Strah = MainWindow.entities.StrahovaiKompania_.ToList();
        public DobavitPachienta()
        {
            InitializeComponent();
            TipSrahPolis.ItemsSource = MainWindow.entities.TypeStrahPolic_.ToList();
            StrahKompania.ItemsSource = MainWindow.entities.StrahovaiKompania_.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Polzovatel_ vod = new Polzovatel_
            {
                familia = familia.Text,
                name = name.Text,
                dateHappyBiz = DateTime.Parse(dateHpBr.Text),
                SeriaPasPort = int.Parse(SeriaPas.Text),
                NomerPasPort = int.Parse(NumberPas.Text),
                telefon = NumberTelefon.Text,
                Email = Email.Text,
                numberStrahPolic = StrahPolis.Text,
                IDRol = 1,
                IDtypetrahPolic = Type.FirstOrDefault(a => a.typetrahPolic == TipSrahPolis.Text).IDtypetrahPolic,
                IDStrahovaiKompania = Strah.FirstOrDefault(a => a.name == StrahKompania.Text).IDStrahovaiKompania,
                kartinka = "Polzovatel"
            };

            Boolean check_Mail = Mail_LIB.Class1.check_Mail(Email.Text);
            if (!check_Mail)
            {
                MessageBox.Show("Поменяйте почту!");
            }
            else if (StrahPolis.Text.Length != 16 || !StrahPolis.Text.All(char.IsDigit))
            {
                MessageBox.Show("Номер полиса должен содержать 16 цифр.");
            }
            else if (!NumberTelefon.Text.StartsWith("+") || NumberTelefon.Text.Length != 12 || !NumberTelefon.Text.Skip(1).All(char.IsDigit))
            {
                MessageBox.Show("Номер телефона должен быть в формате +99999999999.");
            }
            else if (SeriaPas.Text.Length != 4 || !SeriaPas.Text.All(char.IsDigit))
            {
                MessageBox.Show("Серия паспорта должна содержать 4 цифр.");
            }
            else if (NumberPas.Text.Length != 6 || !NumberPas.Text.All(char.IsDigit))
            {
                MessageBox.Show("Номер паспорта должна содержать 6 цифр.");
            }
            else if (dateHpBr.DisplayDate.Date == DateTime.Today)
            {
                MessageBox.Show("Дата не должна быть сегодняшней");
            }

            else if (dateHpBr.DisplayDate.Date > DateTime.Today)
            {
                MessageBox.Show("Дата не может быть в будущем");
            }
            else
            {
                MainWindow.entities.Polzovatel_.Add(vod);
                MainWindow.entities.SaveChanges();
                string Items = $"\n Фамилия: {familia.Text}, \n Имя: {name.Text}, \n Дата рождения: {DateTime.Parse(dateHpBr.Text)}, \n Серия паспорта: {int.Parse(SeriaPas.Text)}, \n Номер паспорта: {int.Parse(NumberPas.Text)}, \n Номер телефонаl: {NumberTelefon.Text}, \n E-mail: {Email.Text}, \n Номер страхового полиса: {StrahPolis.Text}, \n Роль: {2}";
                MessageBox.Show($"Ввод данных:" + Items);
            }

            SozdanZakaz sozdanZakaz = new SozdanZakaz();
            sozdanZakaz.Show();
            this.Hide();
        }
    }
}
