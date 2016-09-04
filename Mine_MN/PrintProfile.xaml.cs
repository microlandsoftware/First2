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
using Microsoft.Win32;
using Stimulsoft.Report.Dictionary;
using maintenance.classes;
using Stimulsoft.Report;
using Stimulsoft.Report.Export;
using System.Threading;

namespace maintenance
{
    /// <summary>
    /// Interaction logic for print_profile.xaml
    /// </summary>
    public partial class PrintProfile : Window
    {
        public static string TypePrint;
        public static string TypeSelect_Tab;
        public static int DeviceCode=0;
        public static int StatusDeviceCode = 0;
        public static string AlarmType = "0";
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");


        public PrintProfile()
        {
            InitializeComponent();

            this.Text_StartDate.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            this.Text_EndDate.FlowDirection = System.Windows.FlowDirection.RightToLeft;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConvertDate Date = new ConvertDate();
            string DateNow = Date.GetDateNow();
            Text_StartDate.Text = DateNow;
            Text_EndDate.Text = DateNow;

            //MessageBox.Show(TypePrint);
            if (TypeSelect_Tab == "ProfileGrid_DefineProgramServiceTime" ||
                TypeSelect_Tab == "ProfileGrid_DefineProgramServiceKilometer" || TypeSelect_Tab == "ProfileGrid_DefineProgramPart")
            {

                Text_StartDate.IsEnabled = false;
                Text_EndDate.IsEnabled = false;
            }
        }

        private void Btn_PrintReport_Click(object sender, RoutedEventArgs e)
        {
            if (Text_TitleReport.Text == "")
            {
                MessageBox.Show("لطفا عنوان گزارش را وارد نمایید", "خطا", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            else if (TypeSelect_Tab == "ProfileGrid_DefineProgramServiceKilometer" ||
                     TypeSelect_Tab == "ProfileGrid_DefineProgramServiceTime" || TypeSelect_Tab == "ProfileGrid_DefineProgramPart")
            {
                if (TypeSelect_Tab == "ProfileGrid_DefineProgramServiceTime")
                    Define_CycleService("time");
                else if (TypeSelect_Tab == "ProfileGrid_DefineProgramServiceKilometer")
                    Define_CycleService("kilometr");
                else if (TypeSelect_Tab == "ProfileGrid_DefineProgramPart")
                    Define_CyclePart();
            }

            else if (CheckDate(Text_StartDate.Text))
            {
                if (CheckDate(Text_EndDate.Text))
                {

                    if (TypeSelect_Tab == "ProfileGrid_ProgramServiceTime")
                        Program_Service("time");
                    else if (TypeSelect_Tab == "ProfileGrid_ProgramServiceKilometer")
                        Program_Service("kilometr");
                    else if (TypeSelect_Tab == "ProfileGrid_ProgramPart")
                        Program_CyclePart();
                    else if (TypeSelect_Tab == "ProfileGrid_BadPart")
                        Program_BadPart();
                    else if (TypeSelect_Tab == "ProfileGrid_EventRule")
                        List_EventRule();
                    else if (TypeSelect_Tab == "ProfileGrid_DeviceStatus")
                        List_StatusDevice();
                    else if (TypeSelect_Tab == "ProfileGrid_Alarm")
                        List_Alarm();
                   

                }
            }

            

        }


        private bool CheckDate(string Date)
        {
            bool Flag = true;
            if (Date.Contains(" "))
            {
                MessageBox.Show("لطفا در وارد کردن تاریخ دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                Flag = false;
            }
            else
            {
                string[] Array = new string[] { };
                Array = Date.Split('/');
                string Year = Array[0];
                string Month = Array[1];
                string Day = Array[2];
                if ((int.Parse(Month) > 12) || int.Parse(Day) > 31)
                {
                    MessageBox.Show("لطفا در وارد کردن تاریخ دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    Flag = false;
                }
                else if ((int.Parse(Month) >= 7 && int.Parse(Month) <= 12) && int.Parse(Day) == 31)
                {
                    MessageBox.Show("نیمه دوم سال 30 روزه است", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    Flag = false;
                }
                else if (int.Parse(Year) <= 1350)
                {
                    MessageBox.Show("لطفا در وارد کردن تاریخ دقت نمایید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    Flag = false;
                }
            }
            return Flag;
        }


        private void Program_Service(string Type)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                ConvertDate DT = new ConvertDate();

                string StartDate = Text_StartDate.Text;
                string EndDate = Text_EndDate.Text;
                string TypeReport = "time";
                if (Type == "kilometr")
                    TypeReport = "kilometr";



                var query = (from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    where c.ID == DeviceCode
                    select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_CycleService.mrt");

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


                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }
                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();
                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportexcel")
                {

                    Report.Render();
                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }
            }

        }


        private void Program_CyclePart()
        {

            ConvertDate DT = new ConvertDate();
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                string StartDate = Text_StartDate.Text;
                string EndDate = Text_EndDate.Text;

                var query = (from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    where c.ID == DeviceCode
                    select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_CyclePart.mrt");

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

                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }
                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportexcel")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }
            }
        }

        private void Program_BadPart()
        {
            ConvertDate DT = new ConvertDate();
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                string startdate = Text_StartDate.Text;
                string enddate = Text_EndDate.Text;

                var query = (from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    where c.ID == DeviceCode
                    select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_BadPart.mrt");

                Report.Dictionary.Databases.Clear();
                Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                Report.Compile();
                Report["StartDate"] = int.Parse(startdate.Replace("/", ""));
                Report["EndDate"] = int.Parse(enddate.Replace("/", ""));

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

                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }
                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportexcel")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

            }
        }


        private void List_EventRule()
        {
            ConvertDate dt = new ConvertDate();
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                string startdate = Text_StartDate.Text;
                string enddate = Text_EndDate.Text;

                var query = (from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    where c.ID == DeviceCode
                    select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_EventRule.mrt");

                Report.Dictionary.Databases.Clear();
                Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                Report.Compile();


                startdate = startdate.Replace("/", "");
                enddate = enddate.Replace("/", "");
                Report["StartDate"] = int.Parse(startdate);
                Report["EndDate"] = int.Parse(enddate);


                Report["DeviceCode"] = DeviceCode;

                if (CheckBox_Date.IsChecked == true)
                {
                    Report["DateNow"] = dt.GetDateNow();
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

                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }
                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportexcel")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

            }
        }


        private void List_Alarm()
        {
            ConvertDate dt = new ConvertDate();
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                string startdate = Text_StartDate.Text;
                string enddate = Text_EndDate.Text;

                var query = (from c in db.ADM_Vehicles
                             join d in db.ADM_Models on c.ModelID equals d.ID
                             join k in db.ADM_Brands on d.BrandID equals k.ID
                             where c.ID == DeviceCode
                             select new { Device = c.Name, Model = d.Name, Brand = k.Name }).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_Alarm.mrt");

                Report.Dictionary.Databases.Clear();
                Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                Report.Compile();
                startdate = startdate.Replace("/", "");
                enddate = enddate.Replace("/", "");
                Report["StartDate"] = int.Parse(startdate);
                Report["EndDate"] = int.Parse(enddate);
                Report["Type"] = AlarmType;
                Report["TypeAlarm"] = AlarmType;
                Report["DeviceCode"] = DeviceCode;

                if (CheckBox_Date.IsChecked == true)
                {
                    Report["DateNow"] = dt.GetDateNow();
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

                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }
                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }
                else if (TypePrint == "exportexcel")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

            }
        }

