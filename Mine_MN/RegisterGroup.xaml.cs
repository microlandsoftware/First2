using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using maintenance.classes;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for registergroup.xaml
    /// </summary>
    public partial class RegisterGroup : Window
    {
       // CollectionViewSource view = new CollectionViewSource();
       // ObservableCollection<show_registergroup> pageshow = new ObservableCollection<show_registergroup>(); 
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public RegisterGroup()
        {
            InitializeComponent();
        }

        private void Btn_RegisterGroup_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (Text_GroupName.Text == "")
                    MessageBox.Show("لطفا نام گروه را وارد نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (Btn_CancelEdit.Visibility == Visibility.Visible)
                    {
                        try
                        {
                            var query = (from c in db.MN_Groups
                                where c.ID == int.Parse(Btn_CancelEdit.Tag.ToString())
                                select c).Single();
                            query.Type = Text_GroupName.Text;
                            query.Description = Text_Description.Text;
                            db.SubmitChanges();
                            MessageBox.Show(" گروه مورد نظر با موفقیت ویرایش شد", "موفقیت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridGroup();
                            Text_GroupName.Text = "";
                            Text_Description.Text = "";
                            Btn_CancelEdit.Visibility = Visibility.Hidden;
                            Btn_RegisterGroup.Content = "ثبت گروه";
                        }
                        catch (Exception)
                        {

                            MessageBox.Show("خطا در ویرایش اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                    }
                    else
                    {
                        try
                        {
                            MN_Group TB = new MN_Group();
                            {
                                TB.Type = Text_GroupName.Text;
                                TB.Description = Text_Description.Text;
                            }
                            db.MN_Groups.InsertOnSubmit(TB);
                            db.SubmitChanges();
                            MessageBox.Show("گروه مورد نظر با موفقیت ثبت شد", "موفقیت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridGroup();
                            Text_GroupName.Text = "";
                            Text_Description.Text = "";
                        }
                        catch (Exception)
                        {

                            MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                }

            }


        }

       

        private void Fill_GridGroup()
        {
            
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Groups orderby c.ID descending select c;
                int itemcount = query.Count();

                List<Show_RegisterGroup> Items = new List<Show_RegisterGroup>();



                foreach (var lot in query)
                {
                    Items.Add(new Show_RegisterGroup() {GroupCode = lot.ID.ToString(), GroupName = lot.Type});
                }
                Grid_RegisterGroup.ItemsSource = Items;
            }

        }


        public class Show_RegisterGroup
        {
            public string GroupName { get; set; }
            public string Description { get; set; }
            public string GroupCode { get; set; }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Fill_GridGroup();

        }

        private void Delete_click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (
                    MessageBox.Show("آیا از حذف گروه اطمینان دارید", "هشدار", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    Button Btn_Code = sender as Button;

                    var query = from c in db.MN_Groups where c.ID == int.Parse(Btn_Code.Tag.ToString()) select c;
                    if (query.Count() >= 1)
                    {
                        
                        try
                        {
                            bool CheckDelete = false;

                            var query2 = from c in db.MN_DefinePartModes
                                where c.GroupID == int.Parse(Btn_Code.Tag.ToString())
                                select c;
                            if (query2.Count() >= 1)
                                CheckDelete = true;
                            else
                            {
                                var query3 = from c in db.MN_DefineServiceCycles
                                    where c.GroupID == int.Parse(Btn_Code.Tag.ToString())
                                    select c;
                                if(query3.Count()>=1)
                                    CheckDelete = true;
                            }

                            if (!CheckDelete)
                            {
                                db.MN_Groups.DeleteOnSubmit(query.Single());
                                db.SubmitChanges();
                                MessageBox.Show("گروه مورد نظر حذف شد", "موفقیت", MessageBoxButton.OK,
                                    MessageBoxImage.Asterisk);
                                Fill_GridGroup();
                            }
                            else
                            {
                                MessageBox.Show("از گروه مورد نظر در تعریف سرویس دوره ای استفاده شده است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("قادر به حذف گروه نیستید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                        }

                        Btn_CancelEdit_Click(sender, e);
                    }
                }
            }
        }

        private void Btn_CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            Text_GroupName.Text = "";
            Text_Description.Text = "";
            Btn_CancelEdit.Visibility = Visibility.Hidden;
            Btn_RegisterGroup.Content = "ثبت گروه";
        }

        private void edit_group(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button Btn_Code = sender as Button;
                var query = (from c in db.MN_Groups where c.ID == int.Parse(Btn_Code.Tag.ToString()) select c).Single();
                Text_GroupName.Text = query.Type;
                Text_Description.Text = query.Description;
                Btn_CancelEdit.Visibility = Visibility.Visible;
                Btn_RegisterGroup.Content = "ثبت تغییرات";
                Btn_CancelEdit.Tag = Btn_Code.Tag.ToString();
            }

        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
       
    }
}
