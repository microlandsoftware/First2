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
    /// Interaction logic for listdevice_profile.xaml
    /// </summary>
    public partial class ShowListDevice_ShowProfile : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public Style FocusVisualStyle { get; set; }
        public ShowListDevice_ShowProfile()
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
           // load_Com_Brand();
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
                            join t in db.VehicleLastStates on c.ID equals Convert.ToInt32(t.DeviceID) into temp
                            from t in temp.DefaultIfEmpty()
                            join l in db.Lables on m.VehicleTypeID equals l.LID
                            where c.ModelID == int.Parse(ValueTag)
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


                            Items.Add(new Show_ListDevice()
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

       
        private void show_profile(object sender, RoutedEventArgs e)
        {


            Button Btn_Code = sender as Button;
            MainWindow.DeviceCode_Profile = int.Parse(Btn_Code.Tag.ToString());
            var myObject = this.Owner as MainWindow;
            myObject.ShowProfile(sender, e);

            this.Close();



        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Com_Brand();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

    }


}