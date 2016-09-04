using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using System.Globalization;
using System.Data;
using maintenance.classes;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace maintenance
{
    /// <summary>

    /// </summary>

    public partial class HardwareStatus : Window
    {

        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string UploadDir = ini.IniReadValue("appSettings", "UploadDir");
        private Thread Thread;
        private int UpdateHardWare;
        private bool ConnectDataBase = true;
        public HardwareStatus()
        {
            InitializeComponent();
            InitializeComponent();


        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Plans select c;
                if(query.Count()>=1)
                  UpdateHardWare = int.Parse(query.Single().DeviceThreadUpdate.ToString());
               else
                UpdateHardWare = 10;
                    
                

                Thread = new Thread(Load_HardWareStatus);
                Thread.IsBackground = true;
                Thread.Start();

            }
            else
                MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");


        }


        private void Load_HardWareStatus()
        {
            while (true)
            {

                this.Dispatcher.Invoke(new Action(() =>
                {

                    ChangeStatus();


                }), null);

                Thread.Sleep(UpdateHardWare * 1000);
            }
        }



        private void ChangeStatus() //تابع نمایش وضعیت سخت افزار ها
        {
            try
            {
                var db = new DataClasses1DataContext(cnn);


                GridMain.Children.Clear();

                TextBlock Header_DeviceCode = new TextBlock(); //عنوان کد دستگاه
                Header_DeviceCode.Text = "کد سخت افزار";
                Header_DeviceCode.TextAlignment = TextAlignment.Center;
                Header_DeviceCode.FontSize = 20;
                Header_DeviceCode.Width = 257;
                Header_DeviceCode.Height = 30;
                Header_DeviceCode.Background = new SolidColorBrush(Colors.Silver);
                Header_DeviceCode.FontFamily = new FontFamily("B nazanin");
                Grid.SetColumn(Header_DeviceCode, 6);
                Grid.SetRow(Header_DeviceCode, 0);
                GridMain.Children.Add(Header_DeviceCode);


                TextBlock Header_DeviceName = new TextBlock(); //عنوان نام دستگاه
                Header_DeviceName.Text = "نام دستگاه";
                Header_DeviceName.TextAlignment = TextAlignment.Center;
                Header_DeviceName.FontSize = 20;
                Header_DeviceName.Width = 257;
                Header_DeviceName.Height = 30;
                Header_DeviceName.Background = new SolidColorBrush(Colors.Silver);
                Header_DeviceName.FontFamily = new FontFamily("B nazanin");
                Grid.SetColumn(Header_DeviceName, 5);
                Grid.SetRow(Header_DeviceName, 0);
                GridMain.Children.Add(Header_DeviceName);


                TextBlock Header_StatusConnect = new TextBlock(); //عنوان وضعیت اتصال
                Header_StatusConnect.Text = "وضعیت اتصال";
                Header_StatusConnect.TextAlignment = TextAlignment.Center;
                Header_StatusConnect.FontSize = 20;
                Header_StatusConnect.Width = 257;
                Header_StatusConnect.Height = 30;
                Header_StatusConnect.Background = new SolidColorBrush(Colors.Silver);
                Header_StatusConnect.FontFamily = new FontFamily("B nazanin");
                Grid.SetColumn(Header_StatusConnect, 4);
                Grid.SetRow(Header_StatusConnect, 0);

                GridMain.Children.Add(Header_StatusConnect);




                TextBlock Header_NetworkStatus = new TextBlock(); //عنوان وضعیت آنتن دهی
                Header_NetworkStatus.Text = "وضعیت آنتن دهی";
                Header_NetworkStatus.TextAlignment = TextAlignment.Center;
                Header_NetworkStatus.FontSize = 20;
                Header_NetworkStatus.Width = 257;
                Header_NetworkStatus.Height = 30;
                Header_NetworkStatus.FontFamily = new FontFamily("B nazanin");
                Header_NetworkStatus.Background = new SolidColorBrush(Colors.Silver);

                Grid.SetColumn(Header_NetworkStatus, 3);
                Grid.SetRow(Header_NetworkStatus, 0);

                GridMain.Children.Add(Header_NetworkStatus);

                TextBlock Header_Ignition = new TextBlock(); // عنوان روشن یا خاموش بودن سخت افزار
                Header_Ignition.Text = "روشن/خاموش";
                Header_Ignition.TextAlignment = TextAlignment.Center;
                Header_Ignition.FontSize = 20;
                Header_Ignition.Width = 257;
                Header_Ignition.Height = 30;
                Header_Ignition.FontFamily = new FontFamily("B nazanin");
                Header_Ignition.Background = new SolidColorBrush(Colors.Silver);

                Grid.SetColumn(Header_Ignition, 2);
                Grid.SetRow(Header_Ignition, 0);
                GridMain.Children.Add(Header_Ignition);



                //TextBlock Header_BatteryCharge = new TextBlock(); // عنوان شارژ باطری
                //Header_BatteryCharge.Text = "شارژ باطری";
                //Header_BatteryCharge.TextAlignment = TextAlignment.Center;
                //Header_BatteryCharge.FontSize = 20;
                //Header_BatteryCharge.Width = 257;
                //Header_BatteryCharge.Height = 30;
                //Header_BatteryCharge.Background = new SolidColorBrush(Colors.Silver);
                //Header_BatteryCharge.FontFamily = new FontFamily("B nazanin");
                //Grid.SetColumn(Header_BatteryCharge, 3);
                //Grid.SetRow(Header_BatteryCharge, 0);

                //GridMain.Children.Add(Header_BatteryCharge);



                TextBlock BBV = new TextBlock(); // دمای دستگاه
                BBV.Text = "دمای دستگاه ";
                BBV.TextAlignment = TextAlignment.Center;
                BBV.FontSize = 20;
                BBV.Width = 257;
                BBV.Height = 30;
                BBV.FontFamily = new FontFamily("B nazanin");
                BBV.Background = new SolidColorBrush(Colors.Silver);
                Grid.SetColumn(BBV, 1);
                Grid.SetRow(BBV, 0);
                GridMain.Children.Add(BBV);


                //TextBlock VBV = new TextBlock(); // عنوان ولتاژ باطری دستگاه
                //VBV.Text = "ولتاژ باطری دستگاه";
                //VBV.TextAlignment = TextAlignment.Center;
                //VBV.FontSize = 20;
                //VBV.Width = 257;
                //VBV.Height = 30;
                //VBV.FontFamily = new FontFamily("B nazanin");
                //VBV.Background = new SolidColorBrush(Colors.Silver);
                //Grid.SetColumn(VBV, 1);
                //Grid.SetRow(VBV, 0);
                //GridMain.Children.Add(VBV);

                TextBlock Header_HardWareStatus = new TextBlock(); // عنوان وضعیت سخت افزار
                Header_HardWareStatus.Text = "وضعیت سخت افزار";
                Header_HardWareStatus.TextAlignment = TextAlignment.Center;
                Header_HardWareStatus.FontSize = 20;
                Header_HardWareStatus.Width = 257;
                Header_HardWareStatus.Height = 30;
                Header_HardWareStatus.FontFamily = new FontFamily("B nazanin");
                Header_HardWareStatus.Background = new SolidColorBrush(Colors.Silver);

                Grid.SetColumn(Header_HardWareStatus, 0);
                Grid.SetRow(Header_HardWareStatus, 0);

                GridMain.Children.Add(Header_HardWareStatus);

                var query = from c in db.InputDataXes
                            join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID into a
                            from d in a.DefaultIfEmpty()
                            join e in db.VehicleLastStates on c.DeviceID equals e.DeviceID 
                            orderby d.Name descending
                            select
                                new
                                {
                                    DeviceID = c.DeviceID,
                                    DeviceName = d.Name != null ? d.Name : "بدون ماشین",
                                    Ignition = c.Ignition,
                                    LastDate = c.Date,
                                    LastTime = c.Time,
                                    BBV = c.DTM,
                                    VBV = c.VBV,
                                    NetworkType = c.NetworkType,
                                    NetworkStatus = c.NetworkStatus,
                                    Status = c.Status,
                                    BAT = c.BAT,
                                    ConnectionState = e.ConnectionState,
                                    Valid = c.Valid
                                };

                int Row = 1;
                foreach (var List in query)
                {

                    RowDefinition GridGow1 = new RowDefinition();
                    GridGow1.Height = new GridLength();
                    GridGow1.MaxHeight = 100;
                    GridMain.RowDefinitions.Add(GridGow1);
                    string IsProblem = "OK";
                    string[] Problem = new string[] { };

                    if (List.Status != null)
                    {
                        Problem = List.Status.Split(':');

                        Problem[0] = Problem[0].ToUpper();

                        if (Problem[0] == "NO")
                        {
                            Border border = new Border();
                            border.Background = new SolidColorBrush(Colors.LightYellow);
                            border.Margin = new Thickness(10, 0, 10, 2);
                            Grid.SetRow(border, Row);
                            Grid.SetColumn(border, 0);
                            Grid.SetColumnSpan(border, 9);
                            GridMain.Children.Add(border);
                            IsProblem = "NO";

                        }
                    }
                    TextBlock DeviceID = new TextBlock();
                    DeviceID.Text = List.DeviceID.ToString(); //نمایش کد دستگاه
                    DeviceID.TextAlignment = TextAlignment.Center;
                    DeviceID.HorizontalAlignment = HorizontalAlignment.Center;
                    DeviceID.VerticalAlignment = VerticalAlignment.Center;
                    DeviceID.FontSize = 20;
                    DeviceID.Width = 257;

                    DeviceID.Foreground = new SolidColorBrush(Colors.Black);
                  //  if (IsProblem == "NO")
                  //      DeviceID.Foreground = new SolidColorBrush(Colors.Snow);

                    DeviceID.FontFamily = new FontFamily("B nazanin");
                    Grid.SetRow(DeviceID, Row);
                    Grid.SetColumn(DeviceID, 6);
                    GridMain.Children.Add(DeviceID);


                    TextBlock DeviceName = new TextBlock();
                    DeviceName.Text = List.DeviceName.ToString(); //نمایش نام دستگاه
                    DeviceName.TextAlignment = TextAlignment.Center;
                    DeviceName.HorizontalAlignment = HorizontalAlignment.Center;
                    DeviceName.VerticalAlignment = VerticalAlignment.Center;
                    DeviceName.FontSize = 20;
                    DeviceName.Width = 257;
                    DeviceName.FontFamily = new FontFamily("B nazanin");

                    DeviceName.Foreground = new SolidColorBrush(Colors.Black);
                 //   if (IsProblem == "NO")
                   //     DeviceName.Foreground = new SolidColorBrush(Colors.Snow);
                    Grid.SetRow(DeviceName, Row);
                    Grid.SetColumn(DeviceName, 5);
                    GridMain.Children.Add(DeviceName);

                    string Connect = "connect.png";

                    if (List.Valid == false)
                        Connect = "disabled.png";

                    else
                    {
                        if (List.ConnectionState == 1)
                            Connect = "connect.png";
                        else if (List.ConnectionState == 2)
                            Connect = "disconnect.png";
                        else if (List.ConnectionState == 3)
                            Connect = "warning_connect.png";

                    }


                    Image Img_Connect = new Image();
                    string PathConnect = UploadDir + "\\DeviceIcon\\" + Connect;
                    if ((!File.Exists(PathConnect)) || Connect == "" || Connect == null)
                        PathConnect = "images/noimage.gif";

                    FileStream stream = new FileStream(PathConnect, FileMode.Open, FileAccess.Read);

                    BitmapImage Src = new BitmapImage();
                    Src.BeginInit();
                    Src.StreamSource = stream;
                    Src.EndInit();
                    Img_Connect.Source = Src;
                    Img_Connect.Width = 30;
                    Img_Connect.Height = 30;
                    Img_Connect.Margin = new Thickness(0, 0, 0, 0);
                    Img_Connect.VerticalAlignment = VerticalAlignment.Center;
                    Img_Connect.HorizontalAlignment = HorizontalAlignment.Center;
                    Grid.SetRow(Img_Connect, Row);
                    Grid.SetColumn(Img_Connect, 4);
                    GridMain.Children.Add(Img_Connect);


                    // وضعیت آنتن دهی
                    string Image_Wifi = "";
                    int Wifi;
                    if (List.NetworkStatus.ToString() == "" || List.NetworkStatus.ToString() == null)
                        Wifi = 0;
                    else
                        Wifi = int.Parse(List.NetworkStatus.ToString());
                    if (Wifi == 0)
                        Image_Wifi = "wifi1.png";
                    else if (Wifi >= 1 && Wifi <= 20)
                        Image_Wifi = "wifi2.png";
                    else if (Wifi > 20 && Wifi <= 40)
                        Image_Wifi = "wifi3.png";
                    else if (Wifi > 40 && Wifi <= 60)
                        Image_Wifi = "wifi4.png";
                    else if (Wifi > 60 && Wifi <= 80)
                        Image_Wifi = "wifi5.png";
                    else if (Wifi > 80 && Wifi <= 100)
                        Image_Wifi = "wifi6.png";


                    string PathImg_Wifi = UploadDir + "\\DeviceIcon\\" + Image_Wifi;
                    if ((!File.Exists(PathImg_Wifi)) || Image_Wifi == "" || Image_Wifi == null)
                        PathImg_Wifi = "images/noimage.gif";



                    FileStream Stream3 = new FileStream(PathImg_Wifi, FileMode.Open, FileAccess.Read);
                    Image Img_WF = new Image();
                    BitmapImage Src_Wf = new BitmapImage();
                    Src_Wf.BeginInit();
                    Src_Wf.StreamSource = Stream3;
                    Src_Wf.EndInit();
                    Img_WF.Source = Src_Wf;
                    Img_WF.Width = 50;
                    Img_WF.Height = 50;
                    Img_WF.Margin = new Thickness(0, 0, 0, 0);
                    Img_WF.VerticalAlignment = VerticalAlignment.Top;
                    Img_WF.HorizontalAlignment = HorizontalAlignment.Center;




                    Grid.SetRow(Img_WF, Row);
                    Grid.SetColumn(Img_WF, 3);
                    GridMain.Children.Add(Img_WF);

                    TextBlock NetworkType = new TextBlock();
                    NetworkType.Text = List.NetworkType; //نمایش نوع شبکه
                    NetworkType.TextAlignment = TextAlignment.Right;
                    NetworkType.VerticalAlignment = VerticalAlignment.Bottom;
                    NetworkType.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    NetworkType.FontSize = 15;
                    NetworkType.FontWeight = FontWeights.Bold;
                    NetworkType.Padding = new Thickness(0, 0, 30, 0);
                    NetworkType.Width = 257;
                    NetworkType.FontFamily = new FontFamily("B nazanin");
                    NetworkType.Margin = new Thickness(20, 0, 0, 0);
                   // if (IsProblem == "NO")
                    //    NetworkType.Foreground = new SolidColorBrush(Colors.Snow);

                    Grid.SetRow(NetworkType, Row);
                    Grid.SetColumn(NetworkType, 3);
                    GridMain.Children.Add(NetworkType);





                    string TextIgnition = "روشن";
                    if (List.Ignition == false)
                        TextIgnition = "خاموش";

                    TextBlock IgnitionStatus = new TextBlock();
                    IgnitionStatus.Text = TextIgnition; //روشن و خاموش بودن دستگاه
                    IgnitionStatus.TextAlignment = TextAlignment.Center;
                    IgnitionStatus.HorizontalAlignment = HorizontalAlignment.Center;
                    IgnitionStatus.VerticalAlignment = VerticalAlignment.Center;
                    IgnitionStatus.FontSize = 20;
                    IgnitionStatus.Width = 257;
                    IgnitionStatus.FontFamily = new FontFamily("B nazanin");

                    IgnitionStatus.Foreground = new SolidColorBrush(Colors.Black);
                 //   if (IsProblem == "NO")
                    //    IgnitionStatus.Foreground = new SolidColorBrush(Colors.Snow);

                    Grid.SetRow(IgnitionStatus, Row);
                    Grid.SetColumn(IgnitionStatus, 2);
                    GridMain.Children.Add(IgnitionStatus);



                    // وضعیت شارژ باطری
                    //string Image_BatteryCharge = "";
                    //;
                    //int Charge;
                    //if (List.BAT.ToString() == "" || List.BAT.ToString() == null)
                    //    Charge = 0;
                    //else
                    //    Charge = int.Parse(List.BAT.ToString());

                    //if (Charge == 0)
                    //    Image_BatteryCharge = "charge1.png";
                    //else if (Charge >= 1 && Charge < 25)
                    //    Image_BatteryCharge = "charge2.png";
                    //else if (Charge >= 25 && Charge < 50)
                    //    Image_BatteryCharge = "charge3.png";
                    //else if (Charge >= 50 && Charge < 75)
                    //    Image_BatteryCharge = "charge4.png";
                    //else if (Charge >= 75)
                    //    Image_BatteryCharge = "charge5.png";

                    //string PathImage_BatteryCharge = UploadDir + "\\DeviceIcon\\" + Image_BatteryCharge;
                    //if ((!File.Exists(PathImage_BatteryCharge)) || PathImage_BatteryCharge == "" ||
                    //    PathImage_BatteryCharge == null)
                    //    PathImage_BatteryCharge = "images/noimage.gif";




                    //FileStream Stream2 = new FileStream(PathImage_BatteryCharge, FileMode.Open, FileAccess.Read);
                    //Image Img_Chareg = new Image();
                    //BitmapImage Src_Charge = new BitmapImage();
                    //Src_Charge.BeginInit();
                    //Src_Charge.StreamSource = Stream2;
                    //Src_Charge.EndInit();
                    //Img_Chareg.Source = Src_Charge;
                    //Img_Chareg.Width = 50;
                    //Img_Chareg.Height = 50;
                    //Img_Chareg.Margin = new Thickness(0, 0, 0, 0);
                    //Img_Chareg.VerticalAlignment = VerticalAlignment.Top;
                    //Img_Chareg.HorizontalAlignment = HorizontalAlignment.Center;

                    //Grid.SetRow(Img_Chareg, Row);
                    //Grid.SetColumn(Img_Chareg, 3);
                    //GridMain.Children.Add(Img_Chareg);

                    TextBlock ShowBBV = new TextBlock(); //ولتاژ باطری سخت افزار
                    ShowBBV.Text = List.BBV.ToString();
                    ShowBBV.TextAlignment = TextAlignment.Center;
                    ShowBBV.HorizontalAlignment = HorizontalAlignment.Center;
                    ShowBBV.VerticalAlignment = VerticalAlignment.Center;
                    ShowBBV.FontSize = 20;
                    ShowBBV.Width = 257;
                    ShowBBV.FontFamily = new FontFamily("B nazanin");
                    ShowBBV.Foreground = new SolidColorBrush(Colors.Black);
                //    if (IsProblem == "NO")
                 //       ShowBBV.Foreground = new SolidColorBrush(Colors.Snow);
                    Grid.SetRow(ShowBBV, Row);
                    Grid.SetColumn(ShowBBV, 1);
                    GridMain.Children.Add(ShowBBV);


               //     TextBlock ShowVBV = new TextBlock(); //ولتاژ باطری دستگاه
               //     ShowVBV.Text = List.VBV.ToString();
               //     ShowVBV.TextAlignment = TextAlignment.Center;
               //     ShowVBV.HorizontalAlignment = HorizontalAlignment.Center;
               //     ShowVBV.VerticalAlignment = VerticalAlignment.Center;
               //     ShowVBV.FontSize = 20;
               //     ShowVBV.Width = 257;
               //     ShowVBV.FontFamily = new FontFamily("B nazanin");
               //     ShowVBV.Foreground = new SolidColorBrush(Colors.Black);
               ////     if (IsProblem == "NO")
               // //        ShowVBV.Foreground = new SolidColorBrush(Colors.Snow);
               //     Grid.SetRow(ShowVBV, Row);
               //     Grid.SetColumn(ShowVBV, 1);
               //     GridMain.Children.Add(ShowVBV);

                    string TextStatus = "";
                    if (List.Status != null && IsProblem == "NO")
                        TextStatus = List.Status.Replace("NO:", "");
                    if (List.Status != null && IsProblem == "OK")
                        TextStatus = List.Status.Replace("OK:", "");




                    TextBlock HardWareStatus = new TextBlock(); //نمایش وضعیت سخت افزار در صورت خراب یا سالم بودن
                    HardWareStatus.Text = TextStatus;
                    HardWareStatus.TextAlignment = TextAlignment.Center;
                    HardWareStatus.HorizontalAlignment = HorizontalAlignment.Left;
                    HardWareStatus.VerticalAlignment = VerticalAlignment.Center;
                    HardWareStatus.FontSize = 15;
                    HardWareStatus.Width = 257;
                    HardWareStatus.FontFamily = new FontFamily("B nazanin");

                    HardWareStatus.Foreground = new SolidColorBrush(Colors.Black);
               //     if (IsProblem == "NO")
                //        HardWareStatus.Foreground = new SolidColorBrush(Colors.Snow);

                    Grid.SetRow(HardWareStatus, Row);
                    Grid.SetColumn(HardWareStatus,0);
                    GridMain.Children.Add(HardWareStatus);


                    Row++;
                    ConnectDataBase = true;

                }
            }
            catch (Exception ex)
            {
                if (ConnectDataBase)
                {
                    MessageBox.Show("عدم برقراری با پایگاه داده");
                    ConnectDataBase = false;
                }
            }

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Thread.Abort();
            }
            catch (Exception)
            {

            }

        }


    }
}
