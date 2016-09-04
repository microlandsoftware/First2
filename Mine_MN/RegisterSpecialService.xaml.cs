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
using System.Windows.Input;
using System.IO;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for RegisterSpecialService.xaml
    /// </summary>
    public partial class RegisterSpecialService : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string SaveFile = ini.IniReadValue("appSettings", "Savefile");
        private static bool Flag = false;
        private static string FileName;
        public RegisterSpecialService()
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
            Load_Com_CycleType();
            load_Com_GroupRepair();


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
                else if (Text_Title.Text == "")
                    MessageBox.Show("لطفا عنوان سرویس را وراد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (GroupRepair.Tag.ToString() == "0")
                    MessageBox.Show("لطفا گروه تعمير را انتخاب نماييد", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                else if (Text_FirstWorking.Text == "")
                    MessageBox.Show(" مقدار کارکرد اولیه دستگاه را وارد نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                else
                {

                    if (Btn_CancelEdit.Visibility == Visibility.Visible)
                    {
                        try
                        {
                            var query = (from c in db.MN_DefineServiceCycles
                                where c.ID == int.Parse(Btn_CancelEdit.Tag.ToString())
                                select c).Single();
                            query.Value = int.Parse(Text_ValueCycle.Text);
                            query.Title = Text_Title.Text;
                            query.CriteriaID = int.Parse(CycleType.Tag.ToString());
                            query.GroupID = int.Parse(GroupRepair.Tag.ToString());
                            query.Schedule = int.Parse(Text_Schedule.Text);
                            query.Alarm = int.Parse(Text_Alarm.Text);
                            query.Warning = int.Parse(Text_Warning.Text);

                            if (Flag)
                            {
                                string AddressFile = SaveFile + query.FileURL;
                                if (File.Exists(AddressFile))
                                {
                                    File.Delete(AddressFile);
                                }

                                string Model = query.VisitCycleID.ToString();

                                string Path = "\\PrintFile\\" + Model + "\\";
                                string FolderPath = SaveFile + Path;

                                if (!Directory.Exists(FolderPath))
                                {
                                    Directory.CreateDirectory(FolderPath);
                                }
                                string[] FileName_Old = new string[] {};
                                FileName_Old = System.IO.Path.GetFileName(FileName).Split('.');
                                if (FileName_Old.Length > 2)
                                {
                                    FileName_Old[1] = FileName_Old[FileName_Old.Length - 1];
                                }
                                FileName_Old[0] = Btn_CancelEdit.Tag.ToString();
                                string NewName = FileName_Old[0] + "." + FileName_Old[1];
                                Path = Path + NewName;
                                string FilePath = FolderPath + NewName;
                                System.IO.File.Copy(FileName, FilePath, true);

                                query.FileURL = Path;

                                Flag = false;
                            }

                            db.SubmitChanges();




                            
                            MessageBox.Show("اطلاعات با موفقیت ویرایش شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridSpecialService();
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
                             var query7=from c in db.ADM_Vehicles where c.ID==int.Parse(Device.Tag.ToString()) select c; 
                            
                             if(int.Parse(CycleType.Tag.ToString())==1)
                             {
                               if(query7.Take(1).Single().ActiveService_Time==false)
                               {
                                     MessageBox.Show("لطفا ابتدا سرویس ساعت دستگاه مورد نظر را فعالسازی کنید", "خطا", MessageBoxButton.OK,MessageBoxImage.Warning);
                                         return;
                                }
                             }
                               else 
                               {
                                  if(query7.Take(1).Single().ActiveService_Kilometer==false)
                               {
                                   MessageBox.Show("لطفا ابتدا سرویس کیلومتر دستگاه مورد نظر را فعالسازی کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                                         return;
                                }
                               }
                                                                                
                            
                            int First_Worker = int.Parse(Text_FirstWorking.Text);
                            if (int.Parse(Text_FirstWorking.Text) >= int.Parse(Text_ValueCycle.Text))
                                First_Worker = int.Parse(Text_FirstWorking.Text)%int.Parse(Text_ValueCycle.Text);
                            MN_DefineServiceCycle TB = new MN_DefineServiceCycle();
                            {

                                TB.CriteriaID = int.Parse(CycleType.Tag.ToString());
                                TB.Value = int.Parse(Text_ValueCycle.Text.ToString());
                                TB.Title = Text_Title.Text;
                                TB.GroupID = int.Parse(GroupRepair.Tag.ToString());
                                TB.VehicleID = int.Parse(Device.Tag.ToString());
                                TB.Sum = First_Worker;
                                TB.Schedule = int.Parse(Text_Schedule.Text);
                                TB.Alarm = int.Parse(Text_Alarm.Text);
                                TB.Warning = int.Parse(Text_Warning.Text);
                                TB.Counter = 0;
                            }
                            db.MN_DefineServiceCycles.InsertOnSubmit(TB);
                            db.SubmitChanges();

                            var query4 = from c in db.MN_SumServices
                                where c.VehicleID == int.Parse(Device.Tag.ToString())
                                select c;
                            if (query4.Count() == 0)
                            {
                                MN_SumService TB3=new MN_SumService();
                                {
                                    TB3.VehicleID = int.Parse(Device.Tag.ToString());
                                    TB3.TimeSum = 0;
                                    TB3.KMSum = 0;
                                }
                                db.MN_SumServices.InsertOnSubmit(TB3);
                                db.SubmitChanges();
                            }


                            if (Flag)
                            {
                                ComboBoxItem TypeItem = (ComboBoxItem) Com_Model.SelectedItem;
                                string Value = TypeItem.Tag.ToString();
                                string Model = Value;
                                string Path = "\\PrintFile\\" + "\\" + Model + "\\";
                                string FolderPath = SaveFile + Path;

                                var lastinsertedId = TB.ID;

                                if (!Directory.Exists(FolderPath))
                                {
                                    Directory.CreateDirectory(FolderPath);
                                }
                                string[] FileName_Old = new string[] {};
                                FileName_Old = System.IO.Path.GetFileName(FileName).Split('.');
                                if (FileName_Old.Length > 2)
                                {
                                    FileName_Old[1] = FileName_Old[FileName_Old.Length - 1];
                                }
                                FileName_Old[0] = lastinsertedId.ToString();
                                string NewName = FileName_Old[0] + "." + FileName_Old[1];
                                Path = Path + NewName;
                                string FilePath = FolderPath + NewName;
                                System.IO.File.Copy(FileName, FilePath, true);


                                var query = (from c in db.MN_DefineServiceCycles
                                    where c.ID == int.Parse(lastinsertedId.ToString())
                                    select c).Single();
                                query.FileURL = Path;
                                db.SubmitChanges();
                                Flag = false;
                            }




                            MessageBox.Show("برنامه مورد نظر با موفقيت ثبت شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridSpecialService();
                            Com_CycleType.SelectedIndex = 0;
                            Text_Title.Text = "";
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

        public class Show_ListProgramSpecailService
        {
            public string Title { get; set; }
            public string DeviceName { get; set; }
            public string CycleType { get; set; }
            public string ValueCycle { get; set; }
            public string Schedule { get; set; }
            public string Warning { get; set; }
            public string Alarm { get; set; }
           
            public string GroupRepair { get; set; }
            public string SpecialServiceCode { get; set; }

        }

        private void Fill_GridSpecialService()
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

                        var query = from c in db.MN_DefineServiceCycles
                            join k in db.ADM_Vehicles on c.VehicleID equals k.ID
                            join m in db.MN_CycleCriteriaTypes on c.CriteriaID equals m.ID
                            join g in db.MN_Groups on c.GroupID equals g.ID
                            where c.VehicleID == int.Parse(valuetag.ToString())
                            orderby c.ID descending
                            select
                                new
                                {
                                    SpecialServiceCode = c.ID,
                                    Title = c.Title,
                                    DeviceName = k.Name,
                                    CycleType = m.Type,
                                    ValueCycle = c.Value,
                                    GroupRepair = g.Type,
                                    Schedule = c.Schedule,
                                    Warning = c.Warning,
                                    Alarm = c.Alarm

                                };

                        List<Show_ListProgramSpecailService> Items = new List<Show_ListProgramSpecailService>();

                        foreach (var Service in query)
                        {
                            Items.Add(new Show_ListProgramSpecailService()
                            {
                                SpecialServiceCode = Service.SpecialServiceCode.ToString(),
                                Title = Service.Title,
                                DeviceName = Service.DeviceName,
                                CycleType = Service.CycleType,
                                ValueCycle = Service.ValueCycle.ToString(),
                                GroupRepair = Service.GroupRepair,
                                Schedule = Service.Schedule.ToString(),
                                Warning = Service.Warning.ToString(),
                                Alarm = Service.Alarm.ToString()
                            });
                        }
                        Grid_RegisterSpecialService.ItemsSource = Items;
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

                    var query = from c in db.MN_DefineServiceCycles
                        where c.ID == int.Parse(Btn_Code.Tag.ToString())
                        select c;
                    if (query.Count() != 0)
                    {
                        try
                        {
                            var query2 = from c in db.MN_CreateServices
                                where c.ServiceCycleID == int.Parse(Btn_Code.Tag.ToString())
                                select c;
                            if (query2.Count() >= 1)
                            {
                                db.MN_CreateServices.DeleteAllOnSubmit(query2);
                                db.SubmitChanges();
                            }

                            db.MN_DefineServiceCycles.DeleteOnSubmit(query.Single());
                            db.SubmitChanges();
                            MessageBox.Show("برنامه مورد نظر حذف شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridSpecialService();
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
                    (from c in db.MN_DefineServiceCycles where c.ID == int.Parse(Btn_Code.Tag.ToString()) select c)
                        .Single();
                Text_ValueCycle.Text = query.Value.ToString();
                Com_CycleType.SelectedValue = int.Parse(query.CriteriaID.ToString());
                Com_GroupRepair.SelectedValue = int.Parse(query.GroupID.ToString());
                Btn_CancelEdit.Tag = query.ID.ToString();
                Btn_CancelEdit.Visibility = Visibility.Visible;
                Btn_RegisterProgram.Content = "ثبت تغییرات";
                lbl_firstworking.Visibility = Visibility.Hidden;
                Text_FirstWorking.Visibility = Visibility.Hidden;
                Text_Schedule.Text = query.Schedule.ToString();
                Text_Alarm.Text = query.Alarm.ToString();
                Text_Warning.Text = query.Warning.ToString();
                Text_Title.Text = query.Title;
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
            Text_Title.Text = "";
            Com_GroupRepair.SelectedIndex = 0;
            lbl_firstworking.Visibility = Visibility.Visible;
            Text_FirstWorking.Visibility = Visibility.Visible;
            Text_Schedule.Text = "";
            Text_Alarm.Text = "";
            Text_Warning.Text = "";

        }

        private void Com_Device_DropDownClosed(object sender, EventArgs e)
        {
            Fill_GridSpecialService();
            Btn_RegisterProgram.Content = "ثبت برنامه";
            Btn_CancelEdit.Visibility = Visibility.Hidden;
            Text_ValueCycle.Text = "";
            Com_CycleType.SelectedIndex = 0;
            Text_Title.Text = "";
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
                    Text_Title.Text = "";
                    Com_GroupRepair.SelectedIndex = 0;
                    lbl_firstworking.Visibility = Visibility.Visible;
                    Text_FirstWorking.Visibility = Visibility.Visible;

                    Grid_RegisterSpecialService.ItemsSource = "";

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
                    Text_Title.Text = "";
                    Com_GroupRepair.SelectedIndex = 0;
                    lbl_firstworking.Visibility = Visibility.Visible;
                    Text_FirstWorking.Visibility = Visibility.Visible;
                    Grid_RegisterSpecialService.ItemsSource = "";
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

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog Dlg = new Microsoft.Win32.OpenFileDialog();

            Dlg.DefaultExt = ".pdf";
            Dlg.Filter = "*.pdf|*.pdf|All Files (*.*)|*.*";
            Dlg.Title = "انتخاب فایل";
            Nullable<bool> Result = Dlg.ShowDialog();


            if (Result == true)
            {
                Flag = true;
                FileName = Dlg.FileName;
            }


        }

    }
}
