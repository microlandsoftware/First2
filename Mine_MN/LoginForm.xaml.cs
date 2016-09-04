using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
  
        private string Message="نام کاربری یا کلمه عبور اشتباه است";
        private string UserId;
        public static byte[] CurrentOperations;
        public static string username, password;
        public static string MessageAccsess = "سطح دسترسی مورد نیاز ، تعریف نشده است";
        public static string TitrMessageAccsess = "سامانه مانیتورینگ معدن";
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public LoginForm()

        {
           
            
            string procName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(procName);

            if (processes.Length > 1)
            {
                MessageBox.Show("برنامه درحال اجرا است");
                this.Close();
            }
            else
            {

                InitializeComponent();
                Text_UserName.Focus();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            UserManagment.Connection = cnn;
            if (UserManagment.LoginUser( Text_UserName.Text.Trim(), Text_Password.Password.Trim()))
            {
                MainWindow.UserName = Text_UserName.Text;
                MainWindow.UserID = UserId;

                MainWindow frmAdmin = new MainWindow();

                if (UserManagment.UserAccess(Text_UserName.Text, UserManagment.Operations.ViewMainTenance))
                {
                    MainWindow.UserName = Text_UserName.Text;
                    MainWindow.UserID = UserId;

                  
                    this.Hide();
                   /// frmAdmin.ShowDialog();

                   Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                   Application.Current.MainWindow = frmAdmin;
                   frmAdmin.Show();


                }
                else
                {
                    MessageBox.Show(MessageAccsess, TitrMessageAccsess);
                }
            }
            else
            {
                MessageBox.Show(Message);
            }
        }

        private void Text_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender,e);
            }
        }
    }
}
