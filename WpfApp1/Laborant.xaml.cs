using iTextSharp.text.pdf;
using iTextSharp.text;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using iText.Layout.Borders;
using iText.Layout.Element;
using Path = System.IO.Path;

namespace WpfApp1
{
    /// <summary>
    /// timer - переменная для таймера
    /// </summary>
    public partial class Laborant : Window
    {
        // public TimeSpan Timedspan = new TimeSpan(2, 30, 0);
      //  public static DateTime taim;
        public static string IpAdress;
        public static List<Zakaz_> listzak = MainWindow.entities.Zakaz_.ToList();
        public static List<Polzovatel_> listpolz = MainWindow.entities.Polzovatel_.ToList();
        public static List<Status_> liststat = MainWindow.entities.Status_.ToList();
        public static List<YslugiVZakazah_> listyslugzak = MainWindow.entities.YslugiVZakazah_.ToList();
        public static List<Yslugi_> listyslugi = MainWindow.entities.Yslugi_.ToList();
        public static DispatcherTimer timer;
        public static bool isWarningShown = false;
        public static TimeSpan Timedspan = new TimeSpan(0, 1, 30);
        public static int idz;
        public static string fm;
        public static DateTime dt;
        public static string nm;
        public static bool IsCreatingNewOrder { get; set; }
        public Laborant()
        {
            InitializeComponent();
            InitializeTimer();

            SortirovkaStatus.ItemsSource = liststat; 
            SortirovkaFIO.ItemsSource = listpolz;
            Result.ItemsSource = MainWindow.entities.Zakaz_.ToList();

            string put = Environment.CurrentDirectory;
            Img.Source = new BitmapImage(new Uri(put + "/Sourse/laborant_1.jpeg"));
            NameName.Content = MainWindow.namef;
            familiName.Content = MainWindow.fam;
            RolPolzov.Content = MainWindow.rol;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Typ(int select)
        {
            var zakaz = MainWindow.entities.Zakaz_.FirstOrDefault(z => z.IDZakaz == select);
            if (zakaz != null)
            {
                zakaz.VapolnenieZakaza = select;
                Result.ItemsSource = MainWindow.entities.Zakaz_.ToList();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Timedspan = Timedspan.Add(TimeSpan.FromSeconds(-1));
            if (Timedspan.TotalSeconds <= 0)
            {
                timer.Stop();
                Vrema.Content = "Время вышло!";
                HistorVhodaVrema();
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
                MessageBox.Show("Приложение заблокировано на 30 минут.");
                Thread.Sleep(1800000); // Задержка на 30 секунд (можно изменить на 10000 для 10 секунд)
                MessageBox.Show("Вы снова можете войти");
            }
            else if (Timedspan.TotalMinutes <= 1 && Timedspan.TotalSeconds > 0 && !isWarningShown)
            {
                MessageBox.Show("Осталось 15 минут!");
                isWarningShown = true;
            }
            Vrema.Content = Timedspan.ToString(@"hh\:mm\:ss");
        }

        private void HistorVhodaVrema()
        {
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                IpAdress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            histor_ history = new histor_();
            history.IDPolzovatel = MainWindow.IDUser;
            history.ip = IpAdress;
            history.DataNach = DateTime.Now;
            history.lastenter = DateTime.Now;
            history.IDPrichina = 2;
           MainWindow.entities.histor_.Add(history);
            MainWindow.entities.SaveChanges();
        }

        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Laborant.IsCreatingNewOrder = false;
            var select = Result.SelectedItem as Zakaz_; 
                SozdanZakaz sozdanZakaz = new SozdanZakaz();
                sozdanZakaz.SetOrderData(
                    select.IDZakaz,
                    select.Polzovatel_.familia,
                    select.DateSozdania 
                );
                sozdanZakaz.Show();            
        }

        private void Dobavit_Click(object sender, RoutedEventArgs e)
        {
            var select = Result.SelectedItem as Zakaz_;
            listyslugzak = MainWindow.entities.YslugiVZakazah_.Where(a => a.IDZakaz == select.IDZakaz).ToList();
            fm = select.Polzovatel_.familia;
            idz = select.IDZakaz;
            dt = select.DateSozdania;
            nm = select.Polzovatel_.name;
            DobavitYslugu dobavitYslugu = new DobavitYslugu();          
            dobavitYslugu.Show();        
        }
        private void Raspech_Click(object sender, RoutedEventArgs e)
        {
            if (Result.SelectedItem is Zakaz_ selectedOrder)
            {
                chek(selectedOrder);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите заказ для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Sosdanie_Click(object sender, RoutedEventArgs e)
        {
            Laborant.IsCreatingNewOrder = true;
            SozdanZakaz sozdanZakaz = new SozdanZakaz();
            sozdanZakaz.Show();   
        }

        private void SortirovkaStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int select = SortirovkaStatus.SelectedIndex;
            if (select >= -1)
            {
                int qwer = liststat[select].IDStatus;
                listzak = MainWindow.entities.Zakaz_.Where(a => a.IDStatus == qwer).ToList();
                Result.ItemsSource = listzak;
            }
        }

        private void SortirovkaFIO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int select = SortirovkaFIO.SelectedIndex;
            if(select >= -1)
            {
                int qwer = listpolz[select].IDPolzovatel;
                listzak = MainWindow.entities.Zakaz_.Where(a => a.IDPolzovatel == qwer).ToList();
                Result.ItemsSource = listzak;
            }
        }

        private void Obnovit_Click(object sender, RoutedEventArgs e)
        {
            var data = MainWindow.entities.Zakaz_.ToList(); 
            Result.ItemsSource = data; 
        }
         private void chek(Zakaz_ currentOrder)
         {
            try
            {
                string user = MainWindow.entities.Polzovatel_.Where(z => z.IDPolzovatel == currentOrder.IDPolzovatel).Select(z => z.familia).FirstOrDefault();
                string fileName = $"Заказ_{currentOrder.IDZakaz}.pdf";
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
                // Создаем PDF-документ
                var document = new iTextSharp.text.Document();
                using (var writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                {
                    document.Open();
                    // Шрифт
                    BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font helvetica = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
                    // Вставка шапки чека
                    document.Add(new iTextSharp.text.Paragraph($"Заказ №{currentOrder.IDZakaz}", helvetica));
                    document.Add(new iTextSharp.text.Paragraph($"Дата: {currentOrder.DateSozdania.ToString("dd-MM-yyyy")}", helvetica));
                    document.Add(new iTextSharp.text.Paragraph($"Пациент: {user}", helvetica));
                    document.Add(new iTextSharp.text.Paragraph("ЭКЛЗ 3851495566", helvetica));
                    document.Add(new iTextSharp.text.Paragraph("Чек №", helvetica));
                    document.Add(new iTextSharp.text.Paragraph($"{DateTime.Now.ToString("g")} СИС", helvetica));
                    // Таблица для услуг
                    PdfPTable table = new PdfPTable(4); // Добавлена колонка для итоговой суммы
                    table.WidthPercentage = 100; // Таблица на всю ширину страницы
                    // Заголовки таблицы
                    PdfPCell cellHeaderCode = new PdfPCell(new Phrase("Код услуги", helvetica));
                    cellHeaderCode.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cellHeaderCode);
                    PdfPCell cellHeaderName = new PdfPCell(new Phrase("Название услуги", helvetica));
                    cellHeaderName.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cellHeaderName);
                    PdfPCell cellHeaderPrice = new PdfPCell(new Phrase("Цена", helvetica));
                    cellHeaderPrice.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cellHeaderPrice);
                    PdfPCell cellHeaderTotal = new PdfPCell(new Phrase("Итог", helvetica));
                    cellHeaderTotal.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                    table.AddCell(cellHeaderTotal);
                    // Заполнение таблицы данными из коллекции
                    var selectedServices = MainWindow.entities.YslugiVZakazah_.Where(s => s.IDZakaz == currentOrder.IDZakaz).ToList();
                    float totalSum = 0; // Переменная для хранения итоговой суммы
                    foreach (var service in selectedServices)
                    {
                        var serviceDetails = MainWindow.entities.Yslugi_.FirstOrDefault(s => s.IDYslugi == service.IDYslugi);
                        if (serviceDetails != null)
                        {
                            float price = (float)serviceDetails.Price;
                            totalSum += price; // Добавляем цену к итоговой сумме
                            table.AddCell(new Phrase(serviceDetails.IDYslugi.ToString(), helvetica)); // Код услуги
                            table.AddCell(new Phrase(serviceDetails.Service, helvetica)); // Название услуги
                            table.AddCell(new Phrase(price.ToString(), helvetica)); // Цена
                            table.AddCell(new Phrase(price.ToString(), helvetica)); // Итог (цена)
                        }
                    }
                    // Добавляем строку с итоговой суммой
                    PdfPCell cellTotalSumLabel = new PdfPCell(new Phrase("Общая сумма:", helvetica));
                    cellTotalSumLabel.Colspan = 3; // Занимает 3 столбца
                    cellTotalSumLabel.HorizontalAlignment = Element.ALIGN_RIGHT; // Выравнивание
                    table.AddCell(cellTotalSumLabel);
                    PdfPCell cellTotalSumValue = new PdfPCell(new Phrase(totalSum.ToString(), helvetica)); 
                    table.AddCell(cellTotalSumValue);
                    // Добавляем таблицу в документ
                    document.Add(table);
                    document.Close();
                    writer.Close();
                    MessageBox.Show("pdf - чек сохранён!");
                }
            }
            catch
            {

            }
        }
    }
}
