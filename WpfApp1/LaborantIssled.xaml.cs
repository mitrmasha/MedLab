using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
//using MessageBox = System.Windows.Forms.MessageBox;
//using MessageBoxOptions = System.Windows.Forms.MessageBoxOptions;
using MessageBox = System.Windows.MessageBox;
using System.Data.Entity.Migrations;


namespace WpfApp1
{
    /// <summary>
    /// timer - переменная для таймера
    /// tipi - переменная для анализатора
    /// </summary>
    public partial class LaborantIssled : Window
    {
        public static DispatcherTimer timer;
        public static bool isWarningShown = false;
        public static TimeSpan Timedspan = new TimeSpan(2, 30, 0);
        public static string IpAdress;
        public static List<YslugiVZakazah_> listyslugzak = MainWindow.entities.YslugiVZakazah_.ToList();
        public static List<Analizator_> listAnaliz = MainWindow.entities.Analizator_.ToList();
        public static List<Yslugi_> listYslugi = MainWindow.entities.Yslugi_.ToList();
        public static string tipi;
        public static TimeSpan timeRemaining;
        public static int NumberDanAnaliz;
        public static int idYslugi;
        public static int qwer;

        /// <summary>
        /// IdLabPol - код лаборанта
        /// </summary>
        public static int IdLabPol;
        private DateTime dateTimeStartAnalyzer;
        public LaborantIssled()
        {
            InitializeComponent();
            InitializeTimer();

            string put = Environment.CurrentDirectory;
            Img.Source = new BitmapImage(new Uri(put + "/Sourse/laborant_2.jpeg"));
            NameName.Content = MainWindow.namef;
            familiName.Content = MainWindow.fam;
            RolPolzov.Content = MainWindow.rol;
            IdLabPol = MainWindow.IDUser;

            SortirovkaAnaliz.ItemsSource = listAnaliz;

            Result.ItemsSource = MainWindow.entities.YslugiVZakazah_.Where(a => a.IDStatus != 2).ToList();
            var DanAnaliz = MainWindow.entities.DanneAnalizatora_.OrderByDescending(z => z.IDDanni).FirstOrDefault();
            int NumberDanAnaliz = DanAnaliz.IDDanni + 1;
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
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
            else if (Timedspan.TotalMinutes <= 15 && Timedspan.TotalSeconds > 0 && !isWarningShown)
            {
                MessageBox.Show("Осталось 15 минут!");
                isWarningShown = true;
            }
            Vrema.Content = Timedspan.ToString(@"hh\:mm\:ss");
        }
        private void Timer_Tick1(object sender, EventArgs e)
        {
            timeRemaining = timeRemaining.Add(TimeSpan.FromSeconds(-1));
            if (timeRemaining.TotalSeconds <= 0)
            {
                timer.Stop(); 
                Loading.Visibility = Visibility.Collapsed;
                Soh.Visibility = Visibility.Visible;
                //  var rez= MessageBox.Show(
                //"Принять данные?", "Данные",
                //MessageBoxButton.YesNo);

                //   if (rez == MessageBoxResult.Yes)
                //   {
                //       GetAnalizator getAnalizator = new GetAnalizator();
                //       int poldan = getAnalizator.progress;

                //       var rezDan = MessageBox.Show(
                //"Одобрить результат?", "Данные",
                //MessageBoxButton.YesNo);

                //       DateTime dateTime = DateTime.Now;
                //       DanneAnalizatora_ danneAnalizatora_ = new DanneAnalizatora_
                //       {
                //           IDDanni = NumberDanAnaliz,
                //           IDAnalizator = qwer,
                //           DateVremaPostupYslugi = DateTime.Now,
                //           IDLaboranta = IdLabPol,
                //           IDYslugaVZakaze = idYslugi,
                //          // VremaVapol = dateTimeStartAnalyzer - dateTime,
                //       };

            }
        }

        private void Vhod_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide();
        }

