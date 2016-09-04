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
using System.Collections.ObjectModel;
using maintenance.classes;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for programing_lot.xaml
    /// </summary>
    public partial class ShowListPart_RegisterProgram : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public ShowListPart_RegisterProgram()
        {
            InitializeComponent();
        }

        private void Text_PartName_TextChanged(object sender, TextChangedEventArgs e)
        {

            FillGridPart();
            
        }

        
        public class Show_Part
        {
            public string PartName { get; set; }
            public string PartModel { get; set; }
            public string Country { get; set; }
            public string PartCode { get; set; }
            
            

        }

        private void FillGridPart()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Parts where c.Name.Contains(Text_PartName.Text) select c;

                List<Show_Part> items = new List<Show_Part>();

                foreach (var lot in query)
                {
                    items.Add(new Show_Part()
                    {
                        PartCode = lot.ID.ToString(),
                        PartName = lot.Name,
                        PartModel = lot.Model,
                        Country = lot.Country
                    });
                }
                Grid_ShowPart.ItemsSource = items;
            }
        }

        private void showform(object sender, RoutedEventArgs e)
        {
            RegisterProgramingPart Child = new RegisterProgramingPart();
            Button btn_code = sender as Button;
            Child.Text_codelot.Text = btn_code.Tag.ToString();
            Child.Owner = this;
            this.Hide();
            Child.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillGridPart();
        }
    }
}
