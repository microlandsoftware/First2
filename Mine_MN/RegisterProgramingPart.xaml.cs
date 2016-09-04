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
using System.Data;
using maintenance.classes;
namespace maintenance
{
    /// <summary>
    /// Interaction logic for register_programing_lot.xaml
    /// </summary>
    public partial class RegisterProgramingPart : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public RegisterProgramingPart()
        {
            InitializeComponent();
            CommandManager.AddPreviewCanExecuteHandler(Text_ValueCycle, canExecute);
            CommandManager.AddPreviewCanExecuteHandler(Text_FirstWorking, canExecute);

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
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query2 = (from c in db.MN_Parts where c.ID == int.Parse(Text_codelot.Text) select c).Single();
                Lbl_PartName.Text = "نام قطعه:" + " " + query2.Name;

                Load_Com_Brand();
                Load_Com_CycleType();
                load_Com_TypeActivity();
                load_Com_GroupRepair();
            }

        }

        private void Com_Barnd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Com_Model_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void Text_ValueCycle_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }


        private void Load_Com_CycleType()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_CycleCriteriaTypes select c;
                foreach (var Items in query)
                {
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.Type;
                    ComItem.Tag = Items.ID.ToString();

                    Com_CycleType.Items.Add(ComItem);
                }
            }
        }

        private void Load_Com_Brand()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.ADM_Brands
                    join l in db.Lables on c.VehicleTypeID equals l.LID
                    select new {Type = c.Name, BrandCode = c.ID, TypeMachin = l.LDscFa};


                foreach (var items in query)
                {
                    ComboBoxItem comitem = new ComboBoxItem();
                    comitem.Content = items.TypeMachin + " " + items.Type;
                    comitem.Tag = items.BrandCode.ToString();
                    Com_Barnd.Items.Add(comitem);
                }
            }
        }

        private void load_Com_TypeActivity()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
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

        private void load_Com_GroupRepair()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Groups select c;
                foreach (var Items in query)
                {
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.Type;
                    ComItem.Tag = Items.ID.ToString();

                    Com_GroupRepair.Items.Add(ComItem);
                }
            }
        }

        private void Btn_RegisterProgram_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem Device = (ComboBoxItem) Com_Device.SelectedItem;
                ComboBoxItem CycleType = (ComboBoxItem) Com_CycleType.SelectedItem;
                ComboBoxItem TypeActivity = (ComboBoxItem) Com_TypeActivity.SelectedItem;
                ComboBoxItem GroupRepair = (ComboBoxItem) Com_GroupRepair.SelectedItem;
                if (Device.Tag.ToString() == "0")
                    MessageBox.Show("لطفا دستگاه مورد نظر را انتخاب نماييد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (Text_Schedule.Text == "")
                    MessageBox.Show("لطفا درصد اعلام برنامه را وارد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (Text_Alarm.Text == "")
                    MessageBox.Show("لطفا درصد آلارم برنامه را وارد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (Text_Warning.Text == "")
                    MessageBox.Show("لطفا درصد هشدار برنامه را وارد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (int.Parse(Text_Schedule.Text) < int.Parse(Text_Warning.Text))
                    MessageBox.Show("درصد هشدار نمی تواند بیشتر از درصد اعلام برنامه باشد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (int.Parse(Text_Alarm.Text) > int.Parse(Text_Warning.Text))
                    MessageBox.Show("درصد  آلارم نمی تواند بیشتر از درصد هشدار باشد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error); 
                else if (int.Parse(Text_Warning.Text) > 50 || int.Parse(Text_Alarm.Text) > 50 ||
                         int.Parse(Text_Schedule.Text) > 50)
                    MessageBox.Show("مقدار درصد ها نمی تواند بیشتر از 50 باشد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (CycleType.Tag.ToString() == "0")
                    MessageBox.Show("لطفا دوره سرويس را انتخاب نماييد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (Text_ValueCycle.Text == "" || int.Parse(Text_ValueCycle.Text) <= 0)
                    MessageBox.Show("لطفا مقدار دوره را وارد نماييد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (TypeActivity.Tag.ToString() == "0")
                    MessageBox.Show("لطفا نوع فعاليت سرويس را انتخاب نماييد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (GroupRepair.Tag.ToString() == "0")
                    MessageBox.Show("لطفا گروه تعمير را انتخاب نماييد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (Text_FirstWorking.Text == "")
                    MessageBox.Show(" مقدار کارکرد اولیه قطعه را وارد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else
                {
                    //var query2 = from c in db.MN_DefinePartModes
                    //             where
                    //                 c.PartID == int.Parse(Text_codelot.Text) &&
                    //                 c.GroupID == int.Parse(GroupRepair.Tag.ToString())
                    //                 && c.CriteriaID == int.Parse(CycleType.Tag.ToString()) && c.Value == Text_ValueCycle.Text &&
                    //                 c.FixTypeID == int.Parse(TypeActivity.Tag.ToString()) &&
                    //                 c.VehicleID == int.Parse(Device.Tag.ToString())
                    //             select c;
                    //if (query2.Count() >= 1)
                    //    MessageBox.Show("برنامه تعميري اين قطعه تکراري است", "خطا", MessageBoxButton.OK,
                    //        MessageBoxImage.Error);
                    //else
                    //{

                    if (Btn_CancelEdit.Visibility == Visibility.Visible)
                    {
                        try
                        {
                            var query = (from c in db.MN_DefinePartModes
                                where c.ID == int.Parse(Btn_CancelEdit.Tag.ToString())
                                select c).Single();
                            query.Value = Text_ValueCycle.Text;
                            query.FixTypeID = int.Parse(TypeActivity.Tag.ToString());
                            query.CriteriaID = int.Parse(CycleType.Tag.ToString());
                            query.GroupID = int.Parse(GroupRepair.Tag.ToString());
                            query.Schedule = int.Parse(Text_Schedule.Text);
                            query.Alarm = int.Parse(Text_Alarm.Text);
                            query.Warning = int.Parse(Text_Warning.Text);

                            db.SubmitChanges();
                            MessageBox.Show("اطلاعات با موفقیت ویرایش شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridProgramPart();
                            Btn_CancelEdit_Click(sender, e);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("خطا در ویرایش اطلاعات", "خطا", MessageBoxButton.OK,
                                MessageBoxImage.Error);

                        }

                    }
                    else
                    {

                        try
                        {
                            var query7 = from c in db.ADM_Vehicles where c.ID == int.Parse(Device.Tag.ToString()) select c;

                            if (int.Parse(CycleType.Tag.ToString()) == 1)
                            {
                                if (query7.Take(1).Single().ActiveService_Time == false)
                                {
                                    MessageBox.Show("لطفا ابتدا سرویس ساعت دستگاه مورد نظر را فعالسازی کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                            }
                            else
                            {
                                if (query7.Take(1).Single().ActiveService_Kilometer == false)
                                {
                                    MessageBox.Show("لطفا ابتدا سرویس کیلومتر دستگاه مورد نظر را فعالسازی کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
                            }
                        
                            
                            
                            
                            int First_Worker = int.Parse(Text_FirstWorking.Text);
                            if (int.Parse(Text_FirstWorking.Text) >= int.Parse(Text_ValueCycle.Text))
                                First_Worker = int.Parse(Text_FirstWorking.Text)%int.Parse(Text_ValueCycle.Text);
                            MN_DefinePartMode TB = new MN_DefinePartMode();
                            {
                                TB.PartID = int.Parse(Text_codelot.Text);
                                TB.CriteriaID = int.Parse(CycleType.Tag.ToString());
                                TB.Value = Text_ValueCycle.Text.ToString();
                                TB.FixTypeID = int.Parse(TypeActivity.Tag.ToString());
                                TB.GroupID = int.Parse(GroupRepair.Tag.ToString());
                                TB.VehicleID = int.Parse(Device.Tag.ToString());
                                TB.Sum = First_Worker;
                                TB.Schedule = int.Parse(Text_Schedule.Text);
                                TB.Alarm = int.Parse(Text_Alarm.Text);
                                TB.Warning = int.Parse(Text_Warning.Text);
                                TB.Counter = 0;
                            }
                            db.MN_DefinePartModes.InsertOnSubmit(TB);
                            db.SubmitChanges();
                            MessageBox.Show("برنامه مورد نظر با موفقيت ثبت شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridProgramPart();
                            Com_CycleType.SelectedIndex = 0;
                            Com_TypeActivity.SelectedIndex = 0;
                            Com_GroupRepair.SelectedIndex = 0;
                            Text_ValueCycle.Text = "";
                            Text_FirstWorking.Text = "0";
                            Text_Schedule.Text = "";
                            Text_Alarm.Text = "";
                            Text_Warning.Text = "";
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }

                    // }
                }
            }

        }

        private void Com_Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        public class Show_ListProgramPart
        {
            public string PartName { get; set; }
            public string DeviceName { get; set; }
            public string CycleType { get; set; }
            public string ValueCycle { get; set; }
            public string Schedule { get; set; }
            public string Warning { get; set; }
            public string Alarm { get; set; }
            public string TypeActivity { get; set; }
            public string GroupRepair { get; set; }
            public string DefinePartCode { get; set; }

        }

        private void Fill_GridProgramPart()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem Device = (ComboBoxItem) Com_Device.SelectedItem;
                if (Com_Device.SelectedItem != null)
                {
                    string Value = Device.Content.ToString();
                    if (Value != "لیست دستگاه")
                    {

                        string valuetag = Device.Tag.ToString();

                        var query = from c in db.MN_DefinePartModes
                            join f in db.MN_Parts on c.PartID equals f.ID
                            join k in db.ADM_Vehicles on c.VehicleID equals k.ID
                            join m in db.MN_CycleCriteriaTypes on c.CriteriaID equals m.ID
                            join g in db.MN_Groups on c.GroupID equals g.ID
                            join t in db.MN_FixTypes on c.FixTypeID equals t.ID
                            where c.VehicleID == int.Parse(valuetag.ToString())
                            orderby c.ID descending
                            select
                                new
                                {
                                    DefinePartCode = c.ID,
                                    PartName = f.Name,
                                    DeviceName = k.Name,
                                    CycleType = m.Type,
                                    ValueCycle = c.Value,
                                    TypeActivity = t.Type,
                                    GroupRepair = g.Type,
                                    Schedule = c.Schedule,
                                    Warning = c.Warning,
                                    Alarm = c.Alarm

                                };

                        List<Show_ListProgramPart> Items = new List<Show_ListProgramPart>();

                        foreach (var Part in query)
                        {
                            Items.Add(new Show_ListProgramPart()
                            {
                                DefinePartCode = Part.DefinePartCode.ToString(),
                                PartName = Part.PartName,
                                DeviceName = Part.DeviceName,
                                CycleType = Part.CycleType,
                                ValueCycle = Part.ValueCycle,
                                TypeActivity = Part.TypeActivity,
                                GroupRepair = Part.GroupRepair,
                                Schedule = Part.Schedule.ToString(),
                                Warning = Part.Warning.ToString(),
                                Alarm = Part.Alarm.ToString()
                            });
                        }
                        Grid_RegisterProgramPart.ItemsSource = Items;
                    }

                }
            }
        }

        private void Delete_ProgramPart(object sender, RoutedEventArgs e)
        {
             var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (
                    MessageBox.Show(" با حذف برنامه تمام سرویس های ایجاد شده این برنامه هم حذف میشود؟ ", "هشدار",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    Button Btn_Code = sender as Button;

                    var query = from c in db.MN_DefinePartModes
                        where c.ID == int.Parse(Btn_Code.Tag.ToString())
                        select c;
                    if (query.Count() != 0)
                    {
                        try
                        {
                            var query2 = from c in db.MN_CreatePartFixes
                                where c.PartModeID == int.Parse(Btn_Code.Tag.ToString())
                                select c;
                            if (query2.Count() >= 1)
                            {
                                db.MN_CreatePartFixes.DeleteAllOnSubmit(query2);
                                db.SubmitChanges();
                            }

                            db.MN_DefinePartModes.DeleteOnSubmit(query.Single());
                            db.SubmitChanges();
                            MessageBox.Show("برنامه مورد نظر حذف شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridProgramPart();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("قادر به حذف برنامه نيستيد", "خطا", MessageBoxButton.OK,
                                MessageBoxImage.Error);

                        }


                    }
                    Btn_CancelEdit_Click(sender, e);
                }
            }
        }

        private void Edit_ProgramPart(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button Btn_Code = sender as Button;
                var query =
                    (from c in db.MN_DefinePartModes where c.ID == int.Parse(Btn_Code.Tag.ToString()) select c).Single();
                Text_ValueCycle.Text = query.Value;
                Com_CycleType.SelectedValue = int.Parse(query.CriteriaID.ToString());
                Com_TypeActivity.SelectedValue = int.Parse(query.FixTypeID.ToString());
                Com_GroupRepair.SelectedValue = int.Parse(query.GroupID.ToString());
                Btn_CancelEdit.Tag = query.ID.ToString();
                Btn_CancelEdit.Visibility = Visibility.Visible;
                Btn_RegisterProgram.Content = "ثبت تغییرات";
                lbl_firstworking.Visibility = Visibility.Hidden;
                Text_FirstWorking.Visibility = Visibility.Hidden;
                Text_Schedule.Text = query.Schedule.ToString();
                Text_Alarm.Text = query.Alarm.ToString();
                Text_Warning.Text = query.Warning.ToString();
            }
        }

        public string SelectedStringValue
        {
            get;
            set;
        }

        private void Btn_CancelEdit_Click(object sender, RoutedEventArgs e)
        {
           
                Btn_RegisterProgram.Content = "ثبت برنامه";
                Btn_CancelEdit.Visibility = Visibility.Hidden;
                Text_ValueCycle.Text = "";
                Com_CycleType.SelectedIndex = 0;
                Com_TypeActivity.SelectedIndex = 0;
                Com_GroupRepair.SelectedIndex = 0;
                lbl_firstworking.Visibility = Visibility.Visible;
                Text_FirstWorking.Visibility = Visibility.Visible;
                Text_Schedule.Text = "";
                Text_Alarm.Text = "";
                Text_Warning.Text = "";

        }

        private void Com_Device_DropDownClosed(object sender, EventArgs e)
        {
            Fill_GridProgramPart();
            Btn_RegisterProgram.Content = "ثبت برنامه";
            Btn_CancelEdit.Visibility = Visibility.Hidden;
            Text_ValueCycle.Text = "";
            Com_CycleType.SelectedIndex = 0;
            Com_TypeActivity.SelectedIndex = 0;
            Com_GroupRepair.SelectedIndex = 0;
            lbl_firstworking.Visibility = Visibility.Visible;
            Text_FirstWorking.Visibility = Visibility.Visible;
            
            
        }

        private void Com_Barnd_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem TypeItem = (ComboBoxItem) Com_Barnd.SelectedItem;
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

                    Btn_RegisterProgram.Content = "ثبت برنامه";
                    Btn_CancelEdit.Visibility = Visibility.Hidden;
                    Text_ValueCycle.Text = "";
                    Com_CycleType.SelectedIndex = 0;
                    Com_TypeActivity.SelectedIndex = 0;
                    Com_GroupRepair.SelectedIndex = 0;
                    lbl_firstworking.Visibility = Visibility.Visible;
                    Text_FirstWorking.Visibility = Visibility.Visible;

                    Grid_RegisterProgramPart.ItemsSource = "";

                }
            }
        }

        private void Com_Model_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                ComboBoxItem TypeItem = (ComboBoxItem) Com_Model.SelectedItem;
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
                    Btn_RegisterProgram.Content = "ثبت برنامه";
                    Btn_CancelEdit.Visibility = Visibility.Hidden;
                    Text_ValueCycle.Text = "";
                    Com_CycleType.SelectedIndex = 0;
                    Com_TypeActivity.SelectedIndex = 0;
                    Com_GroupRepair.SelectedIndex = 0;
                    lbl_firstworking.Visibility = Visibility.Visible;
                    Text_FirstWorking.Visibility = Visibility.Visible;
                    Grid_RegisterProgramPart.ItemsSource = "";
                }
            }
        }

        private void Text_FirstWorking_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