        private  void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            dateTimeStartAnalyzer = DateTime.Now;
            if (Result != null)
            {
                try
                {
                    var selectedService = Result.SelectedItem as YslugiVZakazah_;
                    if (selectedService != null)
                    {
                        //Отправление данных в анализатор
                        Hyperlink hyperlink = ((Hyperlink)(sender));
                        YslugiVZakazah_ yslugiVZakazah_ = ((Hyperlink)(sender)).DataContext as YslugiVZakazah_;
                        idYslugi = yslugiVZakazah_.IDYslugi;

                        Services services1 = new Services();
                        services1.serviceCode = selectedService.IDYslugi;
                        List<Services> services = new List<Services>();
                        services.Add(services1);

                        string patient = selectedService.Zakaz_.IDPolzovatel.ToString();
                        string typ = tipi.ToString();
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/{typ}");
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string json = new JavaScriptSerializer().Serialize(new
                            {
                                patient,
                                services
                            });
                            streamWriter.Write(json);
                        }
                        HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                            MessageBox.Show("Услуги успешно отправлены!");
                        else
                            MessageBox.Show("Ошибка отправки!");

                        //Loadin
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += Timer_Tick1;
                        Loading.Visibility = Visibility.Visible;
                        timeRemaining = TimeSpan.FromSeconds(15);
                        timer.Start();
                    }
                }

                //        var result = MessageBox.Show("Отправить данные", "", MessageBoxButton.OKCancel);
                //        switch (result)
                //        {
                //            case MessageBoxResult.None:
                //                break;
                //            case MessageBoxResult.OK:
                //                //Получение данных с анализатора
                //                GetAnalizator getAnalizators = new GetAnalizator();
                //                httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/{typ}");
                //                httpWebRequest.ContentType = "application/json";
                //                httpWebRequest.Method = "GET";
                //                try
                //                {
                //                     httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                //                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                //                    {
                //                        using (Stream stream = httpResponse.GetResponseStream())
                //                        {
                //                            StreamReader reader = new StreamReader(stream);
                //                            string json = reader.ReadToEnd();
                //                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                //                            getAnalizators = serializer.Deserialize<GetAnalizator>(json);
                //                            if (getAnalizators.patient == null)
                //                            {
                //                                MessageBox.Show("Результаты не отправлены!.\n Возможно, пк сильно нагружен.", "Анализатор", MessageBoxButton.OK, MessageBoxImage.Error);
                //                                return;
                //                            }
                //                        }
                //                        var resultService = getAnalizators.services.First();
                //                        var StrInitilAVG = MainWindow.entities.Yslugi_.Where(x => x.IDYslugi == resultService.serviceCode).FirstOrDefault().InitialAVGDeviation;
                //                        var StrFinalAVG = MainWindow.entities.Yslugi_.Where(x => x.IDYslugi == resultService.serviceCode).FirstOrDefault().FinalAVGDeviation;
                //                        double InitilAVG = 0;
                //                        double FinalAVG = 0;
                //                        if (double.TryParse(StrInitilAVG, out InitilAVG) || double.TryParse(StrFinalAVG, out FinalAVG))
                //                        {
                //                            string originalResult = resultService.result;
                //                            string normalizedResult = originalResult.Replace('.', ',');
                //                            if (normalizedResult.Contains(','))
                //                            {
                //                                int decimalIndex = normalizedResult.IndexOf(',');
                //                                if (normalizedResult.Length > decimalIndex + 7)
                //                                {
                //                                    normalizedResult = normalizedResult.Substring(0, decimalIndex + 7);
                //                                }
                //                            }
                //                            if (double.TryParse(normalizedResult, out double Result))
                //                            {
                //                                if (Result > (FinalAVG * 5) || Result < (InitilAVG / 5))
                //                                {
                //                                    var resultQu = MessageBox.Show($"Значения, полученные с анализатора, отклоняются от среднего в 5 раз." +
                //                                        $"\nВозможен сбой исследователя или некачественный биоматериал", "Анализатор", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                //                                    switch (resultQu)
                //                                    {
                //                                        case MessageBoxResult.Yes:
                //                                            HelperForSaveResult(3, selectedService, resultService);
                //                                            break;
                //                                        case MessageBoxResult.No:
                //                                            HelperForSaveResult(7, selectedService, resultService);
                //                                            break;
                //                                        default:
                //                                            break;
                //                                    }
                //                                }
                //                                else
                //                                {
                //                                    HelperForSaveResult(3, selectedService, resultService);
                //                                }
                //                            }
                //                            else
                //                            {
                //                                HelperForSaveResult(3, selectedService, resultService);
                //                            }
                //                        }
                //                        else
                //                        {
                //                            HelperForSaveResult(3, selectedService, resultService);
                //                        }
                //                    }
                //                }
                //                catch (Exception ex)
                //                {
                //                    MessageBox.Show(ex.ToString());

