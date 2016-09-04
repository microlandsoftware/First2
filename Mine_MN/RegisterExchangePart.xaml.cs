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
using System.Globalization;
using System.Threading;
using maintenance.classes;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for register_xchangelot.xaml
    /// </summary>
    public partial class RegisterExchangePart : Window
    {
        public static string Close_Form = "false";
        public static string ShowForm_SelectDevice="false";
        public static string ShowForm_Selectpart="false";
       // public static readonly Object UnsetValue;
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public RegisterExchangePart()
        {
            InitializeComponent();
            int[] lcids = new int[] { 1065, 1033, 1025 };
            this.Text_BadDate.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            this.Text_DoDate.FlowDirection = System.Windows.FlowDirection.RightToLeft;
        }

        private void Load_Com_GroupRepair()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Groups select c;

                ComboBoxItem Comitem2 = new ComboBoxItem();
                Comitem2.Content = "نوع گروه";
                Comitem2.Tag = "0";
                Com_GroupRepair.Items.Add(Comitem2);
                Com_GroupRepair.SelectedIndex = 0;

                foreach (var Items in query)
                {
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.Type;
                    ComItem.Tag = Items.ID.ToString();

                    Com_GroupRepair.Items.Add(ComItem);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                var query = (from c in db.ADM_Vehicles
                    where c.ID == int.Parse(Text_DeviceName_Select.Tag.ToString())
                    select c).Single();
                var query2 =
                    (from c in db.MN_Parts where c.ID == int.Parse(Text_PartName.Tag.ToString()) select c).Single();
                Text_DeviceName_Select.Text = query.Name;
                Text_PartName.Text = query2.Name;


                Load_Com_GroupRepair();
                Load_Com_TypeActivity();


                Thread Thread = new Thread(new ThreadStart(FillGrid_Exchangepart));
                Thread.Start();

                ConvertDate Date = new ConvertDate();
                string DateNow = Date.GetDateNow();
                Text_BadDate.Text = DateNow;
                Text_DoDate.Text = DateNow;

            }
        }

        private void Load_Com_TypeActivity()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "نوع فعالیت";
                ComItem2.Tag = "0";
                Com_TypeActivity.Items.Add(ComItem2);
                Com_TypeActivity.SelectedIndex = 0;

                var query = from c in db.MN_FixTypes select c;
                foreach (var Items in query)
                {
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.Type;
                    ComItem.Tag = Items.ID.ToString();
                    Com_TypeActivity.Items.Add(ComItem);
                }
            }
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ConvertDate DT = new ConvertDate();
                ;
                string DateNow = DT.GetDateNow();
                string TimeNow = DateTime.Now.ToString("HH:mm:ss");

                if (CheckDate(Text_BadDate.Text))
                {
                    if (CheckDate(Text_DoDate.Text))
                    {
                        if (CheckTime(Text_BadTime.Text))
                        {
                            if (CheckTime(Text_DoTime.Text))
                            {
                                if (Text_BadDate.Text.CompareTo(Text_DoDate.Text) >= 1)
                                    MessageBox.Show("در وارد کردن تاریخ تعمیر دقت نمایید", "خطا", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                                else if ((Text_BadDate.Text.CompareTo(Text_DoDate.Text) == 0) &&
                                         ((Text_BadTime.Text).CompareTo(Text_DoTime.Text)) >= 1)
                                    MessageBox.Show("در وارد کردن ساعت تعمییر دقت نمایید", "خطا",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                else if ((Text_DoDate.Text.CompareTo(DateNow) == 0) &&
                                         ((Text_DoTime.Text.CompareTo(TimeNow) >= 1)))
                                    MessageBox.Show("ساعت وارد شده برای آینده است", "خطا", MessageBoxButton.OK,
                                        MessageBoxImage.Error);

                                else if ((Text_DoDate.Text.CompareTo(DateNow) >= 1) ||
                                         (Text_BadDate.Text.CompareTo(DateNow) >= 1))
                                    MessageBox.Show("تاریخ وارد شده برای آینده است", "خطا",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                else
                                {

                                    ComboBoxItem Repair = (ComboBoxItem) Com_GroupRepair.SelectedItem;
                                    ComboBoxItem Activity = (ComboBoxItem) Com_TypeActivity.SelectedItem;

                                    if ((Repair.Tag.ToString() == "0") || (Activity.Tag.ToString() == "0"))
                                        MessageBox.Show("لطفا تمام فیلد ها را تکمیل نمایید", "خطا",
                                            MessageBoxButton.OK, MessageBoxImage.Error);

                                    else
                                    {

                                        if (BtnCancelEdit.Visibility == Visibility.Visible)
                                        {

                                            try
                                            {
                                                ConvertDate Date = new ConvertDate();
                                                var query =
                                                    (from c in db.MN_PartFixes
                                                        where
                                                            c.ID == int.Parse(BtnCancelEdit.Tag.ToString())
                                                        select c).Single();
                                                query.BreakDate =
                                                    int.Parse(Text_BadDate.Text.Replace("/", ""));
                                                query.BreakTime = TimeSpan.Parse(Text_BadTime.Text);
                                                query.FixDate = int.Parse(Text_DoDate.Text.Replace("/", ""));
                                                query.FixTime = TimeSpan.Parse(Text_DoTime.Text);


                                                query.GroupID = int.Parse(Repair.Tag.ToString());
                                                query.FixTypeID = int.Parse(Activity.Tag.ToString());
                                                db.SubmitChanges();
                                                MessageBox.Show("اطلاعات با موفقیت ویرایش شد", "موفقيت",
                                                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                                FillGrid_Exchangepart();
                                                BtnCancelEdit_Click(sender, e);
                                            }
                                            catch (Exception)
                                            {
                                                MessageBox.Show("خطا در ویرایش اطلاعات", "خطا",
                                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                            }
                                        }

                                        else
                                        {
                                            try
                                            {
                                                ConvertDate Date = new ConvertDate();
                                                MN_PartFix tb = new MN_PartFix();
                                                {
                                                    tb.BreakDate =
                                                        int.Parse(Text_BadDate.Text.Replace("/", ""));
                                                    tb.BreakTime = TimeSpan.Parse(Text_BadTime.Text);
                                                    tb.FixDate = int.Parse(Text_DoDate.Text.Replace("/", ""));
                                                    tb.FixTime = TimeSpan.Parse(Text_DoTime.Text);
                                                    tb.VehicleID =
                                                        int.Parse(Text_DeviceName_Select.Tag.ToString());
                                                    tb.PartID = int.Parse(Text_PartName.Tag.ToString());
                                                    tb.GroupID = int.Parse(Repair.Tag.ToString());
                                                    tb.FixTypeID = int.Parse(Activity.Tag.ToString());
                                                }

                                                db.MN_PartFixes.InsertOnSubmit(tb);
                                                db.SubmitChanges();
                                                MessageBox.Show("اطلاعات با موفقیت ثبت شد", "موفقيت",
                                                    MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                                Text_BadTime.Text = "";
                                                Text_DoTime.Text = "";
                                                Text_BadDate.Text = DateNow;
                                                Text_DoDate.Text = DateNow;
                                                Com_GroupRepair.SelectedIndex = 0;
                                                Com_TypeActivity.SelectedIndex = 0;

                                                FillGrid_Exchangepart();



                                            }
                                            catch (Exception)
                                            {

                                                MessageBox.Show("خطا در ثبت اطلاعات", "خطا",
                                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                            }
                                        }
                                    }

                                }

                            }
                        }
                    }
                }

            }


        }

        private void Text_BadDate_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
           

           
        }

        private void WindowsFormsHost_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {

        }

        private void WindowsFormsHost_LostFocus(object sender, RoutedEventArgs e)
        {
            

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Close_Form == "false")
            {
                e.Cancel = true;
                MessageBox_SelectForm Messsage = new MessageBox_SelectForm();
                Messsage.ShowDialog();
                if (MessageBox_SelectForm.Close == true)
                {
                    if (ShowForm_SelectDevice == "true")
                    {
                        this.Hide();
                        ShowDevice_ExchangePart Child = new ShowDevice_ExchangePart();
                        Child.Owner = App.Current.MainWindow;
                        Child.ShowDialog();
                        e.Cancel = false;

                    }

                    else if (ShowForm_Selectpart == "true")
                    {
                        this.Hide();
                        ShowListPart_ExchangePart Child = new ShowListPart_ExchangePart();
                        Child.Owner = App.Current.MainWindow;
                        Child.Text_DeviceName.Tag = Text_DeviceName_Select.Tag;
                        Child.ShowDialog();
                        e.Cancel = false;

                    }
                    else
                    {
                        e.Cancel = false;
                        Close_Form = "false";
                        ShowForm_SelectDevice = "false";
                        ShowForm_Selectpart = "false";
                        ShowDevice_ExchangePart.C_Brand = null;
                        ShowDevice_ExchangePart.C_Model = null;
                        ShowListPart_ExchangePart.Text_Search = null;

                    }
                }
            }

            
            

            
        }

        private bool CheckDate(string Date)// تابع چک کردن تاریخ
        {
            bool Flag=true;
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

        private bool CheckTime(string Time) // تابه چک کردن ساعت
        {
            bool Flag = true;
            string[] Array = new string[] { };
            if (Time.Contains(" "))
            {
                MessageBox.Show("لطفا در وارد کردن ساعت دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                Flag = false;
            }
            else
            {
                Array = Time.Split(':');
                if ((int.Parse(Array[0]) >= 24) || (int.Parse(Array[1]) >= 60) || (int.Parse(Array[2]) >= 60))
                {
                    MessageBox.Show("لطفا در وارد کردن ساعت دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    Flag = false;
                }
            }
            return Flag;
        }


        public class Show_BadPart
        {
            public string PartName { get; set; }
            public string DeviceName { get; set; }
            public string BadDate { get; set; }
            public string BadTime { get; set; }
            public string RepairDate { get; set; }
            public string RepairTime { get; set; }
            public string GroupRepair { get; set; }
            public string TypeActivity { get; set; }
            public string BadPartCode { get; set; }



        }


        private void FillGrid_Exchangepart()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                var query = from c in db.MN_PartFixes
                    join d in db.MN_Parts on c.PartID equals d.ID
                    join k in db.ADM_Vehicles on c.VehicleID equals k.ID
                    join t in db.MN_FixTypes on c.FixTypeID equals t.ID
                    join g in db.MN_Groups on c.GroupID equals g.ID
                    orderby c.ID descending
                    where c.VehicleID == int.Parse(Text_DeviceName_Select.Tag.ToString())
                    select
                        new
                        {
                            BadPartCode = c.ID,
                            PartName = d.Name,
                            DeviceName = k.Name,
                            BadDate = c.BreakDate,
                            BadTime = c.BreakTime,
                            RepairDate = c.FixDate,
                            RepairTime = c.FixTime,
                            TypeActivity = t.Type,
                            GroupRepair = g.Type
                        };

                List<Show_BadPart> Items = new List<Show_BadPart>();
                this.Dispatcher.Invoke(new Action(() =>
                {
                    foreach (var Part in query)
                    {
                        ConvertDate DT = new ConvertDate();

                        // DateTime baddate=DateTime.Parse(lot.baddate);
                        // DateTime repairdate=DateTime.Parse(lot.repairdate);


                        Items.Add(new Show_BadPart()
                        {
                            BadPartCode = Part.BadPartCode.ToString(),
                            PartName = Part.PartName,
                            DeviceName = Part.DeviceName,
                            BadDate = DT.DateToString(int.Parse(Part.BadDate.ToString())),
                            BadTime = Part.BadTime.ToString(),
                            RepairTime = Part.RepairTime.ToString(),
                            RepairDate = DT.DateToString(int.Parse(Part.RepairDate.ToString())),
                            TypeActivity = Part.TypeActivity,
                            GroupRepair = Part.GroupRepair
                        });

                    }
                    Grid_ExchangePart.ItemsSource = Items;
                }), null);
            }
        }


        private void Delete_ProgramPart(object sender, RoutedEventArgs e)
        {
           var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (
                    MessageBox.Show("آيا از حذف خرابی قطعه ثبت شده اطمينان داريد", "هشدار", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    
                    Button Btn_Code = sender as Button;

                    var query = from c in db.MN_PartFixes
                        where c.ID == int.Parse(Btn_Code.Tag.ToString())
                        select c;
                    if (query.Count() != 0)
                    {
                        try
                        {
                            db.MN_PartFixes.DeleteOnSubmit(query.Single());
                            db.SubmitChanges();
                            MessageBox.Show("خرابی قطعه مورد نظر حذف شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            FillGrid_Exchangepart();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("قادر به حذف خرابی قطعه نیستید", "خطا", MessageBoxButton.OK,
                                MessageBoxImage.Error);

                        }


                    }
                    BtnCancelEdit_Click(sender, e);
                }
            }
        }

        private void edit_programlot(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ConvertDate Date = new ConvertDate();
                Button Btn_Code = sender as Button;
                var query = (from c in db.MN_PartFixes
                    join l in db.MN_Parts on c.PartID equals l.ID

                    where c.ID == int.Parse(Btn_Code.Tag.ToString())
                    select
                        new
                        {
                            PartName = l.Name,
                            BadDate = c.BreakDate,
                            BadTime = c.BreakTime,
                            DoDate = c.FixDate,
                            DoTime = c.FixTime,
                            CGroupRepair = c.GroupID,
                            CTypeActivity = c.FixTypeID
                        }).Single();


                Text_BadDate.Text = Date.DateToString(int.Parse(query.BadDate.ToString()));
                Text_BadTime.Text = query.BadTime.ToString();
                Text_DoDate.Text = Date.DateToString(int.Parse(query.DoDate.ToString()));
                Text_DoTime.Text = query.DoTime.ToString();
                Com_GroupRepair.SelectedValue = query.CGroupRepair;
                Com_TypeActivity.SelectedValue = query.CTypeActivity;
                Text_PartName.Text = query.PartName;
                Btn_Register.Content = "ثبت تغییرات";
                BtnCancelEdit.Visibility = Visibility.Visible;
                BtnCancelEdit.Tag = Btn_Code.Tag.ToString();
            }
        }
        
        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query2 =
                    (from c in db.MN_Parts where c.ID == int.Parse(Text_PartName.Tag.ToString()) select c).Single();
                ConvertDate Date = new ConvertDate();
                string DateNow = Date.GetDateNow();
                Text_BadDate.Text = DateNow;
                Text_DoDate.Text = DateNow;

                Text_BadTime.Text = "";

                Text_DoTime.Text = "";
                Com_GroupRepair.SelectedIndex = 0;
                Com_TypeActivity.SelectedIndex = 0;
                Text_PartName.Text = query2.Name;
                Btn_Register.Content = "ثبت خرابی";
                BtnCancelEdit.Visibility = Visibility.Hidden;
            }
        }
        

    }
}
