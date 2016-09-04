using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using maintenance;
using maintenance.classes;

public class ConvertDate
{
    //public string getdate(DateTime dt) // متد تبدیل تاریخ میلادی به شمسی
    //{
    //    //DateTime dt = DateTime.Now;
    //    PersianCalendar PC = new PersianCalendar();
    //    string day = PC.GetDayOfMonth(dt).ToString();
    //    string month = PC.GetMonth(dt).ToString();
    //    string year = PC.GetYear(dt).ToString();
    //    if (int.Parse(day) <= 9)
    //        day = 0 + day;
    //    if (int.Parse(month) <= 9)
    //        month = 0 + month;
    //    string Date = year + "/" + month + "/" + day;
    //    return Date;

    //}
    public string GetDateNow() // متد دریافت تاریخ امروز
    {
        DateTime dt = DateTime.Now;
        PersianCalendar PC = new PersianCalendar();
        string Day = PC.GetDayOfMonth(dt).ToString();
        string Month = PC.GetMonth(dt).ToString();
        string Year = PC.GetYear(dt).ToString();
        if (int.Parse(Day) <= 9)
            Day = 0 + Day;
        if (int.Parse(Month) <= 9)
            Month = 0 + Month;
        string Date = Year + "/" + Month + "/" + Day;
        return Date;

    }


    public int GetDateTypeInt() // متد دریافت تاریخ امروز
    {
        DateTime dt = DateTime.Now;
        PersianCalendar PC = new PersianCalendar();
        string Day = PC.GetDayOfMonth(dt).ToString();
        string Month = PC.GetMonth(dt).ToString();
        string year = PC.GetYear(dt).ToString();
        if (int.Parse(Day) <= 9)
            Day = 0 + Day;
        if (int.Parse(Month) <= 9)
            Month = 0 + Month;
        string Date = year + Month + Day;
        
        int DateNow = int.Parse(Date);
        return DateNow;

    }

    public string DateToString(int Date) // متد دریافت تاریخ امروز
    {
        try
        {
            string CharDate = Date.ToString();
            string Year = CharDate.Substring(0, 4);
            string Month = CharDate.Substring(4, 2);
            string Day = CharDate.Substring(6, 2);

            string NewDate = Year + "/" + Month + "/" + Day;

            return NewDate;
        }
        catch
        {
            return Date.ToString();
        }
    }



    //public string getdatemiladi(string _Fdate)
    //{
    //    //string Tarikh = date;
    // //   int Rooz = int.Parse(Tarikh.Substring(8, 2));
    // //  //  if (int.Parse(Rooz) <= 9)
    // //   //    Rooz = "0" + Rooz;
    // //   int Maah =int.Parse(Tarikh.Substring(5, 2));
    // ////   if (int.Parse(Maah) <= 9)
    // // //      Maah = "0" + Maah;
    // //   int Saal = int.Parse(Tarikh.Substring(0, 4));
    //    PersianCalendar pcalendar = new PersianCalendar();
    // //   string dt=(pc.ToDateTime(Saal,Maah,Rooz,0,0,0,0).ToString().Substring(0, 10));

    // //   string[] newdate=new string[]{};
    // //    newdate = dt.Split('/');
    // //   string rooz=newdate[1];
    // //   string maah=newdate[0];
    // //   string saal=newdate[2];

    // //   if((int.Parse(rooz)<=9)&&(rooz.Length<=1))
    // //       rooz=0+rooz;
    // //   if((int.Parse(maah)<=9)&&(maah.Length<=1))
    // //       maah=0+maah;

    // //   string miladi = (saal+"-"+maah+"-"+rooz);
        
    // //   return miladi;


    //    DateTime fdate = Convert.ToDateTime(_Fdate);
    //    GregorianCalendar gcalendar = new GregorianCalendar();
    //    DateTime eDate = pcalendar.ToDateTime(
    //         gcalendar.GetYear(fdate),
    //       gcalendar.GetMonth(fdate),
    //       gcalendar.GetDayOfMonth(fdate),
    //           gcalendar.GetHour(fdate),
    //           gcalendar.GetMinute(fdate),
    //           gcalendar.GetSecond(fdate), 0);
    //    string date = eDate.ToShortDateString();


    //    string[] newdate = new string[] { };
    //    newdate = date.Split('/');
    //    string rooz = newdate[0];
    //    string maah = newdate[1];
    //    string saal = newdate[2];

    //    if ((int.Parse(rooz) <= 9) && (rooz.Length <= 1))
    //        rooz = 0 + rooz;
    //    if ((int.Parse(maah) <= 9) && (maah.Length <= 1))
    //        maah = 0 + maah;

    //    string miladi = (saal + "-" + maah + "-" + rooz);

    //    return miladi;

       

    //}


}


