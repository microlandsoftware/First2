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
using  System.IO;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for programing_service.xaml
    /// </summary>
    public partial class RegisterProgramService : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string SaveFile = ini.IniReadValue("appSettings", "Savefile");
        private static bool Flag=false;
        private static string FileName;
        
        public RegisterProgramService()
        {
            InitializeComponent();
            CommandManager.AddPreviewCanExecuteHandler(Text_ValueCycle, canExecute);
          //  Text_ValueCycle.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste,
                                           //    UndoCommand, CanUndoCommand));
        }

        private void canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
              
                e.Handled = true;
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
                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "لیست برند";
                ComItem2.Tag = "0";
                Com_Brand.Items.Add(ComItem2);
                ComboBoxItem ComItem3 = new ComboBoxItem();
                ComItem3.Content = "لیست مدل";
                ComItem3.Tag = "0";
                Com_Model.Items.Add(ComItem3);
                Com_Model.SelectedIndex = 0;
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
            Load_Com_CycleType();
            Load_Com_GroupRepair();
            
        }

        private void Com_Brand_DropDownClosed(object sender, EventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                Text_TitleService.Text = "";
                Text_TitleService_Cycle.Text = "";
                Text_ValueCycle.Text = "";
                Com_CycleType.SelectedIndex = 0;
                Com_GroupRepair.SelectedIndex = 0;
                Btn_Register.Content = "ثبت برنامه";
                Btn_CancelEdit.Visibility = Visibility.Hidden;

                ComboBoxItem TypeItem = (ComboBoxItem) Com_Brand.SelectedItem;
                string Value = TypeItem.Content.ToString();
                Text_TitleService.Tag = "0";
                Fill_GridService();
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
                        var query2 = from c in db.MN_DefineVisitCycles where c.ModelID == int.Parse(ValueTag) select c;

                        if (query2.Count() < 1)
                        {
                            Text_TitleService.IsEnabled = true;
                            Text_TitleService_Cycle.IsEnabled = false;
                            Text_ValueCycle.IsEnabled = false;
                            Com_CycleType.IsEnabled = false;
                            Com_GroupRepair.IsEnabled = false;
                            Btn_SelectFile.IsEnabled = false;
                            Text_Schedule.IsEnabled = false;
                            Text_Alarm.IsEnabled = false;
                            Text_Warning.IsEnabled = false;
                            Text_TitleService.Text = "";
                            Text_TitleService.Tag = "0";
                            Fill_GridService();

                        }
                        else
                        {
                            var q = query2.Single();
                            Text_TitleService_Cycle.IsEnabled = true;
                            Text_ValueCycle.IsEnabled = true;
                            Com_CycleType.IsEnabled = true;
                            Btn_SelectFile.IsEnabled = true;
                            Com_GroupRepair.IsEnabled = true;
                            Text_Schedule.IsEnabled = true;
                            Text_Alarm.IsEnabled = true;
                            Text_Warning.IsEnabled = true;
                            Text_TitleService.IsEnabled = false;
                            Text_TitleService.Text = q.Title;
                            Text_TitleService.Tag = q.ID.ToString();
                            Fill_GridService();

                        }

                    }
                    else
                    {
                        Text_TitleService.IsEnabled = false;
                        Text_TitleService_Cycle.IsEnabled = false;
                        Text_ValueCycle.IsEnabled = false;
                        Com_CycleType.IsEnabled = false;
                        Com_GroupRepair.IsEnabled = false;
                        Text_Schedule.IsEnabled = false;
                        Text_Alarm.IsEnabled = false;
                        Text_Warning.IsEnabled = false;
                        Text_TitleService.Text = "";
                        Text_TitleService.Tag = "0";
                        Fill_GridService();
                        Text_TitleService.Text = "";
                    }
                }
            }
        }

        private void Load_Com_GroupRepair()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Groups select c;

                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "نوع گروه";
                ComItem2.Tag = "0";
                Com_GroupRepair.Items.Add(ComItem2);
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

        private void Load_Com_CycleType()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_CycleCriteriaTypes select c;

                ComboBoxItem ComItem2 = new ComboBoxItem();
                ComItem2.Content = "نوع دوره";
                ComItem2.Tag = "0";
                Com_CycleType.Items.Add(ComItem2);
                Com_CycleType.SelectedIndex = 0;

                foreach (var Items in query)
                {
                    ComboBoxItem ComItem = new ComboBoxItem();
                    ComItem.Content = Items.Type;
                    ComItem.Tag = Items.ID.ToString();
                    Com_CycleType.Items.Add(ComItem);
                }
            }
        }

        private void Btn_Register_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (Text_TitleService.Tag == "0")
                {
                    if (Text_TitleService.Text == "")
                        MessageBox.Show("لطفا عنوان سرویس بازدید را وارد نمایید", "خطا", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else
                    {
                        ComboBoxItem TypeItem = (ComboBoxItem) Com_Model.SelectedItem;
                        string Value = TypeItem.Tag.ToString();
                        try
                        {
                            MN_DefineVisitCycle TB = new MN_DefineVisitCycle();
                            {
                                TB.Title = Text_TitleService.Text;
                                TB.ModelID = int.Parse(Value);
                            }
                            db.MN_DefineVisitCycles.InsertOnSubmit(TB);
                            db.SubmitChanges();
                            MessageBox.Show("اطلاعات با موفقیت ثبت شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Text_TitleService_Cycle.IsEnabled = true;
                            Text_ValueCycle.IsEnabled = true;
                            Com_CycleType.IsEnabled = true;
                            Btn_SelectFile.IsEnabled = true;
                            Com_GroupRepair.IsEnabled = true;
                            Text_TitleService.IsEnabled = false;
                            Text_Alarm.IsEnabled = true;
                            Text_Warning.IsEnabled = true;
                            Text_Schedule.IsEnabled = true;
                            var query =
                                (from c in db.MN_DefineVisitCycles orderby c.ID select c)
                                    .ToArray().Last();
                            Text_TitleService.Tag = query.ID.ToString();


                        }
                        catch (Exception)
                        {
                            MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                        }

                    }
                }
                else
                {
                    ComboBoxItem cycletype_value = (ComboBoxItem) Com_CycleType.SelectedItem;
                    ComboBoxItem grouprepair_value = (ComboBoxItem) Com_GroupRepair.SelectedItem;

                    if (Text_TitleService.Text == "")
                        MessageBox.Show("لطفا عنوان سرویس بازدی را وارد نمایید", "خطا", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else if (Text_TitleService_Cycle.Text == "")
                        MessageBox.Show("لطفا نام سرویس را وارد نمایید", "خطا", MessageBoxButton.OK,
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
                    else if (cycletype_value.Tag.ToString() == "0")
                        MessageBox.Show("لطفا نوع دوره سرویس را انتخاب نمایید", "خطا", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else if (Text_ValueCycle.Text == "" || int.Parse(Text_ValueCycle.Text)<=0)
                        MessageBox.Show("لطفا مقدار دوره سرویس را وارد نمایید", "خطا", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else if (grouprepair_value.Tag.ToString() == "0")
                        MessageBox.Show("لطفا نوع گروه را مشخص نمایید", "خطا", MessageBoxButton.OK,
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
                                query.Title = Text_TitleService_Cycle.Text;
                                query.CriteriaID = int.Parse(cycletype_value.Tag.ToString());
                                query.GroupID = int.Parse(grouprepair_value.Tag.ToString());
                                query.Value = int.Parse(Text_ValueCycle.Text);
                                query.Warning = int.Parse(Text_Warning.Text);
                                query.Alarm = int.Parse(Text_Alarm.Text);
                                query.Schedule = int.Parse(Text_Schedule.Text);
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
                                        FileName_Old[1] = FileName_Old[FileName_Old.Length-1];
                                    }
                                    FileName_Old[0] = Btn_CancelEdit.Tag.ToString();
                                    string NewName = FileName_Old[0] + "." + FileName_Old[1];
                                    Path = Path + NewName;
                                    string FilePath = FolderPath + NewName;
                                    System.IO.File.Copy(FileName, FilePath, true);

                                    query.FileURL = Path;

                                    Flag = false;
                                }

                                var query2 = from c in db.MN_DefineVisitCycles
                                    where c.ID ==int.Parse(Text_TitleService.Tag.ToString())
                                    select c;
                                if (query2.Count() >= 1)
                                {
                                    query2.Take(1).Single().Title = Text_TitleService.Text;
                                }
                                
                                
                                db.SubmitChanges();




                                Btn_CancelEdit_Click(sender, e);
                                MessageBox.Show("سرویس با موفقیت ویرایش شد", "موفقيت", MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
                                Fill_GridService();
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
                                MN_DefineServiceCycle TB = new MN_DefineServiceCycle();
                                {
                                    TB.Title = Text_TitleService_Cycle.Text;
                                    TB.VisitCycleID = int.Parse(Text_TitleService.Tag.ToString());
                                    TB.CriteriaID = int.Parse(cycletype_value.Tag.ToString());
                                    TB.GroupID = int.Parse(grouprepair_value.Tag.ToString());
                                    TB.Value = int.Parse(Text_ValueCycle.Text);
                                    TB.Warning = int.Parse(Text_Warning.Text);
                                    TB.Alarm = int.Parse(Text_Alarm.Text);
                                    TB.Schedule = int.Parse(Text_Schedule.Text);
                                }
                                db.MN_DefineServiceCycles.InsertOnSubmit(TB);
                                db.SubmitChanges();
                                if (Flag)
                                {
                                    string Model = Text_TitleService.Tag.ToString();
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

                                MessageBox.Show("سرویس با موفقیت ثبت شد", "موفقيت", MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
                                Text_ValueCycle.Text = "";
                                Text_TitleService_Cycle.Text = "";
                                Com_CycleType.SelectedIndex = 0;
                                Com_GroupRepair.SelectedIndex = 0;
                                Text_Alarm.Text = "";
                                Text_Warning.Text = "";
                                Text_Schedule.Text = "";
                                Fill_GridService();
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                            }
                        }
                    }
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

        public class Show_Service
        {
            public string ModelName { get; set; }
            public string ServiceName { get; set; }
            public string CycleType { get; set; }
            public string ValueCycle { get; set; }
           public string GroupRepair { get; set; }
            public string ServiceCode { get; set; }
            public string Schedule { get; set; }

            public string Warning { get; set; }
            public string Alarm { get; set; }

        }

        private void Fill_GridService()
        {
            
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_DefineServiceCycles
                    join d in db.MN_Groups on c.GroupID equals d.ID
                    join e in db.MN_CycleCriteriaTypes on c.CriteriaID equals e.ID
                    join t in db.MN_DefineVisitCycles on c.VisitCycleID equals t.ID
                    join m in db.ADM_Models on t.ModelID equals m.ID
                    where t.ID == int.Parse(Text_TitleService.Tag.ToString())
                    orderby c.ID descending
                    select
                        new
                        {
                            ServiceCode = c.ID,
                            ModelName = m.Name,
                            ServiceName = c.Title,
                            CycleType = e.Type,
                            ValueCycle = c.Value,
                            GroupRepair = d.Type,
                            Schedule = c.Schedule,
                            Alarm = c.Alarm,
                            Warning = c.Warning
                        };
                List<Show_Service> items = new List<Show_Service>();

                foreach (var service in query)
                {
                    items.Add(new Show_Service()
                    {
                        ServiceCode = service.ServiceCode.ToString(),
                        ModelName = service.ModelName,
                        ServiceName = service.ServiceName,
                        CycleType = service.CycleType,
                        ValueCycle = service.ValueCycle.ToString(),
                        GroupRepair = service.GroupRepair,
                        Schedule = service.Schedule.ToString(),
                        Warning = service.Warning.ToString(),
                        Alarm = service.Alarm.ToString()
                    });
                }
                Grid_ShowService.ItemsSource = items;
            }
        }


        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
             var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (
                    MessageBox.Show(
                        "آيا از حذف سرویس اطمينان داريد? با حذف سرویس تمام برنامه های ایجاد شده حذف می شود", "هشدار",
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
                            var q = query.Single();
                            string AddressFile = SaveFile + q.FileURL;
                            if (File.Exists(AddressFile))
                            {
                                File.Delete(AddressFile);
                            }

                            db.MN_DefineServiceCycles.DeleteOnSubmit(query.Single());
                            db.SubmitChanges();

                            var query3 = from c in db.MN_CounterServices
                                         where c.ServiceCycleID == int.Parse(Btn_Code.Tag.ToString())
                                         select c;
                            if (query3.Count() >= 1)
                            {
                                db.MN_CounterServices.DeleteAllOnSubmit(query3);
                                db.SubmitChanges();
                            }

                            MessageBox.Show("سرویس مورد نظر حذف شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridService();
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

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button Btn_Code = sender as Button;
                var query = (from c in db.MN_DefineServiceCycles
                    where c.ID == int.Parse(Btn_Code.Tag.ToString())
                    select c).Single();
                Text_TitleService_Cycle.Text = query.Title;
                Text_ValueCycle.Text = query.Value.ToString();
                Text_Alarm.Text = query.Alarm.ToString();
                Text_Warning.Text = query.Warning.ToString();
                Text_Schedule.Text = query.Schedule.ToString();
                Com_CycleType.SelectedValue = query.CriteriaID.ToString();
                Com_GroupRepair.SelectedValue = query.GroupID.ToString();
                Btn_Register.Content = "ثبت تغییرات";

                Btn_CancelEdit.Visibility = Visibility.Visible;
                Btn_CancelEdit.Tag = query.ID.ToString();
                Text_TitleService.IsEnabled = true;
            }
        }

        private void Btn_CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            Text_TitleService_Cycle.Text = "";
            Text_ValueCycle.Text = "";
            Com_CycleType.SelectedIndex = 0;
            Com_GroupRepair.SelectedIndex = 0;
            Btn_Register.Content = "ثبت برنامه";
            Btn_CancelEdit.Visibility = Visibility.Hidden;
            Text_Alarm.Text = "";
            Text_Warning.Text = "";
            Text_Schedule.Text = "";
            Text_TitleService.IsEnabled = false;
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

        private void Text_ValueCycle_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void command_function(object sender, ExecutedRoutedEventArgs e)
        {
            //e.Handled = true; 
            
        }

        private void Text_ValueCycle_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {

        }
        
        
    }
}
