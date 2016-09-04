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
using System.Threading;
namespace maintenance
{
    /// <summary>
    /// Interaction logic for report_badlot.xaml
    /// </summary>
    public partial class ReportBadPart : Window
    {
        public static string Value_Print;
        public static string Type_Print;
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public ReportBadPart()
        {
            InitializeComponent();
            this.Text_StartDate_BadPart.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            this.Text_StartDate_RepairPart.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            this.Text_EndDate_BadPart.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            this.Text_EndDate_RepairPart.FlowDirection = System.Windows.FlowDirection.RightToLeft;



        }

        private void Load_Com_Brand()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.ADM_Brands
                    join l in db.Lables on c.VehicleTypeID equals l.LID
                    select new {Type = c.Name, BrandCode = c.ID, TypeMachin = l.LDscFa};
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
                    ComboBoxItem comitem = new ComboBoxItem();
                    comitem.Content = Items.TypeMachin + " " + Items.Type;
                    comitem.Tag = Items.BrandCode.ToString();
                    Com_Brand.Items.Add(comitem);
                }
                Com_Brand.SelectedIndex = 0;
            }
        }


        private void Com_Brand_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                ComboBoxItem TypeItem = (ComboBoxItem) Com_Brand.SelectedItem;
                string Value = TypeItem.Content.ToString();
                Com_Model.Items.Clear();
                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "لیست مدل";
                ComItem2.Tag = "0";
                Com_Model.Items.Add(ComItem2);
                Com_Model.SelectedIndex = 0;
                if (Value != "لیست برند")
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
                Grid_ListDevice.ItemsSource = "";
            }
        }

        private void Com_Model_DropDownClosed(object sender, EventArgs e)
        {
            FillGrid_ShowListDevice();

        }

        public class show_listdevice
        {
            public string ModelName { get; set; }
            public string BrandName { get; set; }
            public string DeviceName { get; set; }
            public string DeviceCode { get; set; }


        }


        private void FillGrid_ShowListDevice()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                List<show_listdevice> Items = new List<show_listdevice>();
                ComboBoxItem TypeItem = (ComboBoxItem) Com_Model.SelectedItem;
                if (Com_Model.SelectedItem != null)
                {
                    string Value = TypeItem.Content.ToString();
                    if (Value != "لیست مدل")
                    {
                        string valuetag = TypeItem.Tag.ToString();
                        var query = from c in db.ADM_Vehicles
                            join d in db.ADM_Models on c.ModelID equals d.ID
                            join m in db.ADM_Brands on d.BrandID equals m.ID
                            join t in db.VehicleLastStates on c.ID equals Convert.ToInt32(t.DeviceID) into temp
                            from t in temp.DefaultIfEmpty()
                            join l in db.Lables on m.VehicleTypeID equals l.LID
                            where c.ModelID == int.Parse(valuetag)
                            orderby c.Name
                            select
                                new
                                {
                                    DeviceCode = c.ID,
                                    DeviceName = c.Name,
                                    BrandName = m.Name,
                                    ModelName = d.Name,
                                    TypeMachin = l.LDscFa
                                };

                        foreach (var list in query)
                        {


                            Items.Add(new show_listdevice()
                            {
                                DeviceCode = list.DeviceCode.ToString(),
                                ModelName = list.ModelName,
                                BrandName = list.TypeMachin + " " + list.BrandName,
                                DeviceName = list.DeviceName,


                            });
                        }
                        Grid_ListDevice.ItemsSource = Items;
                    }

                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Com_Brand();
            ConvertDate Date = new ConvertDate();
            string DateNow = Date.GetDateNow();
            Text_StartDate_BadPart.Text = DateNow;
            Text_EndDate_BadPart.Text = DateNow;
            Text_StartDate_RepairPart.Text = DateNow;
            Text_EndDate_RepairPart.Text = DateNow;
            Text_StartDate_RepairPart.IsEnabled = false;
            Text_EndDate_RepairPart.IsEnabled = false;
        }

        private void ShowReport(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button Btn_Code = sender as Button;
                var query =
                    (from c in db.ADM_Vehicles where c.ID == int.Parse(Btn_Code.Tag.ToString()) select c).Single();

                string Type = "";
                if (BtnRadio_BadPart.IsChecked == true)
                {
                    if (CheckDate(Text_StartDate_BadPart.Text))
                    {
                        if (CheckDate(Text_EndDate_BadPart.Text))
                        {
                            Type = "badlot";
                        }
                    }

                }
                else if (BtnRadio_RepairPart.IsChecked == true)
                {
                    if (CheckDate(Text_StartDate_RepairPart.Text))
                    {
                        if (CheckDate(Text_EndDate_RepairPart.Text))
                        {
                            Type = "repairlot";
                        }
                    }


                }
                if (Type != "")
                {

                    ReportSetting Child = new ReportSetting();
                    if (BtnRadio_BadPart.IsChecked == true)
                        Value_Print = Text_StartDate_BadPart.Text + "," + Text_EndDate_BadPart.Text + "," +
                                      Btn_Code.Tag.ToString() + "," + Type;
                    else if (BtnRadio_RepairPart.IsChecked == true)
                        Value_Print = Text_StartDate_RepairPart.Text + "," + Text_EndDate_RepairPart.Text + "," +
                                      Btn_Code.Tag.ToString() + "," + Type;
                    Type_Print = "badlot";
                    ReportSetting.Type = "badlot";
                    Child.Text_TitleReport.Text = " گزارش خرابی قطعه دستگاه" + " " + query.Name;
                    Child.Owner = this;
                    Grid_ListDevice.Items.Refresh();
                    Child.ShowDialog();
                    this.Focus();
                }
            }
        }

        private void BtnRadio_BadPart_Click(object sender, RoutedEventArgs e)
        {
            if (BtnRadio_BadPart.IsChecked == true)
            {
                
                Text_StartDate_RepairPart.IsEnabled = false;
                Text_EndDate_RepairPart.IsEnabled = false;
                Text_StartDate_BadPart.IsEnabled = true;
                Text_EndDate_BadPart.IsEnabled = true;
            }
            
        }

        private void BtnRadio_RepairPart_Click(object sender, RoutedEventArgs e)
        {
            if (BtnRadio_RepairPart.IsChecked == true)
            {

                Text_StartDate_BadPart.IsEnabled = false;
                Text_EndDate_BadPart.IsEnabled = false;
                Text_StartDate_RepairPart.IsEnabled = true;
                Text_EndDate_RepairPart.IsEnabled = true;
            }
        }

        private void WindowsFormsHost_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }
        private bool CheckDate(string Date)
        {
            bool Flag = true;
            if (Date.Contains(" "))
            {
                MessageBox.Show("لطفا در وارد کردن تاریخ دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                Flag = false;
            }
            else
            {
                string[] Array = new string[] { };
                Array = Date.Split('/');
                string Year = Array[0];
                string Month = Array[1];
                string Day = Array[2];
                if ((int.Parse(Month) > 12) || int.Parse(Day) > 31)
                {
                    MessageBox.Show("لطفا در وارد کردن تاریخ دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    Flag = false;
                }
                else if ((int.Parse(Month) >= 7 && int.Parse(Month) <= 12) && int.Parse(Day) == 31)
                {
                    MessageBox.Show("نیمه دوم سال 30 روزه است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    Flag = false;
                }
                else if (int.Parse(Year) <= 1350)
                {
                    MessageBox.Show("لطفا در وارد کردن تاریخ دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    Flag = false;
                }
            }
            return Flag;
        }
    }
}
