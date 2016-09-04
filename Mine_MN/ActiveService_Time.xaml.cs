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
using System.Windows.Navigation;
using System.Windows.Shapes;
using maintenance.classes;

namespace maintenance
{
    /// <summary>
    /// </summary>
    public partial class ActiveService_Time : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public ActiveService_Time()
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
        private void Load_Com_Brand()//پر کردن کامبوباکس نمایش نوع برند
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.ADM_Brands
                    join l in db.Lables on c.VehicleTypeID equals l.LID
                    select new {type = c.Name, codebrand = c.ID, typemachin = l.LDscFa};
                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "لیست برند";
                ComItem2.Tag = "0";
                Com_Brand.Items.Add(ComItem2);
                ComboBoxItem ComItem3 = new ComboBoxItem();
                ComItem3.Content = "لیست مدل";
                ComItem3.Tag = "0";
                Com_Model.Items.Add(ComItem3);
                Com_Model.SelectedIndex = 0;
                foreach (var Items in query)
                {
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.typemachin + " " + Items.type;
                    ComItem.Tag = Items.codebrand.ToString();
                    Com_Brand.Items.Add(ComItem);
                }
                Com_Brand.SelectedIndex = 0;
            }
        }

        private void Com_Brand_DropDownClosed(object sender, EventArgs e)//پر کردن کامبوباکس مدل بعد از انتخاب برند
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem TypeItem = (ComboBoxItem) Com_Brand.SelectedItem;
                string value = TypeItem.Content.ToString();

                Com_Model.Items.Clear();
                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "لیست مدل";
                ComItem2.Tag = "0";
                Com_Model.Items.Add(ComItem2);
                Com_Model.SelectedIndex = 0;

                Text_CodeModel.Tag = "0";
                Fill_GridActiveService();

                Text_ValueServiceTime.Text = "";

                if (value != "لیست برند")
                {

                    string ValueTag = TypeItem.Tag.ToString();
                    var query = from c in db.ADM_Models where c.BrandID == int.Parse(ValueTag.ToString()) select c;
                    foreach (var Items in query)
                    {
                        ComboBoxItem ComItem = new ComboBoxItem();
                        ComItem.Content = Items.Name;
                        ComItem.Tag = Items.ID.ToString();
                        Com_Model.Items.Add(ComItem);
                    }
                }
            }

        }

        private void Com_Model_DropDownClosed(object sender, EventArgs e)// پر کردن دیتاگرید(نمایش دستگاه ها) بعد انتخاب مدل از داخل کامبوباکس
        {
           

            ComboBoxItem TypeItem = (ComboBoxItem)Com_Model.SelectedItem;
            if (Com_Model.SelectedItem != null)
            {
                string value = TypeItem.Content.ToString();
                if (value != "لیست مدل")
                {

                    Text_CodeModel.Tag = TypeItem.Tag.ToString();
                    Fill_GridActiveService();


                }
                else
                {
                    Text_CodeModel.Tag = "0";
                    Fill_GridActiveService();
                }
                

            }
            Text_ValueServiceTime.Text = "";

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Com_Brand();
        }

        public class show_activeservice
        {
            public string ModelName { get; set; }
            public string BrandName { get; set; }
            public string DeviceCode { get; set; }
            public string DeviceName { get; set; }
            public string TypeMachin { get; set; }

        }

        private void Fill_GridActiveService() //تابع پرکردن دیتاگرید 
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    join l in db.Lables on k.VehicleTypeID equals l.LID
                    where c.ModelID == int.Parse(Text_CodeModel.Tag.ToString()) && c.ActiveService_Time == false
                    select
                        new
                        {
                            DeviceCode = c.ID,
                            DeviceName = c.Name,
                            BrandName = k.Name,
                            ModelName = d.Name,
                            TypeMachin = l.LDscFa
                        };
                List<show_activeservice> items = new List<show_activeservice>();

                foreach (var Service in query)
                {
                    items.Add(new show_activeservice()
                    {
                        DeviceCode = Service.DeviceCode.ToString(),
                        ModelName = Service.ModelName,
                        BrandName = Service.TypeMachin + " " + Service.BrandName,
                        DeviceName = Service.DeviceName,
                    });
                }
                Grid_ShowActiveService.ItemsSource = items;
            }
        }

        private void TextValue_LostFocus(object sender, RoutedEventArgs e)//تشخیص مقدار وارد شده برای هر دستگاه
        {
            TextBox Text_Value = sender as TextBox;
            string Tag = Text_Value.Tag.ToString();
            string Value = Text_Value.Text.ToString();
            string[] AddValue=new string[]{};
            string[] AddValue2=new string[]{};
          if(Text_ValueServiceTime.Text=="")
            Text_ValueServiceTime.Text =Tag + "=" + Value + ",";
          else
          {
              AddValue= Text_ValueServiceTime.Text.Split(',');
              bool Add = false;
              for (int j = 0; j <= AddValue.Length - 1; j++)
              {
                  AddValue2 = AddValue[j].Split('=');
                  if (AddValue2[0].ToString() == Tag)
                  {
                      Add = true;
                      break;
                  }
              }

              if(Add==false)
                  Text_ValueServiceTime.Text = Text_ValueServiceTime.Text + Tag + "=" + Value + ",";
          }
            string[] Split = new string[]{};
            string Value2 = "";
            Split = Text_ValueServiceTime.Text.Split(',');
             string NewValue="";
            for (int i = 0; i <= Split.Length - 1; i++)
            {
                string[] value_service = new string[] {};
                string value_array = Split[i].ToString();
                if (value_array != "")
                {
                    value_service = value_array.Split('=');
                    NewValue = value_array + ",";
                    if (value_service[0] == Tag)
                    {
                        NewValue = Tag + "=" + Value + ",";
                    }

                    if (Value2 == "")
                        Value2 = NewValue;
                    else
                        Value2 = Value2 + NewValue;
                }
            }
            Text_ValueServiceTime.Text = Value2.ToString();


        }

        private void Verification_Click(object sender, RoutedEventArgs e) //افزودن مقدار اولیه کار کرد به دستگاه بعد از محاسبه تعداد سرویس های انجام شده 
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button Btn_Code = sender as Button;
                string DeviceCode = Btn_Code.Tag.ToString();

                string[] Value_Service = new string[] {};
                string[] Value_Tag = new string[] {};
                string ValueDevice = "";
                Value_Service = Text_ValueServiceTime.Text.Split(',');
                for (int i = 0; i <= Value_Service.Length - 1; i++)
                {
                    Value_Tag = Value_Service[i].Split('=');
                    if (Value_Tag[0] == DeviceCode)
                    {
                        ValueDevice = Value_Tag[1];
                        break;
                    }
                    if (ValueDevice == "")
                        ValueDevice = "0";

                }

                if (ValueDevice == "")
                    MessageBox.Show("لطفا مقدار کار کرد اولیه دستگاه را وارد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                {
                    var query = from c in db.ADM_Vehicles
                        join d in db.MN_DefineVisitCycles on c.ModelID equals d.ModelID
                        join m in db.MN_DefineServiceCycles on d.ID equals m.VisitCycleID
                        where c.ID == int.Parse(DeviceCode) && m.CriteriaID == 1
                        select c;
                    if (query.Count() < 1)
                        MessageBox.Show("برای این مدل دستگاه سرویس ساعت تعریف نشده است", "خطا", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else
                    {
                        var query2 = from c in db.ADM_Vehicles
                            join d in db.MN_DefineVisitCycles on c.ModelID equals d.ModelID
                            join m in db.MN_DefineServiceCycles on d.ID equals m.VisitCycleID
                            where c.ID == int.Parse(DeviceCode) && m.CriteriaID == 1
                            select m;
                        string MinValue = (query2.Min(v => v.Value.Value).ToString());
                        string MaxValue = (query2.Max(v => v.Value.Value).ToString());
                        int CounterService = 0;
                        int Mod;
                        if (int.Parse(ValueDevice) >= int.Parse(MaxValue))
                        {
                            int ration = int.Parse(ValueDevice)/int.Parse(MaxValue);
                            Mod = int.Parse(ValueDevice)%int.Parse(MaxValue);

                            CounterService = Mod/int.Parse(MinValue);


                        }
                        else
                        {
                            CounterService = int.Parse(ValueDevice)/int.Parse(MinValue);
                            Mod = int.Parse(ValueDevice);
                        }

                        var query3 = from c in db.MN_SumServices where c.VehicleID == int.Parse(DeviceCode) select c;
                        if (query3.Count() >= 1)
                        {
                            try
                            {
                                var query4 =
                                    (from c in db.MN_SumServices where c.VehicleID == int.Parse(DeviceCode) select c)
                                        .Single();
                                query4.TimeSum = Mod;
                                var query5 =
                                    (from c in db.ADM_Vehicles where c.ID == int.Parse(DeviceCode) select c).Single();
                                query5.ActiveService_Time = true;

                                db.SubmitChanges();


                                var query7 = from c in db.MN_TotalWorks where c.VehicleID == int.Parse(DeviceCode) select c;
                                if (query7.Count() == 0)
                                {
                                    MN_TotalWork tb = new MN_TotalWork();
                                    {
                                        tb.VehicleID = int.Parse(DeviceCode);
                                        tb.TotalTime =(Convert.ToInt64(ValueDevice)*3600);
                                        tb.TimePortion = 0;
                                       
                                    }
                                    db.MN_TotalWorks.InsertOnSubmit(tb);
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    var q = query7.Take(1).Single();
                                  //  Int64 Value = Convert.ToInt64(q.TotalTime);
                                  
                                        q.TotalTime =( Convert.ToInt64(ValueDevice)*3600);
                                        q.TimePortion = 0;
                                        db.SubmitChanges();

                                }

                               


                                foreach (var NumberService in query2)
                                {
                                    int service = Mod/int.Parse(NumberService.Value.ToString());
                                    var query6 = from c in db.MN_CounterServices
                                        where
                                            c.VehicleID == int.Parse(DeviceCode) &&
                                            c.ServiceCycleID == NumberService.ID
                                        select c;
                                    if (query6.Count() >= 1)
                                    {
                                        var q = query6.Single();
                                        q.TotalService_Time = service;
                                        
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        MN_CounterService TB = new MN_CounterService();
                                        {
                                            TB.VehicleID = int.Parse(DeviceCode);
                                            TB.ServiceCycleID = NumberService.ID;
                                            TB.TotalService_Time = service;
                                            
                                        }
                                        db.MN_CounterServices.InsertOnSubmit(TB);
                                        db.SubmitChanges();
                                    }
                                }




                                MessageBox.Show("میزان ساعت کارکرد اولیه به دستگاه مورد نظر اختصاص داده شد", "موفقيت",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
                                Fill_GridActiveService();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                            }

                        }
                        else
                        {
                            try
                            {
                                MN_SumService TB = new MN_SumService();
                                {
                                    TB.VehicleID = int.Parse(DeviceCode);
                                    TB.TimeSum = Mod;
                                    TB.KMSum = 0;

                                }
                                db.MN_SumServices.InsertOnSubmit(TB);

                                var query5 =
                                    (from c in db.ADM_Vehicles where c.ID == int.Parse(DeviceCode) select c).Single();
                                query5.ActiveService_Time = true;

                                db.SubmitChanges();

                                var query7 = from c in db.MN_TotalWorks where c.VehicleID == int.Parse(DeviceCode) select c;
                                if (query7.Count() == 0)
                                {
                                    MN_TotalWork tb = new MN_TotalWork();
                                    {
                                        tb.VehicleID = int.Parse(DeviceCode);
                                        tb.TotalTime = (Convert.ToInt64(ValueDevice)*3600);
                                        tb.TimePortion = 0;
                                        
                                    }
                                    db.MN_TotalWorks.InsertOnSubmit(tb);
                                    db.SubmitChanges();
                                }
                                else
                                {
                                    var q = query7.Take(1).Single();
                                    //  Int64 Value = Convert.ToInt64(q.TotalTime);

                                    q.TotalTime = (Convert.ToInt64(ValueDevice)*3600);
                                    q.TimePortion = 0;
                                    db.SubmitChanges();

                                }

                                


                                foreach (var NumberService in query2)
                                {
                                    int service = Mod/int.Parse(NumberService.Value.ToString());
                                    var query6 = from c in db.MN_CounterServices
                                        where
                                            c.VehicleID == int.Parse(DeviceCode) &&
                                            c.ServiceCycleID == NumberService.ID
                                        select c;
                                    if (query6.Count() >= 1)
                                    {
                                        var q = query6.Single();
                                        q.TotalService_Time = service;
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        MN_CounterService tb2 = new MN_CounterService();
                                        {
                                            tb2.VehicleID = int.Parse(DeviceCode);
                                            tb2.ServiceCycleID = NumberService.ID;
                                            tb2.TotalService_Time = service;
                                            
                                        }
                                        db.MN_CounterServices.InsertOnSubmit(tb2);
                                        db.SubmitChanges();
                                    }
                                }



                                MessageBox.Show("میزان ساعت کارکرد اولیه به دستگاه مورد نظر اختصاص داده شد", "موفقيت",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
                                Fill_GridActiveService();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                            }


                        }


                    }


                }

            }
        }
    }
}