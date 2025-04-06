using iTextSharp.text.pdf;
using iTextSharp.text;
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
using Path = System.IO.Path;
using iText.Layout.Element;

namespace WpfApp1
{
    public partial class Bugalter : Window
    {
        /// <summary>
        /// listzak - Лист для заказа
        /// listStrah - Лист для страховой компании
        ///  listPolh - лист для пользователей
        ///  idus - для получения кода бухгалтера
        /// </summary>
        public static List<Zakaz_> listzak = MainWindow.entities.Zakaz_.ToList();
        public static List<StrahovaiKompania_> listStrah = MainWindow.entities.StrahovaiKompania_.ToList();
        public static List<Polzovatel_> listPolh = MainWindow.entities.Polzovatel_.ToList();
         public static int idus;
        public Bugalter()
        {
            InitializeComponent();

            string put = Environment.CurrentDirectory;
            Img.Source = new BitmapImage(new Uri(put + "/Sourse/Бухгалтер.jpeg"));
            NameName.Content = MainWindow.namef;
            familiName.Content = MainWindow.fam;
            RolPolzov.Content = MainWindow.rol;
            //var ChetID = MainWindow.entities.Chet_.OrderByDescending(z => z.ID).FirstOrDefault();
            //int NumberZak = ChetID.ID + 1;
            idus = MainWindow.IDUser;
            SortirovkaStrahcomp.ItemsSource = MainWindow.entities.StrahovaiKompania_.ToList();
            Result.ItemsSource = MainWindow.entities.Zakaz_.Where(a => a.IDStatus == 2).ToList();
           var qa = Result.ItemsSource;         
        }

        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
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

        private void DatePikOt_KeyUp(object sender, KeyEventArgs e)
        {
            if (!DatePikOt.SelectedDate.HasValue)
            {
                MessageBox.Show("Пожалуйста, выберите дату 'От'!");
            }
            else if (e.Key == Key.Enter)
            {
                FilterOrders();
            }
        }

        private void DatePikDo_KeyUp(object sender, KeyEventArgs e)
        {
            if (!DatePikDo.SelectedDate.HasValue)
            {
                MessageBox.Show("Пожалуйста, выберите дату 'До'!");
            }
            else if (e.Key == Key.Enter)
            {
                FilterOrders();
            }
        }

        private void FilterOrders()
        {
            if (DatePikOt.SelectedDate.HasValue && DatePikDo.SelectedDate.HasValue)
            {
                DateTime startDate = DatePikOt.SelectedDate.Value;
                DateTime endDate = DatePikDo.SelectedDate.Value.AddDays(1).AddTicks(-1); 
                listzak = MainWindow.entities.Zakaz_.Where(a => a.DateSozdania >= startDate && a.DateSozdania <= endDate && a.IDStatus == 2).ToList();
                Result.ItemsSource = listzak;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите обе даты!");
            }
        }

        private void SortirovkaStrahcomp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int select = SortirovkaStrahcomp.SelectedIndex;
            if (select >= -1)
            {
                int qwer = listStrah[select].IDStrahovaiKompania;
                listzak = MainWindow.entities.Zakaz_.Where(a => a.Polzovatel_.IDStrahovaiKompania == qwer && a.IDStatus == 2).ToList();
                Result.ItemsSource = listzak;
            }
        }

        private void VstChet_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < MainWindow.entities.YslugiVZakazah_.ToList().Count; i++)
            //{
            //    list.Add(new Class2(select[i].IDZakaz, select[i].VapolnenieZakaza, select[i].IDPolzovatel, select[i].IDStatus, select[i].DateSozdania));
            //    for (int j = 0; j < Laborant.listpolz.Count; j++)
            //    {
            //        //list[i].IDPolzovatel;
            //    }
            //}
            //Chet_ chet_ = new Chet_
            //{
            //    IDBug = idus,
            //    Sum = DobavitYslugu.kesh,
            //    dateOt = DatePikOt.DisplayDate,
            //    dateDo = DatePikDo.DisplayDate,
            //};
            //MainWindow.entities.Chet_.Add(chet_);
            //MainWindow.entities.SaveChanges();
            //MessageBox.Show("Счёт выставлен!");

            var selectedOrders = Result.ItemsSource as List<Zakaz_>;
            if (selectedOrders == null || !selectedOrders.Any())
            {
                MessageBox.Show("Нет заказов для обработки.");
                return;
            }
            double vsSum = 0;
            foreach (var order in selectedOrders)
            {
                var user = MainWindow.entities.Polzovatel_.FirstOrDefault(p => p.IDPolzovatel == order.IDPolzovatel);
                if (user != null && user.IDStrahovaiKompania.HasValue)
                {
                    var services = MainWindow.entities.YslugiVZakazah_.Where(y => y.IDZakaz == order.IDZakaz).ToList();
                    foreach (var service in services)
                    {
                        vsSum += service.Yslugi_.Price;
                    }
                }
            }
            // vsSum += DobavitYslugu.kesh;
            Chet_ chet_ = new Chet_
            {
                IDBug = idus,
                Sum = vsSum,
                dateOt = DatePikOt.DisplayDate,
                dateDo = DatePikDo.DisplayDate,
                IDStrahKomp = listStrah.FirstOrDefault(z => z.name == SortirovkaStrahcomp.Text).IDStrahovaiKompania,
            };

            MainWindow.entities.Chet_.Add(chet_);
            MainWindow.entities.SaveChanges();
            MessageBox.Show("Счёт выставлен!");
        }
    }
}