        private void List_StatusDevice()
        {
            ConvertDate dt = new ConvertDate();
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                string startdate = Text_StartDate.Text;
                string enddate = Text_EndDate.Text;

                var query = (from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    where c.ID == DeviceCode
                    select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_StatusDevice.mrt");

                Report.Dictionary.Databases.Clear();
                Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                Report.Compile();
                startdate = startdate.Replace("/", "");
                enddate = enddate.Replace("/", "");
                Report["StartDate"] = int.Parse(startdate);
                Report["EndDate"] = int.Parse(enddate);
                Report["State"] = StatusDeviceCode;
                Report["DeviceCode"] = DeviceCode;

                if (CheckBox_Date.IsChecked == true)
                {
                    Report["DateNow"] = dt.GetDateNow();
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

                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }
                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }
                else if (TypePrint == "exportexcel")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

            }
        }



        private void Define_CycleService(string type)
        {
            ConvertDate dt = new ConvertDate();
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                string startdate = Text_StartDate.Text;
                string enddate = Text_EndDate.Text;

                var query = (from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    where c.ID == DeviceCode
                    select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_DefineService.mrt");

                Report.Dictionary.Databases.Clear();
                Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                Report.Compile();

                if (type == "time")
                    Report["Type"] = "1";
                else
                    Report["Type"] = "2";


                Report["StartDate"] = startdate;
                Report["EndDate"] = enddate;
                Report["State"] = StatusDeviceCode;
                Report["DeviceCode"] = DeviceCode;

                if (CheckBox_Date.IsChecked == true)
                {
                    Report["DateNow"] = dt.GetDateNow();
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

                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }

                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportexcel")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }


            }
        }


        private void Define_CyclePart()
        {
            ConvertDate dt = new ConvertDate();
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                string startdate = Text_StartDate.Text;
                string enddate = Text_EndDate.Text;

                var query = (from c in db.ADM_Vehicles
                    join d in db.ADM_Models on c.ModelID equals d.ID
                    join k in db.ADM_Brands on d.BrandID equals k.ID
                    where c.ID == DeviceCode
                    select new {Device = c.Name, Model = d.Name, Brand = k.Name}).Single();

                StiReport Report = new StiReport();
                Report.Load("Report\\ReportProfile_DefinServicePart.mrt");

                Report.Dictionary.Databases.Clear();
                Report.Dictionary.Databases.Add(new StiSqlDatabase("connect", cnn));

                Report.Compile();


                Report["StartDate"] = startdate;
                Report["EndDate"] = enddate;
                Report["State"] = StatusDeviceCode;
                Report["DeviceCode"] = DeviceCode;

                if (CheckBox_Date.IsChecked == true)
                {
                    Report["DateNow"] = dt.GetDateNow();
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

                if (TypePrint == "print")
                {
                    Report.RenderWithWpf();
                    Report.ShowWithWpf();
                    this.Close();
                }
                else if (TypePrint == "exportpdf")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Adob PDF |*.pdf";
                    s.FileName = "document.pdf";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Pdf, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportword")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Word 2007/2010 |*.docx";
                    s.FileName = "document.docx";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Word2007, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }

                else if (TypePrint == "exportexcel")
                {

                    Report.Render();



                    SaveFileDialog s = new SaveFileDialog();

                    s.Filter = "Microsoft Excel |*.xls";
                    s.FileName = "document.xls";
                    if (s.ShowDialog() == true)
                    {
                        string FileName = s.FileName;
                        Report.ExportDocument(StiExportFormat.Excel, FileName);

                        MessageBox.Show("ذخیره فایل با موفقیت انجام شد");
                        this.Close();
                    }


                }
            }

        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
