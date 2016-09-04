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
    /// Interaction logic for register_changelist_stautsdevice.xaml
    /// </summary>
    public partial class RegisterChangeList_DeviceStauts : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public RegisterChangeList_DeviceStauts()
        {
            InitializeComponent();
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Load_Com_Conditions()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_States where c.Show == true && c.Parent <= 2 select c;
                ComboBoxItem Comitem2 = new ComboBoxItem();
                Comitem2.Content = "لیست وضعیت ها";
                Comitem2.Tag = "0";
                Com_Conditions.Items.Add(Comitem2);

                foreach (var Items in query)
                {
                    ComboBoxItem Comitem = new ComboBoxItem();
                    Comitem.Content = Items.Title;
                    Comitem.Tag = Items.Code.ToString();
                    Com_Conditions.Items.Add(Comitem);
                }
                Com_Conditions.SelectedIndex = 0;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query =
                    (from c in db.ADM_Vehicles where c.ID == int.Parse(Text_DeviceName.Tag.ToString()) select c).Single();
                Text_DeviceName.Content = query.Name.ToString();


                var query2 = from c in db.VehicleLastStates
                    join j in db.ADM_Vehicles on c.DeviceID equals j.DeviceID
                    join d in db.MN_States on Convert.ToInt32(c.State) equals d.ID
                    where j.ID == int.Parse(Text_DeviceName.Tag.ToString())
                    select new {Type = d.Title};
                if (query2.Count() == 1)
                {
                    var q = query2.Single();
                    Text_Status.Content = q.Type;

                }

                else
                    Text_Status.Content = "وضعیتی موجود نسیت";


                Load_Com_Conditions();

            }
        }

        private void Btn_RegisterStatus_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ConvertDate Date = new ConvertDate();


                ComboBoxItem TypeItem2 = (ComboBoxItem) Com_Conditions.SelectedItem;
                string ValueTag_Conditons = TypeItem2.Tag.ToString();


                if (ValueTag_Conditons == "0")
                    MessageBox.Show("لطفا وضعیت جدید دستگاه را انتخاب نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else
                {
                    try
                    {
                        var query2 = (from c in db.ADM_Vehicles
                            where c.ID == int.Parse(Text_DeviceName.Tag.ToString())
                            select new {DeviceID = c.DeviceID}).Single();
                        if (query2.DeviceID != 0 && query2.DeviceID != null)
                        {
                            var query = from c in db.VehicleLastStates
                                join d in db.ADM_Vehicles on c.DeviceID equals d.DeviceID
                                where d.ID == int.Parse(Text_DeviceName.Tag.ToString())
                                select c;
                            if (query.Count() == 1)
                            {
                                var q = query.Single();
                                q.State = Convert.ToByte(ValueTag_Conditons);
                                q.Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));
                                q.Date = Date.GetDateTypeInt();
                                // q.State = Convert.ToBoolean(access);
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
                                VehicleState TB2 = new VehicleState();
                                {
                                    TB2.DeviceID = Convert.ToInt16(query2.DeviceID);
                                    TB2.State = Convert.ToByte(ValueTag_Conditons);
                                    TB2.Date = Date.GetDateTypeInt();
                                    TB2.Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm:ss"));

                                }


                                db.VehicleStates.InsertOnSubmit(TB2);
                                db.SubmitChanges();
                            }


                            MessageBox.Show("وضعیت دستگاه با موفقیت تغییر کرد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                        }
                        else
                        {
                            MessageBox.Show("برای دستگاه مورد نظر سخت افزار انتصاب داده نشده ست", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    catch (Exception)
                    {
                        MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void Window_Closed(object sender, EventArgs e)
        {
           
        }

    }
}
