using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using maintenance.classes;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for AddWorking.xaml
    /// </summary>
    public partial class AddWorking : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public AddWorking()
        {
            InitializeComponent();
        }

        private void canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {

                e.Handled = true;
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Com_Brand();
        }
        private void Load_Com_Brand()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.ADM_Brands
                            join l in db.Lables on c.VehicleTypeID equals l.LID
                            select new { Type = c.Name, BrandCode = c.ID, TypeMachin = l.LDscFa };


                foreach (var items in query)
                {
                    ComboBoxItem comitem = new ComboBoxItem();
                    comitem.Content = items.TypeMachin + " " + items.Type;
                    comitem.Tag = items.BrandCode.ToString();
                    Com_Barnd.Items.Add(comitem);
                }
            }
        }
        private void Text_ValueCycle_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Com_Barnd_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem TypeItem = (ComboBoxItem)Com_Barnd.SelectedItem;
                string Value = TypeItem.Content.ToString();
                if (Value != "لیست برند")
                {
                    string ValueTag = TypeItem.Tag.ToString();
                    Com_Model.Items.Clear();
                    ComboBoxItem ComItem2 = new ComboBoxItem();
                    ComItem2.Content = "لیست مدل";
                    ComItem2.Tag = "0";
                    Com_Model.Items.Add(ComItem2);
                    Com_Model.SelectedIndex = 0;
                    Com_Device.Items.Clear();
                    ComboBoxItem ComItem3 = new ComboBoxItem();
                    ComItem3.Content = "لیست دستگاه";
                    ComItem3.Tag = "0";
                    Com_Device.Items.Add(ComItem3);
                    Com_Device.SelectedIndex = 0;
                    var query = from c in db.ADM_Models where c.BrandID == int.Parse(ValueTag.ToString()) select c;
                    foreach (var Items in query)
                    {
                        ComboBoxItem ComItem = new ComboBoxItem();
                        ComItem.Content = Items.Name;
                        ComItem.Tag = Items.ID.ToString();
                        Com_Model.Items.Add(ComItem);
                    }

                    
                  Grid_RegisterAddWork.ItemsSource ="";
        

                }
            }
        }

        private void Com_Model_DropDownClosed(object sender, EventArgs e)
        {
            
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem TypeItem = (ComboBoxItem)Com_Model.SelectedItem;
                if (Com_Model.SelectedItem != null)
                {
                    string Value = TypeItem.Content.ToString();
                    if (Value != "لیست مدل")
                    {
                        string ValueTag = TypeItem.Tag.ToString();
                        Com_Device.Items.Clear();
                        ComboBoxItem ComItem2 = new ComboBoxItem();
                        ComItem2.Content = "لیست دستگاه";
                        ComItem2.Tag = "0";
                        Com_Device.Items.Add(ComItem2);
                        Com_Device.SelectedIndex = 0;
                        var query = from c in db.ADM_Vehicles where c.ModelID == int.Parse(ValueTag.ToString()) select c;
                        foreach (var Items in query)
                        {
                            ComboBoxItem ComItem = new ComboBoxItem();
                            ComItem.Content = Items.Name;
                            ComItem.Tag = Items.ID.ToString();
                            Com_Device.Items.Add(ComItem);
                        }


                    }
                    
                }
            }
            Grid_RegisterAddWork.ItemsSource ="";
        }

        private void Com_Device_DropDownClosed(object sender, EventArgs e)
        {
            
            Fill_GridSpecialService();
            
        }

        private void Btn_RegisterProgram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
        
           
            var db = new DataClasses1DataContext(cnn);
            ComboBoxItem Device = (ComboBoxItem)Com_Device.SelectedItem;
            if (Text_WorkKilometer.Text == "" || Text_WorkTime.Text == "")
            {
                MessageBox.Show("لطفا تمام مقادیر را وارد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
            }
            else if (Device.Tag.ToString() == "0")
                MessageBox.Show("لطفا دستگاه مورد نظر را انتخاب نمایید", "خطا", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            else if (Text_WorkKilometer.Text == "0" && Text_WorkTime.Text == "0")
                MessageBox.Show("لطفا کارکرد دستگاه را وارد نمایید", "خطا", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            else
            {
                if (
                    MessageBox.Show(" آیا از  مقدار کار کرد وارد شده اطمینان دارید؟ ", "هشدار",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {


                    var query7 = from c in db.ADM_Vehicles where c.ID == int.Parse(Device.Tag.ToString()) select c;

                    if (int.Parse(Text_WorkTime.Text) > 0)
                    {
                        if (query7.Take(1).Single().ActiveService_Time == false)
                        {
                            MessageBox.Show("برای مقدار دهی ساعت دستگاه ابتدا سرویس ساعت دستگاه مورد نظر را فعالسازی کنید ", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else if (int.Parse(Text_WorkKilometer.Text)>0)
                    {
                        if (query7.Take(1).Single().ActiveService_Kilometer == false)
                        {
                            MessageBox.Show("برای مقدار دهی کیلومتر دستگاه ابتدا سرویس کیلومتر دستگاه مورد نظر را فعالسازی کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                        
                    
                    
                    
                    MessageBox.Show("این عملیات ممکن است کمی طول بکشد لطفا منتظر بمانید","",MessageBoxButton.OK,MessageBoxImage.Information);
                    
                    int i = int.Parse(Text_WorkTime.Text);
                    if (i < (int.Parse(Text_WorkKilometer.Text)))
                        i = (int.Parse(Text_WorkKilometer.Text));
                    int WorkTime = int.Parse(Text_WorkTime.Text);
                    int WorkKilometer = int.Parse(Text_WorkKilometer.Text);
                    int InsertWoekTime = 0;
                    int InsertWorkKilometer = 0; 
                  //  var query = from c in db.ADM_Vehicles where c.ID == int.Parse(Device.Tag.ToString()) select c;

                 //   if (query.Count() >= 1)
                 //   {
                        for (int j = 1; j <= i; j++)
                        {
                            if (WorkTime >= 1)
                                InsertWoekTime = 3600;
                            else
                                InsertWoekTime = 0;
                            if (WorkKilometer >= 1)
                                InsertWorkKilometer = 1000;
                            else
                                InsertWorkKilometer = 0;

                            MN_ManualWork tb = new MN_ManualWork();
                            {
                                tb.VehicleID = int.Parse(Device.Tag.ToString());
                                tb.WorkTime = short.Parse(InsertWoekTime.ToString());
                                tb.Distance = short.Parse(InsertWorkKilometer.ToString());
                            }
                            db.MN_ManualWorks.InsertOnSubmit(tb);
                            db.SubmitChanges();

                            WorkTime = WorkTime - 1;
                            WorkKilometer = WorkKilometer - 1;

                        }

                        ConvertDate dt=new ConvertDate();
                        int DateNow = dt.GetDateTypeInt();
                        string TimeNow = DateTime.Now.ToString("HH:mm:ss");
                        MN_AddWork tb2=new MN_AddWork();
                        {
                            tb2.VehicleID = int.Parse(Device.Tag.ToString());
                            tb2.Date = DateNow;
                            tb2.Time = TimeSpan.Parse(TimeNow);
                            tb2.WorkKM = int.Parse(Text_WorkKilometer.Text);
                            tb2.WorkTime = short.Parse(Text_WorkTime.Text);
                        }
                        db.MN_AddWorks.InsertOnSubmit(tb2);
                        db.SubmitChanges();

                        var query = from c in db.MN_ManualWorks select c;
                        db.MN_ManualWorks.DeleteAllOnSubmit(query);
                        db.SubmitChanges();

                      
                        db.ExecuteCommand("truncate table MN_ManualWork");
                     
                       MessageBox.Show("برنامه با موفقیت به دستگاه اختصاص داده شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                        Fill_GridSpecialService();
                        Text_WorkKilometer.Text = "0";
                        Text_WorkTime.Text = "0";

                    }

                    /*else
                    {
                        MessageBox.Show("سخت افزار برای دستگاه تعریف نشده است", "خطا", MessageBoxButton.OK,
                   MessageBoxImage.Error);
                    }*/

                }
          //  }
           }
            catch (Exception)
           {
               MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK,
                 MessageBoxImage.Error);
                
          }
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public class Show_ListAddWork
        {
            public string DeviceName { get; set; }
            public string Time { get; set; }
            public string Date { get; set; }
            public string WorkTime { get; set; }
            public string WorkKM { get; set; }
           
        }

        private void Fill_GridSpecialService()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem Device = (ComboBoxItem)Com_Device.SelectedItem;
                if (Com_Device.SelectedItem != null)
                {
                    string Value = Device.Content.ToString();
                    if (Value != "لیست دستگاه")
                    {

                        string valuetag = Device.Tag.ToString();

                        var query = from c in db.MN_AddWorks
                                    join k in db.ADM_Vehicles on c.VehicleID equals k.ID
                                    where c.VehicleID == int.Parse(valuetag.ToString())
                                    orderby c.ID descending
                                    select
                                        new
                                        {
                                            c,k

                         
                                        };

                        List<Show_ListAddWork> Items = new List<Show_ListAddWork>();

                      ConvertDate dt=new ConvertDate();

                        foreach (var Service in query)
                        {
                            Items.Add(new Show_ListAddWork()
                            {
                                DeviceName = Service.k.Name,
                                Date = dt.DateToString(int.Parse(Service.c.Date.ToString())),
                                Time=Service.c.Time.ToString(),
                                WorkTime =Service.c.WorkTime.ToString(),
                                WorkKM = Service.c.WorkKM.ToString()
                            });
                        }
                        Grid_RegisterAddWork.ItemsSource = Items;
                    }

                }
            }
        }
    }
}
