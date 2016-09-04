using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
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
using System.Threading;
using System.ComponentModel;
using Stimulsoft.Report;
using maintenance.classes;

using RibbonMenuItem = Stimulsoft.Report.Wpf.RibbonMenuItem;
using ThreadState = System.Threading.ThreadState;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string UploadDir = ini.IniReadValue("appSettings", "UploadDir");
        public static string LastStatus_Service = null;
        public static string LastStatus_Part = null;
        public static bool MessageBoxConfirm_Flag;
        public static bool MessageBoxConfirm_FlagCheck;
        private static int UpdateTime;

        private static bool Flag_AttachFile = false;
        private static string FileName;
        private static string ServiceCode_AttachFile = "0";
        public static string SaveFile = ini.IniReadValue("appSettings", "SaveFile");
        public static int DeviceCode_Profile;
        private Thread Thread;
        private Thread ThreadAddDevice;
        private Thread ThreadUpdateGridServiceCycle;
        private Thread ThreadUpdateGridServicePart;
        //  public static string DisplayMode="service" ;
        public static string TabSelect_Print = "ProfileGrid_ProgramServiceTime";
        public static int TabIndex_ShowDeviceService = 0;
        public static bool TabClick = false;
        private bool ConnectDataBase = true;
        public static string UserName;
        public static string UserID;

        private string MessageEr;
        public static string FilterGrid_StartDate = "0";
        public static string FilterGrid_EndDate = "0";
        public MainWindow()
        {

            if (UserManagment.UserAccess(UserName, UserManagment.Operations.ViewMainTenance))
            {
                this.InitializeComponent();
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(1065);
                //انتخاب توع تقویم برای نمایش در کامپوننت تاریخ
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            }
        }






        private int i = 0;



        private void ShowDefineServiceInTab(object sender, RoutedEventArgs e) //تابع که بعد از کلیک روی  عنوان آیتم های تب یا کلیک روی تصویر دستگاه داخل تب اجرا شده و سرویس های تعریف شده  را بر اساس مدل دستگاه یا خود دستگاه نمایش میدهد
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                TopMenu.IsDropDownOpen = false;
                var Btn = sender as Button;
                var Img = sender as Image;



                if ((Grid_ProgramPart.Visibility == Visibility.Hidden) &&
                    (Grid_ProgramService.Visibility == Visibility.Hidden))
                {
                    if (Profile.Visibility == Visibility.Visible)
                        Profile.Visibility = Visibility.Hidden;
                    Grid_ProgramPart.Visibility = Visibility.Visible;
                    Grid_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramPart.Visibility = Visibility.Visible;
                    BackgroundTitlePart.Visibility = Visibility.Visible;
                    BackgroundTitleService.Visibility = Visibility.Visible;
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                    Column2.Width = new GridLength(1, GridUnitType.Star);
                    MnuItem_ProgramPart.IsChecked = true;
                    MenuItem_ProgramService.IsChecked = true;

                }

                TabDefineService.Items.Clear();
                TabItem TabTime = new TabItem();
                StackPanel Panel = new StackPanel();

                if (Btn != null)
                {
                    string[] Array = new string[] { };
                    Array = Btn.Tag.ToString().Split(',');

                    int ModelCode = int.Parse(Array[0]);
                    TabIndex_ShowDeviceService = int.Parse(Array[1]);
                    Tab_ShowDeviceService.SelectedIndex = TabIndex_ShowDeviceService;
                    TabClick = true;


                    var query = from c in db.MN_DefineVisitCycles
                                join d in db.MN_DefineServiceCycles on c.ID equals d.VisitCycleID
                                where c.ModelID == ModelCode && d.CriteriaID == 1
                                select new { Value = d.Value };
                    var query3 =
                        (from c in db.ADM_Models where c.ID == ModelCode select c).Single();
                    if (query.Count() >= 1)
                    {


                        foreach (var ServiceTime in query)
                        {
                            Button NewBtn = new Button();
                            NewBtn.Margin = new Thickness(5, 5, 5, 5);
                            NewBtn.Background = Brushes.AntiqueWhite;
                            NewBtn.Width = 50;
                            NewBtn.Cursor = Cursors.Hand;
                            NewBtn.Content = ServiceTime.Value;
                            NewBtn.Tag = "valuetime," + ServiceTime.Value + ",codemodel," + ModelCode;
                            NewBtn.Click += FillGridServiceCycle;


                            Panel.Children.Add(NewBtn);
                        }
                    }
                    else
                    {
                        TextBlock Text = new TextBlock();

                        Text.FontSize = 16;
                        Text.Text = "برای مدل" + query3.Name + " سرویس ساعت ثبت نشده است";
                        Text.Foreground = Brushes.Red;
                        Text.Margin = new Thickness(5, 5, 5, 5);
                        Panel.Children.Add(Text);

                    }

                    Panel.Orientation = Orientation.Horizontal;
                    Panel.VerticalAlignment = VerticalAlignment.Center;
                    TabTime.Header = "سرویس های ساعت" + "(" + query3.Name.ToString() + ")";
                    TabTime.Content = Panel;
                    TabDefineService.Items.Add(TabTime);

                    StackPanel Panel2 = new StackPanel();
                    var query2 = from c in db.MN_DefineVisitCycles
                                 join d in db.MN_DefineServiceCycles on c.ID equals d.VisitCycleID
                                 where c.ModelID == ModelCode && d.CriteriaID == 2
                                 select new { value = d.Value };
                    if (query2.Count() >= 1)
                    {
                        foreach (var service_kilometr in query2)
                        {
                            Button NewBtn = new Button();
                            NewBtn.Margin = new Thickness(5, 5, 5, 5);
                            NewBtn.Background = Brushes.AntiqueWhite;
                            NewBtn.Width = 50;
                            NewBtn.Cursor = Cursors.Hand;
                            NewBtn.Content = service_kilometr.value;
                            NewBtn.Tag = "valuekilometr," + service_kilometr.value + ",codemodel," + ModelCode;
                            NewBtn.Click += FillGridServiceCycle;

                            Panel2.Children.Add(NewBtn);
                        }
                    }
                    else
                    {
                        TextBlock Text = new TextBlock();

                        Text.FontSize = 16;
                        Text.Text = "برای مدل" + query3.Name + " سرویس کیلومتر ثبت نشده است";
                        Text.Foreground = Brushes.Red;
                        Panel2.Children.Add(Text);
                    }
                    Panel2.Orientation = Orientation.Horizontal;
                    TabItem TabKilometer = new TabItem();
                    TabKilometer.Header = "سرویس های کیلومتر" + "(" + query3.Name.ToString() + ")";
                    TabKilometer.Content = Panel2;
                    TabDefineService.Items.Add(TabKilometer);
                    Row2.Height = GridLength.Auto;
                    TabDefineService.SelectedIndex = 0;
                    TabDefineService.Visibility = Visibility.Visible;

                    LastStatus_Service = "model," + ModelCode;
                    LastStatus_Part = "model," + ModelCode;
                }


                else if (Img != null)
                {
                    string[] Array = new string[] { };
                    Array = Img.Tag.ToString().Split(',');
                    string DeviceCode = Array[1];

                    //var query = from c in db.MN_DefineVisitCycles
                    //            join d in db.MN_DefineServiceCycles on c.ID equals d.VisitCycleID
                    //            join m in db.ADM_Vehicles on c.ModelID equals  m.ModelID
                    //            where (m.ID == int.Parse(DeviceCode.ToString()) || d.VehicleID == int.Parse(DeviceCode.ToString())) && d.CriteriaID == 1
                    //            select
                    //new {Value = d.Value};
                    var query = (from c in db.MN_DefineServiceCycles
                                 where c.VehicleID == int.Parse(DeviceCode.ToString()) && c.CriteriaID == 1
                                 select new { c.Value }).Union
                        (from c in db.MN_DefineServiceCycles
                         join d in db.MN_DefineVisitCycles on c.VisitCycleID equals d.ID
                         join k in db.ADM_Vehicles on d.ModelID equals k.ModelID
                         where k.ID == int.Parse(DeviceCode.ToString()) && c.CriteriaID == 1
                         select new { c.Value }).Distinct();

                    var query3 =
                        (from c in db.ADM_Vehicles
                         join d in db.ADM_Models on c.ModelID equals d.ID
                         where c.ID == int.Parse(DeviceCode.ToString())
                         select c).Single();
                    if (query.Count() >= 1)
                    {


                        foreach (var ServiceTime in query)
                        {
                            Button NewBtn = new Button();
                            NewBtn.Margin = new Thickness(5, 5, 5, 5);
                            NewBtn.Background = Brushes.AntiqueWhite;
                            NewBtn.Width = 50;
                            NewBtn.Cursor = Cursors.Hand;
                            NewBtn.Content = ServiceTime.Value;
                            NewBtn.Tag = "valuetime," + ServiceTime.Value + ",codedevice," + DeviceCode.ToString();
                            NewBtn.Click += FillGridServiceCycle;


                            Panel.Children.Add(NewBtn);
                        }
                    }
                    else
                    {
                        TextBlock Text = new TextBlock();

                        Text.FontSize = 16;
                        Text.Text = "برای دستگاه" + " " + query3.Name + " " + " سرویس ساعت ثبت نشده است";
                        Text.Foreground = Brushes.Red;
                        Text.Margin = new Thickness(5, 5, 5, 5);
                        Panel.Children.Add(Text);

                    }

                    Panel.Orientation = Orientation.Horizontal;
                    Panel.VerticalAlignment = VerticalAlignment.Center;
                    TabTime.Header = " سرویس های ساعت دستگا" + "(" + query3.Name.ToString() + ")";
                    TabTime.Content = Panel;
                    TabDefineService.Items.Add(TabTime);

                    StackPanel Panel2 = new StackPanel();

                    var query2 = (from c in db.MN_DefineServiceCycles
                                  where c.VehicleID == int.Parse(DeviceCode.ToString()) && c.CriteriaID == 2
                                  select new { c.Value }).Union
                        (from c in db.MN_DefineServiceCycles
                         join d in db.MN_DefineVisitCycles on c.VisitCycleID equals d.ID
                         join k in db.ADM_Vehicles on d.ModelID equals k.ModelID
                         where k.ID == int.Parse(DeviceCode.ToString()) && c.CriteriaID == 2
                         select new { c.Value }).Distinct();

                    //var query2  = from c in db.MN_DefineVisitCycles into lc
                    //              from c in 
                    //            join d in db.MN_DefineServiceCycles on c.ID equals d.VisitCycleID into ld
                    //            from d in ld.DefaultIfEmpty()
                    //            join m in db.ADM_Vehicles on c.ModelID equals  m.ModelID
                    //              where (m.ID == int.Parse(DeviceCode.ToString()) || d.VehicleID == int.Parse(DeviceCode.ToString())) && d.CriteriaID == 2
                    //            select new { Value = d.Value };
                    if (query2.Count() >= 1)
                    {
                        foreach (var ServiceKilometer in query2)
                        {
                            Button NewBtn = new Button();
                            NewBtn.Margin = new Thickness(5, 5, 5, 5);
                            NewBtn.Background = Brushes.AntiqueWhite;
                            NewBtn.Width = 50;
                            NewBtn.Cursor = Cursors.Hand;
                            NewBtn.Content = ServiceKilometer.Value;
                            NewBtn.Tag = "valuekilometr," + ServiceKilometer.Value + ",codedevice," +
                                         DeviceCode.ToString();
                            NewBtn.Click += FillGridServiceCycle;

                            Panel2.Children.Add(NewBtn);
                        }
                    }
                    else
                    {
                        TextBlock Text = new TextBlock();

                        Text.FontSize = 16;
                        Text.Text = "برای دستگاه" + " " + query3.Name + " " + " سرویس کیلومتر ثبت نشده است";
                        Text.Foreground = Brushes.Red;
                        Panel2.Children.Add(Text);
                    }
                    Panel2.Orientation = Orientation.Horizontal;
                    TabItem TabKilometer = new TabItem();
                    TabKilometer.Header = "سرویس های کیلومتر دستگاه" + "(" + query3.Name.ToString() + ")";
                    TabKilometer.Content = Panel2;
                    TabDefineService.Items.Add(TabKilometer);
                    Row2.Height = GridLength.Auto;
                    TabDefineService.SelectedIndex = 0;
                    TabDefineService.Visibility = Visibility.Visible;

                }

                Tab_ShowDeviceService.SelectedIndex = TabIndex_ShowDeviceService;
            }
        }

        int ServiceCount;
        int PartCount;
        private void AddDeviceInTab(object sender, EventArgs e)//تابع اضافه کردن دستگاه هایی که سرویس دوره ای یا سرویس قطعه دارند به تب نمایش دستگاه ها
        {

            var db = new DataClasses1DataContext(cnn);
            try
            {
                var query = from c in db.ADM_Models select c;
                this.Tab_ShowDeviceService.Items.Clear();
                int TabIndex = 0;
                foreach (var Value in query)
                {

                    bool AddTab = false;
                    string TypeMachin = "";
                    TabItem TabItem = new TabItem();

                    StackPanel Panel = new StackPanel();

                    Panel.Orientation = Orientation.Horizontal;

                    int ModelCode = Value.ID;

                    var query2 = from c in db.ADM_Vehicles where c.ModelID == ModelCode select c;
                    foreach (var ValueDevice in query2)
                    {


                        var query3 = from c in db.MN_CreateServices
                                     where c.VehicleID == ValueDevice.ID && c.Confirm == false
                                     select c;
                        ServiceCount = query3.Count();
                        var query4 = from c in db.MN_CreatePartFixes
                                     where c.VehicleID == ValueDevice.ID && c.Confirm == false
                                     select c;
                        PartCount = query4.Count();

                        var query5 = from c in db.ADM_Vehicles
                                     join d in db.Lables on c.Type equals d.LID
                                     join k in db.ADM_Models on c.ModelID equals k.ID
                                     join n in db.ADM_Brands on k.BrandID equals n.ID
                                     where c.ID == ValueDevice.ID
                                     select
                                         new
                                         {
                                             Image = c.PictureUrl,
                                             Brand = n.Name,
                                             Type = d.LDscFa,
                                             TypeMachin = c.Type,
                                             TypeMachinCode = c.Type
                                         };



                        var query6 = from d in db.ADM_Vehicles
                                     join c in db.VehicleLastStates on d.DeviceID equals c.DeviceID
                                     join m in db.MN_States on c.State equals m.Code
                                     where d.ID == ValueDevice.ID
                                     select new
                                     {
                                         DeviceID = c.DeviceID,
                                         Icon = m.StatusIcon,
                                         m.ImageVehicle_BySatet,
                                         m.ImageVehicle_ByAlarm,
                                         m.ImageVehicle_ByWarning


                                     };

                        if (query5.Count() == 1)
                        {
                            var q5 = query5.Single();
                            if (query6.Count() == 1)
                            {

                                var q = query6.Single();

                                GetVehicleService service = new GetVehicleService();

                                string TypeService = service.GetServiceCycle_Status(ValueDevice.ID);
                                // متد چک کردن آیا دستگاه برای برانامه های سرویس دوره ای دارای آلارم یا وارنینگ است
                                string TypePart = service.GetServiceLot_Status(ValueDevice.ID);
                                // متد چک کردن آیا دستگاه برای برانامه های قطعات دارای آلارم یا وارنینگ است
                                string ImageMachin = q.ImageVehicle_BySatet;

                                if (TypeService == "1" || TypePart == "1")
                                    ImageMachin = q.ImageVehicle_ByWarning;
                                if (TypeService == "2" || TypePart == "2")
                                    ImageMachin = q.ImageVehicle_ByAlarm;


                                TypeMachin = q5.Type + " " + q5.Brand;

                                string Path = UploadDir + "\\VehiclePics\\" + q5.TypeMachinCode + "\\" + ImageMachin;
                                // ایجاد تصویر دستگاه با توجه به وضعیت 

                                if ((!File.Exists(Path)) || ImageMachin == "" || ImageMachin == null)
                                    Path = "images/noimage.gif";

                                FileStream stream = new FileStream(Path, FileMode.Open, FileAccess.Read);

                                Image Image = new Image();
                                BitmapImage src = new BitmapImage();
                                src.BeginInit();
                                src.StreamSource = stream;
                                src.EndInit();
                                Image.Source = src;
                                Image.Width = 120;
                                Image.Height = 120;
                                Image.Margin = new Thickness(0, 0, 0, 0);
                                Image.Tag = "device," + ValueDevice.ID;
                                Image.MouseLeftButtonDown += FillGridServiceCycle;
                                Image.MouseLeftButtonDown += FillGridServicePart;
                                Image.MouseLeftButtonDown += ShowDefineServiceInTab;
                                Image.VerticalAlignment = VerticalAlignment.Top;
                                Image.HorizontalAlignment = HorizontalAlignment.Center;



                                Border Border = new Border();
                                Border.Background = new SolidColorBrush(Colors.White);
                                Border.BorderThickness = new Thickness(5);
                                Border.BorderBrush = new SolidColorBrush(Colors.White);
                                Border.CornerRadius = new CornerRadius(15);
                                Border.Margin = new Thickness(1, 1, 2, 1);

                                Grid DynamicGrid = new Grid(); //تعریف گرید

                                ColumnDefinition gridColumn1 = new ColumnDefinition();
                                ColumnDefinition gridColumn2 = new ColumnDefinition();
                                DynamicGrid.ColumnDefinitions.Add(gridColumn1);
                                DynamicGrid.ColumnDefinitions.Add(gridColumn2);

                                RowDefinition gridRow1 = new RowDefinition(); // تعریف سطر برای گرید
                                gridRow1.Height = new GridLength(30);
                                RowDefinition gridRow2 = new RowDefinition();
                                gridRow2.Height = new GridLength(60);
                                RowDefinition gridRow3 = new RowDefinition();
                                gridRow3.Height = new GridLength(10);

                                DynamicGrid.RowDefinitions.Add(gridRow1);
                                DynamicGrid.RowDefinitions.Add(gridRow2);
                                DynamicGrid.RowDefinitions.Add(gridRow3);


                                Button BtnService = new Button(); // تعریف کلید برای نمایش تعداد برنامه های دستگاه

                                BtnService.Tag = "device," + ValueDevice.ID;

                                BtnService.Content = PartCount + ServiceCount;
                                BtnService.ToolTip = "تعداد برنامه ها";
                                BtnService.Foreground = new SolidColorBrush(Colors.Red);
                                BtnService.Background = new SolidColorBrush(Colors.White);
                                BtnService.Width = 25;
                                BtnService.Height = 20;
                                BtnService.FontWeight = FontWeights.Bold;
                                BtnService.Margin = new Thickness(10, 2, 0, 0);
                                BtnService.Cursor = Cursors.Hand;
                                BtnService.Click += FillGridServiceCycle;
                                BtnService.Click += FillGridServicePart;
                                BtnService.VerticalAlignment = VerticalAlignment.Bottom;
                                BtnService.HorizontalAlignment = HorizontalAlignment.Right;



                                TextBlock Text = new TextBlock(); // تعریف تکست بلاک برای نمایش نماد دستگاه
                                Text.Text = ValueDevice.Name.ToString();
                                Text.FontSize = 14;
                                Text.FontWeight = FontWeights.Bold;
                                Text.Foreground = new SolidColorBrush(Colors.Red);
                                Text.Margin = new Thickness(0, 0, 0, 0);
                                Text.VerticalAlignment = VerticalAlignment.Bottom;
                                Text.HorizontalAlignment = HorizontalAlignment.Center;
                                Text.Background = new SolidColorBrush(Colors.White);
                                Text.TextAlignment = TextAlignment.Center;


                                Grid.SetRow(BtnService, 0);
                                Grid.SetColumn(BtnService, 2);
                                Grid.SetRow(Image, 1);
                                Grid.SetColumn(Image, 0);
                                Grid.SetRowSpan(Image, 2);
                                Grid.SetColumnSpan(Image, 2);

                                Grid.SetRow(Text, 0);
                                Grid.SetColumn(Text, 0);

                                Grid.SetColumnSpan(Text, 2);


                                if ((PartCount + ServiceCount) >= 1)
                                // چک کردن آیا دستگاه دارای سرویس قطعات یا سرویس دوره ای است برای اضافه شدن دستگاه به تب
                                {

                                    DynamicGrid.Children.Add(Image);
                                    DynamicGrid.Children.Add(BtnService);


                                    string PathIcon = UploadDir + "\\VehiclePics\\icon\\" + q.Icon;
                                    // نمایش آیکون با توجه به وضعیت دستگاه

                                    if ((!File.Exists(PathIcon)) || q.Icon == "" || q.Icon == null)
                                        PathIcon = "images/noicon.png";

                                    FileStream stream2 = new FileStream(PathIcon,
                                        FileMode.Open, FileAccess.Read);
                                    Image Icon = new Image();
                                    BitmapImage src_icon = new BitmapImage();
                                    src_icon.BeginInit();
                                    src_icon.StreamSource = stream2;
                                    src_icon.EndInit();
                                    Icon.Source = src_icon;
                                    Icon.Width = 30;
                                    Icon.Height = 30;
                                    Icon.Cursor = Cursors.Hand;
                                    Icon.Margin = new Thickness(0, 0, 0, 25);
                                    Icon.VerticalAlignment = VerticalAlignment.Bottom;
                                    Icon.HorizontalAlignment = HorizontalAlignment.Center;
                                    Icon.Tag = "device," + ValueDevice.ID;
                                    Icon.MouseLeftButtonDown += FillGridServiceCycle;
                                    Icon.MouseLeftButtonDown += FillGridServicePart;
                                    Icon.MouseLeftButtonDown += ShowDefineServiceInTab;
                                    Grid.SetRow(Icon, 1);
                                    Grid.SetColumn(Icon, 1);

                                    DynamicGrid.Children.Add(Icon);

                                    DynamicGrid.Children.Add(Text);

                                    AddTab = true;
                                    Border.Child = DynamicGrid;
                                    Panel.Children.Add(Border);
                                }
                            }
                        }
                    }
                        if (AddTab == true)
                        {

                            Button BtnHeaderTab = new Button();

                            BtnHeaderTab.Content = "(" + Value.Name + ")" + " " + TypeMachin;
                            BtnHeaderTab.Background = new SolidColorBrush(Colors.Snow);
                            BtnHeaderTab.BorderThickness = new Thickness(0);
                            BtnHeaderTab.FlowDirection = FlowDirection.RightToLeft;
                            BtnHeaderTab.Tag = ModelCode + "," + TabIndex;
                            TabIndex++;
                            BtnHeaderTab.Click += ShowDefineServiceInTab;
                            BtnHeaderTab.Click += FillGridServiceCycle;
                            BtnHeaderTab.Click += FillGridServicePart;
                            TabItem.Content = Panel;
                            TabItem.Header = BtnHeaderTab;
                            TabItem.Background = Brushes.Beige;

                            this.Tab_ShowDeviceService.Items.Add(TabItem);
                        }



                    

                    Tab_ShowDeviceService.SelectedIndex = TabIndex_ShowDeviceService;
                    ConnectDataBase = true;
                }
            }

            catch (Exception er)
            {
                if (ConnectDataBase)
                {

                    MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");
                    this.Cursor = Cursors.Hand;

                    ConnectDataBase = false;

                }

            }


        }

        private void RegisterPart_Click(object sender, RoutedEventArgs e)
        {

            RegisterPart Child = new RegisterPart();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();

        }

        private void DefineSpecialService_Click(object sender, RoutedEventArgs e)
        {

            RegisterSpecialService Child = new RegisterSpecialService();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();

        }

        private void RegisterGroup_Click(object sender, RoutedEventArgs e)
        {

            RegisterGroup Child = new RegisterGroup();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();

        }

        private void DefineProgramingPart_Click(object sender, RoutedEventArgs e)
        {

            ShowListPart_RegisterProgram Child = new ShowListPart_RegisterProgram();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();

        }



        private void DefineVisitService_Click(object sender, RoutedEventArgs e)
        {

            RegisterProgramService Child = new RegisterProgramService();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();

        }

        private void ActiveServiceTime_Click(object sender, RoutedEventArgs e)
        {

            ActiveService_Time Child = new ActiveService_Time();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();


        }

        private void ActiveServiceKilometer_Click(object sender, RoutedEventArgs e)
        {

            ActiveService_Kilometer Child = new ActiveService_Kilometer();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();


        }

        private void AddWorking_Click(object sender, RoutedEventArgs e)
        {

            AddWorking Child = new AddWorking();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();


        }


        private void ResetWorking_Click(object sender, RoutedEventArgs e)
        {

            ResetWorking Child = new ResetWorking();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();


        }
        public class ShowProgramServiceCycle
        {
            public string DeviceName { get; set; }
            public string BrandName { get; set; }
            public string ModelName { get; set; }
            public string ServiceTitle { get; set; }
            public string FunctionService { get; set; }
            public string TotalWork { get; set; }
            public string GroupRepair { get; set; }
            public string CreateDate { get; set; }
            public string CreateTime { get; set; }
            public string CreateServiceCode { get; set; }
            public string ModelCode { get; set; }

            public string ValueService { get; set; }

            public string CycleType { get; set; }

            public string DeviceCode { get; set; }

            public string Working { get; set; }

            public string SumTotalWork { get; set; }

            public string WorkingInService { get; set; }

            public string Alarm { get; set; }
            public string Warning { get; set; }

            public string Schedule { get; set; }

            public string WorkingInCreareSevice { get; set; }

        }

        public void FillGridServiceCycle(object sender, EventArgs e) // تابع نمایش سرویس های رسیده دستگاه داخل گرید
        {
            var db = new DataClasses1DataContext(cnn);
            try
            {
                TopMenu.IsDropDownOpen = false;
                if (Profile.Visibility == Visibility.Visible)
                {
                    Grid_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramService.Visibility = Visibility.Visible;
                    BackgroundTitleService.Visibility = Visibility.Visible;
                    Grid_ProgramPart.Visibility = Visibility.Visible;
                    TextTitle_ProgramPart.Visibility = Visibility.Visible;
                    BackgroundTitlePart.Visibility = Visibility.Visible;
                    Profile.Visibility = Visibility.Hidden;

                }

                MenuItem Menu = sender as MenuItem;

                if ((Grid_ProgramPart.Visibility == Visibility.Hidden) &&
                    (Grid_ProgramService.Visibility == Visibility.Hidden) && (Menu != null))
                {
                    Grid_ProgramPart.Visibility = Visibility.Visible;
                    Grid_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramPart.Visibility = Visibility.Visible;
                    BackgroundTitlePart.Visibility = Visibility.Visible;
                    BackgroundTitleService.Visibility = Visibility.Visible;
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                    Column2.Width = new GridLength(1, GridUnitType.Star);
                    MnuItem_ProgramPart.IsChecked = true;
                    MenuItem_ProgramService.IsChecked = true;

                }

                if (Grid_ProgramService.Visibility == Visibility.Hidden)
                {
                    Grid_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramService.Visibility = Visibility.Visible;
                    BackgroundTitleService.Visibility = Visibility.Visible;
                    MenuItem_ProgramService.IsChecked = true;
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                }


                string DeviceCode = "";
                string BrandCode = "";
                string ModelCode = "";
                string Value = "";
                string TypeService = "";


                string[] Type = new string[] { };
                if (Menu != null)
                {


                    Type = (Menu.Tag.ToString().Split(','));


                    if (Type[0] == "all")
                        LastStatus_Service = null;
                    if (Type[0] == "device")
                    {
                        DeviceCode = Type[1].ToString();
                        LastStatus_Service = Menu.Tag.ToString();
                        TabDefineService.Visibility = Visibility.Hidden;
                        Row2.Height = new GridLength(0);
                    }

                    if (Type[0] == "brand")
                    {
                        BrandCode = Type[1].ToString();
                        LastStatus_Service = Menu.Tag.ToString();
                        TabDefineService.Visibility = Visibility.Hidden;
                        Row2.Height = new GridLength(0);
                    }

                    if (Type[0] == "model")
                    {
                        ModelCode = Type[1].ToString();
                        LastStatus_Service = Menu.Tag.ToString();
                        TabDefineService.Visibility = Visibility.Hidden;
                        Row2.Height = new GridLength(0);
                    }


                }

                TabItem Tab = sender as TabItem;
                if (Tab != null)
                {
                    ModelCode = Tab.Tag.ToString();
                    LastStatus_Service = "model," + Tab.Tag.ToString();
                }

                Button Btn = sender as Button;
                if (Btn != null)
                {
                    string[] Val = new string[] { };
                    Val = Btn.Tag.ToString().Split(',');
                    if (Val[0] == "device")
                    {
                        DeviceCode = Val[1];
                        LastStatus_Service = Btn.Tag.ToString();
                        TabDefineService.Visibility = Visibility.Hidden;
                        Row2.Height = new GridLength(0);
                    }




                    else if (Val[0] == "valuekilometr" || Val[0] == "valuetime")
                    {
                        Value = Val[1];
                        if (Val[0] == "valuekilometr")
                        {
                            TypeService = "2";
                            if (Val[2] == "codemodel")
                                ModelCode = Val[3].ToString();
                            else
                                DeviceCode = Val[3].ToString();
                            LastStatus_Service = Btn.Tag.ToString();

                        }
                        else
                        {
                            TypeService = "1";

                            if (Val[2] == "codemodel")
                                ModelCode = Val[3].ToString();
                            else
                                DeviceCode = Val[3].ToString();

                            LastStatus_Service = Btn.Tag.ToString();

                        }


                    }



                }

                var Img = sender as Image;
                if (Img != null)
                {
                    string[] array = new string[] { };

                    LastStatus_Service = Img.Tag.ToString();

                }

                if (LastStatus_Service != null)
                {
                    string[] Array = new string[] { };
                    Array = LastStatus_Service.Split(',');
                    if (Array[0] == "device")
                        DeviceCode = Array[1];
                    else if (Array[0] == "brand")
                        BrandCode = Array[1];
                    else if (Array[0] == "model")
                        ModelCode = Array[1];

                    else if (Array[0] == "valuekilometr")
                    {
                        Value = Array[1];
                        TypeService = "2";
                        if (Array[2] == "codemodel")
                            ModelCode = Array[3];
                        else
                            DeviceCode = Array[3];



                    }
                    else if (Array[0] == "valuetime")
                    {
                        Value = Array[1];
                        TypeService = "1";
                        if (Array[2] == "codemodel")
                            ModelCode = Array[3];
                        else
                            DeviceCode = Array[3];
                    }

                }
                else
                {
                    ModelCode = "";
                    BrandCode = "";
                    DeviceCode = "";
                    TypeService = "";

                    Dispatcher.BeginInvoke(new Action(() => { TabDefineService.Visibility = Visibility.Hidden; }));
                    Dispatcher.BeginInvoke(new Action(() => { Row2.Height = new GridLength(0); }));
                }

                List<ShowProgramServiceCycle> items = new List<ShowProgramServiceCycle>();


                var query = from c in db.MN_CreateServices
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefineServiceCycles on c.ServiceCycleID equals k.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join y in db.MN_CycleCriteriaTypes on k.CriteriaID equals y.ID
                            join l in db.Lables on b.VehicleTypeID equals l.LID
                            join s in db.MN_TotalWorks on d.ID equals s.VehicleID into ho
                            from s in ho.DefaultIfEmpty()
                            join p in db.MN_SumServices on d.ID equals p.VehicleID
                            orderby c.ID
                            where
                                c.Confirm == false &&
                                (d.ID.ToString().Contains(DeviceCode) && b.ID.ToString().Contains(BrandCode) &&
                                 m.ID.ToString().Contains(ModelCode) &&
                                 SqlMethods.Like(k.Value.ToString(), "%" + Value + "%") &&
                                 k.CriteriaID.ToString().Contains(TypeService))
                            select
                                new
                                {
                                    CreateServiceCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    ServiceTitle = k.Title,
                                    GroupRepair = g.Type,
                                    CreateDate = c.CreateDate,
                                    CreatTime = c.CreateTime,
                                    FunctionService = c.ServiceWork + " " + y.Type,

                                    ModelCode = m.ID.ToString(),
                                    ValueService = k.Value,
                                    CycleType = y.ID,
                                    DeviceCode = d.ID,

                                    TypeMachin = l.LDscFa,
                                    SumKilometer = s.TotalKM,
                                    SumHours = s.TotalTime,
                                    TextCyclecType = y.Type,
                                    WorkingInServiceHours = p.TimeSum,
                                    WorkingInServiceKilometer = p.KMSum,
                                    Warning = k.Warning.ToString(),
                                    Alarm = k.Alarm.ToString(),
                                    VisitCycleID = k.VisitCycleID,
                                    Schedule = k.Schedule,
                                    ServiceCycleID = k.ID,
                                    WorkingInCreareSevice = c.TotalWork

                                };



                if (Value != "")
                {
                    if (DeviceCode != "")
                    {
                        query = from c in db.MN_CreateServices
                                join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                                join m in db.ADM_Models on d.ModelID equals m.ID
                                join b in db.ADM_Brands on m.BrandID equals b.ID
                                join k in db.MN_DefineServiceCycles on c.ServiceCycleID equals k.ID
                                join g in db.MN_Groups on k.GroupID equals g.ID
                                join y in db.MN_CycleCriteriaTypes on k.CriteriaID equals y.ID
                                join l in db.Lables on b.VehicleTypeID equals l.LID
                                join s in db.MN_TotalWorks on d.ID equals s.VehicleID into ho
                                from s in ho.DefaultIfEmpty()

                                join p in db.MN_SumServices on d.ID equals p.VehicleID
                                orderby c.ID
                                where
                                    c.Confirm == false &&
                                    (d.ID == int.Parse(DeviceCode) && SqlMethods.Like(k.Value.ToString(), Value) &&
                                     k.CriteriaID.ToString().Contains(TypeService))
                                select new
                                {
                                    CreateServiceCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    ServiceTitle = k.Title,
                                    GroupRepair = g.Type,
                                    CreateDate = c.CreateDate,
                                    CreatTime = c.CreateTime,
                                    FunctionService = c.ServiceWork + " " + y.Type,

                                    ModelCode = m.ID.ToString(),
                                    ValueService = k.Value,
                                    CycleType = y.ID,
                                    DeviceCode = d.ID,

                                    TypeMachin = l.LDscFa,
                                    SumKilometer = s.TotalKM,
                                    SumHours = s.TotalTime,
                                    TextCyclecType = y.Type,
                                    WorkingInServiceHours = p.TimeSum,
                                    WorkingInServiceKilometer = p.KMSum,
                                    Warning = k.Warning.ToString(),
                                    Alarm = k.Alarm.ToString(),
                                    VisitCycleID = k.VisitCycleID,
                                    Schedule = k.Schedule,
                                    ServiceCycleID = k.ID,
                                    WorkingInCreareSevice = c.TotalWork

                                };
                    }
                    else
                    {
                        query = from c in db.MN_CreateServices
                                join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                                join m in db.ADM_Models on d.ModelID equals m.ID
                                join b in db.ADM_Brands on m.BrandID equals b.ID
                                join k in db.MN_DefineServiceCycles on c.ServiceCycleID equals k.ID
                                join g in db.MN_Groups on k.GroupID equals g.ID
                                join y in db.MN_CycleCriteriaTypes on k.CriteriaID equals y.ID
                                join l in db.Lables on b.VehicleTypeID equals l.LID
                                join s in db.MN_TotalWorks on d.ID equals s.VehicleID into ho
                                from s in ho.DefaultIfEmpty()

                                join p in db.MN_SumServices on d.ID equals p.VehicleID
                                orderby c.ID
                                where
                                    c.Confirm == false &&
                                    (d.ID.ToString().Contains(DeviceCode) && b.ID.ToString().Contains(BrandCode) &&
                                     m.ID.ToString().Contains(ModelCode) && SqlMethods.Like(k.Value.ToString(), Value) &&
                                     k.CriteriaID.ToString().Contains(TypeService))
                                select new
                                {
                                    CreateServiceCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    ServiceTitle = k.Title,
                                    GroupRepair = g.Type,
                                    CreateDate = c.CreateDate,
                                    CreatTime = c.CreateTime,
                                    FunctionService = c.ServiceWork + " " + y.Type,

                                    ModelCode = m.ID.ToString(),
                                    ValueService = k.Value,
                                    CycleType = y.ID,
                                    DeviceCode = d.ID,

                                    TypeMachin = l.LDscFa,
                                    SumKilometer = s.TotalKM,
                                    SumHours = s.TotalTime,
                                    TextCyclecType = y.Type,
                                    WorkingInServiceHours = p.TimeSum,
                                    WorkingInServiceKilometer = p.KMSum,
                                    Warning = k.Warning.ToString(),
                                    Alarm = k.Alarm.ToString(),
                                    VisitCycleID = k.VisitCycleID,
                                    Schedule = k.Schedule,
                                    ServiceCycleID = k.ID,
                                    WorkingInCreareSevice = c.TotalWork


                                };
                    }
                }



                foreach (var listprogram in query)
                {

                    ConvertDate dt = new ConvertDate();
                    Int64 SumTotalWork = 0;
                    Int64 WorkingInService = 0;

                    string CycleType = "";
                    if (listprogram.CycleType == 1)
                    {
                        SumTotalWork = Int64.Parse(listprogram.SumHours.ToString()) / 3600;
                        if (listprogram.VisitCycleID != null)
                        {
                            WorkingInService = Int64.Parse(listprogram.WorkingInServiceHours.ToString());
                            CycleType = listprogram.TextCyclecType;

                        }
                    }
                    else
                    {
                        SumTotalWork = Int64.Parse(listprogram.SumKilometer.ToString()) / 1000;
                        if (listprogram.VisitCycleID != null)
                        {
                            WorkingInService = Int64.Parse(listprogram.WorkingInServiceKilometer.ToString());
                            CycleType = listprogram.TextCyclecType;

                        }

                    }
                    items.Add(new ShowProgramServiceCycle()
                    {
                        CreateServiceCode = listprogram.CreateServiceCode.ToString(),
                        DeviceName = listprogram.DeviceName,
                        BrandName = listprogram.TypeMachin + " " + listprogram.BrandName,
                        ModelName = listprogram.ModelName,
                        ServiceTitle = listprogram.ServiceTitle,
                        GroupRepair = listprogram.GroupRepair,
                        CreateDate = dt.DateToString(int.Parse(listprogram.CreateDate.ToString())),
                        CreateTime = listprogram.CreatTime.ToString(),
                        FunctionService = listprogram.FunctionService.ToString(),

                        ModelCode = ModelCode,
                        ValueService = listprogram.ValueService.ToString(),
                        CycleType = listprogram.CycleType.ToString(),
                        DeviceCode = listprogram.DeviceCode.ToString(),

                        SumTotalWork = SumTotalWork.ToString() + " " + listprogram.TextCyclecType,
                        WorkingInService = WorkingInService.ToString() + " " + CycleType,
                        Alarm = listprogram.Alarm,
                        Warning = listprogram.Warning,
                        Schedule = listprogram.Schedule.ToString(),
                        WorkingInCreareSevice = listprogram.WorkingInCreareSevice.ToString(),
                    });
                }


                Dispatcher.BeginInvoke(new Action(() => { Grid_ProgramService.ItemsSource = items; }));

                ChangeTitleServiceCycle(); //تابع تغییر عنوان گرید سرویس های دوره ای باتوجه به اطلاعاتی که نمایش مدهد



                if (Thread.ThreadState == ThreadState.Aborted)
                {
                    Thread = new Thread(new ThreadStart(delegate() { function_load_allprogram(sender, e); }));
                    Thread.IsBackground = true;
                    Thread.Start();
                }

                ConnectDataBase = true;
            }
            catch (Exception ex)
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

        private void Grid_ProgramService_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            try
            {
                SelectDeviceBothService(); // تایع ایجاد منو های برای نمایش سرویس های دوره ای و سرویس های قطعات
                SelectServiceCycle(); // تایع ایجاد منو های برای نمایش  سرویس های قطعات
                SelectServicePart(); // تایع ایجاد منو های برای نمایش سرویس های دوره ای 


                var query = from c in db.MN_Plans select c;
                if (query.Count() >= 1)
                    UpdateTime = int.Parse(query.Single().ScheduleThreadUpdate.ToString());
                else
                    UpdateTime = 10;




                Thread = new Thread(new ThreadStart(delegate() { function_load_allprogram(sender, e); }));
                //ترد نمایش سرویس های رسیده دستگاه ها
                Thread.IsBackground = true;
                Thread.Start();
            }
            catch (Exception Me)
            {
                MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");
                this.Close();
            }

        }

        public class ShowProgramCyclePart
        {
            public string DeviceName { get; set; }
            public string BrandName { get; set; }
            public string ModelName { get; set; }
            public string PartName { get; set; }
            public string FunctionPart { get; set; }
            public string TotalWork { get; set; }
            public string GroupRepair { get; set; }
            public string TypeActivity { get; set; }
            public string CreateDate { get; set; }
            public string CreateTime { get; set; }
            public string CreatePartCode { get; set; }
            public string ValueService { get; set; }
            public string CycleType { get; set; }
            public string DeviceCode { get; set; }

            public string Working { get; set; }

            public string SumTotalWork { get; set; }
            public string Alarm { get; set; }
            public string Warning { get; set; }
            public string Schedule { get; set; }
            public string WorkingInCreateService { get; set; }


        }

        public void FillGridServicePart(object sender, EventArgs e) //نمایش سرویس های قطعات رسیده داخل گرید
        {
            var db = new DataClasses1DataContext(cnn);
            try
            {
                TopMenu.IsDropDownOpen = false;
                if (Profile.Visibility == Visibility.Visible)
                    Profile.Visibility = Visibility.Hidden;

                string DeviceCode = "";
                string BrandCode = "";
                string ModelCode = "";

                MenuItem Menu = sender as MenuItem;


                if ((Grid_ProgramPart.Visibility == Visibility.Hidden) &&
                    (Grid_ProgramService.Visibility == Visibility.Hidden) && (Menu != null))
                {
                    Grid_ProgramPart.Visibility = Visibility.Visible;
                    Grid_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramService.Visibility = Visibility.Visible;
                    TextTitle_ProgramPart.Visibility = Visibility.Visible;
                    BackgroundTitlePart.Visibility = Visibility.Visible;
                    BackgroundTitleService.Visibility = Visibility.Visible;
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                    Column2.Width = new GridLength(1, GridUnitType.Star);
                    MnuItem_ProgramPart.IsChecked = true;
                    MenuItem_ProgramService.IsChecked = true;

                }

                if (Grid_ProgramPart.Visibility == Visibility.Hidden)
                {
                    Grid_ProgramPart.Visibility = Visibility.Visible;
                    TextTitle_ProgramPart.Visibility = Visibility.Visible;
                    BackgroundTitlePart.Visibility = Visibility.Visible;
                    MnuItem_ProgramPart.IsChecked = true;
                    Column2.Width = new GridLength(1, GridUnitType.Star);
                }

                string[] Type = new string[] { };
                if (Menu != null)
                {
                    Type = (Menu.Tag.ToString().Split(','));

                    if (Type[0] == "all")
                        LastStatus_Part = null;

                    if (Type[0] == "device")
                    {
                        DeviceCode = Type[1].ToString();
                        LastStatus_Part = Menu.Tag.ToString();
                    }

                    if (Type[0] == "brand")
                    {
                        BrandCode = Type[1].ToString();
                        LastStatus_Part = Menu.Tag.ToString();

                    }

                    if (Type[0] == "model")
                    {
                        ModelCode = Type[1].ToString();
                        LastStatus_Part = Menu.Tag.ToString();

                    }
                }

                TabItem Tab = sender as TabItem;
                if (Tab != null)
                {
                    ModelCode = Tab.Tag.ToString();
                    LastStatus_Part = "model," + Tab.Tag.ToString();

                }

                var Img = sender as Image;
                if (Img != null)
                {
                    string[] array = new string[] { };
                    // array = 
                    LastStatus_Part = Img.Tag.ToString();

                }

                Button btn = sender as Button;
                if (btn != null)
                {
                    string[] val = new string[] { };
                    val = btn.Tag.ToString().Split(',');
                    if (val[0] == "device")
                    {
                        DeviceCode = val[1];
                        LastStatus_Part = btn.Tag.ToString();
                    }
                    // if (val[0] == "model")
                    // {
                    //     ModelCode = val[1];
                    //      LastStatus_Part = val[0] + "," + val[1];
                    //   }

                    //else
                    //    value = val[1];

                }

                if (LastStatus_Part != null)
                {
                    string[] Array = new string[] { };
                    Array = LastStatus_Part.Split(',');
                    if (Array[0] == "device")
                        DeviceCode = Array[1];
                    else if (Array[0] == "brand")
                        BrandCode = Array[1];
                    else if (Array[0] == "model")
                        ModelCode = Array[1];


                }
                else
                {
                    ModelCode = "";
                    BrandCode = "";
                    DeviceCode = "";

                }

                List<ShowProgramCyclePart> items = new List<ShowProgramCyclePart>();

                var query = from c in db.MN_CreatePartFixes
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefinePartModes on c.PartModeID equals k.ID
                            join l in db.MN_Parts on k.PartID equals l.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join t in db.MN_FixTypes on k.FixTypeID equals t.ID
                            join y in db.MN_CycleCriteriaTypes on k.CriteriaID equals y.ID
                            join o in db.Lables on b.VehicleTypeID equals o.LID
                            join s in db.MN_TotalWorks on d.ID equals s.VehicleID into ho
                            from s in ho.DefaultIfEmpty()

                            where
                                c.Confirm == false &&
                                (d.ID.ToString().Contains(DeviceCode) && b.ID.ToString().Contains(BrandCode) &&
                                 m.ID.ToString().Contains(ModelCode))
                            orderby m.ID
                            select
                                new
                                {
                                    CreatPartCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    TypeActivity = t.Type,
                                    GroupRepair = g.Type,
                                    CreateDate = c.CreateDate,
                                    CreateTime = c.CreateTime,
                                    FunctionPart = c.PartWork + " " + y.Type,

                                    PartName = l.Name,
                                    DeviceCode = d.ID,
                                    CycleType = y.ID,

                                    ValueService = k.Value,
                                    TypeMachin = o.LDscFa,
                                    SumHours = s.TotalTime,
                                    SumKilometer = s.TotalKM,
                                    TextCycleType = y.Type,
                                    Alarm = k.Alarm.ToString(),
                                    Warning = k.Warning.ToString(),
                                    Schedule = k.Schedule.ToString(),
                                    WorkingInCreateService = c.TotalWork.ToString()
                                };

                foreach (var listprogram in query)
                {


                    ConvertDate dt = new ConvertDate();

                    Int64 SumTotalWork = 0;
                    if (listprogram.CycleType == 1)
                        SumTotalWork = Int64.Parse(listprogram.SumHours.ToString()) / 3600;
                    else
                        SumTotalWork = Int64.Parse(listprogram.SumKilometer.ToString()) / 1000;

                    items.Add(new ShowProgramCyclePart()
                    {
                        CreatePartCode = listprogram.CreatPartCode.ToString(),
                        DeviceName = listprogram.DeviceName,
                        BrandName = listprogram.TypeMachin + " " + listprogram.BrandName,
                        ModelName = listprogram.ModelName,
                        TypeActivity = listprogram.TypeActivity,
                        GroupRepair = listprogram.GroupRepair,
                        CreateDate = dt.DateToString(int.Parse(listprogram.CreateDate.ToString())),
                        CreateTime = listprogram.CreateTime.ToString(),
                        FunctionPart = listprogram.FunctionPart.ToString(),
                        WorkingInCreateService = listprogram.WorkingInCreateService,
                        PartName = listprogram.PartName,
                        ValueService = listprogram.ValueService,
                        DeviceCode = listprogram.DeviceCode.ToString(),
                        CycleType = listprogram.CycleType.ToString(),
                        SumTotalWork = SumTotalWork.ToString() + " " + listprogram.TextCycleType,
                        Alarm = listprogram.Alarm,
                        Warning = listprogram.Warning,
                        Schedule = listprogram.Schedule
                    });
                }


                Dispatcher.BeginInvoke(new Action(() => { Grid_ProgramPart.ItemsSource = items; }));

                ChangeTitlePartService(); //تابع تغییر عنوان گرید سرویس های دوره ای باتوجه به اطلاعاتی که نمایش مدهد

                if (Thread.ThreadState == ThreadState.Aborted)
                {
                    Thread = new Thread(new ThreadStart(delegate() { function_load_allprogram(sender, e); }));
                    Thread.IsBackground = true;
                    Thread.Start();
                }
                ConnectDataBase = true;
            }
            catch (Exception ex)
            {
                if (ConnectDataBase)
                {

                    MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");
                    this.Cursor = Cursors.Hand;

                    ConnectDataBase = false;

                }
            }

        }


        private void SelectDeviceBothService() // تابه ایجاد منو بر اساس دستگاه ،برند،مدل و همه سرویس ها
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                MenuItem MenuAll = new MenuItem();
                MenuAll.Header = "همه دستگاه ها";
                MenuAll.Tag = "all,0";
                MenuAll.Cursor = Cursors.Hand;
                MenuAll.Click += FillGridServiceCycle;
                MenuAll.Click += FillGridServicePart;
                MenuAll.Click += ShowBothServiceCycle;
                Menu_SelectDeviceBothService.Items.Add(MenuAll);


                var query = from c in db.ADM_Brands
                            join l in db.Lables on c.VehicleTypeID equals l.LID
                            select new { Type = c.Name, Code = c.ID, TypeMachin = l.LDscFa };

                MenuItem BrandMenu = new MenuItem();
                BrandMenu.Header = "بر اساس برند";
                BrandMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item = new MenuItem();
                    BrandMenu_Item.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item.Cursor = Cursors.Hand;
                    BrandMenu_Item.Tag = "brand," + brand.Code;
                    BrandMenu_Item.Click += FillGridServiceCycle;
                    BrandMenu_Item.Click += FillGridServicePart;
                    BrandMenu_Item.Click += ShowBothServiceCycle;
                    BrandMenu.Items.Add(BrandMenu_Item);
                }
                Menu_SelectDeviceBothService.Items.Add(BrandMenu); //اضافه شدن منو برند ها


                MenuItem ModelMenu = new MenuItem(); // اضافه شدن بر اساس مدل
                ModelMenu.Header = "بر اساس مدل";
                ModelMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item2 = new MenuItem();
                    BrandMenu_Item2.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item2.Cursor = Cursors.Hand;
                    BrandMenu_Item2.Tag = "brand," + brand.Code;


                    var query2 = from c in db.ADM_Models
                                 join d in db.ADM_Brands on c.BrandID equals d.ID
                                 where d.ID == brand.Code
                                 select c;
                    foreach (var ModelItem in query2)
                    {
                        MenuItem ModelMenuItem = new MenuItem();
                        ModelMenuItem.Header = ModelItem.Name;
                        ModelMenuItem.Cursor = Cursors.Hand;
                        ModelMenuItem.Tag = "model," + ModelItem.ID;
                        ModelMenuItem.Click += FillGridServiceCycle;
                        ModelMenuItem.Click += FillGridServicePart;
                        ModelMenuItem.Click += ShowBothServiceCycle;
                        BrandMenu_Item2.Items.Add(ModelMenuItem);
                    }
                    ModelMenu.Items.Add(BrandMenu_Item2);
                }
                Menu_SelectDeviceBothService.Items.Add(ModelMenu);


                MenuItem DeviceMenu = new MenuItem();
                DeviceMenu.Header = "بر اساس دستگاه";
                DeviceMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item2 = new MenuItem();
                    BrandMenu_Item2.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item2.Cursor = Cursors.Hand;

                    var query2 = from c in db.ADM_Models
                                 join d in db.ADM_Brands on c.BrandID equals d.ID
                                 where d.ID == brand.Code
                                 select c;
                    foreach (var ModelItem in query2)
                    {
                        MenuItem ModelMenu_Item = new MenuItem();
                        ModelMenu_Item.Header = ModelItem.Name;
                        ModelMenu_Item.Cursor = Cursors.Hand;

                        var query3 = from c in db.ADM_Vehicles where c.ModelID == ModelItem.ID select c;
                        foreach (var DeviceItem in query3)
                        {
                            MenuItem DeviceMenuItem_ = new MenuItem();
                            DeviceMenuItem_.Header = DeviceItem.Name;
                            DeviceMenuItem_.Cursor = Cursors.Hand;
                            DeviceMenuItem_.Tag = "device," + DeviceItem.ID;
                            DeviceMenuItem_.Click += FillGridServiceCycle;
                            DeviceMenuItem_.Click += FillGridServicePart;
                            DeviceMenuItem_.Click += ShowBothServiceCycle;
                            ModelMenu_Item.Items.Add(DeviceMenuItem_);
                        }

                        BrandMenu_Item2.Items.Add(ModelMenu_Item);
                    }
                    DeviceMenu.Items.Add(BrandMenu_Item2);
                }
                Menu_SelectDeviceBothService.Items.Add(DeviceMenu);
            }

        }

        private void VerificationServiceCycle(object sender, EventArgs e) //تابع تایید کردن سرویس های دوره ای انجام شده
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button Btn = sender as Button;
                MessageBox_ConfirmService Message = new MessageBox_ConfirmService();
                if (Flag_AttachFile == false || int.Parse(ServiceCode_AttachFile) != int.Parse(Btn.Tag.ToString()))
                    Message.text_attachfile.Visibility = Visibility.Visible;
                Message.ShowDialog();
                if (MessageBoxConfirm_Flag)
                {
                    string Path = "";
                    if (Flag_AttachFile)
                    {
                        var query = (from c in db.MN_CreateServices
                                     join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                                     where c.ID == int.Parse(Btn.Tag.ToString())
                                     select new { ModelID = d.ModelID }).Union
                            (from c in db.MN_DefineVisitCycles
                             join d in db.MN_DefineServiceCycles on c.ID equals d.VisitCycleID
                             join k in db.MN_CreateServices on d.ID equals k.ServiceCycleID
                             where k.ID == int.Parse(Btn.Tag.ToString())
                             select new { ModelID = c.ModelID }).Single();
                        string Model = query.ModelID.ToString();

                        Path = "\\attachfile\\" + "\\" + Model + "\\";
                        string FolderPath = SaveFile + Path;

                        if (!Directory.Exists(FolderPath))
                        {
                            Directory.CreateDirectory(FolderPath);
                        }
                        string[] FileNameold = new string[] { };
                        FileNameold = System.IO.Path.GetFileName(FileName).Split('.');
                        if (FileNameold.Length > 2)
                        {
                            FileNameold[1] = FileNameold[FileNameold.Length - 1];
                        }
                        string NewName = Btn.Tag.ToString() + "." + FileNameold[1];
                        string FilePath = FolderPath + NewName;
                        Path = Path + NewName;
                        System.IO.File.Copy(FileName, FilePath, true);

                        Flag_AttachFile = false;
                    }

                    ConvertDate dt = new ConvertDate();
                    int DateNow = dt.GetDateTypeInt();
                    TimeSpan TimeNow = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));
                    if (MessageBoxConfirm_FlagCheck)
                    {
                        var query = (from c in db.MN_CreateServices
                                     join d in db.MN_DefineServiceCycles on c.ServiceCycleID equals d.ID
                                     where c.ID == int.Parse(Btn.Tag.ToString())
                                     select new { Device = c.VehicleID, CycleType = d.CriteriaID }).Single();

                        //var query2 = from c in db.MN_CreateServices
                        //             join d in db.MN_DefineServiceCycles on c.ServiceCycleID equals d.ID
                        //             where
                        //                 c.ID <= int.Parse(Btn.Tag.ToString()) && c.VehicleID == query.Device &&
                        //                 c.Confirm == false && d.CriteriaID == query.CycleType
                        //             select c;


                        //foreach (var Val in query2)
                        //{
                        //    Val.Confirm = true;
                        //    Val.DoDate = DateNow;
                        //    Val.DoTime = TimeNow;
                        //    if (Val.ID == int.Parse(Btn.Tag.ToString()))
                        //        Val.FileURL = Path;
                        //    db.SubmitChanges();
                        //}

                        db.ExecuteCommand("update MN_CreateService set Confirm=1  where MN_CreateService.ID<=" +
                                          int.Parse(Btn.Tag.ToString()) + " and MN_CreateService.VehicleID = " +
                                          query.Device +
                                          " and MN_CreateService.Confirm = 0 and MN_CreateService.ServiceCycleID in(select MN_DefineServiceCycle.ID from MN_CreateService join MN_DefineServiceCycle on MN_CreateService.ServiceCycleID=MN_DefineServiceCycle.ID where  MN_DefineServiceCycle.CriteriaID =" +
                                          query.CycleType + " )");


                        //  db.ExecuteCommand("UPDATE MN_CreateService SET IsActive = 0 WHERE MasterId = 1");



                    }
                    else
                    {
                        var query = (from c in db.MN_CreateServices
                                     where c.ID == int.Parse(Btn.Tag.ToString())
                                     select c).Single();
                        query.FileURL = Path;
                        query.Confirm = true;
                        query.DoDate = DateNow;
                        query.DoTime = TimeNow;
                        db.SubmitChanges();
                    }


                    ThreadUpdateGridServiceCycle = new Thread(new ThreadStart(delegate() { ThreadFillGridCycleService(sender, e); }));
                    ThreadUpdateGridServiceCycle.IsBackground = true;
                    ThreadUpdateGridServiceCycle.Start();
                    ThreadAddDevice = new Thread(new ThreadStart(delegate() { ThreadAddDeviceTab(sender, e); }));
                    ThreadAddDevice.IsBackground = true;
                    ThreadAddDevice.Start();



                    MessageBoxConfirm_Flag = false;
                    MessageBox.Show("سرویس مورد نظر تایید شد");




                }

            }

        }

        private void ThreadAddDeviceTab(object sender, EventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {

                    AddDeviceInTab(sender, e);
                    ThreadAddDevice.Abort();

                }), null);

            }
            catch (Exception)
            {


            }


        }


        private void ThreadFillGridCycleService(object sender, EventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {

                    FillGridServiceCycle(sender, e);
                    ThreadUpdateGridServiceCycle.Abort();

                }), null);

            }
            catch (Exception)
            {


            }


        }


        private void VerificationPart(object sender, EventArgs e) // تابع تایید سرویس های قطعات انجام شده
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ConvertDate dt = new ConvertDate();
                int DateNow = dt.GetDateTypeInt();
                TimeSpan TimeNow = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));


                Button Btn = sender as Button;
                if (
                    MessageBox.Show("آيا از انجام شدن برنامه اطمينان داريد", "هشدار", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var query =
                        (from c in db.MN_CreatePartFixes where c.ID == int.Parse(Btn.Tag.ToString()) select c).Single();
                    query.Confirm = true;
                    query.DoDate = DateNow;
                    query.DoTime = TimeNow;
                    db.SubmitChanges();


                    ThreadAddDevice = new Thread(new ThreadStart(delegate() { ThreadAddDeviceTab(sender, e); }));
                    ThreadAddDevice.IsBackground = true;
                    ThreadAddDevice.Start();
                    ThreadUpdateGridServicePart = new Thread(new ThreadStart(delegate() { ThreadFillGridServicePart(sender, e); }));
                    ThreadUpdateGridServicePart.IsBackground = true;
                    ThreadUpdateGridServicePart.Start();
                    MessageBox.Show("سرویس مورد نظر تایید شد");



                }
            }
        }

        private void ThreadFillGridServicePart(object sender, EventArgs e)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {

                    FillGridServicePart(sender, e);
                    ThreadUpdateGridServicePart.Abort();

                }), null);

            }
            catch (Exception)
            {


            }


        }

        private void Grid_ProgramService_LoadingRow(object sender, DataGridRowEventArgs e)// تشخیص آیا سرویس دوره ای ایجاده شده دارای آلارم یا وارنینگ است
        {

            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ShowProgramServiceCycle Row = e.Row.DataContext as ShowProgramServiceCycle;

                int ServiceValue = int.Parse(Row.ValueService);

                Int64 Working;
                int DeviceCode = int.Parse(Row.DeviceCode);
                if (int.Parse(Row.CycleType) == 1)
                {

                    // var query2 = (from c in db.MN_TotalWorks where c.VehicleID == DeviceCode select c).Single();
                    // Working = Int64.Parse(query2.TotalTime.ToString()) / 3600;
                    Working = Int64.Parse(Row.SumTotalWork.Replace("ساعت", ""));
                }
                else
                {

                    // var query2 = (from c in db.MN_TotalWorks where c.VehicleID == DeviceCode select c).Single();
                    //  Working = Int64.Parse(query2.TotalKM.ToString()) / 100000;
                    Working = Int64.Parse(Row.SumTotalWork.Replace("کیلومتر", ""));
                }
                int Warning = int.Parse(Row.Warning);
                int Alarm = int.Parse(Row.Alarm);
                int Schedule = int.Parse(Row.Schedule);



                double PercentAlarm = Math.Ceiling((Convert.ToDouble(Alarm) * Convert.ToDouble(ServiceValue) / 100));
                double PercentWarning = Math.Ceiling((Convert.ToDouble(Warning) * Convert.ToDouble(ServiceValue) / 100));
                double PercentSchedule = Math.Ceiling((Convert.ToDouble(Schedule) * Convert.ToDouble(ServiceValue) / 100));

                double ServiceAlarm = ServiceValue - PercentAlarm;
                double ServiceWarning = ServiceValue - PercentWarning;
                double ServiceSchedule = ServiceValue - PercentSchedule;

                Double MinessWorking = Int64.Parse(Row.WorkingInCreareSevice) - ServiceSchedule;

                Double WorkingWarning = Int64.Parse(ServiceWarning.ToString()) + MinessWorking;
                Double WorkingAlarm = Int64.Parse(ServiceAlarm.ToString()) + MinessWorking;

                if (WorkingAlarm <= Working)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Crimson);
                    e.Row.Foreground = new SolidColorBrush(Colors.White);
                }

                else if (WorkingWarning <= Working)
                {
                    e.Row.Background = new SolidColorBrush(Colors.LightYellow);
                    e.Row.Foreground = new SolidColorBrush(Colors.Black);
                }

                else
                {
                    e.Row.Background = new SolidColorBrush(Colors.Snow);
                    e.Row.Foreground = new SolidColorBrush(Colors.Black);
                }




            }

        }

        private void Grid_ProgramService_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
        }

        private void ShowDeviceStatus(object sender, RoutedEventArgs e)
        {
            if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_BreakState))
            {
    
            ShowStatusDevice Child = new ShowStatusDevice();
            Child.Owner = App.Current.MainWindow;
            Child.Show();
            }
            else
                MessageBox.Show(UserManagment.MessageError);

        }

        private void ShowDeviceStatusTypeMachin(object sender, RoutedEventArgs e)
        {
            if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_TypeDeviceState))
            {
   
            ShowStatusDevice_TypeDevice Child = new ShowStatusDevice_TypeDevice();
            Child.Owner = App.Current.MainWindow;
            Child.Show();
            }
            else
                MessageBox.Show(UserManagment.MessageError);

        }

        private void Grid_ProgramPart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Grid_ProgramPart_LoadingRow(object sender, DataGridRowEventArgs e)// تشخیص آیا سرویس قطعات ایجاده شده دارای آلارم یا وارنینگ است
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ShowProgramCyclePart Row = e.Row.DataContext as ShowProgramCyclePart;
                int ServiceValue = int.Parse(Row.ValueService);

                Int64 Working;
                int DeviceCode = int.Parse(Row.DeviceCode);
                if (int.Parse(Row.CycleType) == 1)
                {

                    // var query2 = (from c in db.MN_TotalWorks where c.VehicleID == DeviceCode select c).Single();
                    //  Working = Int64.Parse(query2.TotalTime.ToString()) / 3600;
                    Working = Int64.Parse(Row.SumTotalWork.Replace("ساعت", ""));
                }
                else
                {

                    //var query2 = (from c in db.MN_TotalWorks where c.VehicleID == DeviceCode select c).Single();
                    //  Working = Int64.Parse(query2.TotalKM.ToString()) / 100000;
                    Working = Int64.Parse(Row.SumTotalWork.Replace("کیلومتر", ""));
                }
                int Warning = int.Parse(Row.Warning);
                int Alarm = int.Parse(Row.Alarm);
                int Schedule = int.Parse(Row.Schedule);


                double PercentAlarm = Math.Ceiling((Convert.ToDouble(Alarm) * Convert.ToDouble(ServiceValue) / 100));
                double PercentWarning = Math.Ceiling((Convert.ToDouble(Warning) * Convert.ToDouble(ServiceValue) / 100));
                double PercentSchedule = Math.Ceiling((Convert.ToDouble(Schedule) * Convert.ToDouble(ServiceValue) / 100));

                double ServiceAlarm = ServiceValue - PercentAlarm;
                double ServiceWarning = ServiceValue - PercentWarning;
                double ServiceSchedule = ServiceValue - PercentSchedule;

                Double MinessWorking = Int64.Parse(Row.WorkingInCreateService) - ServiceSchedule;

                Double WorkingWarning = Int64.Parse(ServiceWarning.ToString()) + MinessWorking;
                Double WorkingAlarm = Int64.Parse(ServiceAlarm.ToString()) + MinessWorking;

                if (WorkingAlarm <= Working)
                {
                    e.Row.Background = new SolidColorBrush(Colors.Crimson);
                    e.Row.Foreground = new SolidColorBrush(Colors.White);
                }

                else if (WorkingWarning <= Working)
                {
                    e.Row.Background = new SolidColorBrush(Colors.LightYellow);
                    e.Row.Foreground = new SolidColorBrush(Colors.Black);
                }

                else
                {
                    e.Row.Background = new SolidColorBrush(Colors.Snow);
                    e.Row.Foreground = new SolidColorBrush(Colors.Black);
                }



            }
        }

        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            Setting Child = new Setting();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();

        }

        private void ChangeStatusDevice(object sender, RoutedEventArgs e)
        {
            if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_ChangeState))
            {

            RegisterChangeStatus_Device Child = new RegisterChangeStatus_Device();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();
            }
            else
                MessageBox.Show(UserManagment.MessageError);

        }


        private void Grid_DragOver(object sender, DragEventArgs e)
        {


        }

        private void ChangeVisibleGridPart(object sender, RoutedEventArgs e)//نمایش گرید مربوط به سرویس قطعات
        {
            MenuItem Menu = sender as MenuItem;

            //  RibbonMenuItem Menu = sender as RibbonMenuItem;

            if (Menu.IsChecked == false)
            {

                Menu.IsChecked = true;
                Grid_ProgramPart.Visibility = Visibility.Visible;
                TextTitle_ProgramPart.Visibility = Visibility.Visible;
                BackgroundTitlePart.Visibility = Visibility.Visible;

                if (Grid_ProgramService.Visibility == Visibility.Hidden)
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                if (Grid_ProgramService.Visibility == Visibility.Visible)
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                if ((Grid_ProgramService.Visibility == Visibility.Hidden) &&
                        (Grid_ProgramPart.Visibility == Visibility.Visible))
                {
                    Column2.Width = new GridLength(0, GridUnitType.Star);
                }


            }
            else
            {
                Menu.IsChecked = false;
                Grid_ProgramPart.Visibility = Visibility.Hidden;
                TextTitle_ProgramPart.Visibility = Visibility.Hidden;
                BackgroundTitlePart.Visibility = Visibility.Hidden;
                if (Grid_ProgramService.Visibility == Visibility.Hidden)
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                else
                    Column1.Width = new GridLength(0, GridUnitType.Star);

            }





        }

        private void ChangeVisibleGridService(object sender, RoutedEventArgs e)//نمایش گرید مربوط به سرویس های دوره ای
        {
            MenuItem Menu = sender as MenuItem;
            // RibbonMenuItem Menu = sender as RibbonMenuItem;
            if (Menu.IsChecked == false)
            {
                Menu.IsChecked = true;
                Grid_ProgramService.Visibility = Visibility.Visible;
                TextTitle_ProgramService.Visibility = Visibility.Visible;
                BackgroundTitleService.Visibility = Visibility.Visible;
                if (Grid_ProgramPart.Visibility == Visibility.Hidden)
                    Column2.Width = new GridLength(1, GridUnitType.Star);
                if (Grid_ProgramPart.Visibility == Visibility.Visible)
                    Column2.Width = new GridLength(1, GridUnitType.Star);
                else if ((Grid_ProgramService.Visibility == Visibility.Visible) &&
                         (Grid_ProgramPart.Visibility == Visibility.Hidden))
                {
                    Column1.Width = new GridLength(0, GridUnitType.Star);
                }

            }
            else
            {
                Menu.IsChecked = false;
                Grid_ProgramService.Visibility = Visibility.Hidden;
                TextTitle_ProgramService.Visibility = Visibility.Hidden;
                BackgroundTitleService.Visibility = Visibility.Hidden;
                if (Grid_ProgramPart.Visibility == Visibility.Hidden)
                    Column2.Width = new GridLength(1, GridUnitType.Star);
                else
                    Column2.Width = new GridLength(0, GridUnitType.Star);




            }


        }

        private void ShowTabDevice(object sender, RoutedEventArgs e)
        {
            MenuItem Menu = sender as MenuItem;
            // RibbonMenuItem Menu = sender as RibbonMenuItem;
            if (Menu.IsChecked == false)
            {
                Menu.IsChecked = true;
                Row5.Height = new GridLength(0, GridUnitType.Auto);

            }

            else
            {
                Menu.IsChecked = false;
                Row5.Height = new GridLength(0);
                TabDefineService.Visibility = Visibility.Hidden;

            }
        }

        private void ExchangePart_Click(object sender, RoutedEventArgs e)
        {

            ShowDevice_ExchangePart Child = new ShowDevice_ExchangePart();
            Child.Owner = App.Current.MainWindow;
            Child.Show();

        }

        private void ChangeTitleServiceCycle()//تابع تغییر عنوان گرید مربوط به سرویس های دوره ای
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (LastStatus_Service != null)
                {


                    string DeviceCode = "";
                    string BrandCode = "";
                    string ModelCode = "";
                    string Value = "";
                    string TypeService = "";

                    string[] Array = new string[] { };
                    Array = LastStatus_Service.Split(',');
                    if (Array[0] == "device")
                    {
                        DeviceCode = Array[1];
                        var query = (from c in db.ADM_Vehicles where c.ID == int.Parse(DeviceCode) select c).Single();
                        Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                {
                                    TextTitle_ProgramService.Text = "درحال مشاهده برنامه های سرویس دوره ای دستگاه" + " " +
                                                                    query.Name;
                                }));

                    }
                    else if (Array[0] == "brand")
                    {
                        BrandCode = Array[1];
                        var query = (from c in db.ADM_Brands where c.ID == int.Parse(BrandCode) select c).Single();
                        Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                {
                                    TextTitle_ProgramService.Text = "درحال مشاهده برنامه های سرویس دوره ای برند" + " " +
                                                                    query.Name;
                                }));
                    }
                    else if (Array[0] == "model")
                    {
                        ModelCode = Array[1];
                        var query = (from c in db.ADM_Models where c.ID == int.Parse(ModelCode) select c).Single();
                        Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                {
                                    TextTitle_ProgramService.Text = "درحال مشاهده برنامه های سرویس دوره ای مدل" + " " +
                                                                    query.Name;
                                }));
                    }


                    else if (Array[0] == "valuekilometr")
                    {
                        Value = Array[1];
                        TypeService = "2";
                        if (Array[2] == "codemodel")
                        {
                            ModelCode = Array[3];
                            var query = (from c in db.ADM_Models where c.ID == int.Parse(ModelCode) select c).Single();
                            Dispatcher.BeginInvoke(
                                new Action(
                                    () =>
                                    {
                                        TextTitle_ProgramService.Text = "درحال مشاهده برنامه های سرویس دوره ای مدل" + " " +
                                                                        query.Name + " " + "سرویس" + " " + Value + " " +
                                                                        "کیلومتر";
                                    }));
                        }
                        else if (Array[2] == "codedevice")
                        {
                            DeviceCode = Array[3];
                            var query = (from c in db.ADM_Vehicles where c.ID == int.Parse(DeviceCode) select c).Single();
                            Dispatcher.BeginInvoke(
                                new Action(
                                    () =>
                                    {
                                        TextTitle_ProgramService.Text = "درحال مشاهده برنامه های سرویس دوره ای دستگاه" +
                                                                        " " + query.Name + " " + "سرویس" + " " + Value +
                                                                        " " + "کیلومتر";
                                    }));
                        }


                    }
                    else if (Array[0] == "valuetime")
                    {
                        Value = Array[1];
                        TypeService = "1";
                        if (Array[2] == "codemodel")
                        {
                            ModelCode = Array[3];

                            var query = (from c in db.ADM_Models where c.ID == int.Parse(ModelCode) select c).Single();
                            Dispatcher.BeginInvoke(
                                new Action(
                                    () =>
                                    {
                                        TextTitle_ProgramService.Text = "درحال مشاهده برنامه های سرویس دوره ای مدل" + " " +
                                                                        query.Name + " " + "سرویس" + " " + Value + " " +
                                                                        "ساعته";
                                    }));
                        }
                        else if (Array[2] == "codedevice")
                        {
                            DeviceCode = Array[3];
                            var query = (from c in db.ADM_Vehicles where c.ID == int.Parse(DeviceCode) select c).Single();
                            Dispatcher.BeginInvoke(
                                new Action(
                                    () =>
                                    {
                                        TextTitle_ProgramService.Text = "درحال مشاهده برنامه های سرویس دوره ای دستگاه" +
                                                                        " " + query.Name + " " + "سرویس" + " " + Value +
                                                                        " " + "ساعته";
                                    }));


                        }

                    }
                }
                else
                {
                    Dispatcher.BeginInvoke(
                        new Action(() => { TextTitle_ProgramService.Text = "درحال مشاهده تمام برنامه های سرویس دوره ای"; }));

                }
            }
        }

        private void ChangeTitlePartService() //تابع تغییر عنوان گرید مربوط به سرویس های قطعات
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                string DeviceCode = "";
                string CodeBrand = "";
                string ModelCode = "";
                string[] Array = new string[] { };
                if (LastStatus_Part != null)
                {
                    Array = LastStatus_Part.Split(',');

                    if (Array[0] == "device")
                    {
                        DeviceCode = Array[1];
                        var query = (from c in db.ADM_Vehicles where c.ID == int.Parse(DeviceCode) select c).Single();
                        Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                {
                                    TextTitle_ProgramPart.Text = "درحال مشاهده برنامه های قطعات دستگاه" + " " +
                                                                 query.Name;
                                }));
                    }
                    else if (Array[0] == "brand")
                    {
                        CodeBrand = Array[1];
                        var query = (from c in db.ADM_Brands where c.ID == int.Parse(CodeBrand) select c).Single();
                        Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                {
                                    TextTitle_ProgramPart.Text = "درحال مشاهده برنامه های قطعات برند" + " " + query.Name;
                                }));
                    }
                    else if (Array[0] == "model")
                    {
                        ModelCode = Array[1];
                        var query = (from c in db.ADM_Models where c.ID == int.Parse(ModelCode) select c).Single();
                        Dispatcher.BeginInvoke(
                            new Action(
                                () =>
                                {
                                    TextTitle_ProgramPart.Text = "درحال مشاهده برنامه های قطعات مدل" + " " + query.Name;
                                }));
                    }
                }
                else
                {

                    Dispatcher.BeginInvoke(
                        new Action(
                            () => { TextTitle_ProgramPart.Text = "درحال مشاهد تمام برنامه های تعویض و تعمیر قطعات "; }));
                }
            }
        }

        private void ChangeListStatusDevice(object sender, RoutedEventArgs e)
        {
            if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_ChangeStateList))
            {
 
            ShowListDevice_ChangeStatus Child = new ShowListDevice_ChangeStatus();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();
            }
            else
                MessageBox.Show(UserManagment.MessageError);

        }


        private void ShowProfileWindow(object sender, RoutedEventArgs e)
        {
            if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_DeviceProfile))
            {
                ShowListDevice_ShowProfile Child = new ShowListDevice_ShowProfile();
                Child.Owner = App.Current.MainWindow;
                Child.ShowDialog();
            }
            else
                MessageBox.Show(UserManagment.MessageError);

        }

        private void ShowHardWareStatus(object sender, RoutedEventArgs e)
        {
            if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_DeviceState))
            {

                HardwareStatus Child = new HardwareStatus();
                Child.Owner = App.Current.MainWindow;
                Child.Show();
            }
            else
                MessageBox.Show(UserManagment.MessageError);

        }

        private void ShowAlarm(object sender, RoutedEventArgs e)
        {
            if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_AlarmShow))
            {
                Alarm Child = new Alarm();
                Child.Owner = App.Current.MainWindow;
                Child.Show();
            }
            else
                MessageBox.Show(UserManagment.MessageError);

        }


        private void ReportProgramService_Click(object sender, RoutedEventArgs e)
        {
           if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_ReportVisitSycle))
            {

            ReportProgramService Child = new ReportProgramService();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();
            }
           else
               MessageBox.Show(UserManagment.MessageError);

        }

        private void ReportProgramPart_Click(object sender, RoutedEventArgs e)
        {
           if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_ReportServicePart))
            {


            ReportProgramPart Child = new ReportProgramPart();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();
            }
           else
               MessageBox.Show(UserManagment.MessageError);

        }

        private void ReportBadPart_Click(object sender, RoutedEventArgs e)
        {
             if (UserManagment.UserAccess(UserManagment.UserName, UserManagment.Operations.MN_ReportBreak))
            {
            ReportBadPart Child = new ReportBadPart();
            Child.Owner = App.Current.MainWindow;
            Child.ShowDialog();
            }
             else
                 MessageBox.Show(UserManagment.MessageError);


        }


        private void function_load_allprogram(object sender, EventArgs e) // ترد نمایش سرویس های رسیده
        {
            while (true)
            {

                this.Dispatcher.Invoke(new Action(() =>
                {

                    FillGridServiceCycle(sender, e);
                    FillGridServicePart(sender, e);
                    AddDeviceInTab(sender, e);

                }), null);


                Thread.Sleep((UpdateTime * 60) * 1000);

            }
        }


        private void ShowFile(object sender, EventArgs e) // تابع نمایش فایل ضمیمه شده به برنامه
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button Btn = sender as Button;

                var query = (from c in db.MN_CreateServices
                             join d in db.MN_DefineServiceCycles on c.ServiceCycleID equals d.ID
                             where c.ID == int.Parse(Btn.Tag.ToString())
                             select d).Single();


                if (File.Exists(SaveFile + query.FileURL))
                {
                    Grid_ProgramService.Items.Refresh();
                    System.Diagnostics.Process.Start(SaveFile + query.FileURL);

                }
                else
                    MessageBox.Show("فایل برنامه ضمیمه نشده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






        private void AttachFile(object sender, RoutedEventArgs e)// نمایش دیالوگ انتخاب فایل برای ضمیمه کردن
        {

            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                Button Btn = sender as Button;

                dlg.Filter = "*.pdf|*.pdf|All Files (*.*)|*.*";
                dlg.Title = "انتخاب فایل";
                Nullable<bool> Result = dlg.ShowDialog();


                if (Result == true)
                {
                    Flag_AttachFile = true;
                    FileName = dlg.FileName;
                    ServiceCode_AttachFile = Btn.Tag.ToString();
                }

            }
        }



        private void ShowGridServiceCycle(object sender, RoutedEventArgs e)// نمایش گرید سرویس های دوره ای
        {

            Column1.Width = new GridLength(0, GridUnitType.Star);
            Column2.Width = new GridLength(1, GridUnitType.Star);

            MnuItem_ProgramPart.IsChecked = false;
            MenuItem_ProgramService.IsChecked = true;


        }

        private void ShowGridServicePart(object sender, RoutedEventArgs e) //نمایش گرید سرویس های قطعات
        {

            Column2.Width = new GridLength(0, GridUnitType.Star);
            Column1.Width = new GridLength(1, GridUnitType.Star);

            MenuItem_ProgramService.IsChecked = false;
            MnuItem_ProgramPart.IsChecked = true;
        }

        private void ShowBothServiceCycle(object sender, RoutedEventArgs e)// نمایش هرو گرید سرویس های دوره ای و سرویس های قطعات
        {

            Column2.Width = new GridLength(1, GridUnitType.Star);
            Column1.Width = new GridLength(1, GridUnitType.Star);
            MenuItem_ProgramService.IsChecked = true;
            MnuItem_ProgramPart.IsChecked = true;

        }


        private void SelectServiceCycle()// اضافه شدن منو برای نمایش سرویس های دوره ای
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                MenuItem MenuAll = new MenuItem();
                MenuAll.Header = "همه دستگاه ها";
                MenuAll.Tag = "all,0";
                MenuAll.Cursor = Cursors.Hand;
                MenuAll.Click += FillGridServiceCycle;
                MenuAll.Click += FillGridServicePart;
                MenuAll.Click += ShowGridServiceCycle;
                Menu_SelectServiceCycle.Items.Add(MenuAll);


                var query = from c in db.ADM_Brands
                            join l in db.Lables on c.VehicleTypeID equals l.LID
                            select new { Type = c.Name, Code = c.ID, TypeMachin = l.LDscFa };

                MenuItem BrandMenu = new MenuItem();
                BrandMenu.Header = "بر اساس برند";
                BrandMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item = new MenuItem();
                    BrandMenu_Item.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item.Cursor = Cursors.Hand;
                    BrandMenu_Item.Tag = "brand," + brand.Code;
                    BrandMenu_Item.Click += FillGridServiceCycle;
                    BrandMenu_Item.Click += FillGridServicePart;
                    BrandMenu_Item.Click += ShowGridServiceCycle;
                    BrandMenu.Items.Add(BrandMenu_Item);
                }
                Menu_SelectServiceCycle.Items.Add(BrandMenu); //اضافه شدن منو برند ها


                MenuItem ModelMenu = new MenuItem(); // اضافه شدن بر اساس مدل
                ModelMenu.Header = "بر اساس مدل";
                ModelMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item2 = new MenuItem();
                    BrandMenu_Item2.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item2.Cursor = Cursors.Hand;
                    BrandMenu_Item2.Tag = "brand," + brand.Code;


                    var query2 = from c in db.ADM_Models
                                 join d in db.ADM_Brands on c.BrandID equals d.ID
                                 where d.ID == brand.Code
                                 select c;
                    foreach (var ModelItem in query2)
                    {
                        MenuItem ModelMenuItem = new MenuItem();
                        ModelMenuItem.Header = ModelItem.Name;
                        ModelMenuItem.Cursor = Cursors.Hand;
                        ModelMenuItem.Tag = "model," + ModelItem.ID;
                        ModelMenuItem.Click += FillGridServiceCycle;
                        ModelMenuItem.Click += FillGridServicePart;
                        ModelMenuItem.Click += ShowGridServiceCycle;
                        BrandMenu_Item2.Items.Add(ModelMenuItem);
                    }
                    ModelMenu.Items.Add(BrandMenu_Item2);
                }
                Menu_SelectServiceCycle.Items.Add(ModelMenu);


                MenuItem DeviceMenu = new MenuItem();
                DeviceMenu.Header = "بر اساس دستگاه";
                DeviceMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item2 = new MenuItem();
                    BrandMenu_Item2.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item2.Cursor = Cursors.Hand;

                    var query2 = from c in db.ADM_Models
                                 join d in db.ADM_Brands on c.BrandID equals d.ID
                                 where d.ID == brand.Code
                                 select c;
                    foreach (var ModelItem in query2)
                    {
                        MenuItem ModelMenu_Item = new MenuItem();
                        ModelMenu_Item.Header = ModelItem.Name;
                        ModelMenu_Item.Cursor = Cursors.Hand;

                        var query3 = from c in db.ADM_Vehicles where c.ModelID == ModelItem.ID select c;
                        foreach (var DeviceItem in query3)
                        {
                            MenuItem DeviceMenuItem_ = new MenuItem();
                            DeviceMenuItem_.Header = DeviceItem.Name;
                            DeviceMenuItem_.Cursor = Cursors.Hand;
                            DeviceMenuItem_.Tag = "device," + DeviceItem.ID;
                            DeviceMenuItem_.Click += FillGridServiceCycle;
                            DeviceMenuItem_.Click += FillGridServicePart;
                            DeviceMenuItem_.Click += ShowGridServiceCycle;
                            ModelMenu_Item.Items.Add(DeviceMenuItem_);
                        }

                        BrandMenu_Item2.Items.Add(ModelMenu_Item);
                    }
                    DeviceMenu.Items.Add(BrandMenu_Item2);
                }
                Menu_SelectServiceCycle.Items.Add(DeviceMenu);

            }
        }

        private void SelectServicePart() // اضافه شدن منو برای نمایش سرویس های قطعات
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                MenuItem MenuAll = new MenuItem();
                MenuAll.Header = "همه دستگاه ها";
                MenuAll.Tag = "all,0";
                MenuAll.Cursor = Cursors.Hand;
                MenuAll.Click += FillGridServiceCycle;
                MenuAll.Click += FillGridServicePart;
                MenuAll.Click += ShowGridServicePart;
                Menu_SelectServicePart.Items.Add(MenuAll);


                var query = from c in db.ADM_Brands
                            join l in db.Lables on c.VehicleTypeID equals l.LID
                            select new { Type = c.Name, Code = c.ID, TypeMachin = l.LDscFa };

                MenuItem BrandMenu = new MenuItem();
                BrandMenu.Header = "بر اساس برند";
                BrandMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item = new MenuItem();
                    BrandMenu_Item.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item.Cursor = Cursors.Hand;
                    BrandMenu_Item.Tag = "brand," + brand.Code;
                    BrandMenu_Item.Click += FillGridServiceCycle;
                    BrandMenu_Item.Click += FillGridServicePart;
                    BrandMenu_Item.Click += ShowGridServicePart;
                    BrandMenu.Items.Add(BrandMenu_Item);
                }
                Menu_SelectServicePart.Items.Add(BrandMenu); //اضافه شدن منو برند ها


                MenuItem ModelMenu = new MenuItem(); // اضافه شدن بر اساس مدل
                ModelMenu.Header = "بر اساس مدل";
                ModelMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item2 = new MenuItem();
                    BrandMenu_Item2.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item2.Cursor = Cursors.Hand;
                    BrandMenu_Item2.Tag = "brand," + brand.Code;


                    var query2 = from c in db.ADM_Models
                                 join d in db.ADM_Brands on c.BrandID equals d.ID
                                 where d.ID == brand.Code
                                 select c;
                    foreach (var ModelItem in query2)
                    {
                        MenuItem ModelMenuItem = new MenuItem();
                        ModelMenuItem.Header = ModelItem.Name;
                        ModelMenuItem.Cursor = Cursors.Hand;
                        ModelMenuItem.Tag = "model," + ModelItem.ID;
                        ModelMenuItem.Click += FillGridServiceCycle;
                        ModelMenuItem.Click += FillGridServicePart;
                        ModelMenuItem.Click += ShowGridServicePart;
                        BrandMenu_Item2.Items.Add(ModelMenuItem);
                    }
                    ModelMenu.Items.Add(BrandMenu_Item2);
                }
                Menu_SelectServicePart.Items.Add(ModelMenu);


                MenuItem DeviceMenu = new MenuItem();
                DeviceMenu.Header = "بر اساس دستگاه";
                DeviceMenu.Cursor = Cursors.Hand;

                foreach (var brand in query)
                {
                    MenuItem BrandMenu_Item2 = new MenuItem();
                    BrandMenu_Item2.Header = brand.TypeMachin + " " + brand.Type;
                    BrandMenu_Item2.Cursor = Cursors.Hand;

                    var query2 = from c in db.ADM_Models
                                 join d in db.ADM_Brands on c.BrandID equals d.ID
                                 where d.ID == brand.Code
                                 select c;
                    foreach (var ModelItem in query2)
                    {
                        MenuItem ModelMenu_Item = new MenuItem();
                        ModelMenu_Item.Header = ModelItem.Name;
                        ModelMenu_Item.Cursor = Cursors.Hand;

                        var query3 = from c in db.ADM_Vehicles where c.ModelID == ModelItem.ID select c;
                        foreach (var DeviceItem in query3)
                        {
                            MenuItem DeviceMenuItem_ = new MenuItem();
                            DeviceMenuItem_.Header = DeviceItem.Name;
                            DeviceMenuItem_.Cursor = Cursors.Hand;
                            DeviceMenuItem_.Tag = "device," + DeviceItem.ID;
                            DeviceMenuItem_.Click += FillGridServiceCycle;
                            DeviceMenuItem_.Click += FillGridServicePart;
                            DeviceMenuItem_.Click += ShowGridServicePart;
                            ModelMenu_Item.Items.Add(DeviceMenuItem_);
                        }

                        BrandMenu_Item2.Items.Add(ModelMenu_Item);
                    }
                    DeviceMenu.Items.Add(BrandMenu_Item2);
                }
                Menu_SelectServicePart.Items.Add(DeviceMenu);


            }
        }



        public void ShowProfile(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (!(Thread.ThreadState == ThreadState.Aborted))
                    Thread.Abort();
                Row2.Height = new GridLength(0);


                var query = (from c in db.ADM_Vehicles
                             join d in db.ADM_Models on c.ModelID equals d.ID
                             join b in db.ADM_Brands on d.BrandID equals b.ID
                             join k in db.MN_TotalWorks on c.ID equals k.VehicleID into wo
                             from k in wo.DefaultIfEmpty()
                             join l in db.Lables on b.VehicleTypeID equals l.LID
                             join s in db.VehicleLastStates on c.DeviceID equals s.DeviceID into st
                             from s in st.DefaultIfEmpty()
                             join o in db.MN_States on s.State equals o.Code into news
                             from o in news.DefaultIfEmpty()
                             where c.ID == DeviceCode_Profile
                             select
                                 new
                                 {
                                     Image = o.ImageVehicle_Profile,
                                     BrandName = b.Name,
                                     ModelName = d.Name,
                                     DeviceName = c.Name,
                                     TotalKilometer = k.TotalKM,
                                     TotalHourse = k.TotalTime,
                                     TypeMachin = l.LDscFa,
                                     TypeMachinCode = c.Type.ToString()
                                 }).Single();

                Image Image = new Image();
                BitmapImage src = new BitmapImage();

                string Path = UploadDir + "\\VehiclePics\\" + query.TypeMachinCode + "\\" + query.Image;

                if (!File.Exists(Path))
                    Path = "images/noimage.gif";

                FileStream stream =
                    new FileStream(Path,
                        FileMode.Open, FileAccess.Read);

                src.BeginInit();
                src.StreamSource = stream;
                src.EndInit();
                Image.Source = src;
                Image.Width = 150;
                Image.Height = 150;
                Image.Margin = new Thickness(0, 0, 0, 0);
                Image.VerticalAlignment = VerticalAlignment.Center;
                Image.HorizontalAlignment = HorizontalAlignment.Center;
                ImageDevice.Children.Clear();
                ImageDevice.Children.Add(Image);

                Text_BrandName.Text = query.TypeMachin + " " + query.BrandName;
                Text_ModelName.Text = query.ModelName;
                Text_DeviceName.Text = query.DeviceName;

                FillGridProfileServiceTime(sender, e);
                FillGridProfileServiceKilometer(sender, e);
                FillGridProfileServicePart(sender, e);
                //FillGridServicePart(sender, e);
                FillGridProfileBadPart(sender, e);

                Text_TotalKilometer.Text = (Convert.ToInt64(query.TotalKilometer / 1000)).ToString();
                Text_TotalHours.Text = (Convert.ToInt64(query.TotalHourse / 3600)).ToString();

                Load_Com_ListStatus();
                Load_Com_ListAlarm();
                FillGridProfileStatusDevice(sender, e);
                FillGrid_DefineServiceTime(sender, e);
                FillGridDefineServiceKilometer(sender, e);
                FillGridDefineServicePart(sender, e);
                FillGridEventRule(sender, e);
                FillGridAlarm(sender, e);

                Grid_ProgramService.Visibility = Visibility.Hidden;
                Grid_ProgramPart.Visibility = Visibility.Hidden;
                TextTitle_ProgramService.Visibility = Visibility.Hidden;
                TextTitle_ProgramPart.Visibility = Visibility.Hidden;
                BackgroundTitlePart.Visibility = Visibility.Hidden;
                BackgroundTitleService.Visibility = Visibility.Hidden;
                Profile.Visibility = Visibility.Visible;

                TabDefineService.Visibility = Visibility.Hidden;

                // Tab_ShowProfile.SelectedIndex = 0;
                //     TabSelect_Print = "ProfileGrid_ProgramServiceTime";
            }
        }

        public class ShowProfile_ServiceCycle
        {

            public string ServiceTitle { get; set; }
            public string FunctionService { get; set; }
            public string GroupRepair { get; set; }
            public string CreatDate { get; set; }
            public string CreatTime { get; set; }
            public string CreatServiceCode { get; set; }
            public string Status { get; set; }

            public string TotalWork { get; set; }

            public string AttachFile { get; set; }



        }

        public void FillGridProfileServiceTime(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_ServiceCycle> Items = new List<ShowProfile_ServiceCycle>();


                var query = (from c in db.MN_CreateServices
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefineServiceCycles on c.ServiceCycleID equals k.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join o in db.MN_CycleCriteriaTypes on k.CriteriaID equals o.ID
                            orderby c.ID descending
                            where c.VehicleID == DeviceCode_Profile && o.ID == 1

                            select
                                new
                                {
                                    CreatServiceCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    ServiceTitle = k.Title,
                                    GroupRepair = g.Type,
                                    CreatDate = c.CreateDate,
                                    CreatTime = c.CreateTime,
                                    FunctionService = c.ServiceWork,
                                    ModelCode = m.ID.ToString(),
                                    Confirm = c.Confirm,

                                    AttachFile = c.FileURL


                                }).Take(50);

                if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                {
                    int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                    int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));
                    query = from c in db.MN_CreateServices
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefineServiceCycles on c.ServiceCycleID equals k.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join o in db.MN_CycleCriteriaTypes on k.CriteriaID equals o.ID
                            orderby c.ID descending
                            where c.VehicleID == DeviceCode_Profile && o.ID == 1 && c.CreateDate >= StartDate && c.CreateDate <= EndDate

                            select
                                new
                                {
                                    CreatServiceCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    ServiceTitle = k.Title,
                                    GroupRepair = g.Type,
                                    CreatDate = c.CreateDate,
                                    CreatTime = c.CreateTime,
                                    FunctionService = c.ServiceWork,
                                    ModelCode = m.ID.ToString(),
                                    Confirm = c.Confirm,

                                    AttachFile = c.FileURL


                                };
                    FilterGrid_StartDate = "0";
                    FilterGrid_EndDate = "0";
                }

                foreach (var listprogram in query)
                {


                    ConvertDate dt = new ConvertDate();

                    string Status = "تایید شده";
                    if (listprogram.Confirm == false)
                        Status = "تایید نشده";
                    string AttachFile = "فایل ضمیمه شده ";
                    if (listprogram.AttachFile == null)
                        AttachFile = "فایل ضمیمه نشده ";

                    Items.Add(new ShowProfile_ServiceCycle()
                    {
                        CreatServiceCode = listprogram.CreatServiceCode.ToString(),
                        ServiceTitle = listprogram.ServiceTitle,
                        GroupRepair = listprogram.GroupRepair,
                        CreatDate = dt.DateToString(int.Parse(listprogram.CreatDate.ToString())),
                        CreatTime = listprogram.CreatTime.ToString(),
                        FunctionService = listprogram.FunctionService.ToString(),
                        Status = Status,
                        AttachFile = AttachFile
                    });
                }

                ProfileGrid_ProgramServiceTime.ItemsSource = Items;
            }
        }


        public void FillGridProfileServiceKilometer(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_ServiceCycle> Items = new List<ShowProfile_ServiceCycle>();


                var query = (from c in db.MN_CreateServices
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefineServiceCycles on c.ServiceCycleID equals k.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join o in db.MN_CycleCriteriaTypes on k.CriteriaID equals o.ID
                            orderby c.ID descending
                            where c.VehicleID == DeviceCode_Profile && o.ID == 2

                            select
                                new
                                {
                                    CreatServiceCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    ServiceTitle = k.Title,
                                    GroupRepair = g.Type,
                                    CreatDate = c.CreateDate,
                                    CreatTime = c.CreateTime,
                                    FunctionService = c.ServiceWork,
                                    ModelCode = m.ID.ToString(),
                                    Confirm = c.Confirm,

                                    AttachFile = c.FileURL


                                }).Take(50);

                if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                {
                    int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                    int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));
                    query = from c in db.MN_CreateServices
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefineServiceCycles on c.ServiceCycleID equals k.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join o in db.MN_CycleCriteriaTypes on k.CriteriaID equals o.ID
                            orderby c.ID descending
                            where c.VehicleID == DeviceCode_Profile && o.ID == 2 && c.CreateDate >= StartDate && c.CreateDate <= EndDate

                            select
                                new
                                {
                                    CreatServiceCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    ServiceTitle = k.Title,
                                    GroupRepair = g.Type,
                                    CreatDate = c.CreateDate,
                                    CreatTime = c.CreateTime,
                                    FunctionService = c.ServiceWork,
                                    ModelCode = m.ID.ToString(),
                                    Confirm = c.Confirm,

                                    AttachFile = c.FileURL


                                };
                    FilterGrid_StartDate = "0";
                    FilterGrid_EndDate = "0";
                }

                foreach (var listprogram in query)
                {


                    ConvertDate dt = new ConvertDate();

                    string Status = "تایید شده";
                    if (listprogram.Confirm == false)
                        Status = "تایید نشده";
                    string AttachFile = "فایل ضمیمه شده ";
                    if (listprogram.AttachFile == null)
                        AttachFile = "فایل ضمیمه نشده ";

                    Items.Add(new ShowProfile_ServiceCycle()
                    {
                        CreatServiceCode = listprogram.CreatServiceCode.ToString(),
                        ServiceTitle = listprogram.ServiceTitle,
                        GroupRepair = listprogram.GroupRepair,
                        CreatDate = dt.DateToString(int.Parse(listprogram.CreatDate.ToString())),
                        CreatTime = listprogram.CreatTime.ToString(),
                        FunctionService = listprogram.FunctionService.ToString(),
                        Status = Status,

                        AttachFile = AttachFile
                    });
                }

                ProfileGrid_ProgramServiceKilometer.ItemsSource = Items;
            }
        }

        public class ShowProfile_ServicePart
        {

            public string PartName { get; set; }
            public string FunctionPart { get; set; }
            public string GroupRepair { get; set; }
            public string TypeActivity { get; set; }
            public string CreateDate { get; set; }
            public string CreateTime { get; set; }
            public string CreatPartCode { get; set; }
            public string Status { get; set; }
            public string TotalWork { get; set; }

        }

        public void FillGridProfileServicePart(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_ServicePart> items = new List<ShowProfile_ServicePart>();

                var query = (from c in db.MN_CreatePartFixes
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefinePartModes on c.PartModeID equals k.ID
                            join l in db.MN_Parts on k.PartID equals l.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join t in db.MN_FixTypes on k.FixTypeID equals t.ID
                            where c.VehicleID == DeviceCode_Profile
                            orderby m.ID descending
                            select
                                new
                                {
                                    CreatPartCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    TypeActivity = t.Type,
                                    GroupRepair = g.Type,
                                    CreateDate = c.CreateDate,
                                    CreateTime = c.CreateTime,
                                    FunctionPart = c.PartWork,
                                    PartName = l.Name,
                                    Confirm = c.Confirm,

                                }).Take(50);

                if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                {
                    int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                    int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));

                    query = from c in db.MN_CreatePartFixes
                            join d in db.ADM_Vehicles on c.VehicleID equals d.ID
                            join m in db.ADM_Models on d.ModelID equals m.ID
                            join b in db.ADM_Brands on m.BrandID equals b.ID
                            join k in db.MN_DefinePartModes on c.PartModeID equals k.ID
                            join l in db.MN_Parts on k.PartID equals l.ID
                            join g in db.MN_Groups on k.GroupID equals g.ID
                            join t in db.MN_FixTypes on k.FixTypeID equals t.ID
                            where c.VehicleID == DeviceCode_Profile && c.CreateDate >= StartDate && c.CreateDate <= EndDate
                            orderby m.ID descending
                            select
                                new
                                {
                                    CreatPartCode = c.ID,
                                    DeviceName = d.Name,
                                    BrandName = b.Name,
                                    ModelName = m.Name,
                                    TypeActivity = t.Type,
                                    GroupRepair = g.Type,
                                    CreateDate = c.CreateDate,
                                    CreateTime = c.CreateTime,
                                    FunctionPart = c.PartWork,
                                    PartName = l.Name,
                                    Confirm = c.Confirm,

                                };
                    FilterGrid_StartDate = "0";
                    FilterGrid_EndDate = "0";

                }

                foreach (var listprogram in query)
                {

                    string Status = "تایید شده";
                    if (listprogram.Confirm == false)
                        Status = "تایید نشده";

                    ConvertDate dt = new ConvertDate();


                    items.Add(new ShowProfile_ServicePart()
                    {
                        CreatPartCode = listprogram.CreatPartCode.ToString(),
                        TypeActivity = listprogram.TypeActivity,
                        GroupRepair = listprogram.GroupRepair,
                        CreateDate = dt.DateToString(int.Parse(listprogram.CreateDate.ToString())),
                        CreateTime = listprogram.CreateTime.ToString(),
                        FunctionPart = listprogram.FunctionPart.ToString(),
                        PartName = listprogram.PartName,
                        Status = Status,

                    });
                }
                ProfileGrid_ProgramPart.ItemsSource = items;
            }
        }

        public class ShowProfile_BadPart
        {

            public string PartName { get; set; }
            public string BadDate { get; set; }
            public string BadTime { get; set; }
            public string RepairDate { get; set; }
            public string RepairTime { get; set; }
            public string GroupRepair { get; set; }
            public string TypeActivity { get; set; }

        }

        public void FillGridProfileBadPart(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_BadPart> Items = new List<ShowProfile_BadPart>();
                ConvertDate date = new ConvertDate();


                var query = from c in db.MN_PartFixes
                            join d in db.MN_Groups on c.GroupID equals d.ID
                            join m in db.MN_FixTypes on c.FixTypeID equals m.ID
                            join k in db.MN_Parts on c.PartID equals k.ID
                            where c.VehicleID == DeviceCode_Profile
                            orderby c.ID descending
                            select
                                new
                                {
                                    PartName = k.Name,
                                    BadDate = c.BreakDate,
                                    BadTime = c.BreakTime,
                                    RepairDate = c.FixDate,
                                    RepairTime = c.FixTime,
                                    GroupRepair = d.Type,
                                    TypeActivity = m.Type
                                };

                if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                {
                    int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                    int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));


                    query = from c in db.MN_PartFixes
                            join d in db.MN_Groups on c.GroupID equals d.ID
                            join m in db.MN_FixTypes on c.FixTypeID equals m.ID
                            join k in db.MN_Parts on c.PartID equals k.ID
                            where c.VehicleID == DeviceCode_Profile && (c.BreakDate >= StartDate && c.BreakDate <= EndDate) || (c.FixDate >= StartDate && c.FixDate <= EndDate)
                            orderby c.ID descending
                            select
                                new
                                {
                                    PartName = k.Name,
                                    BadDate = c.BreakDate,
                                    BadTime = c.BreakTime,
                                    RepairDate = c.FixDate,
                                    RepairTime = c.FixTime,
                                    GroupRepair = d.Type,
                                    TypeActivity = m.Type
                                };
                    FilterGrid_StartDate = "0";
                    FilterGrid_EndDate = "0";

                }


                foreach (var list in query)
                {
                    ConvertDate dt = new ConvertDate();

                    Items.Add(new ShowProfile_BadPart()
                    {
                        PartName = list.PartName,
                        BadDate = dt.DateToString(int.Parse(list.BadDate.ToString())),
                        BadTime = list.BadTime.ToString(),
                        RepairDate = dt.DateToString(int.Parse(list.RepairDate.ToString())),
                        RepairTime = list.RepairTime.ToString(),
                        GroupRepair = list.GroupRepair,
                        TypeActivity = list.TypeActivity
                    });
                }
                ProfileGrid_BadPart.ItemsSource = Items;

                Text_NumberBadPart.Text = query.Count().ToString();
            }
        }



        private void Load_Com_ListStatus()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Com_ListStatus.Items.Clear();

                var query = from c in db.MN_States where c.Parent != 0 && c.Parent <= 2 select c;

                ComboBoxItem comitem2 = new ComboBoxItem();
                comitem2.Content = "همه وضعیت ها";
                comitem2.Tag = "0";
                Com_ListStatus.Items.Add(comitem2);
                Com_ListStatus.SelectedIndex = 0;

                foreach (var items in query)
                {
                    ComboBoxItem comitem = new ComboBoxItem();
                    comitem.Content = items.Title;
                    comitem.Tag = items.Code.ToString();

                    Com_ListStatus.Items.Add(comitem);
                }
            }
        }


        private void Load_Com_ListAlarm()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Com_ListAlarm.Items.Clear();

                var query = from c in db.ADM_AlarmSymbols  select c;

                ComboBoxItem comitem2 = new ComboBoxItem();
                comitem2.Content = "همه آلارم ها";
                comitem2.Tag = "0";
                Com_ListAlarm.Items.Add(comitem2);
                Com_ListAlarm.SelectedIndex = 0;

                foreach (var items in query)
                {
                    ComboBoxItem comitem = new ComboBoxItem();
                    comitem.Content = items.Message;
                    comitem.Tag = items.Symbol.ToString();

                    Com_ListAlarm.Items.Add(comitem);
                }
            }
        }



        public class ShowProfile_DeviceStatus
        {

            public string DeviceCode { get; set; }
            public string Status { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }

        }

        public void FillGridProfileStatusDevice(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_DeviceStatus> items = new List<ShowProfile_DeviceStatus>();

                ComboBoxItem typeItem = (ComboBoxItem)Com_ListStatus.SelectedItem;
                string value = typeItem.Tag.ToString();
                var query = (from c in db.ADM_Vehicles
                            join d in db.VehicleStates on c.DeviceID equals d.DeviceID
                            join s in db.MN_States on d.State equals s.Code
                            orderby d.ID descending
                            where c.ID == DeviceCode_Profile

                            select
                                new
                                {
                                    DeviceCode = d.DeviceID,
                                    Status = s.Title,
                                    Time = d.Time,
                                    Date = d.Date,

                                }).Take(50);




                if (value != "0")
                {
                    query = (from c in db.ADM_Vehicles
                            join d in db.VehicleStates on c.DeviceID equals d.DeviceID
                            join s in db.MN_States on d.State equals s.Code
                            orderby d.ID descending
                            where c.ID == DeviceCode_Profile && s.Code == int.Parse(value)

                            select
                                new
                                {
                                    DeviceCode = d.DeviceID,
                                    Status = s.Title,
                                    Time = d.Time,
                                    Date = d.Date,

                                }).Take(50);
                }


                if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                {
                    int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                    int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));
                    query = from c in db.ADM_Vehicles
                            join d in db.VehicleStates on c.DeviceID equals d.DeviceID
                            join s in db.MN_States on d.State equals s.Code
                            orderby d.ID descending
                            where c.ID == DeviceCode_Profile && (d.Date >= StartDate && d.Date <= EndDate)


                            select
                                new
                                {
                                    DeviceCode = d.DeviceID,
                                    Status = s.Title,
                                    Time = d.Time,
                                    Date = d.Date,

                                };




                    if (value != "0")
                    {
                        query = from c in db.ADM_Vehicles
                                join d in db.VehicleStates on c.DeviceID equals d.DeviceID
                                join s in db.MN_States on d.State equals s.Code
                                orderby d.ID descending
                                where c.ID == DeviceCode_Profile && s.Code == int.Parse(value) && (d.Date >= StartDate && d.Date <= EndDate)

                                select
                                    new
                                    {
                                        DeviceCode = d.DeviceID,
                                        Status = s.Title,
                                        Time = d.Time,
                                        Date = d.Date,

                                    };
                    }



                    FilterGrid_StartDate = "0";
                    FilterGrid_EndDate = "0";


                }



                foreach (var list in query)
                {


                    ConvertDate ConvertDate = new ConvertDate();
                    string NewDate = ConvertDate.DateToString(int.Parse(list.Date.ToString()));

                    items.Add(new ShowProfile_DeviceStatus()
                    {
                        DeviceCode = list.DeviceCode.ToString(),
                        Status = list.Status,
                        Time = list.Time.ToString(),
                        Date = NewDate
                    });
                }

                ProfileGrid_DeviceStatus.ItemsSource = items;
            }

        }

        private void Com_ListStatus_DropDownClosed(object sender, EventArgs e)
        {
            FillGridProfileStatusDevice(sender, e);
        }

        private void Com_ListAlarm_DropDownClosed(object sender, EventArgs e)
        {
            FillGridAlarm(sender, e);
        }
        public class ShowProfile_DefineServiceTime
        {

            public string Title { get; set; }
            public string Value { get; set; }
            public string cycleType { get; set; }
            public string GroupRepair { get; set; }
            public string AttachFile { get; set; }
            public string DefineServiceCycleCode { get; set; }

        }

        private void FillGrid_DefineServiceTime(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_DefineServiceTime> items = new List<ShowProfile_DefineServiceTime>();

                var query = (from c in db.ADM_Vehicles
                             join d in db.MN_DefineVisitCycles on c.ModelID equals d.ModelID
                             join s in db.MN_DefineServiceCycles on d.ID equals s.VisitCycleID
                             join k in db.MN_Groups on s.GroupID equals k.ID
                             join o in db.MN_CycleCriteriaTypes on s.CriteriaID equals o.ID
                             orderby s.ID descending
                             where c.ID == DeviceCode_Profile && s.CriteriaID == 1

                             select
                                 new
                                 {
                                     Title = s.Title,
                                     Value = s.Value + " " + o.Type,
                                     GroupReapir = k.Type,
                                     AttachFile = s.FileURL,
                                     DefineServiceCycleCode = s.ID

                                 }).Union(from c in db.MN_DefineServiceCycles
                                          join k in db.MN_Groups on c.GroupID equals k.ID
                                          join o in db.MN_CycleCriteriaTypes on c.CriteriaID equals o.ID
                                          where c.VehicleID == DeviceCode_Profile && c.CriteriaID == 1
                                          select new
                                          {
                                              Title = c.Title,
                                              Value = c.Value + " " + o.Type,
                                              GroupReapir = k.Type,
                                              AttachFile = c.FileURL,
                                              DefineServiceCycleCode = c.ID
                                          });

                foreach (var list in query)
                {


                    string Attach = "فایل ضمیمه شده";
                    if (list.AttachFile == null)
                        Attach = "فایل ضمیمه نشده";

                    items.Add(new ShowProfile_DefineServiceTime()
                    {
                        Title = list.Title,
                        AttachFile = Attach,
                        GroupRepair = list.GroupReapir,
                        Value = list.Value.ToString(),
                        DefineServiceCycleCode = list.DefineServiceCycleCode.ToString()
                    });
                }

                ProfileGrid_DefineProgramServiceTime.ItemsSource = items;
            }

        }

        public class ShowProfile_DefineServiceKilometer
        {

            public string Title { get; set; }
            public string Value { get; set; }
            public string GroupRepair { get; set; }
            public string AttachFile { get; set; }

            public string DefineServiceCycleCode { get; set; }


        }

        private void FillGridDefineServiceKilometer(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_DefineServiceKilometer> items = new List<ShowProfile_DefineServiceKilometer>();





                var query = (from c in db.ADM_Vehicles
                             join d in db.MN_DefineVisitCycles on c.ModelID equals d.ModelID
                             join s in db.MN_DefineServiceCycles on d.ID equals s.VisitCycleID
                             join k in db.MN_Groups on s.GroupID equals k.ID
                             join o in db.MN_CycleCriteriaTypes on s.CriteriaID equals o.ID
                             orderby s.ID descending
                             where c.ID == DeviceCode_Profile && s.CriteriaID == 2

                             select
                                 new
                                 {
                                     Title = s.Title,
                                     Value = s.Value + " " + o.Type,
                                     GroupReapir = k.Type,
                                     AttachFile = s.FileURL,
                                     DefineServiceCycleCode = s.ID

                                 }).Union(from c in db.MN_DefineServiceCycles
                                          join k in db.MN_Groups on c.GroupID equals k.ID
                                          join o in db.MN_CycleCriteriaTypes on c.CriteriaID equals o.ID
                                          where c.VehicleID == DeviceCode_Profile && c.CriteriaID == 2
                                          select new
                                          {
                                              Title = c.Title,
                                              Value = c.Value + " " + o.Type,
                                              GroupReapir = k.Type,
                                              AttachFile = c.FileURL,
                                              DefineServiceCycleCode = c.ID
                                          });


                foreach (var list in query)
                {


                    string Attach = "فایل ضمیمه شده";
                    if (list.AttachFile == null)
                        Attach = "فایل ضمیمه نشده";

                    items.Add(new ShowProfile_DefineServiceKilometer()
                    {
                        Title = list.Title,
                        AttachFile = Attach,
                        GroupRepair = list.GroupReapir,
                        Value = list.Value.ToString(),
                        DefineServiceCycleCode = list.DefineServiceCycleCode.ToString()
                    });
                }

                ProfileGrid_DefineProgramServiceKilometer.ItemsSource = items;
            }
        }

        public class ShowProfile_DefineServicePart
        {

            public string PartName { get; set; }
            public string Value { get; set; }
            public string CycleType { get; set; }
            public string GroupRepair { get; set; }
            public string TypeActivity { get; set; }

        }

        private void FillGridDefineServicePart(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_DefineServicePart> items = new List<ShowProfile_DefineServicePart>();


                var query = from c in db.ADM_Vehicles
                            join d in db.MN_DefinePartModes on c.ID equals d.VehicleID
                            join s in db.MN_CycleCriteriaTypes on d.CriteriaID equals s.ID
                            join t in db.MN_FixTypes on d.FixTypeID equals t.ID
                            join k in db.MN_Groups on d.GroupID equals k.ID
                            join l in db.MN_Parts on d.PartID equals l.ID
                            orderby d.ID descending
                            where c.ID == DeviceCode_Profile

                            select
                                new
                                {
                                    PartName = l.Name,
                                    CycleType = s.Type,
                                    Value = d.Value,
                                    GroupReapir = k.Type,
                                    TypeActivity = t.Type

                                };


                foreach (var list in query)
                {
                    items.Add(new ShowProfile_DefineServicePart()
                    {
                        PartName = list.PartName,
                        TypeActivity = list.TypeActivity,
                        GroupRepair = list.GroupReapir,
                        Value = list.Value.ToString(),
                        CycleType = list.CycleType
                    });
                }

                ProfileGrid_DefineProgramPart.ItemsSource = items;
            }

        }

        private void show_file_profile(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button btn = sender as Button;

                var query = (
                    from d in db.MN_DefineServiceCycles
                    where d.ID == int.Parse(btn.Tag.ToString())
                    select d).Single();

                // File.Open(query.addressfile,FileMode.Open);
                if (File.Exists(SaveFile + query.FileURL))
                {
                    System.Diagnostics.Process.Start(SaveFile + query.FileURL);

                }
                else
                    MessageBox.Show("فایل برنامه ضمیمه نشده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);


            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }



        private void ShowAttachFile(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button btn = sender as Button;

                var query = (
                    from d in db.MN_CreateServices
                    where d.ID == int.Parse(btn.Tag.ToString())
                    select d).Single();


                if (File.Exists(SaveFile + query.FileURL))
                {
                    System.Diagnostics.Process.Start(SaveFile + query.FileURL);

                }
                else
                    MessageBox.Show("فایل برنامه ضمیمه نشده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);


            }

        }


        public class ShowProfile_EventRule
        {

            public string EventRuleName { get; set; }
            public string EventRuleText { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string DriverName { get; set; }
            public string ShapeName { get; set; }


        }

        public void FillGridEventRule(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_EventRule> items = new List<ShowProfile_EventRule>();

                var query = (from c in db.FM_EventRules
                            join d in db.FM_EventRuleFireds on c.ID equals d.EventID
                            join r in db.User_UserProfiles on Convert.ToInt32(d.UserID) equals r.ID
                            join s in db.FM_ShapeSubLayers on d.ShapeID equals s.ID
                            join m in db.ADM_Vehicles on d.DeviceID equals m.DeviceID
                             orderby c.ID descending
                            where m.ID == DeviceCode_Profile
                            select
                                new
                                {
                                    EventRuleName = c.Name,
                                    EventRuleText = c.AlarmMessage,
                                    Date = d.Date,
                                    Time = d.Time,
                                    DriverName = r.Name + " " + r.Family,
                                    ShapeNAme = s.NameShape
                                }).Take(50);

                if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                {

                    int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                    int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));
                    query = from c in db.FM_EventRules
                            join d in db.FM_EventRuleFireds on c.ID equals d.EventID
                            join r in db.User_UserProfiles on Convert.ToInt32(d.UserID) equals r.ID
                            join s in db.FM_ShapeSubLayers on d.ShapeID equals s.ID
                            join m in db.ADM_Vehicles on d.DeviceID equals m.DeviceID
                            where m.ID == DeviceCode_Profile && d.Date >= StartDate && d.Date <= EndDate
                            select
                                new
                                {
                                    EventRuleName = c.Name,
                                    EventRuleText = c.AlarmMessage,
                                    Date = d.Date,
                                    Time = d.Time,
                                    DriverName = r.Name + " " + r.Family,
                                    ShapeNAme = s.NameShape
                                };
                    FilterGrid_StartDate = "0";
                    FilterGrid_EndDate = "0";
                }

                foreach (var list in query)
                {
                    ConvertDate ConvertDate = new ConvertDate();
                    string NewDate = ConvertDate.DateToString(int.Parse(list.Date.ToString()));
                    items.Add(new ShowProfile_EventRule()
                    {
                        EventRuleName = list.EventRuleName,
                        EventRuleText = list.EventRuleText,
                        Date = NewDate,
                        Time = list.Time.ToString(),
                        DriverName = list.DriverName,
                        ShapeName = list.ShapeNAme
                    });
                }
                ProfileGrid_EventRule.ItemsSource = items;
            }
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

        public void FillGridAlarm(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                List<ShowProfile_Alarm> items = new List<ShowProfile_Alarm>();
                ComboBoxItem typeItem = (ComboBoxItem)Com_ListAlarm.SelectedItem;
                string value = typeItem.Tag.ToString();

                if (value == "0")
                {
                    var query = (from c in db.FM_AlarmReports
                                 join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                 join k in db.ADM_AlarmSymbols on c.Symbol.Replace(" ", "") equals k.Symbol.Replace(" ", "")

                                 where d.ID == DeviceCode_Profile 
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

                                     }).Take(50);

                    if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                    {

                        int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                        int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));
                       
                        query = from c in db.FM_AlarmReports
                                join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                join k in db.ADM_AlarmSymbols on c.Symbol.Replace(" ", "") equals k.Symbol.Replace(" ", "")

                                where d.ID == DeviceCode_Profile  && (c.StartDate >= StartDate && c.EndDate <= EndDate)/* || (c.EndDate >= StartDate && c.EndDate <= EndDate)*/
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

                                    };
                        FilterGrid_StartDate = "0";
                        FilterGrid_EndDate = "0";
                    }

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
                }

                else
                {
                    var query = from c in db.FM_AlarmReports
                                join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                join k in db.ADM_AlarmSymbols on c.Symbol.Replace(" ", "") equals k.Symbol.Replace(" ", "")

                                where d.ID == DeviceCode_Profile && k.Symbol.Replace(" ", "") == value.Replace(" ", "")
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

                                    };

                    if (FilterGrid_StartDate != "0" && FilterGrid_EndDate != "0")
                    {

                        int StartDate = int.Parse(FilterGrid_StartDate.Replace("/", ""));
                        int EndDate = int.Parse(FilterGrid_EndDate.Replace("/", ""));

                        query = from c in db.FM_AlarmReports
                                join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                join k in db.ADM_AlarmSymbols on c.Symbol.Replace(" ", "") equals k.Symbol.Replace(" ", "")

                                where d.ID == DeviceCode_Profile && k.Symbol.Replace(" ", "") == value.Replace(" ", "") && (c.StartDate >= StartDate && c.EndDate <= EndDate)/* || (c.EndDate >= StartDate && c.EndDate <= EndDate)*/
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

                                    };
                        FilterGrid_StartDate = "0";
                        FilterGrid_EndDate = "0";
                    }

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
                }
                
                ProfileGrid_Alaram.ItemsSource = items;
            }
        }



        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            // Thread.Abort();
            //  ShowProfile child=new ShowProfile();
            //   child.Close();

            Application.Current.Shutdown();

        }



        private void Function_TabSelect_Print(object sender, RoutedEventArgs e)
        {
            TabItem tab = sender as TabItem;
            TabSelect_Print = tab.Tag.ToString();


        }


        private void ReportProfile(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            ComboBoxItem typeItem = (ComboBoxItem)Com_ListStatus.SelectedItem;
            string value = typeItem.Tag.ToString();
            ComboBoxItem typeItem2 = (ComboBoxItem)Com_ListAlarm.SelectedItem;
            string valueAlarm = typeItem2.Tag.ToString();

            PrintProfile Child = new PrintProfile();
            Child.Owner = this;
            PrintProfile.TypePrint = btn.Tag.ToString();
            PrintProfile.TypeSelect_Tab = TabSelect_Print;
            PrintProfile.DeviceCode = DeviceCode_Profile;
            PrintProfile.StatusDeviceCode = int.Parse(value);
            PrintProfile.AlarmType = valueAlarm;
            Child.ShowDialog();


        }

        private void FilterGrid(object sender, RoutedEventArgs e)
        {
            if (TabSelect_Print != "ProfileGrid_DefineProgramServiceTime" &&
                TabSelect_Print != "ProfileGrid_DefineProgramServiceKilometer" &&
                TabSelect_Print != "ProfileGrid_DefineProgramPart")
            {

                FilterGrid Child = new FilterGrid();
                Child.Owner = this;
                maintenance.FilterGrid.TypeSelect_Tab = TabSelect_Print;
                Child.ShowDialog();
            }
        }



        private void ali_Click(object sender, RoutedEventArgs e)
        {


            int k = 0;
            var db = new DataClasses1DataContext(cnn);
            for (int i = 0; i <= 2600; i++)
            {
                if (k <= 650)
                {
                    InputData tb = new InputData();
                    {
                        tb.Distance = 0;
                        tb.WorkTime = 3600;
                        tb.DeviceID = 2001;
                    }
                    db.InputDatas.InsertOnSubmit(tb);
                    db.SubmitChanges();
                    k++;
                }

                InputData tb2 = new InputData();
                {
                    tb2.Distance = 25000;
                    tb2.WorkTime = 0;
                    tb2.DeviceID = 2002;
                }
                db.InputDatas.InsertOnSubmit(tb2);
                db.SubmitChanges();



            }

            MessageBox.Show("tamam shod");


        }



    }
}