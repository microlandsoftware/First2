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
    /// Interaction logic for show_device_status.xaml
    /// </summary>
    public partial class ShowStatusDevice : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string UploadDir = ini.IniReadValue("appSettings", "UploadDir");
        public static string NumberRows = ini.IniReadValue("appSettings", "NumberRows");
        private Thread Thread;
        private static int UpdateTime ;
        private static int TypeMaching=0;
        private bool ConnectDataBase = true;
        public int CreateNewColumn = 10; 
       
        public ShowStatusDevice()
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
                Load_Com_SelectDevice();
                var query = from c in db.MN_Plans select c;
                if (query.Count() >= 1)
                    UpdateTime = int.Parse(query.Single().VehicleThreadUpdate.ToString());
                else
                    UpdateTime = 10;
                    
                
                Thread = new Thread(LoadDevice);
                Thread.IsBackground = true;
                Thread.Start();
            }
            else
                MessageBox.Show("عدم برقراری ارتباط با پایگاه داده");
                
            
        }

        private void LoadDevice()
        {
            while (true)
            {
               
                this.Dispatcher.Invoke(new Action(() =>
                {

                    DeviceStatus();
                  
                }), null);

                Thread.Sleep(UpdateTime * 1000); 
            }
        }

       
        public void DeviceStatus()
        {
            try
            {
            var db = new DataClasses1DataContext(cnn);
            
                this.Cursor = Cursors.Wait;
                this.IsEnabled = false;
                
               


                PageGrid.Children.Clear();
                StackPanel MainPanel = new StackPanel();
                MainPanel.Orientation = Orientation.Horizontal;
                MainPanel.Margin = new Thickness(10, 10, 10, 10);

                var query = from c in db.MN_States where c.Parent != 0 && c.Parent <= 2 select c;



                foreach (var status in query)
                {
                    int StatusCode = Convert.ToInt32(status.Code);
                     int Skip = 0;
                     var query2 = from c in db.VehicleLastStates
                        join d in db.ADM_Vehicles on Convert.ToInt32(c.DeviceID) equals d.DeviceID
                        join t in db.MN_States on c.State equals t.Code
                        where c.State == StatusCode
                        select
                            new
                            {
                                DeviceName = d.Name,
                                DeviceCode = d.ID,
                                Icon = t.StatusIcon,
                                ID = d.ID,
                                DeviceID = c.DeviceID,
                                TypeMachinCode = d.Type,
                                Image_ByStat = t.ImageVehicle_BySatet,
                                Image_Alarm = t.ImageVehicle_ByAlarm,
                                Image_Warning = t.ImageVehicle_ByWarning

                            };

                     if (TypeMaching != 0)
                     {
                         query2 = (from c in db.VehicleLastStates
                                   join d in db.ADM_Vehicles on Convert.ToInt32(c.DeviceID) equals d.DeviceID
                                   join t in db.MN_States on c.State equals t.Code
                                   where c.State == StatusCode && d.Type == TypeMaching
                                   select
                                       new
                                       {
                                           DeviceName = d.Name,
                                           DeviceCode = d.ID,
                                           Icon = t.StatusIcon,
                                           ID = d.ID,
                                           DeviceID = c.DeviceID,
                                           TypeMachinCode = d.Type,
                                           Image_ByStat = t.ImageVehicle_BySatet,
                                           Image_Alarm = t.ImageVehicle_ByAlarm,
                                           Image_Warning = t.ImageVehicle_ByWarning
                                       });
                     }
                      
                     if (query2.Count() >= 1)
                     {
                         int Ration = query2.Count() / CreateNewColumn;
                         Skip = 0;
                         for (int i = 0; i <= Ration; i++)
                         {


                             StackPanel Panel = new StackPanel();

                             Panel.Orientation = Orientation.Vertical;
                             Panel.VerticalAlignment = VerticalAlignment.Top;
                             Panel.HorizontalAlignment = HorizontalAlignment.Center;
                             Panel.Margin = new Thickness(5, 5, 5, 5);

                             TextBlock Text = new TextBlock();
                             Text.Text = status.Title.ToString();
                             Text.FontFamily = new FontFamily("B nazanin");
                             Text.FontWeight = FontWeights.Bold;
                             Text.TextAlignment = TextAlignment.Center;
                             Text.Background = new SolidColorBrush(Colors.Silver);
                             Text.FontSize = 16;
                             Text.Width = 140;
                             //text.Height = 40;
                             Panel.Children.Add(Text);

                             Separator separator = new Separator();

                             separator.Width = 140;
                             Panel.Children.Add(separator);


                             query2 = (from c in db.VehicleLastStates
                                       join d in db.ADM_Vehicles on Convert.ToInt32(c.DeviceID) equals d.DeviceID
                                       join t in db.MN_States on c.State equals t.Code
                                       where c.State == StatusCode 
                                       select
                                           new
                                           {
                                               DeviceName = d.Name,
                                               DeviceCode = d.ID,
                                               Icon = t.StatusIcon,
                                               ID = d.ID,
                                               DeviceID = c.DeviceID,
                                               TypeMachinCode = d.Type,
                                               Image_ByStat = t.ImageVehicle_BySatet,
                                               Image_Alarm = t.ImageVehicle_ByAlarm,
                                               Image_Warning = t.ImageVehicle_ByWarning
                                           }).Skip(CreateNewColumn * i).Take(CreateNewColumn);




                             if (TypeMaching != 0)
                             {
                                 query2 = (from c in db.VehicleLastStates
                                           join d in db.ADM_Vehicles on Convert.ToInt32(c.DeviceID) equals d.DeviceID
                                           join t in db.MN_States on c.State equals t.Code
                                           where c.State == StatusCode && d.Type == TypeMaching
                                           select
                                               new
                                               {
                                                   DeviceName = d.Name,
                                                   DeviceCode = d.ID,
                                                   Icon = t.StatusIcon,
                                                   ID = d.ID,
                                                   DeviceID = c.DeviceID,
                                                   TypeMachinCode = d.Type,
                                                   Image_ByStat = t.ImageVehicle_BySatet,
                                                   Image_Alarm = t.ImageVehicle_ByAlarm,
                                                   Image_Warning = t.ImageVehicle_ByWarning
                                               }).Skip(CreateNewColumn * i).Take(CreateNewColumn);
                             }




                             foreach (var Device in query2)
                             {
                                 GetVehicleService service = new GetVehicleService();

                                 string typeservice = service.GetServiceCycle_Status(Device.ID);
                                 string typelot = service.GetServiceLot_Status(Device.ID);
                                 string imagemachin = Device.Image_ByStat;

                                 if (typeservice == "1" || typelot == "1")
                                     imagemachin = Device.Image_Warning;
                                 if (typeservice == "2" || typelot == "2")
                                     imagemachin = Device.Image_Alarm;







                                 TextBlock Text_Device = new TextBlock();

                                 Text_Device.Text = Device.DeviceName;
                                 Text_Device.FontFamily = new FontFamily("Tahoma");
                                 Text_Device.FontWeight = FontWeights.Bold;
                                 Text_Device.FontSize = 14;
                                 Text_Device.TextAlignment = TextAlignment.Center;
                                 Text_Device.Foreground = new SolidColorBrush(Colors.OrangeRed);
                                 Text_Device.Background = new SolidColorBrush(Colors.AliceBlue);
                                 Text_Device.Width = 120;
                                 Text_Device.HorizontalAlignment = HorizontalAlignment.Center;
                                 Text_Device.VerticalAlignment = VerticalAlignment.Top;
                                 Text_Device.Height = 25;

                                 Image Image = new Image();

                                 string path = UploadDir + "\\VehiclePics\\" + Device.TypeMachinCode + "\\" + imagemachin;

                                 if ((!File.Exists(path)) || imagemachin == null || imagemachin == "")
                                     path = "images/noimage.gif";

                                 FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);

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
                                 Image.MouseLeftButtonDown += Show_ChangeStatusDevice;
                                 Image.Tag = Device.DeviceCode;
                                 Image.Cursor = Cursors.Hand;

                                 Image Icon = new Image();

                                 string PathIcon = UploadDir + "\\VehiclePics\\icon\\" + Device.Icon;
                                 if (!File.Exists(PathIcon) || Device.Icon == null || Device.Icon == "")
                                     PathIcon = "images/noicon.png";

                                 FileStream stream3 = new FileStream(PathIcon,
                                     FileMode.Open, FileAccess.Read);

                                 BitmapImage Src_Icon = new BitmapImage();
                                 Src_Icon.BeginInit();
                                 Src_Icon.StreamSource = stream3;
                                 Src_Icon.EndInit();
                                 Icon.Source = Src_Icon;
                                 Icon.Width = 30;
                                 Icon.Height = 30;
                                 Icon.Tag = Device.DeviceCode;
                                 Icon.Cursor = Cursors.Hand;
                                 Icon.MouseLeftButtonDown += Show_ChangeStatusDevice;
                                 Icon.Margin = new Thickness(0, 0, 0, 25);
                                 Icon.VerticalAlignment = VerticalAlignment.Center;
                                 Icon.HorizontalAlignment = HorizontalAlignment.Center;



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

                                 Grid.SetRow(Icon, 2);
                                 Grid.SetColumn(Icon, 1);

                                 DynamicGrid.Children.Add(Image);
                                 DynamicGrid.Children.Add(Icon);
                                 DynamicGrid.Children.Add(Text_Device);
                                 border.Child = DynamicGrid;


                                 Panel.Children.Add(border);

                             }


                             MainPanel.Children.Add(Panel);
                         }
                     }
                }




                PageGrid.Children.Add(MainPanel);

               

                this.Cursor = Cursors.Hand;
                this.IsEnabled = true;
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
                else
                {
                    
                    this.Cursor = Cursors.Hand;
                    this.IsEnabled = true;

                }
            }
        }

        private void Show_ChangeStatusDevice(object sender, RoutedEventArgs e)
        {
            RegisterChangeStatus_Device Child = new RegisterChangeStatus_Device();
            Image img= sender as Image;
            Child.Text_DeviceCode.Tag = img.Tag.ToString();
            Child.Owner = this;
            Thread.Abort();
            Child.ShowDialog();
         //   device_status();
            if (Thread.ThreadState == ThreadState.Aborted)
            {
                Thread = new Thread(LoadDevice);
                Thread.IsBackground = true;
                Thread.Start();
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


        


        

        private void Load_Com_SelectDevice()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.Lables
                    orderby c.LID
                    where c.LGrp == 6 && c.LSubGrp != 0
                    select new {Title = c.LDscFa, TypeMachin = c.LID};
                ComboBoxItem comitem2 = new ComboBoxItem();
                comitem2.Content = "همه دستگاه ها";
                comitem2.Tag = 0;
                int add = Com_SelectDevice.Items.Add(comitem2);

                foreach (var items in query)
                {
                    ComboBoxItem comitem = new ComboBoxItem();
                    comitem.Content = items.Title;
                    comitem.Tag = items.TypeMachin.ToString();

                    Com_SelectDevice.Items.Add(comitem);
                }
                Com_SelectDevice.SelectedIndex = 0;
            }
        }

        private void Com_SelectDevice_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem device = (ComboBoxItem) Com_SelectDevice.SelectedItem;
                TypeMaching = int.Parse(device.Tag.ToString());
                DeviceStatus();
            }

        }


    }
}