public class GetVehicleService
{
    private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
    public static string cnn = ini.IniReadValue("appSettings", "cnn");
    public string GetServiceCycle_Status(int ID)
    {
        string Type = "0";
        Int64 Working;
        Int64 WorkingTime;
        Int64 WorkingKilometer; 
        var db = new DataClasses1DataContext(cnn);
        if (db.DatabaseExists())
        {
           
            string TypeAlarm = "";
            string TypeWarning = "";
            var query2 = from c in db.MN_CreateServices
                join d in db.MN_DefineServiceCycles on c.ServiceCycleID equals d.ID
                join y in db.MN_CycleCriteriaTypes on d.CriteriaID equals y.ID 
                join t in db.MN_TotalWorks on c.VehicleID equals t.VehicleID
                where c.VehicleID == ID && c.Confirm == false
                select
                    new
                    {
                        ValueService = d.Value.ToString(),
                        CycleType = y.ID,
                        Alarm = d.Alarm.ToString(),
                        Warning = d.Warning.ToString(),
                        Schedule=d.Schedule.ToString(),
                        WorkingInCreateService = c.TotalWork,
                        TotalWorkTime=(t.TotalTime).ToString(),
                        TotalWorkKilometer=(t.TotalKM.ToString()).ToString()
                    };
            foreach (var q in query2)
            {
                
                int ValueService = int.Parse(q.ValueService);
                int PercentService;
                
                int DeviceCode = ID;
                if (q.CycleType == 1)
                    Working = Int64.Parse(q.TotalWorkTime)/3600;
                else
                  Working = Int64.Parse(q.TotalWorkKilometer)/1000;
                
                int Warning = int.Parse(q.Warning);
                int Alarm = int.Parse(q.Alarm);
                int Schedule = int.Parse(q.Schedule);

                double PercentAlarm = Math.Ceiling((Convert.ToDouble(Alarm) * Convert.ToDouble(ValueService) / 100));
                double PercentWarning = Math.Ceiling((Convert.ToDouble(Warning) * Convert.ToDouble(ValueService) / 100));

                double PercentSchedule = Math.Ceiling((Convert.ToDouble(Schedule) * Convert.ToDouble(ValueService) / 100));

               

                double ServiceAlarm = ValueService - PercentAlarm;
                double ServiceWarning = ValueService - PercentWarning;
                double ServiceSchedule = ValueService - PercentSchedule;

                
                Double MinessWorking = Int64.Parse(q.WorkingInCreateService.ToString()) - ServiceSchedule;
                Double WorkingWarning = Int64.Parse(ServiceWarning.ToString()) + MinessWorking;
                Double WorkingAlarm = Int64.Parse(ServiceAlarm.ToString()) + MinessWorking;
                
               
                 if (WorkingAlarm <= Working)
                {
                    TypeAlarm = "2";
                    break;
                }
                else if (WorkingWarning <= Working)
                {
                    TypeWarning = "1";
                }
                
            }

            if (TypeAlarm != "")
                Type = TypeAlarm;
            else if (TypeWarning != "")
                Type = TypeWarning;
            else
                Type = "0";

           
        }
        return Type;
    }

    public string GetServiceLot_Status(int ID)
    {
        string Type = "0";


        var db = new DataClasses1DataContext(cnn);
        if (db.DatabaseExists())
        {
           
            string TypeAlarm = "";
            string TypeWarning = "";
            var query2 = from c in db.MN_CreatePartFixes
                join d in db.MN_DefinePartModes on c.PartModeID equals d.ID
                join y in db.MN_CycleCriteriaTypes on d.CriteriaID equals y.ID
                join t in db.MN_TotalWorks on c.VehicleID equals t.VehicleID
                where c.VehicleID == ID && c.Confirm == false
                select
                    new
                    {
                        ValueService = d.Value.ToString(),
                        CycleType = y.ID,
                        
                        Alarm = d.Alarm.ToString(),
                        Warning = d.Warning.ToString(),
                        Schedule=d.Schedule.ToString(),
                        WorkingInCreateService = c.TotalWork,
                        TotalWorkTime = (t.TotalTime).ToString(),
                        TotalWorkKilometer = (t.TotalKM.ToString()).ToString()
                    };
            foreach (var q in query2)
            {
                //  var query = (from c in db.MN_Plans select c).Single();
                int ValueService = int.Parse(q.ValueService);
                int PercentService;
                Int64 Working;
                int DeviceCode = ID;
                if (q.CycleType == 1)
                    Working = Int64.Parse(q.TotalWorkTime) / 3600;
                else
                    Working = Int64.Parse(q.TotalWorkKilometer) / 1000;
                

                int Warning = int.Parse(q.Warning);
                int Alarm = int.Parse(q.Alarm);
                int Schedule = int.Parse(q.Schedule);

                double PercentAlarm = Math.Ceiling((Convert.ToDouble(Alarm) * Convert.ToDouble(ValueService) / 100));
                double PercentWarning = Math.Ceiling((Convert.ToDouble(Warning) * Convert.ToDouble(ValueService) / 100));
                double PercentSchedule = Math.Ceiling((Convert.ToDouble(Schedule) * Convert.ToDouble(ValueService) / 100));

                double ServiceAlarm = ValueService - PercentAlarm;
                double ServiceWarning = ValueService - PercentWarning;
                double ServiceSchedule = ValueService - PercentSchedule;


                Double MinessWorking = Int64.Parse(q.WorkingInCreateService.ToString()) - ServiceSchedule;
                Double WorkingWarning = Int64.Parse(ServiceWarning.ToString()) + MinessWorking;
                Double WorkingAlarm = Int64.Parse(ServiceAlarm.ToString()) + MinessWorking;


                if (WorkingAlarm <= Working)
                {
                    TypeAlarm = "2";
                    break;
                }
                else if (WorkingWarning <= Working)
                {
                    TypeWarning = "1";
                }
            }

            if (TypeAlarm != "")
                Type = TypeAlarm;
            else if (TypeWarning != "")
                Type = TypeWarning;
            else
                Type = "0";

           


        }
        return Type;
    }

   

}

public class ConnectDataBase
{
    private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
    public static string cnn = ini.IniReadValue("appSettings", "cnn");

    public bool Check()
    {
        //var db = new DataClasses1DataContext(cnn);
        //if (db.DatabaseExists())
        //    return true;
        //else
        //    return false;
        SqlConnection scn = new SqlConnection(cnn);
        Boolean state;
       try
        {
            scn.Open();
            state = true;
        }
        catch (Exception)
        {
            state = false;
        }
        return state;
     }
}