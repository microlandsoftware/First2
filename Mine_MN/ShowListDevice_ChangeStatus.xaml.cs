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
    /// Interaction logic for changelist_status_device.xaml
    /// </summary>
    public partial class ShowListDevice_ChangeStatus : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public ShowListDevice_ChangeStatus()
        {
            InitializeComponent();
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
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.TypeMachin + " " + Items.Type;
                    ComItem.Tag = Items.BrandCode.ToString();
                    Com_Brand.Items.Add(ComItem);
                }
                Com_Brand.SelectedIndex = 0;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Com_Brand();
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
                        ComboBoxItem comitem = new ComboBoxItem();
                        comitem.Content = Items.Name;
                        comitem.Tag = Items.ID.ToString();
                        Com_Model.Items.Add(comitem);
                    }



                    Grid_ListDevice.ItemsSource = null;
                }

            }
        }

        private void Com_Model_DropDownClosed(object sender, EventArgs e)
        {
            FillGrid_ShowListDevice();

        }

        public class Show_ListDevice
        {
            public string ModelName { get; set; }
            public string BrandName { get; set; }
            public string DeviceName { get; set; }
            public string DeviceCode { get; set; }
            public string Status { get; set; }
           

        }


        private void FillGrid_ShowListDevice()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                List<Show_ListDevice> Items = new List<Show_ListDevice>();
                ComboBoxItem TypeItem = (ComboBoxItem) Com_Model.SelectedItem;
                if (Com_Model.SelectedItem != null)
                {
                    string Value = TypeItem.Content.ToString();
                    if (Value != "لیست مدل")
                    {
                        string ValueTag = TypeItem.Tag.ToString();
                        var query = from c in db.ADM_Vehicles
                            join d in db.ADM_Models on c.ModelID equals d.ID
                            join m in db.ADM_Brands on d.BrandID equals m.ID
                            join t in db.VehicleLastStates on c.DeviceID equals Convert.ToInt32(t.DeviceID) into temp
                            from t in temp.DefaultIfEmpty()
                            join k in db.MN_States on t.State equals k.Code into temp2
                            from k in temp2.DefaultIfEmpty()
                            join l in db.Lables on m.VehicleTypeID equals l.LID
                            where c.ModelID == int.Parse(ValueTag)
                            select
                                new
                                {
                                    deviceCode = c.ID,
                                    DeviceName = c.Name,
                                    BrandName = m.Name,
                                    ModelName = d.Name,
                                    Status = k.Title,
                                    TypeMachin = l.LDscFa
                                };

                        foreach (var list in query)
                        {

                            string Status = list.Status;
                            if (list.Status == null)
                                Status = "وضعیتی موجود نیست";


                            Items.Add(new Show_ListDevice()
                            {
                                DeviceCode = list.deviceCode.ToString(),
                                ModelName = list.ModelName,
                                BrandName = list.TypeMachin + " " + list.BrandName,
                                DeviceName = list.DeviceName,
                                Status = Status,

                            });
                        }
                        Grid_ListDevice.ItemsSource = Items;
                    }

                }
            }
        }

        private void change_status(object sender, RoutedEventArgs e)
        {

            RegisterChangeList_DeviceStauts Child = new RegisterChangeList_DeviceStauts();

            Button Btn_Code = sender as Button;
            Child.Text_DeviceName.Tag = Btn_Code.Tag.ToString();
            Child.ShowDialog();
            FillGrid_ShowListDevice();

        }

    }


}