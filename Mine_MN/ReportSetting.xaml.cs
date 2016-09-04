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
using Stimulsoft.Report.Dictionary;
using maintenance.classes;
using Stimulsoft.Report;

namespace myfunc
{
    public class main
    {
        public static void showmessage(string text)
        {
            MessageBox.Show(text);
        }
    }
}
namespace maintenance
{
    /// <summary>
    /// Interaction logic for Report_setting.xaml
    /// </summary>
    public partial class ReportSetting : Window
    {
        public static bool Date;
        public static bool User;
        public static bool RepeaTtitle;
        public static string Title;
        public static string Type;
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public ReportSetting()
        {
            InitializeComponent();
        }

        private void Btn_PrintReport_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                if (Text_TitleReport.Text == "")
                    MessageBox.Show("لطفا عنوان گزارش را وراد نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    if (CheckBox_Date.IsChecked == false)
                        Date = false;
                    else
                        Date = true;

                    if (CheckBox_User.IsChecked == false)
                        User = false;
                    else
                        User = true;
                    if (Type == "badlot")
                    {
                        ConvertDate DT = new ConvertDate();
                        string[] Array = new string[] {};
                        Array = ReportBadPart.Value_Print.Split(',');
                        string StartDate = Array[0];
                        string EndDate = Array[1];
                        int DeviceCode = Convert.ToInt32(Array[2]);
                        string TypeReport = Array[3];

                        var query = (from c in db.ADM_Vehicles
                            join d in db.ADM_Models on c.ModelID equals d.ID
                            join k in db.ADM_Brands on d.BrandID equals k.ID
                            where c.ID == DeviceCode
                            select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                        StiReport Report = new StiReport();
                        Report.Load("Report\\Report_BadPart.mrt");

                        Report.Dictionary.Databases.Clear();
                        Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                        Report.Compile();
                        Report["StartDate"] = int.Parse(StartDate.Replace("/", ""));
                        Report["EndDate"] = int.Parse(EndDate.Replace("/", ""));
                        Report["Type"] = TypeReport;
                        Report["DeviceCode"] = DeviceCode;

                        if (CheckBox_Date.IsChecked == true)
                        {
                            Report["DateNow"] = DT.GetDateNow();
                            Report["TextDate"] = ":تاریخ";
                        }
                        if (CheckBox_User.IsChecked == true)
                        {
                            Report["User"] = MainWindow.UserName;
                            Report["TextUser"] = ":کاربر";
                        }
                        Report["Title"] = Text_TitleReport.Text;
                        Report["Brand"] = query.Brand;
                        Report["Model"] = query.Model;
                        Report["Device"] = query.Device;

                        Report.RenderWithWpf();
                        Report.ShowWithWpf();



                    }
                    else if (Type == "report_lot")
                    {
                        ConvertDate DT = new ConvertDate();
                        string[] Array = new string[] {};
                        Array = ReportProgramPart.Value_Print.Split(',');
                        string StartDate = Array[0];
                        string EndDate = Array[1];
                        int DeviceCode = Convert.ToInt32(Array[2]);
                        string TypeReport = Array[3];

                        var query = (from c in db.ADM_Vehicles
                            join d in db.ADM_Models on c.ModelID equals d.ID
                            join k in db.ADM_Brands on d.BrandID equals k.ID
                            where c.ID == DeviceCode
                            select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                        StiReport Report = new StiReport();
                        Report.Load("Report\\Report_CyclePart.mrt");

                        Report.Dictionary.Databases.Clear();
                        Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                        Report.Compile();
                        Report["StartDate"] = int.Parse(StartDate.Replace("/", ""));
                        Report["EndDate"] = int.Parse(EndDate.Replace("/", ""));
                        Report["Type"] = TypeReport;
                        Report["DeviceCode"] = DeviceCode;

                        if (CheckBox_Date.IsChecked == true)
                        {
                            Report["DateNow"] = DT.GetDateNow();
                            Report["TextDate"] = ":تاریخ";
                        }
                        if (CheckBox_User.IsChecked == true)
                        {
                            Report["User"] = MainWindow.UserName;
                            Report["TextUser"] = ":کاربر";
                        }
                        Report["Title"] = Text_TitleReport.Text;
                        Report["Brand"] = query.Brand;
                        Report["Model"] = query.Model;
                        Report["Device"] = query.Device;

                        Report.RenderWithWpf();
                        Report.ShowWithWpf();


                    }
                    else if (Type == "report_service")
                    {
                        ConvertDate DT = new ConvertDate();
                        string[] Array = new string[] {};
                        Array = ReportProgramService.Value_Print.Split(',');
                        string StartDate = Array[0];
                        string EndDate = Array[1];
                        int DeviceCode = Convert.ToInt32(Array[2]);
                        string TypeReport = Array[3];

                        var query = (from c in db.ADM_Vehicles
                            join d in db.ADM_Models on c.ModelID equals d.ID
                            join k in db.ADM_Brands on d.BrandID equals k.ID
                            where c.ID == DeviceCode
                            select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                        StiReport Report = new StiReport();
                        Report.Load("Report\\Report_CycleService.mrt");

                        Report.Dictionary.Databases.Clear();
                        Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                        Report.Compile();
                        Report["StartDate"] = int.Parse(StartDate.Replace("/", ""));
                        Report["EndDate"] = int.Parse(EndDate.Replace("/", ""));
                        Report["DeviceCode"] = DeviceCode;
                        if (CheckBox_Date.IsChecked == true)
                        {
                            Report["DateNow"] = DT.GetDateNow();
                            Report["TextDate"] = ":تاریخ";
                        }
                        if (CheckBox_User.IsChecked == true)
                        {
                            Report["User"] = MainWindow.UserName;
                            Report["TextUser"] = ":کاربر";
                        }
                        Report["Title"] = Text_TitleReport.Text;
                        Report["Brand"] = query.Brand;
                        Report["Model"] = query.Model;
                        Report["Device"] = query.Device;

                        Report["Type"] = TypeReport;

                        Report.RenderWithWpf();
                        Report.ShowWithWpf();


                    }

                    this.Close();
                }
            }

        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Btn_Exit.Focus();
        }
    }
}
