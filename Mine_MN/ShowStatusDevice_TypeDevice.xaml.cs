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
    /// Interaction logic for show_device_status_typemachin.xaml
    /// </summary>
    public partial class ShowStatusDevice_TypeDevice : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string UploadDir = ini.IniReadValue("appSettings", "UploadDir");
        public static string NumberRows = ini.IniReadValue("appSettings", "NumberRows");
        private Thread Thread;
        private static int UpdateTime;
        private bool ConnectDataBase = true;
        public int CreateNewColumn = 10; 
        
        public ShowStatusDevice_TypeDevice()
        {
            InitializeComponent();
           
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                CreateNewColumn = int.Parse(NumberRows);
            }
            catch
            {

            }
               var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Plans select c;
                if (query.Count() >= 1)
                    UpdateTime = int.Parse(query.Single().VehicleThreadUpdate.ToString());
                else
                    UpdateTime = 10;
                    
                
                Thread = new Thread(function_load_device);
                Thread.IsBackground = true;
                Thread.Start();
            }
            else
                MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");
                
            
        }

        private void function_load_device()
        {
            while (true)
            {

                this.Dispatcher.Invoke(new Action(() =>
                {
                   

                        StatusDevice();
                    

                }), null);

                Thread.Sleep( UpdateTime*1000);
            }
        }

        private void StatusDevice()
        {
            try
            {
                this.Cursor = Cursors.Wait;
               
                var db = new DataClasses1DataContext(cnn);
              //  FillTrack();
                var query = from c in db.Lables
                    orderby c.LID
                    where c.LGrp == 6 && c.LSubGrp != 0 
                    select new {Title = c.LDscFa, TypeMachin = c.LID};
             //   PageGrid.Children.Clear();

                StackPanel MainPanel = new StackPanel();

                MainPanel.Orientation = Orientation.Horizontal;
                MainPanel.Margin = new Thickness(10, 10, 10, 10);

             
                int Skip = 0;
              
               
                foreach (var status in query)
                {
                    var queryCount = from c in db.ADM_Vehicles where c.Type == status.TypeMachin && c.DeviceID!=0 select c;

                    if (queryCount.Count() >= 1)
                    {
                        int Ration = queryCount.Count() / CreateNewColumn;
                        Skip = 0;
                        for (int i = 0; i <= Ration; i++)
                        {
                            StackPanel Panel = new StackPanel();

                            Panel.Orientation = Orientation.Vertical;
                            Panel.VerticalAlignment = VerticalAlignment.Top;
                            Panel.HorizontalAlignment = HorizontalAlignment.Center;
                            Panel.Margin = new Thickness(5, 5, 5, 5);
                            TextBlock text = new TextBlock();
                            text.Text = status.Title.ToString();
                            text.FontFamily = new FontFamily("B nazanin");
                            text.FontWeight = FontWeights.Bold;
                            text.TextAlignment = TextAlignment.Center;
                            text.Background = new SolidColorBrush(Colors.Silver);
                            text.FontSize = 16;
                            text.Width = 140;
                            Panel.Children.Add(text);
                            Separator Separator = new Separator();

                            int TypeMachin = int.Parse(status.TypeMachin.ToString());

                            var query2 = (from c in db.ADM_Vehicles where c.Type == TypeMachin && c.DeviceID != 0 select c).Skip(CreateNewColumn * i).Take(CreateNewColumn);
                        
                            Panel.Children.Add(Separator);



                            foreach (var Device in query2)
                            {
                                var query3 = from c in db.VehicleLastStates
                                             join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                             join s in db.MN_States on c.State equals s.Code
                                             join m in db.ADM_Models on d.ModelID equals m.ID
                                             where c.DeviceID == Device.DeviceID
                                             select
                                                 new
                                                 {
                                                     status = s.Title,
                                                     image = d.PictureUrl,
                                                     RMId = d.ID,

                                                     StatusCode = s.Code,
                                                     Icon = s.StatusIcon,
                                                     Name = d.Name,
                                                     DeviceID = d.DeviceID,
                                                     TypeMachin = d.Type,
                                                     ConditionCode = s.Code,
                                                     TypeMachinCode = d.Type,
                                                     Image_ByStat = s.ImageVehicle_BySatet,
                                                     Image_Alarm = s.ImageVehicle_ByAlarm,
                                                     Image_Warning = s.ImageVehicle_ByWarning
                                                 };
                                if (query3.Count() == 1)
                                {
                                    var q = query3.Single();

                                    GetVehicleService service = new GetVehicleService();

                                    string TypeService = service.GetServiceCycle_Status(Device.ID);
                                    string TypePart = service.GetServiceLot_Status(Device.ID);
                                    string ImageMachin = q.Image_ByStat;

                                    if (TypeService == "1" || TypePart == "1")
                                        ImageMachin = q.Image_Warning;
                                    if (TypeService == "2" || TypePart == "2")
                                        ImageMachin = q.Image_Alarm;






                                    TextBlock Text_Device = new TextBlock();

                                    Text_Device.Text = q.Name;
                                    Text_Device.FontFamily = new FontFamily("Tahoma");
                                    Text_Device.FontWeight = FontWeights.Bold;
                                    Text_Device.FontSize = 14;
                                    Text_Device.Width = 120;
                                    Text_Device.TextAlignment = TextAlignment.Center;
                                    Text_Device.Foreground = new SolidColorBrush(Colors.OrangeRed);
                                    Text_Device.Background = new SolidColorBrush(Colors.AliceBlue);

                                    string path = UploadDir + "\\VehiclePics\\" + q.TypeMachin + "\\" + ImageMachin;
                                    if ((!File.Exists(path)) || ImageMachin == null || ImageMachin == "")
                                        path = "images/noimage.gif";
                                    FileStream stream =
                                        new FileStream(path,
                                            FileMode.Open, FileAccess.Read);
                                    Image Image = new Image();
                                    BitmapImage src = new BitmapImage();
                                    src.BeginInit();
                                    src.StreamSource = stream;
                                    src.EndInit();
                                    Image.Source = src;
                                    Image.Width = 120;
                                    Image.Height = 120;
                                    Image.Margin = new Thickness(0, 0, 0, 0);
                                    Image.VerticalAlignment = VerticalAlignment.Top;
                                    Image.HorizontalAlignment = HorizontalAlignment.Center;
                                    Image.MouseLeftButtonDown += changeinfo_grid;
                                    Image.Tag = "device," + q.RMId;
                                    Image.Cursor = Cursors.Hand;



                                    Grid DynamicGrid = new Grid();
                                    //   DynamicGrid.ShowGridLines = true;
                                    ColumnDefinition gridCol1 = new ColumnDefinition();

                                    ColumnDefinition gridCol2 = new ColumnDefinition();

                                    DynamicGrid.ColumnDefinitions.Add(gridCol1);

                                    DynamicGrid.ColumnDefinitions.Add(gridCol2);

                                    RowDefinition gridRow1 = new RowDefinition();

                                    gridRow1.Height = new GridLength(0.3, GridUnitType.Star);

                                    RowDefinition gridRow2 = new RowDefinition();

                                    gridRow2.Height = new GridLength(0.3, GridUnitType.Star);

                                    RowDefinition gridRow3 = new RowDefinition();

                                    gridRow3.Height = new GridLength(0.9, GridUnitType.Star);

                                    DynamicGrid.RowDefinitions.Add(gridRow1);

                                    DynamicGrid.RowDefinitions.Add(gridRow2);

                                    DynamicGrid.RowDefinitions.Add(gridRow3);

                                    Border border = new Border();

                                    //border.Background = new SolidColorBrush(Colors.Bisque);

                                    border.BorderThickness = new Thickness(5);

                                    //  border.BorderBrush = new SolidColorBrush(Colors.OliveDrab);
                                    border.Height = 100;
                                    border.CornerRadius = new CornerRadius(15);
                                    border.Margin = new Thickness(1, 1, 2, 1);

                                    Grid.SetRow(Text_Device, 0);
                                    Grid.SetColumn(Text_Device, 0);
                                    Grid.SetColumnSpan(Text_Device, 2);
                                    Grid.SetRow(Image, 1);
                                    Grid.SetColumn(Image, 0);
                                    Grid.SetRowSpan(Image, 2);
                                    Grid.SetColumnSpan(Image, 2);


                                    DynamicGrid.Children.Add(Image);
                                    DynamicGrid.Children.Add(Text_Device);

                                    Image Icon = new Image();

                                    string PathIcon = UploadDir + "\\VehiclePics\\icon\\" + q.Icon;
                                    if ((!File.Exists(PathIcon)) || q.Icon == "" || q.Icon == null)
                                        PathIcon = "images/noicon.png";


                                    FileStream stream2 = new FileStream(PathIcon,
                                        FileMode.Open, FileAccess.Read);

                                    BitmapImage src_icon = new BitmapImage();
                                    src_icon.BeginInit();
                                    src_icon.StreamSource = stream2;
                                    src_icon.EndInit();
                                    Icon.Source = src_icon;
                                    Icon.Width = 30;
                                    Icon.Height = 30;
                                    Icon.Tag = "device," + q.RMId;
                                    Icon.Cursor = Cursors.Hand;
                                    Icon.MouseLeftButtonDown += changeinfo_grid;
                                    Icon.Margin = new Thickness(0, 0, 0, 25);
                                    Icon.VerticalAlignment = VerticalAlignment.Center;
                                    Icon.HorizontalAlignment = HorizontalAlignment.Center;

                                    Grid.SetRow(Icon, 2);
                                    Grid.SetColumn(Icon, 1);

                                    DynamicGrid.Children.Add(Icon);


                                    border.Child = DynamicGrid;


                                    Panel.Children.Add(border);

                                }

                            }
                            MainPanel.Children.Add(Panel);
                        }
                    }
                }

                PageGrid.Children.Add(MainPanel);

             
                this.Cursor = Cursors.Hand;
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

        public void FillTrack()

        {
            try
            {
                this.Cursor = Cursors.Wait;
                for (int i = 0; i < 4; i++)
                {
                    var db = new DataClasses1DataContext(cnn);
                    var query = from c in db.Lables
                        orderby c.LID
                        where c.LGrp == 6 && c.LSubGrp != 0 && c.LID == 32
                        select new {Title = c.LDscFa, TypeMachin = c.LID};
                    PageGrid.Children.Clear();

                    StackPanel MainPanel = new StackPanel();

                    MainPanel.Orientation = Orientation.Horizontal;
                    MainPanel.Margin = new Thickness(10, 10, 10, 10);

                    foreach (var status in query)
                    {

                        StackPanel Panel = new StackPanel();

                        Panel.Orientation = Orientation.Vertical;
                        Panel.VerticalAlignment = VerticalAlignment.Top;
                        Panel.HorizontalAlignment = HorizontalAlignment.Center;
                        Panel.Margin = new Thickness(5, 5, 5, 5);
                        TextBlock text = new TextBlock();
                        text.Text = status.Title.ToString();
                        text.FontFamily = new FontFamily("B nazanin");
                        text.FontWeight = FontWeights.Bold;
                        text.TextAlignment = TextAlignment.Center;
                        text.Background = new SolidColorBrush(Colors.Silver);
                        text.FontSize = 16;
                        text.Width = 140;
                        Panel.Children.Add(text);
                        Separator Separator = new Separator();

                        int TypeMachin = int.Parse(status.TypeMachin.ToString());
                        var query2 = (from c in db.ADM_Vehicles where c.Type == TypeMachin  select c).Take(20) ;

                        Panel.Children.Add(Separator);



                        foreach (var Device in query2)
                        {
                            var query3 = from c in db.VehicleLastStates
                                join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                join s in db.MN_States on c.State equals s.Code
                                join m in db.ADM_Models on d.ModelID equals m.ID
                                where c.DeviceID == Device.DeviceID
                                select
                                    new
                                    {
                                        status = s.Title,
                                        image = d.PictureUrl,
                                        RMId = d.ID,

                                        StatusCode = s.Code,
                                        Icon = s.StatusIcon,
                                        Name = d.Name,
                                        DeviceID = d.DeviceID,
                                        TypeMachin = d.Type,
                                        ConditionCode = s.Code,
                                        TypeMachinCode = d.Type,
                                        Image_ByStat = s.ImageVehicle_BySatet,
                                        Image_Alarm = s.ImageVehicle_ByAlarm,
                                        Image_Warning = s.ImageVehicle_ByWarning
                                    };
                            if (query3.Count() == 1)
                            {
                                var q = query3.Single();

                                GetVehicleService service = new GetVehicleService();

                                string TypeService = service.GetServiceCycle_Status(Device.ID);
                                string TypePart = service.GetServiceLot_Status(Device.ID);
                                string ImageMachin = q.Image_ByStat;

                                if (TypeService == "1" || TypePart == "1")
                                    ImageMachin = q.Image_Warning;
                                if (TypeService == "2" || TypePart == "2")
                                    ImageMachin = q.Image_Alarm;






                                TextBlock Text_Device = new TextBlock();

                                Text_Device.Text = q.Name;
                                Text_Device.FontFamily = new FontFamily("B nazanin");
                                Text_Device.FontWeight = FontWeights.Bold;
                                Text_Device.FontSize = 14;
                                Text_Device.Width = 120;
                                Text_Device.TextAlignment = TextAlignment.Center;
                                Text_Device.Foreground = new SolidColorBrush(Colors.OrangeRed);
                                Text_Device.Background = new SolidColorBrush(Colors.AliceBlue);

                                string path = UploadDir + "\\VehiclePics\\" + q.TypeMachin + "\\" + ImageMachin;
                                if ((!File.Exists(path)) || ImageMachin == null || ImageMachin == "")
                                    path = "images/noimage.gif";
                                FileStream stream =
                                    new FileStream(path,
                                        FileMode.Open, FileAccess.Read);
                                Image Image = new Image();
                                BitmapImage src = new BitmapImage();
                                src.BeginInit();
                                src.StreamSource = stream;
                                src.EndInit();
                                Image.Source = src;
                                Image.Width = 120;
                                Image.Height = 120;
                                Image.Margin = new Thickness(0, 0, 0, 0);
                                Image.VerticalAlignment = VerticalAlignment.Top;
                                Image.HorizontalAlignment = HorizontalAlignment.Center;
                                Image.MouseLeftButtonDown += changeinfo_grid;
                                Image.Tag = "device," + q.RMId;
                                Image.Cursor = Cursors.Hand;



                                Grid DynamicGrid = new Grid();
                                //   DynamicGrid.ShowGridLines = true;
                                ColumnDefinition gridCol1 = new ColumnDefinition();

                                ColumnDefinition gridCol2 = new ColumnDefinition();

                                DynamicGrid.ColumnDefinitions.Add(gridCol1);

                                DynamicGrid.ColumnDefinitions.Add(gridCol2);

                                RowDefinition gridRow1 = new RowDefinition();

                                gridRow1.Height = new GridLength(0.3, GridUnitType.Star);

                                RowDefinition gridRow2 = new RowDefinition();

                                gridRow2.Height = new GridLength(0.3, GridUnitType.Star);

                                RowDefinition gridRow3 = new RowDefinition();

                                gridRow3.Height = new GridLength(0.9, GridUnitType.Star);

                                DynamicGrid.RowDefinitions.Add(gridRow1);

                                DynamicGrid.RowDefinitions.Add(gridRow2);

                                DynamicGrid.RowDefinitions.Add(gridRow3);

                                Border border = new Border();

                                //border.Background = new SolidColorBrush(Colors.Bisque);

                                border.BorderThickness = new Thickness(5);

                                //  border.BorderBrush = new SolidColorBrush(Colors.OliveDrab);
                                border.Height = 100;
                                border.CornerRadius = new CornerRadius(15);
                                border.Margin = new Thickness(1, 1, 2, 1);

                                Grid.SetRow(Text_Device, 0);
                                Grid.SetColumn(Text_Device, 0);
                                Grid.SetColumnSpan(Text_Device, 2);
                                Grid.SetRow(Image, 1);
                                Grid.SetColumn(Image, 0);
                                Grid.SetRowSpan(Image, 2);
                                Grid.SetColumnSpan(Image, 2);


                                DynamicGrid.Children.Add(Image);
                                DynamicGrid.Children.Add(Text_Device);

                                Image Icon = new Image();

                                string PathIcon = UploadDir + "\\VehiclePics\\icon\\" + q.Icon;
                                if ((!File.Exists(PathIcon)) || q.Icon == "" || q.Icon == null)
                                    PathIcon = "images/noicon.png";


                                FileStream stream2 = new FileStream(PathIcon,
                                    FileMode.Open, FileAccess.Read);

                                BitmapImage src_icon = new BitmapImage();
                                src_icon.BeginInit();
                                src_icon.StreamSource = stream2;
                                src_icon.EndInit();
                                Icon.Source = src_icon;
                                Icon.Width = 30;
                                Icon.Height = 30;
                                Icon.Tag = "device," + q.RMId;
                                Icon.Cursor = Cursors.Hand;
                                Icon.MouseLeftButtonDown += changeinfo_grid;
                                Icon.Margin = new Thickness(0, 0, 0, 25);
                                Icon.VerticalAlignment = VerticalAlignment.Center;
                                Icon.HorizontalAlignment = HorizontalAlignment.Center;

                                Grid.SetRow(Icon, 2);
                                Grid.SetColumn(Icon, 1);

                                DynamicGrid.Children.Add(Icon);


                                border.Child = DynamicGrid;


                                Panel.Children.Add(border);

                            }

                        }
                        MainPanel.Children.Add(Panel);

                    }

                    PageGrid.Children.Add(MainPanel);


                    this.Cursor = Cursors.Hand;
                    ConnectDataBase = true;
                }
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
        

        public void changeinfo_grid(object sender, EventArgs e)
        {
          
            Image img = sender as Image;
           var myObject = this.Owner as MainWindow;
            myObject.FillGridServiceCycle(sender, e);
            myObject.FillGridServicePart(sender, e);
            
        }


    }
}
