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
    /// Interaction logic for messagebox_selectform.xaml
    /// </summary>
    public partial class MessageBox_SelectForm : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static bool Close;
        public MessageBox_SelectForm()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
           
                if (Btn_Close.IsChecked == true)
                {
                    RegisterExchangePart.Close_Form = "true";
                    RegisterExchangePart.ShowForm_SelectDevice = "false";
                    RegisterExchangePart.ShowForm_Selectpart = "false";
                }
                else if (Btn_RegisterBadParts.IsChecked == true)
                {
                    RegisterExchangePart.Close_Form = "false";
                    RegisterExchangePart.ShowForm_SelectDevice = "false";
                    RegisterExchangePart.ShowForm_Selectpart = "true";
                }

                else if (Btn_SelectDevice.IsChecked == true)
                {
                    RegisterExchangePart.Close_Form = "false";
                    RegisterExchangePart.ShowForm_SelectDevice = "true";
                    RegisterExchangePart.ShowForm_Selectpart = "false";
                }
            Close = true;

                this.Close();
            

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Close = false;
        }
    }
}
