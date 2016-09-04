using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for show_list_xchangelot.xaml
    /// </summary>
    public partial class ShowListPart_ExchangePart : Window
    {
        public static string Text_Search;
        public static readonly Object UnsetValue;
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public ShowListPart_ExchangePart()
        {
            InitializeComponent();
        }

        
        private void Text_SearchPart_TextChanged(object sender, TextChangedEventArgs e)
        {

            FillGrid_Part();
            Text_Search = Text_SearchPart.Text;
        }


        public class Show_Part
        {
            public string PartName { get; set; }
            public string PartModel { get; set; }
            public string Country { get; set; }
            public string PartCode { get; set; }



        }

        private void FillGrid_Part()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Parts where c.Name.Contains(Text_SearchPart.Text) select c;

                List<Show_Part> Items = new List<Show_Part>();

                foreach (var lot in query)
                {
                    Items.Add(new Show_Part()
                    {
                        PartCode = lot.ID.ToString(),
                        PartName = lot.Name,
                        PartModel = lot.Model,
                        Country = lot.Country
                    });
                }
                Grid_ShowPart.ItemsSource = Items;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query =
                    (from c in db.ADM_Vehicles where c.ID == int.Parse(Text_DeviceName.Tag.ToString()) select c).Single();
                Text_DeviceName.Text = query.Name;
                if (Text_Search != null)
                    Text_SearchPart.Text = Text_Search;
                FillGrid_Part();

                Text_DeviceName.FocusVisualStyle = null;
            }
        }

        private void RegisterExchangePart(object sender, RoutedEventArgs e)
        {


            RegisterExchangePart Child = new RegisterExchangePart();
                Button Btn_Code = sender as Button;
                Child.Text_PartName.Tag = Btn_Code.Tag.ToString();
                Child.Text_DeviceName_Select.Tag = Text_DeviceName.Tag.ToString();
                Child.Owner = App.Current.MainWindow;
               
                this.Hide();
                Child.ShowDialog();
            
               
        }

        private void Window_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {


            Text_Search = null;
            ShowDevice_ExchangePart.C_Brand = null;
            ShowDevice_ExchangePart.C_Model = null;


        }

        
    }
}
