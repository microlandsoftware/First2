using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    /// Interaction logic for registerlot.xaml
    /// </summary>
   
    public partial class RegisterPart : Window
    {
        CollectionViewSource view = new CollectionViewSource();
        ObservableCollection<show_registerlot> PageShow = new ObservableCollection<show_registerlot>();
        int CurrentPageIndex = 0;
        int ItemPerPage = 9;
        int TotalPage = 0;
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public RegisterPart()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_registerlot_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (Text_PartName.Text == "")
                    MessageBox.Show("لطفا نام قطعه را وارد نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {

                    if (Btn_CancelEdit.Visibility == Visibility.Hidden)
                    {
                        MN_Part tb = new MN_Part();
                        {
                            tb.Name = Text_PartName.Text;
                            tb.Model = Text_PartModel.Text;
                            tb.Country = Text_Country.Text;
                        }

                        db.MN_Parts.InsertOnSubmit(tb);
                        db.SubmitChanges();
                        try
                        {
                            Fill_GridPart();
                            MessageBox.Show("قطعه مورد نظر با موفقیت ثبت شد");

                            Text_PartModel.Text = "";
                            Text_Country.Text = "";
                            Text_PartName.Text = "";

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                    else
                    {
                       

                        try
                        {
                            var query = (from c in db.MN_Parts
                                         where c.ID == int.Parse(Btn_CancelEdit.Tag.ToString())
                                         select c).Single();
                            query.Name = Text_PartName.Text;
                            query.Model = Text_PartModel.Text;
                            query.Country = Text_Country.Text;


                            db.SubmitChanges();
                            MessageBox.Show("اطلاعات با موفقیت ویرایش شد", " موفقیت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridPart();
                            Btn_CancelEdit.Visibility = Visibility.Hidden;
                            Btn_RegisterPart.Content = "ثبت قطعه";
                            Text_PartName.Text = "";
                            Text_PartModel.Text = "";
                            Text_Country.Text = "";

                        }
                        catch (Exception)
                        {
                            MessageBox.Show("خطا در ویرایش اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                        }



                    }
                }
            }
        }


        private void Fill_GridPart()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                PageShow.Clear();

                var query = from c in db.MN_Parts orderby c.ID descending select c;
                int itemcount = query.Count();
                //  List<show_registerlot> items = new List<show_registerlot>();
                foreach (var Part in query)
                {

                    if (Part.ID.ToString() != "")
                        PageShow.Add(new show_registerlot()
                        {
                            PartCode = Part.ID.ToString(),
                            PartName = Part.Name,
                            PartModel = Part.Model,
                            Country = Part.Country
                        });
                }
                // gride_registerlot.ItemsSource = items;

                TotalPage = itemcount/ItemPerPage;
                if (itemcount%ItemPerPage != 0)
                {
                    TotalPage += 1;
                }

                view.Source = PageShow;

                view.Filter += new FilterEventHandler(view_Filter);

                this.Grid_RegisterPart.DataContext = view;
                ShowCurrentPageIndex();
                this.tbTotalPage.Text = TotalPage.ToString();
            }
        }

        public class show_registerlot
        {
            public string PartName { get; set; }
            public string PartModel { get; set; }
            public string Country { get; set; }
            public string PartCode { get; set; }
            
        }

        private void Delete_Part(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (
                    MessageBox.Show(
                        "آیا از حذف قطعه اطمینان دارید؟ با حذف قطعه تمام برنامه ها و خرابی های ثبت شده هم حذف می شود؟",
                        "هشدار", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    Button Btn_Code = sender as Button;

                    var query = from c in db.MN_Parts where c.ID == int.Parse(Btn_Code.Tag.ToString()) select c;
                    if (query.Count() >= 1)
                    {

                        try
                        {
                            var query3 = from c in db.MN_CreatePartFixes
                                join m in db.MN_DefinePartModes on c.PartModeID equals m.ID
                                where m.PartID == int.Parse(Btn_Code.Tag.ToString())
                                select c;
                            db.MN_CreatePartFixes.DeleteAllOnSubmit(query3);
                            var query2 = from c in db.MN_DefinePartModes
                                where c.PartID == int.Parse(Btn_Code.Tag.ToString())
                                select c;
                            db.MN_DefinePartModes.DeleteAllOnSubmit(query2);

                            var query4 = from c in db.MN_PartFixes
                                where c.PartID == int.Parse(Btn_Code.Tag.ToString())
                                select c;
                            db.MN_PartFixes.DeleteAllOnSubmit(query4);

                            db.MN_Parts.DeleteOnSubmit(query.Single());
                            db.SubmitChanges();
                            MessageBox.Show("قطعه مورد نظر حذف شد", "موفقیت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            Fill_GridPart();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("قادر به حذف قطعه نیستید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                        }
                    }
                    btn_cancel_edit_Click(sender, e);
                }
            }
        }

        private void Edit_Part(object sender, RoutedEventArgs e)
        {
            
            
            
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                Button btn_code = sender as Button;
                var query = (from c in db.MN_Parts where c.ID == int.Parse(btn_code.Tag.ToString()) select c).Single();
                Text_PartName.Text = query.Name;
                Text_PartModel.Text = query.Model;
                Text_Country.Text = query.Country;
                Btn_CancelEdit.Visibility = Visibility.Visible;
                Btn_RegisterPart.Content = "ثبت تغییرات";
                Btn_CancelEdit.Tag = btn_code.Tag.ToString();
            }

        }

        private void btn_cancel_edit_Click(object sender, RoutedEventArgs e)
        {
            Text_PartName.Text = "";
            Text_PartModel.Text = "";
          
            Text_Country.Text = "";
            Btn_CancelEdit.Visibility = Visibility.Hidden;
            Btn_RegisterPart.Content = "ثبت قطعه";
        }

       

        private void ShowCurrentPageIndex()
        {
            this.tbCurrentPage.Text = (CurrentPageIndex + 1).ToString();
        }

     
  
        void view_Filter(object sender, FilterEventArgs e)
        {
            int index = PageShow.IndexOf((show_registerlot)e.Item);

            if (index >= ItemPerPage * CurrentPageIndex && index < ItemPerPage * (CurrentPageIndex + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void btnFirst_Click(object sender, RoutedEventArgs e)
        {
            // Display the first page
            if (CurrentPageIndex != 0)
            {
                CurrentPageIndex = 0;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            // Display previous page
            if (CurrentPageIndex > 0)
            {
                CurrentPageIndex--;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // Display next page
            if (CurrentPageIndex < TotalPage - 1)
            {
                CurrentPageIndex++;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void btnLast_Click(object sender, RoutedEventArgs e)
        {
            // Display the last page
            if (CurrentPageIndex != TotalPage - 1)
            {
                CurrentPageIndex = TotalPage - 1;
                view.View.Refresh();
            }
            ShowCurrentPageIndex();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          Fill_GridPart();
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
