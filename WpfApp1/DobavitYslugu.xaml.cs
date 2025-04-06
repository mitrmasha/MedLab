using Aspose.BarCode.Generation;
using iTextSharp.text.pdf;
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
using iTextSharp.text;
using Path = System.IO.Path;
using System.IO;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace WpfApp1
{
    /// <summary>
    /// chBarcode - переменная для штри-кода
    /// list - лист класса
    /// </summary>
    public partial class DobavitYslugu : Window
    {
        /// <summary>
        /// selektdaychen - лист класса для рассчёта дней и цены
        /// selecthen - переменная для счёта цены
        /// select - переменная для счёта дней
        /// </summary>
        public int chBarcode = 0;
        List<Yslugi_> selectedServices = MainWindow.entities.Yslugi_.ToList();
        public static List<Class1> selektdaychen = new List<Class1>();
        public static double selecthen = 0;
        public static int select = 0;
        /// <summary>
        /// list - лист класса
        /// TextShtrih - переменная для штри-кода
        /// kesh - переменная для хранения цены
        /// </summary>
        public List<Class1> list = new List<Class1>();
        public static List<Zakaz_> listzak = MainWindow.entities.Zakaz_.ToList();
        public static List<YslugiVZakazah_> listys = MainWindow.entities.YslugiVZakazah_.ToList();
        /// <summary>
        /// /// listzak - лист заказа
        /// listys - лист услуги в заказе
        /// </summary>
        public static string TextShtrih;
        public static int kesh;

        /// <summary>
        /// DobavitYslugu - окно для добавления услуги
        /// </summary>
        public DobavitYslugu()
        {
            InitializeComponent();

            for (int i = 0; i < MainWindow.entities.Yslugi_.ToList().Count; i++)
            {

                list.Add(new Class1(selectedServices[i].IDYslugi,
                    selectedServices[i].Service, selectedServices[i].Price,
                    selectedServices[i].SrokVapol,
                    selectedServices[i].Ot, selectedServices[i].Do,
                    false));
                for (int j = 0; j < Laborant.listyslugzak.Count; j++)
                {
                    if (list[i].IDYslugi == Laborant.listyslugzak[j].IDYslugi)
                    {
                        list[i].qs = true;
                    }
                }
            }
            Result.ItemsSource = list;
            SetOrderData();
        }

        private void ShtrihCod_Click(object sender, RoutedEventArgs e)
        {
            if (NumberShtrih.Text == "")
            {
                Hyperlink hyperlink = (Hyperlink)sender;
                Class1 yslugi_ = hyperlink.DataContext as Class1;

                if (yslugi_ != null)
                {
                    var selectedService = Result.SelectedItem as Class1; 

                    if (selectedService == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите услугу из списка.");
                        return; 
                    }
                    int numberZakaz;
                    int.TryParse(NumberZakaz.Text, out numberZakaz);
                    // Формирование числа для штрих-кода
                    Random r = new Random();
                    int numberStrihCod = r.Next(100000, 1000000);
                    string textShtrih = numberStrihCod.ToString();
                    NumberShtrih.Text = textShtrih;
                    var yslugiVZakazah = MainWindow.entities.YslugiVZakazah_.FirstOrDefault(z => z.IDYslugi == selectedService.IDYslugi && z.IDZakaz == numberZakaz);
                    if (yslugiVZakazah != null)
                    {
                        yslugiVZakazah.ShtihKod = textShtrih; 
                    }
                    else
                    {
                        MessageBox.Show("Услуга не найдена в заказе.");
                        return;
                    }

                    // Формирование штрих-кода
                    MessageBox.Show("Штрих-код формируется");
                    var imageType = "Jpeg";
                    var imageFormat = (BarCodeImageFormat)Enum.Parse(typeof(BarCodeImageFormat), imageType);
                    var encodeType = EncodeTypes.Code128;
                    string imagePath = "Code128" + chBarcode.ToString() + "." + imageType;

                    var ti = textShtrih + Date.SelectedDate;
                    BarcodeGenerator generator = new BarcodeGenerator(encodeType, ti);
                    generator.Save(imagePath, imageFormat);
                    generator.Save("1" + imagePath, imageFormat);
                    GenBarcode.Source = new BitmapImage(new Uri(Path.GetFullPath("1" + imagePath)));
                    generator.Dispose();
                    chBarcode++;
                    // Сохранение в PDF
                    var document = new iTextSharp.text.Document();
                    using (var writer = PdfWriter.GetInstance(document, new FileStream("result.pdf", FileMode.Create)))
                    {
                        document.Open();

                        // Используем using для управления потоком
                        using (var imageStream = new FileStream(Environment.CurrentDirectory + @"\1" + imagePath, FileMode.Open, FileAccess.Read))
                        {
                            var logo = iTextSharp.text.Image.GetInstance(imageStream);
                            logo.SetAbsolutePosition(0, 680);
                            writer.DirectContent.AddImage(logo);
                        } // Поток imageStream будет закрыт здесь

                        document.Close();
                    }
                    MainWindow.entities.SaveChanges();
                    MessageBox.Show("Штрих-код сохранен и PDF-документ создан.");
                }
            }
        }

        /// <summary>
        /// SetOrderData - метод для получения данных
        /// </summary>
        public void SetOrderData()
        {
            NumberZakaz.Text = Laborant.idz.ToString();
            NameName.Text = Laborant.nm;
            familiName.Text = Laborant.fm;
            Date.SelectedDate = Laborant.dt;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int numberZakaz;
            int.TryParse(NumberZakaz.Text, out numberZakaz);
            var sqw = MainWindow.entities.Zakaz_.FirstOrDefault(z => z.IDZakaz == numberZakaz);
            int vrem;
            int.TryParse(VremaVapol.Text, out vrem);
            sqw.VapolnenieZakaza = vrem;
            MainWindow.entities.SaveChanges();
            Laborant laborant = new Laborant();
            this.Hide();
            int.TryParse(Chena.Text, out kesh);
        }

        private void Ysluga_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Class1 sS = checkBox.DataContext as Class1;

            if (sS != null && !selektdaychen.Contains(sS))
            {
                selektdaychen.Add(sS);
            }
            selecthen = selektdaychen.Sum(ser => ser.Price);
            select = selektdaychen.Sum(ser => ser.SrokVapol);
            Chena.Text = selecthen.ToString();
            VremaVapol.Text = select.ToString();

            if (e.Source != null)
            {
                CheckBox check = ((CheckBox)(sender));
                Class1 yslugi_ = ((CheckBox)(sender)).DataContext as Class1;
                if (yslugi_ != null)
                {
                    if (check.IsChecked == true)
                    {
                        int numberZakaz;
                        if (int.TryParse(NumberZakaz.Text, out numberZakaz))
                        {
                            // Проверяем, существует ли уже такая услуга для данного заказа
                            var existingService = MainWindow.entities.YslugiVZakazah_.FirstOrDefault(z => z.IDZakaz == numberZakaz && z.IDYslugi == yslugi_.IDYslugi);
                            if (NumberShtrih == null)
                            {
                                MessageBox.Show("Сформируйте штрих-код!");
                            }
                            else
                            {
                                if (existingService == null)
                                {
                                    YslugiVZakazah_ yslugiVZakazah_ = new YslugiVZakazah_
                                    {
                                        IDZakaz = numberZakaz,
                                        IDYslugi = yslugi_.IDYslugi,
                                        IDStatus = 3,
                                        ShtihKod = NumberShtrih.Text,
                                    };
                                    MainWindow.entities.YslugiVZakazah_.Add(yslugiVZakazah_);
                                    MainWindow.entities.SaveChanges();
                                    MessageBox.Show("Услуги добавлены!");
                                }
                            }

                        }
                    }
                }
            }
        }

        private void Ysluga_Indeterminate(object sender, RoutedEventArgs e)
        {
        }

        private void Ysluga_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            Class1 sS = checkBox.DataContext as Class1;

            if (sS != null && selektdaychen.Contains(sS))
            {
                selektdaychen.Remove(sS);
            }
            selecthen = selektdaychen.Sum(ser => ser.Price);
            select = selektdaychen.Sum(ser => ser.SrokVapol);
            Chena.Text = selecthen.ToString();
            VremaVapol.Text = select.ToString();

            if (e.Source != null)
            {
                CheckBox check = ((CheckBox)(sender));
                Class1 yslugi_ = ((CheckBox)(sender)).DataContext as Class1;
                if (yslugi_ != null)
                {
                    if (check.IsChecked == false)
                    {
                        int numberZakaz;
                        if (int.TryParse(NumberZakaz.Text, out numberZakaz))
                        {
                            var zs = MainWindow.entities.YslugiVZakazah_.FirstOrDefault(z => z.IDZakaz == numberZakaz && z.IDYslugi == yslugi_.IDYslugi);
                            if (zs != null)
                            {
                                MainWindow.entities.YslugiVZakazah_.Remove(zs);
                                MainWindow.entities.SaveChanges();
                                MessageBox.Show("Услуги удалены!");
                            }
                        }
                    }
                }
            }
        }

        private void NumberShtrih_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Устанавливаем фокус на DataGrid
                Result.Focus();
                // Вызываем обработчик для DataGrid
                Result_KeyDown(sender, e);
            }
        }

        private void Result_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                if (NumberShtrih.Text.Length == 6)
                {
                    var selectedService = Result.SelectedItem as Class1;

                    if (selectedService == null)
                    {
                        MessageBox.Show("Пожалуйста, выберите услугу из списка.");
                        return;
                    }

                    int numberZakaz;
                    if (!int.TryParse(NumberZakaz.Text, out numberZakaz))
                    {
                        MessageBox.Show("Неверный номер заказа.");
                        return;
                    }

                    // Формирование числа для штрих-кода
                   string textShtrih = NumberShtrih.Text;

                    // Получаем соответствующий объект YslugiVZakazah_
                    var yslugiVZakazah = MainWindow.entities.YslugiVZakazah_.FirstOrDefault(z => z.IDYslugi == selectedService.IDYslugi && z.IDZakaz == numberZakaz);
                    if (yslugiVZakazah != null)
                    {
                        yslugiVZakazah.ShtihKod = textShtrih;
                    }
                    else
                    {
                        MessageBox.Show("Услуга не найдена в заказе.");
                        return;
                    }

                    // Формирование штрих-кода
                    MessageBox.Show("Штрих-код формируется");
                    var imageType = "Jpeg";
                    var imageFormat = (BarCodeImageFormat)Enum.Parse(typeof(BarCodeImageFormat), imageType);
                    var encodeType = EncodeTypes.Code128;
                    string imagePath = "Code128" + chBarcode.ToString() + "." + imageType;

                    var ti = textShtrih + Date.SelectedDate; 
                    BarcodeGenerator generator = new BarcodeGenerator(encodeType, ti);
                    generator.Save(imagePath, imageFormat);
                    generator.Save("1" + imagePath, imageFormat);
                    GenBarcode.Source = new BitmapImage(new Uri(Path.GetFullPath("1" + imagePath)));
                    generator.Dispose();
                    chBarcode++;

                    // Сохранение в PDF
                    var document = new iTextSharp.text.Document();
                    using (var writer = PdfWriter.GetInstance(document, new FileStream("result.pdf", FileMode.Create)))
                    {
                        document.Open();

                        // Используем using для управления потоком
                        using (var imageStream = new FileStream(Environment.CurrentDirectory + @"\1" + imagePath, FileMode.Open, FileAccess.Read))
                        {
                            var logo = iTextSharp.text.Image.GetInstance(imageStream);
                            logo.SetAbsolutePosition(0, 680);
                            writer.DirectContent.AddImage(logo);
                        } // Поток imageStream будет закрыт здесь

                        document.Close();
                    }

                    // Сохранение изменений в базе данных
                    MainWindow.entities.SaveChanges();
                    MessageBox.Show("Штрих-код сохранен и PDF-документ создан.");
                }
                else
                {
                    MessageBox.Show("В коде должно быть 6 знаков!");
                }
            }
        }

        private void Result_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedService = Result.SelectedItem as YslugiVZakazah_;
            if (selectedService != null)
            {
                // Элемент выбран
                MessageBox.Show("Вы выбрали услугу: " + selectedService.IDYslugi);
            }
        }
    }
}