                //                }
                //                break;
                //            case MessageBoxResult.Cancel:
                //                break;
                //            case MessageBoxResult.Yes:
                //                break;
                //            case MessageBoxResult.No:
                //                break;
                //            default:
                //                break;
                //        }                       
                //    }
                //}
                //catch (InvalidCastException)
                //{
                //    MessageBox.Show("Ошибка кода!");
                //    return;
                //}
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

            }
        }

        private void SortirovkaAnaliz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int select = SortirovkaAnaliz.SelectedIndex;

               qwer = listAnaliz[select].IDAnalizator;
               tipi = listAnaliz[select].Analizator;

               listyslugzak = MainWindow.entities.YslugiVZakazah_.Where(a => (a.Yslugi_.metka == qwer || a.Yslugi_.metka == 3) && a.IDStatus != 2).ToList();
               Result.ItemsSource = listyslugzak;          
        }

        public void HelperForSaveResult(int status, YslugiVZakazah_ selectedService, Services resultService)
        {
            //selectedService.IDStatus = status;
            //selectedService.Result = resultService.result;
            //MainWindow.entities.YslugiVZakazah_.AddOrUpdate(selectedService);
            //MainWindow.entities.SaveChanges();
            TimeSpan leadTime = DateTime.Now - dateTimeStartAnalyzer;
            string VremaVapol = leadTime.ToString();
            //DanneAnalizatora_ newAnalyzerData = new DanneAnalizatora_()
            //{
            //    //IDDanni = NumberDanAnaliz,
            //    IDAnalizator = qwer,
            //    DateVremaPostupYslugi = DateTime.Now,
            //    IDLaboranta = IdLabPol,
            //    IDYslugaVZakaze = idYslugi,
            //    VremaVapol = VremaVapol,
            //};
            //MainWindow.entities.DanneAnalizatora_.Add(newAnalyzerData);
            //MainWindow.entities.SaveChanges();
            var listServicesInOrder = MainWindow.entities.YslugiVZakazah_.Where(x => x.IDZakaz == selectedService.IDZakaz).ToList();
            bool allServicesCompleted = listServicesInOrder.All(x => x.IDStatus == 1);
            if (allServicesCompleted)
            {
                MessageBox.Show("Все услуги в заказе выполнены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                var order = MainWindow.entities.Zakaz_.Where(x => x.IDZakaz == selectedService.IDZakaz).FirstOrDefault();
                if (order != null)
                {
                    order.IDStatus = 2;
                    MainWindow.entities.Zakaz_.AddOrUpdate(order);
                    MainWindow.entities.SaveChanges();
                }
            }
        }

        public void Helper(int status, YslugiVZakazah_ selectedService, Services resultService)
        {
            var zx = MainWindow.entities.YslugiVZakazah_.FirstOrDefault(z=> z.IDZakaz == selectedService.IDZakaz && z.IDYslugi == selectedService.IDYslugi);
            int zk = zx.IDYslugaVZakaze;
            zx.IDStatus = status;
            zx.Result = resultService.result;
            MainWindow.entities.SaveChanges();
            TimeSpan leadTime = DateTime.Now - dateTimeStartAnalyzer;
            string VremaVapol = leadTime.ToString();
            DanneAnalizatora_ newAnalyzerData = new DanneAnalizatora_()
            {
                //IDDanni = NumberDanAnaliz,
                IDAnalizator = qwer,
                DateVremaPostupYslugi = DateTime.Now,
                IDLaboranta = IdLabPol,
                IDYslugaVZakaze = zk,
                VremaVapol = VremaVapol,
            };
            MainWindow.entities.DanneAnalizatora_.Add(newAnalyzerData);
            MainWindow.entities.SaveChanges();
            var listServicesInOrder = MainWindow.entities.YslugiVZakazah_.Where(x => x.IDZakaz == selectedService.IDZakaz).ToList();
            bool allServicesCompleted = listServicesInOrder.All(x => x.IDStatus == 1);
            if (allServicesCompleted)
            {
                MessageBox.Show("Все услуги в заказе выполнены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                var order = MainWindow.entities.Zakaz_.Where(x => x.IDZakaz == selectedService.IDZakaz).FirstOrDefault();
                if (order != null)
                {
                    order.IDStatus = 2;
                    MainWindow.entities.Zakaz_.AddOrUpdate(order);
                    MainWindow.entities.SaveChanges();
                }
            }
        }

        private void Soh_Click(object sender, RoutedEventArgs e)
        {
            dateTimeStartAnalyzer = DateTime.Now;
            if (Result != null)
            {
                try
                {
                    var selectedService = Result.SelectedItem as YslugiVZakazah_;
                    string typ = tipi.ToString();
                    var result = MessageBox.Show("Получить данные", "", MessageBoxButton.OKCancel);
                        switch (result)
                        {
                            case MessageBoxResult.None:
                                break;
                            case MessageBoxResult.OK:
                                //Получение данных с анализатора
                                GetAnalizator getAnalizators = new GetAnalizator();
                              var  httpWebRequest = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/{typ}");
                                httpWebRequest.ContentType = "application/json";
                                httpWebRequest.Method = "GET";
                                try
                                {
                                  var  httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                                    {
                                        using (Stream stream = httpResponse.GetResponseStream())
                                        {
                                            StreamReader reader = new StreamReader(stream);
                                            string json = reader.ReadToEnd();
                                            JavaScriptSerializer serializer = new JavaScriptSerializer();
                                            getAnalizators = serializer.Deserialize<GetAnalizator>(json);
                                            if (getAnalizators.patient == null)
                                            {
                                                MessageBox.Show("Результаты не отправлены!.\n Возможно, пк сильно нагружен.", "Анализатор", MessageBoxButton.OK, MessageBoxImage.Error);
                                                return;
                                            }
                                        }
                                        var resultService = getAnalizators.services.First();
                                    //if (double.TryParse(resultService.result, out double Result))
                                    //{

                                    //}
                                    var StrInitilAVG = MainWindow.entities.Yslugi_.Where(x => x.IDYslugi == resultService.serviceCode).FirstOrDefault().Ot;
                                    var StrFinalAVG = MainWindow.entities.Yslugi_.Where(x => x.IDYslugi == resultService.serviceCode).FirstOrDefault().Do;                                   
                                     string originalResult = resultService.result;
                                    string normalizedResult = originalResult.Replace('.', ',');
                                    if (normalizedResult.Contains(','))
                                    {
                                        int decimalIndex = normalizedResult.IndexOf(',');
                                        if (normalizedResult.Length > decimalIndex + 7)
                                        {
                                            normalizedResult = normalizedResult.Substring(0, decimalIndex + 7);
                                        }
                                    }
                                    if (double.TryParse(normalizedResult, out double Result))
                                    { 
                                                if (Result > (StrFinalAVG * 5) || Result < (StrInitilAVG / 5))
                                                {
                                                    var resultQu = MessageBox.Show($"Значения, полученные с анализатора, отклоняются от среднего в 5 раз." +
                                                        $"\nВозможен сбой исследователя или некачественный биоматериал,Принять/отклонить ", "Анализатор", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                                    switch (resultQu)
                                                    {
                                                        case MessageBoxResult.Yes:
                                                    Helper(3, selectedService, resultService);
                                                            break;
                                                        case MessageBoxResult.No:
                                                    MessageBox.Show("Данные не записаны");
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                                else
                                                {
                                            Helper(3, selectedService, resultService);
                                            MessageBox.Show("Данные записываются");
                                        }
                                            }
                                            else
                                            {
                                        Helper(3, selectedService, resultService);
                                        MessageBox.Show("Данные записываются");
                                    }
                                    }
                                //else
                                //{
                                //    HelperForSaveResult(3, selectedService, resultService);
                                //}

                            }
                            catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());

                                }
                                break;
                            case MessageBoxResult.Cancel:
                                break;
                            case MessageBoxResult.Yes:
                                break;
                            case MessageBoxResult.No:
                                break;
                            default:
                                break;
                        }
                    }
                
                catch (InvalidCastException)
                {
                    MessageBox.Show("Ошибка кода!");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
        }
    }   
}
