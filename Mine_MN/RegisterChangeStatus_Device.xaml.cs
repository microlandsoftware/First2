using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using maintenance.classes;
using MessageBox = System.Windows.MessageBox;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for change_device_status.xaml
    /// </summary>
    public partial class RegisterChangeStatus_Device : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public RegisterChangeStatus_Device()
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

                Com_Device.Items.Clear();
                ComboBoxItem ComItem4 = new ComboBoxItem();
                ComItem4.Content = "لیست دستگاه";
                ComItem4.Tag = "0";
                Com_Device.Items.Add(ComItem4);
                Com_Device.SelectedIndex = 0;

                foreach (var items in query)
                {
                    ComboBoxItem comitem = new ComboBoxItem();
                    comitem.Content = items.TypeMachin + " " + items.Type;
                    comitem.Tag = items.BrandCode.ToString();
                    Com_Brand.Items.Add(comitem);
                }
                Com_Brand.SelectedIndex = 0;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_Com_Brand();


            if (Text_DeviceCode.Tag != null)
            {
                var db = new DataClasses1DataContext(cnn);
                if (db.DatabaseExists())
                {
                    var query = (from c in db.ADM_Vehicles
                        join d in db.ADM_Models on c.ModelID equals d.ID
                        join b in db.ADM_Brands on d.BrandID equals b.ID
                        where c.ID == int.Parse(Text_DeviceCode.Tag.ToString())
                        select new {ModelCode = d.ID, BrandCode = b.ID, DeviceCode = c.ID}).Single();

                    Com_Brand.SelectedValue = query.BrandCode.ToString();
                    Com_Brand_DropDownClosed(sender, e);

                    Com_Model.SelectedValue = query.ModelCode.ToString();
                    Com_Model_DropDownClosed(sender, e);


                    Com_Device.SelectedValue = query.DeviceCode.ToString();
                    Com_Device_DropDownClosed(sender, e);
                    Com_Model.IsEnabled = false;
                    Com_Brand.IsEnabled = false;
                    Com_Device.IsEnabled = false;

                }


                
            }

            Load_Com_Conditions();
        }

        private void Com_Brand_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);

            if (db.DatabaseExists())
            {
                ComboBoxItem TypeItem = (ComboBoxItem) Com_Brand.SelectedItem;
                string Value = TypeItem.Content.ToString();
                Text_Status.Text = "";
                Text_Status_Actual.Visibility = Visibility.Hidden;
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

                if (Value != "لیست برند")
                {

                    string ValueTag = TypeItem.Tag.ToString();
                    var query = from c in db.ADM_Models where c.BrandID == int.Parse(ValueTag.ToString()) select c;
                    foreach (var items in query)
                    {
                        ComboBoxItem comitem = new ComboBoxItem();
                        comitem.Content = items.Name;
                        comitem.Tag = items.ID.ToString();
                        Com_Model.Items.Add(comitem);
                    }



                }
            }

        }

        private void Com_Model_DropDownClosed(object sender, EventArgs e)
        {

            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Text_Status.Text = "";
                Text_Status_Actual.Visibility = Visibility.Hidden;

                ComboBoxItem TypeItem = (ComboBoxItem) Com_Model.SelectedItem;
                string Value = TypeItem.Content.ToString();


                Com_Device.Items.Clear();
                ComboBoxItem ComItem3 = new ComboBoxItem();
                ComItem3.Content = "لیست دستگاه";
                ComItem3.Tag = "0";
                Com_Device.Items.Add(ComItem3);
                Com_Device.SelectedIndex = 0;

                if (Value != "لیست مدل")
                {

                    string valuetag = TypeItem.Tag.ToString();
                    var query = from c in db.ADM_Vehicles where c.ModelID == int.Parse(valuetag.ToString()) select c;
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

        private void Load_Com_Conditions()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                var query = from c in db.MN_States where c.Show == true && c.Parent <= 2 select c;
                Com_Conditions.Items.Clear();
                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "لیست وضعیت ها";
                ComItem2.Tag = "0";
                Com_Conditions.Items.Add(ComItem2);


                foreach (var Items in query)
                {
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.Title;
                    ComItem.Tag = Items.Code.ToString();
                    Com_Conditions.Items.Add(ComItem);
                }
                Com_Conditions.SelectedIndex = 0;
            }
        }

        private void Com_Device_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Text_Status.Text = "";
                Text_Status_Actual.Visibility = Visibility.Hidden;

                ComboBoxItem typeItem = (ComboBoxItem) Com_Device.SelectedItem;
                string value = typeItem.Content.ToString();

                if (value != "لیست دستگاه")
                {
                    string valuetag = typeItem.Tag.ToString();
                    var query = from c in db.VehicleLastStates
                        join m in db.ADM_Vehicles on c.DeviceID equals m.DeviceID
                        join d in db.MN_States on c.State equals d.Code
                        where m.ID == int.Parse(valuetag)
                        select new {Status = d.Title};
                    if (query.Count() == 1)
                    {
                        var q = query.Single();
                        Text_Status_Actual.Visibility = Visibility.Visible;
                        Text_Status.Text = q.Status;

                    }
                    else
                    {
                        Text_Status_Actual.Visibility = Visibility.Visible;
                        Text_Status.Text = "وضعیتی موجود نیست";

                    }


                }

            }
        }

        private void Btn_RegisterStatus_Click(object sender, RoutedEventArgs e)
        {
           var db=new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ConvertDate Date = new ConvertDate();

                ComboBoxItem TypeItem = (ComboBoxItem) Com_Device.SelectedItem;
                string ValueTag_Device = TypeItem.Tag.ToString();


                ComboBoxItem TypeItem2 = (ComboBoxItem) Com_Conditions.SelectedItem;
                string ValueTag_Conditons = TypeItem2.Tag.ToString();


                if (ValueTag_Device == "0")
                    MessageBox.Show("لطفا دستگاه مورد نظر را انتخاب نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (ValueTag_Conditons == "0")
                    MessageBox.Show("لطفا وضعیت جدید دستگاه را انتخاب نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                {
                    try
                    {
                        var query2 = (from c in db.ADM_Vehicles
                            where c.ID == int.Parse(ValueTag_Device)
                            select new {DeviceID = c.DeviceID}).Single();

                        var query = from c in db.VehicleLastStates
                            join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                            where d.ID == int.Parse(ValueTag_Device)
                            select c;



                        if (query.Count() == 1)
                        {

                            var q = query.Single();
                            q.State = Convert.ToByte(ValueTag_Conditons);
                            q.Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));
                            q.Date = Date.GetDateTypeInt();
                            db.SubmitChanges();

                            VehicleState TB = new VehicleState();
                            {
                                TB.DeviceID = Convert.ToInt16(query2.DeviceID);
                                TB.State = Convert.ToByte(ValueTag_Conditons);
                                TB.Date = Date.GetDateTypeInt();
                                TB.Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

                            }
                            db.VehicleStates.InsertOnSubmit(TB);
                            db.SubmitChanges();
                        }



                        else
                        {

                            VehicleLastState TB = new VehicleLastState();
                            {
                                TB.DeviceID = Convert.ToInt16(query2.DeviceID);
                                TB.State = Convert.ToByte(ValueTag_Conditons);
                                TB.Date = Date.GetDateTypeInt();
                                TB.Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

                            }

                            db.VehicleLastStates.InsertOnSubmit(TB);
                            db.SubmitChanges();
                            VehicleState tb2 = new VehicleState();
                            {
                                tb2.DeviceID = Convert.ToInt16(query2.DeviceID);
                                tb2.State = Convert.ToByte(ValueTag_Conditons);
                                tb2.Date = Date.GetDateTypeInt();
                                tb2.Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

                            }
                            db.VehicleStates.InsertOnSubmit(tb2);
                            db.SubmitChanges();


                        }

                        Text_Status.Text = TypeItem2.Content.ToString();

                        if (Text_DeviceCode.Tag != null)
                        {
                            this.Close();

                        }

                        else
                            MessageBox.Show("وضعیت دستگاه با موفقیت تغییر کرد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                    }

                    catch (Exception)
                    {
                        MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



    }
}
