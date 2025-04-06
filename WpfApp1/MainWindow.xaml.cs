using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using Path = System.IO.Path;

namespace WpfApp1
{
    /// <summary>
    /// log - для хранения логина
    /// rol - для хранения роли
    /// IDUser - для хранения пользователя
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MedLabEntities entities = new MedLabEntities();
        public static string log = "";
        public static string rol = "";
        public static string fam;
        public static string namef;
        public static int qwer;
        public static string POrol;
        public static int IDUser;
        public static int IDRoli;
        public static string pwd;

        /// <summary>
        /// Type - лист для типа страхавого полиса
        /// Strah - лист для страховой компании
        /// </summary>

        public static List<TypeStrahPolic_> Type = entities.TypeStrahPolic_.ToList();
        public static List<StrahovaiKompania_> Strah = entities.StrahovaiKompania_.ToList();

        /// <summary>
        /// IpAdress - переменная для Ip-адреса
        /// polzoV - для листа пользователей
        /// </summary>


        public static string IpAdress;
        public static List<Polzovatel_> polzoV = entities.Polzovatel_.ToList();

        public MainWindow()
        {
            InitializeComponent();

            TipSrahPolis.ItemsSource = entities.TypeStrahPolic_.ToList();
            StrahKompania.ItemsSource = entities.StrahovaiKompania_.ToList();

            string allowchar = string.Empty;
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char[] a = { ',' };
            string[] ar = allowchar.Split(a);
            pwd = string.Empty;
            string temp = string.Empty;
            System.Random r = new System.Random();

            for (int i = 0; i < 6; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];
                pwd += temp;
            }
            CaptchaText.Text = pwd;
          //  CaptchaText.LayoutTransform = new RotateTransform(r.Next(-10,10));
        }    
        private void HistorVhodaOhi()
        {
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                IpAdress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            histor_ history = new histor_();
            history.IDPolzovatel = IDUser;
            history.ip = IpAdress;
            history.DataNach = DateTime.Now;
            history.lastenter = DateTime.Now;
            history.IDPrichina = 3;
            entities.histor_.Add(history);
            entities.SaveChanges();
        }
        private void HistorVhodaYspex()
        {
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                IpAdress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            histor_ history = new histor_();
            history.IDPolzovatel = IDUser;
            history.ip = IpAdress;
            history.DataNach = DateTime.Now;
            history.lastenter = DateTime.Now;
            history.IDPrichina = 1;
            entities.histor_.Add(history);
            entities.SaveChanges();
        }
        private void VhodSystem()
        {
            MessageBox.Show("Вы не робот!");
            switch (rol)
            {
                case "администратор":
                    Admin admin = new Admin();
                    admin.Show();
                    this.Hide();
                    break;
                case "пациент":
                    Polzovatel polzovatel = new Polzovatel();
                    polzovatel.Show();
                    this.Hide();
                    break;
                case "лоборант":
                    Laborant laborant = new Laborant();
                    laborant.Show();
                    this.Hide();
                    break;
                case "бухгалтер":
                    Bugalter bugalter = new Bugalter();
                    bugalter.Show();
                    this.Hide();
                    break;
                case "лоборант-исследователь":
                    LaborantIssled laborantIssled = new LaborantIssled();
                    laborantIssled.Show();
                    this.Hide();
                    break;
            }
        }
        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            string hashedPassword = "";
            if (Password.Visibility == Visibility.Hidden)
                Password.Text = PasswordP.Password;
            else
                PasswordP.Password = Password.Text;
      
            var ProverkaVhoda = entities.Polzovatel_.FirstOrDefault(x => x.login == Login.Text);
            if (ProverkaVhoda != null)
            {
                log = ProverkaVhoda.login;
                rol = ProverkaVhoda.Rol_.Rol;
                qwer = ProverkaVhoda.IDPolzovatel;
                fam = ProverkaVhoda.familia;
                IDRoli = Convert.ToInt32(ProverkaVhoda.IDRol);
                namef = ProverkaVhoda.name;
                POrol = ProverkaVhoda.password;
                IDUser = ProverkaVhoda.IDPolzovatel;

                hashedPassword = ProverkaVhoda.password;

                byte[] buffer4;
                if (hashedPassword == null)
                {
                    MessageBox.Show("Нет");
                }
                if (porol.Text == null)
                {
                    throw new ArgumentNullException("Пароль");
                }

                if (hashedPassword.Length >= 20)
                {
                    byte[] src = Convert.FromBase64String(hashedPassword);
                    if ((src.Length != 0x31) || (src[0] != 0))
                    {
                        MessageBox.Show("Нет");
                    }
                    byte[] dst = new byte[0x10];
                    Buffer.BlockCopy(src, 1, dst, 0, 0x10);
                    byte[] buffer3 = new byte[0x20];
                    Buffer.BlockCopy(src, 0x11, buffer3, 0, 0x20);
                    using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(Password.Text, dst, 0x3e8))
                    {
                        buffer4 = bytes.GetBytes(0x20);
                    }
                    string pas = Convert.ToBase64String(buffer4);
                    string pas1 = Convert.ToBase64String(buffer3);
                    if (pas == pas1)
                    {
                        if (CaptchaText.Visibility == Visibility.Visible)
                        {
                            if (caph.Text != pwd || caph.Text == "")
                            {
                                MessageBox.Show("Капча введена неверно!");

                                MainWindow window = new MainWindow(); this.Hide();
                                MessageBox.Show("Капча была введена не правильно, приложение будет заблокировано на 10 секунд"); window.Show();
                                MessageBox.Show("Приложение будет заблокировано на 10 секунд.");
                                HistorVhodaOhi(); Thread.Sleep(10000); // Замораживаем UI-поток на 10 секунд
                                MessageBox.Show("Вы снова можете войти");
                              
                            }
                            else
                            {
                                HistorVhodaYspex();
                                VhodSystem();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Yes");
                            HistorVhodaYspex();
                            VhodSystem();
                        }                       
                    }
                    else
                    {
                        MessageBox.Show("No");
                        if (CaptchaText.Visibility == Visibility.Hidden)
                        {
                            CaptchaText.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            if (caph.Text != pwd || caph.Text == "")
                            {
                                MessageBox.Show("Капча введена неверно!");

                                MainWindow window = new MainWindow(); this.Hide();
                                MessageBox.Show("Капча была введена не правильно, приложение будет заблокировано на 10 секунд"); window.Show();
                                MessageBox.Show("Приложение будет заблокировано на 10 секунд.");
                                HistorVhodaOhi(); Thread.Sleep(10000); // Замораживаем UI-поток на 10 секунд
                                MessageBox.Show("Вы снова можете войти");
                            }
                        }
                        MessageBox.Show("Не правильно введён логин или пароль, а если вы не зарегистрированы, зарегистрируйтесь!");                                     
                    }
                       
                }
                // Проверка логина и пароля
                else if (hashedPassword != Password.Text)
                {
                    MessageBox.Show("Ошибка в логине или пароле!");
                    if (CaptchaText.Visibility == Visibility.Hidden)
                    {
                        CaptchaText.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (caph.Text != pwd || caph.Text == "")
                        {
                            MessageBox.Show("Капча введена неверно!");

                            MainWindow window = new MainWindow(); this.Hide();
                            MessageBox.Show("Капча была введена не правильно, приложение будет заблокировано на 10 секунд"); window.Show();
                            MessageBox.Show("Приложение будет заблокировано на 10 секунд.");
                            HistorVhodaOhi(); Thread.Sleep(10000); // Замораживаем UI-поток на 10 секунд
                            MessageBox.Show("Вы снова можете войти");
                        }
                    }                
                }
                else
                {
                    if (CaptchaText.Visibility == Visibility.Visible)
                    {
                        if (caph.Text != pwd || caph.Text == "")
                        {
                            MessageBox.Show("Капча введена неверно!");

                            MainWindow window = new MainWindow(); this.Hide();
                            MessageBox.Show("Капча была введена не правильно, приложение будет заблокировано на 10 секунд"); window.Show();
                            MessageBox.Show("Приложение будет заблокировано на 10 секунд.");
                            HistorVhodaOhi(); Thread.Sleep(10000); // Замораживаем UI-поток на 10 секунд
                            MessageBox.Show("Вы снова можете войти");
                        }
                        else
                        {
                            HistorVhodaYspex();
                            VhodSystem();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Yes");
                        HistorVhodaYspex();
                        VhodSystem();
                    }
                }
            }   
        }
        private void Registrachia_Click(object sender, RoutedEventArgs e)
        {
            tab.SelectedIndex = 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            familia.Clear();
            name.Clear();
            login.Clear();
            porol.Clear();
            podtvergdenieParol.Clear();
            dateHpBr.SelectedDate = null;
            SeriaPas.Clear();
            NumberPas.Clear();
            NumberTelefon.Clear();
            Email.Clear();
            StrahPolis.Clear();
            TipSrahPolis.ItemsSource = null;
            StrahKompania.ItemsSource = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] salt;
            byte[] buffer2;
            if (porol.Text == null)
            {
                throw new ArgumentNullException("пароль");
            }

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(porol.Text, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            string pas = Convert.ToBase64String(dst);

            Polzovatel_ vod = new Polzovatel_
            {
                familia = familia.Text,
                name = name.Text,
                login = login.Text,
                password = pas,
                dateHappyBiz = DateTime.Parse(dateHpBr.Text),
                SeriaPasPort = int.Parse(SeriaPas.Text),
                NomerPasPort = int.Parse(NumberPas.Text),
                telefon = NumberTelefon.Text,
                Email = Email.Text,
                numberStrahPolic = StrahPolis.Text,
                IDRol = 1,
                IDtypetrahPolic = Type.FirstOrDefault(a => a.typetrahPolic == TipSrahPolis.Text).IDtypetrahPolic,
                IDStrahovaiKompania = Strah.FirstOrDefault(a => a.name == StrahKompania.Text).IDStrahovaiKompania,
                kartinka = "Polzovatel",
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
            else if (porol.Text != podtvergdenieParol.Text)
            {
                MessageBox.Show("Пароль должен совпадать!");
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
                entities.Polzovatel_.Add(vod);
                entities.SaveChanges();
                string Items = $"\n Фамилия: {familia.Text}, \n Имя: {name.Text}, \n Дата рождения: {DateTime.Parse(dateHpBr.Text)}, \n Логин: {login.Text}, \n Пароль: {porol.Text},\n Серия паспорта: {int.Parse(SeriaPas.Text)}, \n Номер паспорта: {int.Parse(NumberPas.Text)}, \n Номер телефонаl: {NumberTelefon.Text}, \n E-mail: {Email.Text}, \n Номер страхового полиса: {StrahPolis.Text}, \n Роль: {2}";
                MessageBox.Show($"Ввод данных:" + Items);
            }           
        }
        private void Zvezda_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Visibility == Visibility.Hidden)
            {
                Password.Visibility = Visibility.Visible;
                PasswordP.Visibility = Visibility.Hidden;
                Password.Text = PasswordP.Password;

            }
            else
            {
                PasswordP.Visibility = Visibility.Visible;
                Password.Visibility = Visibility.Hidden;
                PasswordP.Password = Password.Text;
            }
        }
        private void GenerCapch_Click(object sender, RoutedEventArgs e)
        {
            string allowchar = string.Empty;
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char[] a = { ',' };
            string[] ar = allowchar.Split(a);
            pwd = string.Empty;
            string temp = string.Empty;
            System.Random r = new System.Random();

            for (int i = 0; i < 6; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];

                pwd += temp;
            }

            CaptchaText.Text = pwd;
        }
    }
}
