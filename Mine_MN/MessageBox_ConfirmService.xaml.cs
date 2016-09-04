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

namespace maintenance
{
    /// <summary>
    /// Interaction logic for messagebox.xaml
    /// </summary>
    public partial class MessageBox_ConfirmService : Window
    {
        private bool Close=false;
        public MessageBox_ConfirmService()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Btn_Yes_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MessageBoxConfirm_Flag = true;
            if (CheckBox.IsChecked == true)
                MainWindow.MessageBoxConfirm_FlagCheck = true;
            else
                MainWindow.MessageBoxConfirm_FlagCheck = false;
            Close = true;
            this.Close();

            
        }

        private void Btn_No_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.MessageBoxConfirm_Flag = false;
            if (CheckBox.IsChecked == true)
                MainWindow.MessageBoxConfirm_FlagCheck = true;
            else
                MainWindow.MessageBoxConfirm_FlagCheck = false;
             Close = true;
            this.Close();
           
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Close==false)
            {
                MainWindow.MessageBoxConfirm_Flag = false;
                MainWindow.MessageBoxConfirm_FlagCheck = false;
            }
        }
    }
}
