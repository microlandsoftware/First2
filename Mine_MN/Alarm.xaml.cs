using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

namespace maintenance
{
    /// <summary>
    /// Interaction logic for alarm.xaml
    /// </summary>
    public partial class Alarm : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string UploadDir = ini.IniReadValue("appSettings", "UploadDir");    
        //connection string
        //i added this comment for test Git
        //i added this comment for test Git 2    
        private Thread Thread;
        private static int UpdateTime;
        private bool ConnectDataBase = true;
        public Alarm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Plans select c;
                if (query.Count() >= 1)
                    UpdateTime = int.Parse(query.Single().AlarmThreadUpdate.ToString());
                        //دریافت مقدار زمانی که ترد باید اجرا شود 
                else
                    UpdateTime = 10;
                    
                
                NewAlarm();
                Thread = new Thread(Function_LoadAlarm);
                Thread.IsBackground = true;
                Thread.Start();
            }
            else
                MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");
                
        }

        private void Function_LoadAlarm()
        {
            while (true)
            {

                this.Dispatcher.Invoke(new Action(() =>
                {

                    NewAlarm();
                   FillGridAlarm();

                }), null);

                Thread.Sleep(UpdateTime*1000);
            }
        }


        private void Confirm_Alarm(object sender, RoutedEventArgs e) //تابع  تائید کردن آلارم
        {
            var Img = sender as Image;
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (
                    MessageBox.Show("آیااز وضعیت رسیدگی به آلارم اطمینان دارید ", "هشدار", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    string[] Array = new string[] {};
                    Array = Img.Tag.ToString().Split(',');
                    int DeviceCode = int.Parse(Array[1]);
                    int AlarmCode = int.Parse(Array[0]);
                    var query =
                        (from c in db.InputDataXes where c.DeviceID == short.Parse(DeviceCode.ToString()) select c)
                            .Single();

                    string[] TextAlarm = new string[] {};
                    string NewTextAlarm = "";
                    TextAlarm = query.Alarm.Split(':');
                    for (int i = 0; i <= TextAlarm.Length - 1; i++)
                    {
                        if (TextAlarm[i] != AlarmCode.ToString())
                        {
                            NewTextAlarm = NewTextAlarm + TextAlarm[i];
                            if (i + 1 < TextAlarm.Length)
                            {
                                if (TextAlarm[i + 1] != " ")
                                    NewTextAlarm = NewTextAlarm + ":";
                            }
                        }
                    }
                    if (NewTextAlarm == "AL:")
                        NewTextAlarm = null;
                    query.Alarm = NewTextAlarm;
                    db.SubmitChanges();
                    NewAlarm();
                }

            }

        }

        private void NewAlarm()//تابع نمایش آلارم های رخداده
        {
            var db = new DataClasses1DataContext(cnn);
            try
            {

            
                var query = from c in db.InputDataXes where c.Alarm != null && c.Alarm != "" && c.Alarm.Length>=4 && c.Alarm.Substring(0,3)=="AL:" select c;
                MainGrid.Children.Clear();
                StackPanel MainPanel = new StackPanel();
                MainPanel.Orientation = Orientation.Horizontal;
                MainPanel.HorizontalAlignment = HorizontalAlignment.Center;


                foreach (var Alarm in query)
                {
                    bool Continue=false;
                    string[] Array = new string[] {};
                    int C_Device = Convert.ToInt32(Alarm.DeviceID);
                    var query2 = from c in db.ADM_Vehicles where c.DeviceID == Alarm.DeviceID select c;

                    if(query2.Count()>=1)
                    {


                    StackPanel SubPanel = new StackPanel();
                    SubPanel.Orientation = Orientation.Vertical;

                    TextBlock Text = new TextBlock();
                    Text.Text = query2.Take(1).Single().Name;
                    Text.FontFamily = new FontFamily("Tahoma");
                    Text.FontWeight = FontWeights.Bold;
                    Text.TextAlignment = TextAlignment.Center;
                    Text.Background = new SolidColorBrush(Colors.Silver);
                    Text.FontSize = 16;
                    Text.Width = 140;
                    Text.Margin = new Thickness(5, 10, 5, 0);
                    Separator Sep = new Separator();
                    Sep.Width = 140;

                    Array = Alarm.Alarm.Replace(" ", "").Split(':');

                    foreach (var AlarmCode in Array)
                     {
                         var query3 = from c in db.ADM_ConfigAlarms where c.AlarmID.ToString() == AlarmCode && c.DeviceID == Alarm.DeviceID select c;
                        if(query3.Count()>=1)
                        {
                            Continue = true;
                            break;
                        }
                     }

                    if (Continue)
                    {
                        SubPanel.Children.Add(Text);
                        SubPanel.Children.Add(Sep);



                        foreach (var AlarmCode in Array)
                        {
                            if (AlarmCode != "" && IsNumber(AlarmCode))
                            {
                                var query3 = from c in db.ADM_ConfigAlarms
                                             where c.DeviceID == Alarm.DeviceID && c.AlarmID == Convert.ToByte(AlarmCode)
                                             select c;
                                if (query3.Count() >= 1)
                                {

                                    var query4 = from c in db.ADM_AlarmSymbols
                                                 where c.Symbol == query3.Take(1).Single().Symbol
                                                 select c;

                                    string ToolTip = "";
                                    if (query4.Count() >= 1)
                                    {
                                        var q = query4.Take(1).Single();
                                        ToolTip = q.Message;

                                        string Path = UploadDir + ("\\DeviceIcon\\" + q.Symbol).Trim() + ".png";
                                        if ((!File.Exists(Path)))
                                            Path = "images/noimage.gif";


                                        FileStream stream = new FileStream(Path, FileMode.Open,
                                            FileAccess.Read);
                                        Image Image = new Image();
                                        BitmapImage src = new BitmapImage();
                                        src.BeginInit();
                                        src.StreamSource = stream;
                                        src.EndInit();
                                        Image.Source = src;
                                        Image.Width = 70;
                                        Image.Height = 70;
                                        Image.Margin = new Thickness(0, 0, 0, 0);
                                        Image.VerticalAlignment = VerticalAlignment.Top;
                                        Image.HorizontalAlignment = HorizontalAlignment.Center;
                                        Image.ToolTip = ToolTip;
                                        Image.MouseLeftButtonDown += Confirm_Alarm;
                                        Image.Tag = AlarmCode + "," + Alarm.DeviceID;
                                        Image.Cursor = Cursors.Hand;

                                        TextBlock TextAlarm = new TextBlock();
                                        TextAlarm.Text = ToolTip.ToString();
                                        TextAlarm.FontFamily = new FontFamily("B nazanin");
                                        TextAlarm.FontWeight = FontWeights.Bold;
                                        TextAlarm.TextAlignment = TextAlignment.Center;
                                        TextAlarm.Foreground = new SolidColorBrush(Colors.Red);
                                        TextAlarm.Margin = new Thickness(0, 0, 0, 0);
                                       // TextAlarm.Background = new SolidColorBrush(Colors.Silver);
                                        TextAlarm.FontSize = 12;
                                        //TextAlarm.Width = 140;
                                        //text.Height = 40;

                                        StackPanel Panel = new StackPanel();

                                        Panel.Orientation = Orientation.Vertical;
                                        Panel.VerticalAlignment = VerticalAlignment.Top;
                                        Panel.HorizontalAlignment = HorizontalAlignment.Center;
                                     //   Panel.Margin = new Thickness(5, 5, 5, 5);

                                        Panel.Children.Add(Image);
                                        Panel.Children.Add(TextAlarm);
                                        

                                        SubPanel.Children.Add(Panel);
                                    }
                                }
                            }
                        }
                    }
                    MainPanel.Children.Add(SubPanel);
                }

            }

                MainGrid.Children.Add(MainPanel);
                ConnectDataBase = true;
            }
            catch (Exception)
            {
                if (ConnectDataBase)
                {

                    MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");
                    this.Cursor = Cursors.Hand;
                    this.IsEnabled = true;
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

        public static bool IsNumber( string str)
        {
            return str.All(Char.IsNumber);
        }



        public class ShowProfile_Alarm
        {

            public string DeviceName { get; set; }
            public string StartTime { get; set; }
            public string StartDate { get; set; }
            public string EndTime { get; set; }
            public string EndDate { get; set; }
            public string Duration { get; set; }
            public string DurationDay { get; set; }
            public string Symbole { get; set; }

        }

        public void FillGridAlarm()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_Alarm> items = new List<ShowProfile_Alarm>();

              

                  var  query = (from c in db.FM_AlarmReports
                            join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                join k in db.ADM_AlarmSymbols on c.Symbol.Replace(" ", "") equals k.Symbol.Replace(" ", "")
                                orderby c.ID descending
                            select
                                new
                                {
                                    StartDate = c.StartDate,
                                    StartTime = c.StartTime,
                                    EndTime = c.EndTime,
                                    EndDate = c.EndDate,
                                    Duration = c.DurationTime,
                                    DurationDay = c.DurationDay,
                                    DeviceName = d.Name,
                                    Symbole = k.Message

                                }).Take(20);
                    
                

                foreach (var list in query)
                {
                    ConvertDate ConvertDate = new ConvertDate();
                    string StartDate = ConvertDate.DateToString(int.Parse(list.StartDate.ToString()));
                    string EndDate = ConvertDate.DateToString(int.Parse(list.EndDate.ToString()));
                    items.Add(new ShowProfile_Alarm()
                    {
                        DeviceName = list.DeviceName,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        StartTime = list.StartTime.ToString(),
                        EndTime = list.EndTime.ToString(),
                        Duration = list.Duration.ToString(),
                        DurationDay = list.DurationDay.ToString(),
                        Symbole = list.Symbole
                    });
                }
                ProfileGrid_Alaram.ItemsSource = items;
                
            }
            }
        



    }
}
