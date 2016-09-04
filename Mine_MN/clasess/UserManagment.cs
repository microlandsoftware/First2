using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

public class UserManagment
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// Function For String Input

    #region CorrectPersianCharacter

    public static string CorrectPersianCharacter(string value)
    {
        if (value == null)
        {
            return value;
        }
        else
        {
            if (value.IndexOf('ی') > -1 || value.IndexOf('ک') > -1)
            {
                value = value.Replace('ی', 'ي');
                value = value.Replace('ک', 'ك');
            }
            return value;
        }
    }

    #endregion

    #region PreventSqlInjection

    public static string PreventSqlInjection(string text)
    {
        if (text != null)
        {
            text = text.Replace("'", "");
            text = text.Replace("''", "");
            text = text.Replace("%%", "");
            text = text.Replace("--", "");
            text = text.Replace("/*", "");
            text = text.Replace("*/", "");
            text = text.Replace("*", "");
            text = text.Replace(";", "");
            return text;
        }
        else
        {
            return text;
        }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// For Encrypt And Dycrypt

    #region Encrypt String

    public static string EncryptString(string text)
    {
        byte[] IV = new byte[8] {0, 255, 99, 155, 3, 77, 88, 203};

        string Key = "123456789";

        byte[] buffer = Encoding.UTF8.GetBytes(text);

        TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();

        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        triple.IV = IV;

        triple.Key = md5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(Key));

        byte[] encodeText = triple.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length);

        return Convert.ToBase64String(encodeText);
    }

    #endregion

    #region Decrypt String

    public static string DecryptString(string EncryptText)
    {
        byte[] IV = new byte[8] {0, 255, 99, 155, 3, 77, 88, 203};
        string Key = "123456789";
        byte[] decodeText = null;

        try
        {
            byte[] buffer = Convert.FromBase64String(EncryptText);

            TripleDESCryptoServiceProvider triple = new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            triple.IV = IV;

            triple.Key = md5.ComputeHash(ASCIIEncoding.UTF8.GetBytes(Key));

            decodeText = triple.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length);
        }
        catch
        {
            return "";
        }
        return Encoding.UTF8.GetString(decodeText);
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// Connection State

    #region Connection State

    public static Boolean ConnectionState(string connection)
    {
        SqlConnection scn = new SqlConnection(connection);
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

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// PersianDate 

    #region fill tarikh

    public static string Filltarikh()
    {
        PersianCalendar dateshamsi = new PersianCalendar();
        string shamsiTodayy = dateshamsi.GetYear(DateTime.Now).ToString();
        string shamsiTodaym = dateshamsi.GetMonth(DateTime.Now).ToString();
        if (shamsiTodaym.Length == 1)
        {
            shamsiTodaym = "0" + shamsiTodaym;
        }
        string shamsiTodayd = dateshamsi.GetDayOfMonth(DateTime.Now).ToString();
        if (shamsiTodayd.Length == 1)
        {
            shamsiTodayd = "0" + shamsiTodayd;
        }
        string date = shamsiTodayy + "/" + shamsiTodaym + "/" + shamsiTodayd;
        return date;
    }

    #endregion

    #region Complete Date

    public static string CompleteDate(string date)
    {
        string[] datepart;

        datepart = date.Split('/');

        if (datepart[1].Length == 1)
        {
            datepart[1] = "0" + datepart[1];
        }
        if (datepart[2].Length == 1)
        {
            datepart[2] = "0" + datepart[2];
        }
        string dates = datepart[0] + "/" + datepart[1] + "/" + datepart[2];
        return dates;
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// Check National Code 

    #region Check National Code

    public static Boolean CheckNationalCode(string nationalcode)
    {
        if (nationalcode == "")
        {
            _messageerror = "کد ملی صحیح می باشد";
            _state = true;
        }
        else
        {
            try
            {
                char[] chArray = nationalcode.ToCharArray();
                int[] numArray = new int[chArray.Length];
                for (int i = 0; i < chArray.Length; i++)
                {
                    numArray[i] = (int) char.GetNumericValue(chArray[i]);
                }
                int num2 = numArray[9];

                switch (nationalcode)
                {
                    case "0000000000":
                    case "1111111111":
                    case "22222222222":
                    case "33333333333":
                    case "4444444444":
                    case "5555555555":
                    case "6666666666":
                    case "7777777777":
                    case "8888888888":
                    case "9999999999":
                        _messageerror = "کد ملی وارد شده صحیح نمی باشد";
                        _state = false;
                        break;
                }
                if (_state)
                {
                    int num3 = ((((((((numArray[0]*10) + (numArray[1]*9)) + (numArray[2]*8)) + (numArray[3]*7)) +
                                   (numArray[4]*6)) + (numArray[5]*5)) + (numArray[6]*4)) + (numArray[7]*3)) +
                               (numArray[8]*2);
                    int num4 = num3%11;
                    if ((((num4 < 2) && (num2 == num4)) || ((num4 >= 2) && (11 - num4 == num2))))
                    {
                        _messageerror = "کد ملی صحیح می باشد";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = "کد ملی نامعتبر است";
                        _state = false;
                    }
                }
                else
                {
                    _messageerror = "کد ملی وارد شده صحیح نمی باشد";
                    _state = false;
                }
            }
            catch (Exception)
            {
                _messageerror = "لطفا یک عدد 10 رقمی وارد کنید";
                _state = false;
            }
        }
        return _state;
    }

    #endregion

    #region validmelli

    public static bool validmelli(string melicode)
    {
        try
        {
            int intSum = 0, i = 0, intC, intD, intP;
            if (melicode.Length != 10)
                return (false);
            if (melicode == "0000000000" || melicode == "1111111111" ||
                melicode == "2222222222" || melicode == "3333333333"
                || melicode == "4444444444" || melicode == "5555555555" ||
                melicode == "6666666666" || melicode == "777777777"
                || melicode == "8888888888" || melicode == "9999999999")
                return (false);
            if (melicode.Substring(0, 3) == "108") return true;
            for (i = 0; i < 9; i++)
                intSum += int.Parse(melicode.Substring(i, 1))*(10 - i);
            intD = intSum%11;
            intC = 11 - intD;
            intP = int.Parse(melicode.Substring(9, 1));
            if ((intD < 2 && intP == intD) || (intD >= 2 && intP == intC))
                return (true);
            else
                return (false);
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Check Use Natinal Code

    public static Boolean CheckUseNationalCode(int id, int groupid, string nationalcode)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        if (id != 0)
        {
            if (ConnectionState(_connection))
            {

                if (nationalcode == "")
                {
                    _state = true;
                    _messageerror = "کد ملی وارد شده تکراری نمی باشد";
                }
                else
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_UserProfile where User_UserProfile.NationalCode = '" +
                                      nationalcode +
                                      "' and User_UserProfile.DelState =0 and User_UserProfile.TypeGroup =" + groupid +
                                      " and User_UserProfile.ID != " + id;
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Insert");

                    if (dt.Tables["Insert"].Rows.Count == 0)
                    {
                        _state = true;
                        _messageerror = "کد ملی وارد شده تکراری نمی باشد";
                    }
                    else
                    {
                        _state = false;
                        _messageerror = "کد ملی وارد شده تکراری می باشد";
                    }
                }
            }
            else
            {
                _state = false;
                _messageerror = "ازتباط با دیتابیس برقرار نمی باشد";
            }
        }
        else
        {
            if (ConnectionState(_connection))
            {
                if (nationalcode == "")
                {
                    _state = true;
                    _messageerror = "کد ملی وارد شده تکراری نمی باشد";
                }
                else
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_UserProfile where User_UserProfile.NationalCode = '" +
                                      nationalcode + "' and User_UserProfile.TypeGroup =" + groupid +
                                      " and User_UserProfile.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Insert");

                    if (dt.Tables["Insert"].Rows.Count == 0)
                    {
                        _state = true;
                        _messageerror = "کد ملی وارد شده تکراری نمی باشد";
                    }
                    else
                    {
                        _state = false;
                        _messageerror = "کد ملی وارد شده تکراری می باشد";
                    }
                }
            }
            else
            {
                _state = false;
                _messageerror = "ازتباط با دیتابیس برقرار نمی باشد";
            }
        }
        return _state;
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// For insert and update and edit

    #region Insert User Profile In Database

    public static Boolean InsertUserProfile(string username, string password, string name, string family,
        string fathername, string nationalcode, string ssn, string brithday, string tel, string mobile, string address,
        string descriptin, string pictureurl, byte typegroup, string evidence, string situationjob, string experience,
        string authentication, string typecontract, Boolean activestate)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        short evidences, situationjobs, experiences, authentications, typecontracts;

        int activestates;

        try
        {
            if (evidence == "" || evidence == null)
            {
                evidences = -1;
            }
            else
            {
                evidences = short.Parse(evidence);
            }
            if (situationjob == "" || situationjob == null)
            {
                situationjobs = -1;
            }
            else
            {
                situationjobs = short.Parse(situationjob);
            }
            if (experience == "" || experience == null)
            {
                experiences = -1;
            }
            else
            {
                experiences = short.Parse(experience);
            }
            if (authentication == "" || authentication == null)
            {
                authentications = -1;
            }
            else
            {
                authentications = short.Parse(authentication);
            }
            if (typecontract == "" || typecontract == null)
            {
                typecontracts = -1;
            }
            else
            {
                typecontracts = short.Parse(typecontract);
            }

            if (activestate)
            {
                activestates = 1;
            }
            else
            {
                activestates = 0;
            }

            if (ConnectionState(_connection))
            {
                if (CheckUseNationalCode(0, int.Parse(typegroup.ToString()), nationalcode))
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_UserProfile where User_UserProfile.UserName = '" + username +
                                      "' and User_UserProfile.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Insert");
                    if (dt.Tables["Insert"].Rows.Count == 0)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"insert into User_UserProfile(UserName,Password,Name,Family,FatherName,NationalCode,SSN,Birthday,Tel,Mobile,Address,
								Description,PictureUrl,TypeGroup,Evidence,SituationJob,Experience,Authentication,TypeContract,ActiveState,DelState)
								VALUES (N'" + username + "', N'" + EncryptString(password) + "', N'" + name + "', N'" + family + "', N'" +
                                          fathername + "', N'" + nationalcode + "', N'" + ssn + "', N'" + brithday +
                                          "', N'" + tel
                                          + "', N'" + mobile + "', N'" + address + "', N'" + descriptin + "', N'" +
                                          pictureurl + "', " + typegroup + ", " + evidences + ", " + situationjobs +
                                          ", " + experiences +
                                          ", " + authentications + ", " + typecontracts + ", " + activestates + ", 0)";

                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();
                        _messageerror = @"رکورد مورد نظر ثبت گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"نام کاربری وارد شده تکراری می باشد";
                        _state = false;
                    }

                    dt.Dispose();
                    sda.Dispose();
                    scm.Dispose();
                    scn.Dispose();
                }
                else
                {
                    // message = "ارتباط با دیتابیس برقرار نمی باشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update User Profile In Database

    public static Boolean UpdateUserProfile(int id, string username, string password, string name, string family,
        string fathername, string nationalcode, string ssn, string brithday, string tel, string mobile, string address,
        string descriptin, string pictureurl, byte typegroup, string evidence, string situationjob, string experience,
        string authentication, string typecontract, Boolean activestate)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        short evidences, situationjobs, experiences, authentications, typecontracts;

        int activestates;

        try
        {
            if (evidence == "" || evidence == null)
            {
                evidences = -1;
            }
            else
            {
                evidences = short.Parse(evidence);
            }
            if (situationjob == "" || situationjob == null)
            {
                situationjobs = -1;
            }
            else
            {
                situationjobs = short.Parse(situationjob);
            }
            if (experience == "" || experience == null)
            {
                experiences = -1;
            }
            else
            {
                experiences = short.Parse(experience);
            }
            if (authentication == "" || authentication == null)
            {
                authentications = -1;
            }
            else
            {
                authentications = short.Parse(authentication);
            }
            if (typecontract == "" || typecontract == null)
            {
                typecontracts = -1;
            }
            else
            {
                typecontracts = short.Parse(typecontract);
            }

            if (activestate)
            {
                activestates = 1;
            }
            else
            {
                activestates = 0;
            }

            if (ConnectionState(_connection))
            {
                if (CheckUseNationalCode(id, int.Parse(typegroup.ToString()), nationalcode))
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_UserProfile where User_UserProfile.ID = " + id +
                                      " and User_UserProfile.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Select");
                    if (dt.Tables["Select"].Rows.Count == 1)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"select * from User_UserProfile where User_UserProfile.UserName = N'" +
                                          username + "' and User_UserProfile.ID != " + id +
                                          " and User_UserProfile.DelState =0";
                        sda.SelectCommand = scm;
                        sda.Fill(dt, "Update");

                        if (dt.Tables["Update"].Rows.Count == 0)
                        {
                            scm.Connection = scn;
                            scm.CommandText = @"update User_UserProfile
                                            set UserName = N'" + username + "' , Password= N'" + EncryptString(password) +
                                              "' , Name = N'" +
                                              name + "' , Family= N'" + family + "' , FatherName= N'" + fathername +
                                              "' , NationalCode= N'" + nationalcode + "' , SSN= N'" + ssn +
                                              "' , Birthday= N'" +
                                              brithday + "' , Tel= N'" + tel + "' , Mobile= N'" + mobile +
                                              "' , Address= N'" + address + "' , Description= N'" + descriptin +
                                              "' , PictureUrl= N'" + pictureurl + "' , TypeGroup= " + typegroup +
                                              " , Evidence= " +
                                              evidences + " , Experience= " + experiences + " , SituationJob= " +
                                              situationjobs + " , Authentication= " + authentications +
                                              " , TypeContract= " + typecontracts + " , ActiveState= " +
                                              activestates +
                                              " where User_UserProfile.ID = " + id;

                            scn.Open();
                            scm.ExecuteNonQuery();
                            scn.Close();
                            _messageerror = @"رکورد مورد نظر ویرایش گردید";
                            _state = true;
                        }
                        else
                        {
                            _messageerror = @"نام کاربری وارد شده تکراری می باشد";
                            _state = false;
                        }
                    }
                    else
                    {
                        _messageerror = @"رکورد مورد نظر یافت نشد";
                        _state = false;
                    }
                    dt.Dispose();
                    sda.Dispose();
                    scm.Dispose();
                    scn.Dispose();
                }
                else
                {
                    // message = "ارتباط با دیتابیس برقرار نمی باشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete User Profile In Database

    public static Boolean DeleteUserProfile(int id)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_UserProfile where User_UserProfile.ID = " + id +
                                  " and User_UserProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Update");
                if (dt.Tables["Update"].Rows.Count == 1)
                {
                    DataRow drRow = dt.Tables["Update"].Rows[0];

                    if (drRow.ItemArray.GetValue(21).ToString() != "True")
                    {
                        scm.Connection = scn;
                        scm.CommandText =
                            @"update User_UserProfile set DelState = 1 where User_UserProfile.DelState =0 and User_UserProfile.ID = " +
                            id;

                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();

                        _picurl = drRow.ItemArray.GetValue(13).ToString();

                        _messageerror = @"رکورد مورد نظر حذف گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"شما نمی توانید مدیر سیستم را حذف کنید";
                        _state = true;
                        _picurl = "";
                    }
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                    _picurl = "";
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
                _picurl = "";
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            _picurl = "";
        }
        return _state;
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////

    #region Insert Role Profile In Database

    public static Boolean InsertRoleProfile(string namefa, string nameen, string description)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_RoleProfile where User_RoleProfile.RoleNameEN = N'" + nameen +
                                  "' and" +
                                  " User_RoleProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");
                if (dt.Tables["Insert"].Rows.Count == 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"insert into User_RoleProfile(RoleNameFa,RoleNameEN,RoleDescription,DelState)
                                                Values (N'" + namefa + "',N'" + nameen + "',N'" + description + "',0)";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();
                    _messageerror = @"رکورد مورد نظر ثبت گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"نام وارد شده تکراری می باشد";
                    _state = false;
                }

                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update Role Profile In Database

    public static Boolean UpdateRoleProfile(int id, string namefa, string nameen, string description)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_RoleProfile where User_RoleProfile.ID = " + id +
                                  " and User_RoleProfile.DelState =0 and User_RoleProfile.RoleNameEN = 'Administrator'";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    _messageerror = @"شما نمی توانید ادمین سیستم را ویرایش کنید";
                    _state = false;
                }
                else
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_RoleProfile where User_RoleProfile.ID = " + id +
                                      " and User_RoleProfile.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Select");
                    if (dt.Tables["Select"].Rows.Count == 1)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"select * from User_RoleProfile where User_RoleProfile.RoleNameEN = N'" +
                                          nameen + "' and User_RoleProfile.ID != " +
                                          id + " and User_RoleProfile.DelState =0";
                        sda.SelectCommand = scm;
                        sda.Fill(dt, "Update");

                        if (dt.Tables["Update"].Rows.Count == 0)
                        {
                            scm.Connection = scn;
                            scm.CommandText = @"update User_RoleProfile
                                                    set RoleNameFa = N'" + namefa + "',RoleNameEN=N'" + nameen +
                                              "',RoleDescription=N'" + description +
                                              "' where User_RoleProfile.ID = " + id;
                            scn.Open();
                            scm.ExecuteNonQuery();
                            scn.Close();
                            _messageerror = @"رکورد مورد نظر ویرایش گردید";
                            _state = true;
                        }
                        else
                        {
                            _messageerror = @"نام وارد شده تکراری می باشد";
                            _state = false;
                        }
                    }
                    else
                    {
                        _messageerror = @"رکورد مورد نظر یافت نشد";
                        _state = false;
                    }
                    dt.Dispose();
                    sda.Dispose();
                    scm.Dispose();
                    scn.Dispose();
                }
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete Role Profile In Database

    public static Boolean DeleteRoleProfile(int id)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_RoleProfile where User_RoleProfile.ID = " + id +
                                  " and User_RoleProfile.DelState =0 and User_RoleProfile.RoleNameEN = 'Administrator'";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    _messageerror = @"شما نمی توانید نقش ادمین سیستم را حذف کنید";
                    _state = false;
                }
                else
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_RoleProfile where User_RoleProfile.ID = " + id +
                                      " and User_RoleProfile.DelState =0 ";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Select");
                    if (dt.Tables["Select"].Rows.Count == 1)
                    {
                        scm.Connection = scn;
                        scm.CommandText =
                            @"update User_RoleProfile set DelState = 1 where User_RoleProfile.DelState =0 and User_RoleProfile.ID = " +
                            id;

                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();

                        _messageerror = @"رکورد مورد نظر حذف گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"رکورد مورد نظر یافت نشد";
                        _state = false;
                    }
                    dt.Dispose();
                    sda.Dispose();
                    scm.Dispose();
                    scn.Dispose();
                }
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////

    #region Insert Permission Profile In Database

    public static Boolean InsertPermissionProfile(string namefa, string nameen, string description, short groupid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_PermissionProfile where User_PermissionProfile.PermissionNameEn = N'" + nameen +
                    "' and" +
                    " User_PermissionProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");
                if (dt.Tables["Insert"].Rows.Count == 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"insert into User_PermissionProfile(PermissionNameFa,PermissionNameEn,PermissionDescription,GroupID,DelState)
                                                Values (N'" + namefa + "',N'" + nameen + "',N'" + description + "', " +
                                      groupid + ",0)";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();
                    _messageerror = @"رکورد مورد نظر ثبت گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"نام وارد شده تکراری می باشد";
                    _state = false;
                }

                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update Permission Profile In Database

    public static Boolean UpdatePermissionProfile(int id, string namefa, string nameen, string description,
        short groupid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_PermissionProfile where User_PermissionProfile.ID = " + id +
                                  " and User_PermissionProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_PermissionProfile where User_PermissionProfile.PermissionNameEn = N'" +
                        nameen + "' and User_PermissionProfile.ID != " +
                        id + " and User_PermissionProfile.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Update");

                    if (dt.Tables["Update"].Rows.Count == 0)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"update User_PermissionProfile
                                                    set PermissionNameFa = N'" + namefa + "',PermissionNameEn= N'" +
                                          nameen + "',PermissionDescription= N'" + description +
                                          "',GroupID=" + groupid + " where User_PermissionProfile.ID = " + id;
                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();
                        _messageerror = @"رکورد مورد نظر ویرایش گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"نام وارد شده تکراری می باشد";
                        _state = false;
                    }
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete Permission Profile In Database

    public static Boolean DeletePermissionProfile(int id)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_PermissionProfile where User_PermissionProfile.ID = " + id +
                                  " and User_PermissionProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"update User_PermissionProfile set DelState = 1 where User_PermissionProfile.DelState =0 and User_PermissionProfile.ID = " +
                        id;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    _messageerror = @"رکورد مورد نظر حذف گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////

    #region Insert Group In Database

    public static Boolean InsertGroup(string namefa, string nameen, string description, string typegroup)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        short typegroups;

        if (typegroup == "" || typegroup == null)
        {
            typegroups = -1;
        }
        else
        {
            typegroups = short.Parse(typegroup);
        }

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupsProfile where User_GroupsProfile.GroupNameEn = N'" + nameen +
                                  "' and" +
                                  " User_GroupsProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");
                if (dt.Tables["Insert"].Rows.Count == 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"insert into User_GroupsProfile(GroupNameFa,GroupNameEn,GroupDescription,TypeGroup,DelState)
                                                Values (N'" + namefa + "', N'" + nameen + "', N'" + description + "'," +
                                      typegroups + ",0)";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();
                    _messageerror = @"رکورد مورد نظر ثبت گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"نام وارد شده تکراری می باشد";
                    _state = false;
                }

                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update Group In Database

    public static Boolean UpdateGroup(int id, string namefa, string nameen, string description, string typegroup)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        short typegroups;

        if (typegroup == "" || typegroup == null)
        {
            typegroups = -1;
        }
        else
        {
            typegroups = short.Parse(typegroup);
        }

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupsProfile where User_GroupsProfile.ID = " + id +
                                  " and User_GroupsProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_GroupsProfile where User_GroupsProfile.GroupNameEn = N'" +
                                      nameen + "' and User_GroupsProfile.ID != " +
                                      id + " and User_GroupsProfile.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Update");

                    if (dt.Tables["Update"].Rows.Count == 0)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"update User_GroupsProfile
                                                    set GroupNameFa = N'" + namefa + "',GroupNameEn=N'" + nameen +
                                          "',GroupDescription=N'" + description +
                                          "',TypeGroup=" + typegroups + " where User_GroupsProfile.ID = " + id;
                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();
                        _messageerror = @"رکورد مورد نظر ویرایش گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"نام وارد شده تکراری می باشد";
                        _state = false;
                    }
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete Group In Database

    public static Boolean DeleteGroup(int id)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupsProfile where User_GroupsProfile.ID = " + id +
                                  " and User_GroupsProfile.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"update User_GroupsProfile set DelState = 1 where User_GroupsProfile.DelState =0 and User_GroupsProfile.ID = " +
                        id;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    _messageerror = @"رکورد مورد نظر حذف گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                    dt.Dispose();
                    sda.Dispose();
                    scm.Dispose();
                    scn.Dispose();
                }
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////

    #region Insert GroupRoles In Database

    public static Boolean InsertGroupRoles(int groupid, int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string grouprolehsh = EncryptString(groupid + "?" + roleid);
        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupRole where User_GroupRole.GroupID = " + groupid +
                                  " and User_GroupRole.RoleID = " + roleid + " and" +
                                  " User_GroupRole.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");
                if (dt.Tables["Insert"].Rows.Count == 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"insert into User_GroupRole(GroupID,RoleID,GroupRoleHsh,DelState)
                                                Values (" + groupid + "," + roleid + ",N'" + grouprolehsh + "',0)";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();
                    _messageerror = @"رکورد مورد نظر ثبت گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر در دیتابیس موجود می باشد";
                    _state = true;
                }

                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update GroupRoles In Database

    public static Boolean UpdateGroupRoles(int id, int groupid, int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string grouprolehsh = EncryptString(groupid + "?" + roleid);

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupRole where User_GroupRole.ID = " + id +
                                  " and User_GroupRole.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_GroupRole where User_GroupRole.GroupID = " + groupid +
                                      " and User_GroupRole.RoleID = " + roleid + " and User_GroupRole.ID != " +
                                      id + " and User_GroupRole.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Update");

                    if (dt.Tables["Update"].Rows.Count == 0)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"update User_GroupRole
                                                    set GroupID =" + groupid + ",RoleID=" + roleid + ",GroupRoleHsh=N'" +
                                          grouprolehsh +
                                          "' where User_GroupRole.ID = " + id;
                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();
                        _messageerror = @"رکورد مورد نظر ویرایش گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"این رکورد از قبل وجود دارد";
                        _state = true;
                    }
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete GroupRoles In Database

    public static Boolean DeleteGroupRoles(int id)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupRole where User_GroupRole.ID = " + id +
                                  " and User_GroupRole.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"update User_GroupRole set DelState = 1 where User_GroupRole.ID = " + id +
                                      " and User_GroupRole.DelState =0";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    _messageerror = @"رکورد مورد نظر حذف گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////

    #region Insert RolePermissions In Database

    public static Boolean InsertRolePermissions(int roleid, int permissionid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string rolepermissionhsh = EncryptString(roleid + "?" + permissionid);

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_RolePermission where User_RolePermission.RoleID = " + roleid +
                                  " and User_RolePermission.PermissionID = " + permissionid + " and" +
                                  " User_RolePermission.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"insert into User_RolePermission(RoleID,PermissionID,RolePermissionHsh,DelState)
                                                Values (" + roleid + "," + permissionid + ",N'" + rolepermissionhsh +
                                      "',0)";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();
                    _messageerror = @"رکورد مورد نظر ثبت گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر در دیتابیس موجود می باشد";
                    _state = true;
                }

                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update RolePermissions In Database

    public static Boolean UpdateRolePermissions(int id, int roleid, int permissionid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string rolepermissionhsh = EncryptString(roleid + "?" + permissionid);

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_RolePermission where User_RolePermission.ID = " + id +
                                  " and User_RolePermission.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_RolePermission where User_RolePermission.RoleID = " + roleid +
                                      " and User_RolePermission.PermissionID = " + roleid +
                                      " and User_RolePermission.ID != " +
                                      id + " and User_RolePermission.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Update");

                    if (dt.Tables["Update"].Rows.Count == 0)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"update User_RolePermission
                                                    set RoleID = " + roleid + ",PermissionID=" + permissionid +
                                          ",RolePermissionHsh=N'" + rolepermissionhsh +
                                          "' where User_RolePermission.ID = " + id;
                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();
                        _messageerror = @"رکورد مورد نظر ویرایش گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"این رکورد از قبل وجود دارد";
                        _state = true;
                    }
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete RolePermissions In Database

    public static Boolean DeleteRolePermissions(int rolid, int perid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_RolePermission where User_RolePermission.RoleID = " + rolid +
                                  " and User_RolePermission.PermissionID = " + perid +
                                  " and User_RolePermission.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"update User_RolePermission set DelState = 1 where User_RolePermission.RoleID = " + rolid +
                        " and User_RolePermission.PermissionID = " + perid +
                        " and User_RolePermission.DelState =0";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    _messageerror = @"رکورد مورد نظر حذف گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////

    #region Insert GroupUsers In Database

    public static Boolean InsertGroupUsers(int userid, int groupid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string usergrouphsh = EncryptString(userid + "?" + groupid);

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupUser where User_GroupUser.UserID = " + userid +
                                  " and User_GroupUser.GroupID = " + groupid + " and" +
                                  " User_GroupUser.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"insert into User_GroupUser(UserID,GroupID,UserGroupHsh,DelState)
                                                Values (" + userid + "," + groupid + ",N'" + usergrouphsh + "',0)";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();
                    _messageerror = @"رکورد مورد نظر ثبت گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر در دیتابیس موجود می باشد";
                    _state = true;
                }

                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update GroupUsers In Database

    public static Boolean UpdateGroupUsers(int id, int userid, int groupid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string usergrouphsh = EncryptString(userid + "?" + groupid);

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupUser where User_GroupUser.ID = " + id +
                                  " and User_GroupUser.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_GroupUser where User_GroupUser.UserID = " + userid +
                                      " and User_GroupUser.GroupID = " + groupid + " and User_GroupUser.ID != " +
                                      id + " and User_GroupUser.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Update");

                    if (dt.Tables["Update"].Rows.Count == 0)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"update User_RolePermission
                                                    set UserID = " + userid + ",GroupID=" + groupid + ",UserGroupHsh=N'" +
                                          usergrouphsh +
                                          "' where User_RolePermission.ID = " + id;
                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();
                        _messageerror = @"رکورد مورد نظر ویرایش گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"این رکورد از قبل وجود دارد";
                        _state = true;
                    }
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete GroupUsers In Database

    public static Boolean DeleteGroupUsers(int id)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupUser where User_GroupUser.ID = " + id +
                                  " and User_GroupUser.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"update User_GroupUser set DelState = 1 where User_GroupUser.ID = " + id +
                                      " and User_GroupUser.DelState =0";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    _messageerror = @"رکورد مورد نظر حذف گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////

    #region Insert UserRoles In Database

    public static Boolean InsertUserRoles(int userid, int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string userrolehsh = EncryptString(userid + "?" + roleid);

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_UserRole where User_UserRole.UserID = " + userid +
                                  " and User_UserRole.RoleID = " + roleid + " and User_UserRole.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"insert into User_UserRole(UserID,RoleID,UserRoleHsh,DelState)
                                                Values (" + userid + "," + roleid + ",N'" + userrolehsh + "',0)";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();
                    _messageerror = @"رکورد مورد نظر ثبت گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر در دیتابیس موجود می باشد";
                    _state = true;
                }

                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Update UserRoles In Database

    public static Boolean UpdateUserRoles(int id, int userid, int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        string userrolehsh = EncryptString(userid + "?" + roleid);

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_UserRole where User_UserRole.ID = " + id +
                                  " and User_UserRole.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_UserRole where User_UserRole.UserID = " + userid +
                                      " and User_UserRole.RoleID = " + roleid + " and User_UserRole.ID != " +
                                      id + " and User_UserRole.DelState =0";
                    sda.SelectCommand = scm;
                    sda.Fill(dt, "Update");

                    if (dt.Tables["Update"].Rows.Count == 0)
                    {
                        scm.Connection = scn;
                        scm.CommandText = @"update User_UserRole
                                                    set UserID = " + userid + ",RoleID=" + roleid + ",UserRoleHsh=N'" +
                                          userrolehsh +
                                          "' where User_UserRole.ID = " + id;
                        scn.Open();
                        scm.ExecuteNonQuery();
                        scn.Close();
                        _messageerror = @"رکورد مورد نظر ویرایش گردید";
                        _state = true;
                    }
                    else
                    {
                        _messageerror = @"این رکورد از قبل وجود دارد";
                        _state = true;
                    }
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    #region Delete UserRoles In Database

    public static Boolean DeleteUserRoles(int userid, int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_UserRole where User_UserRole.UserID = " + userid +
                                  " and User_UserRole.RoleID = " + roleid +
                                  " and User_UserRole.DelState =0";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Select");
                if (dt.Tables["Select"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"update User_UserRole set DelState = 1 where User_UserRole.UserID = " + userid +
                                      " and User_UserRole.RoleID = " + roleid +
                                      " and User_UserRole.DelState =0";
                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    _messageerror = @"رکورد مورد نظر حذف گردید";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکورد مورد نظر یافت نشد";
                    _state = false;
                }
                dt.Dispose();
                sda.Dispose();
                scm.Dispose();
                scn.Dispose();
            }
            else
            {
                _messageerror = "ارتباط با دیتابیس برقرار نمی باشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return _state;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// For Fill GridView

    #region Select From User

    public static DataTable SelectAllUserFrom1Group(short typegroup1, short typegroup2, short typegroup3,
        short typegroup4)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_UserProfile where User_UserProfile.DelState = 0 and User_UserProfile.TypeGroup=" +
                    typegroup1 + "or User_UserProfile.TypeGroup=" + typegroup2
                    + "or User_UserProfile.TypeGroup=" + typegroup3 + "or User_UserProfile.TypeGroup=" + typegroup4;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select us.Name,us.Family,us.FatherName,us.NationalCode,us.Birthday,
                                            lb.LDscFa as situationjob,lbe.LDscFa as evidence,us.UserName,
	                                        us.Mobile,us.Description,us.ID from User_UserProfile as us
                                            left join Lables as lb on us.SituationJob = lb.LID
                                            left join Lables as lbe on us.Evidence = lbe.LID  
                                            where us.DelState = 0 and us.TypeGroup=" + typegroup1 + " or us.TypeGroup=" +
                                      typegroup2
                                      + " or us.TypeGroup=" + typegroup3 + " or us.TypeGroup=" + typegroup4
                                      + " order by us.ID desc";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select From Role

    public static DataTable SelectAllRole()
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_RoleProfile where User_RoleProfile.DelState = 0 and User_RoleProfile.RoleNameFa !='--'";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_RoleProfile where User_RoleProfile.DelState = 0 and User_RoleProfile.RoleNameFa !='--'";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select Row not Incloude 20 From Role

    public static DataTable SelectRoleNotIncloude()
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_RoleProfile where User_RoleProfile.DelState = 0 and User_RoleProfile.ID > 20";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_RoleProfile where User_RoleProfile.DelState = 0 and User_RoleProfile.ID > 20";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select From Permission

    public static DataTable SelectAllPermission()
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select usrper.ID,PermissionNameFa,PermissionNameEn,PermissionDescription
                                            from User_PermissionProfile usrper
                                            where usrper.DelState = 0 ";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select usrper.ID,PermissionNameFa,PermissionNameEn,PermissionDescription
                                            from User_PermissionProfile usrper
                                            where usrper.DelState = 0 ";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select From Group

    public static DataTable SelectAllGroup()
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_GroupsProfile where User_GroupsProfile.DelState = 0select ID,GroupNameFa,GroupNameEn,GroupDescription,case TypeGroup when 1 then N'کاربر' when 2 then N'نقش' end as TypeGroup from User_GroupsProfile
                                            where User_GroupsProfile.DelState = 0 ";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select ID,GroupNameFa,GroupNameEn,GroupDescription,case TypeGroup when 1 then N'کاربر' when 2 then N'نقش' end as TypeGroup from User_GroupsProfile
                                                where User_GroupsProfile.DelState = 0";
                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select From Group Role

    public static DataTable SelectAllGroupRole(int groupid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupRole where User_GroupRole.GroupID =" + groupid +
                                  " and User_GroupRole.DelState = 0 ";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_GroupRole where User_GroupRole.GroupID =" + groupid +
                                      " and User_GroupRole.DelState = 0";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select From Role Permission

    public static DataTable SelectAllRolePermission(int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_RolePermission where User_RolePermission.RoleID =" + roleid +
                                  " and User_RolePermission.DelState = 0 ";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_RolePermission where User_RolePermission.RoleID =" + roleid +
                                      " and User_RolePermission.DelState = 0";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select From GroupUser

    public static DataTable SelectAllGroupUser(int userid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_GroupUser where User_GroupUser.UserID =" + userid +
                                  " and User_GroupUser.DelState = 0 ";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_GroupUser where User_GroupUser.UserID =" + userid +
                                      " and User_GroupUser.DelState = 0";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select From UserRole

    public static DataTable SelectAllUserRole(int userid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select * from User_UserRole where User_UserRole.UserID =" + userid +
                                  " and User_UserRole.DelState = 0 ";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select * from User_UserRole where User_UserRole.UserID =" + userid +
                                      " and User_UserRole.DelState = 0";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select Permission For User

    public static DataTable SelectAllPermissionForUsers(string username)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select per.ID,per.GroupID,per.DelState,per.PermissionDescription,per.PermissionNameEn,per.PermissionNameFa from User_UserProfile us
                                            join User_UserRole usr on us.ID = usr.UserID
                                            join User_RoleProfile rol on usr.RoleID = rol.ID
                                            join User_RolePermission rolp on rol.ID = rolp.RoleID
                                            join User_PermissionProfile per on rolp.PermissionID = per.ID
                                        where us.DelState =0 and usr.DelState =0 and rol.DelState =0 and rolp.DelState =0 and per.DelState =0
                                                 and BINARY_CHECKSUM(us.UserName) = BINARY_CHECKSUM(N'" + username +
                                  "')";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select per.ID,per.GroupID,per.DelState,per.PermissionDescription,per.PermissionNameEn,per.PermissionNameFa from User_UserProfile us
                                            join User_UserRole usr on us.ID = usr.UserID
                                            join User_RoleProfile rol on usr.RoleID = rol.ID
                                            join User_RolePermission rolp on rol.ID = rolp.RoleID
                                            join User_PermissionProfile per on rolp.PermissionID = per.ID
                                        where us.DelState =0 and usr.DelState =0 and rol.DelState =0 and rolp.DelState =0 and per.DelState =0
                                                 and BINARY_CHECKSUM(us.UserName) = BINARY_CHECKSUM(N'" + username +
                                      "')";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select Permission For User

    public static DataTable SelectAllPermissionForRoles(int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select per.ID,per.GroupID,per.DelState,per.PermissionDescription,per.PermissionNameEn,per.PermissionNameFa from User_RoleProfile rol
                                            join User_RolePermission rolp on rol.ID = rolp.RoleID
                                            join User_PermissionProfile per on rolp.PermissionID = per.ID
                                        where rol.DelState =0 and rolp.DelState =0 and per.DelState =0
                                              and rol.ID = " + roleid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText = @"select per.ID,per.GroupID,per.DelState,per.PermissionDescription,per.PermissionNameEn,per.PermissionNameFa from User_RoleProfile rol
                                            join User_RolePermission rolp on rol.ID = rolp.RoleID
                                            join User_PermissionProfile per on rolp.PermissionID = per.ID
                                        where rol.DelState =0 and rolp.DelState =0 and per.DelState =0
                                              and rol.ID = " + roleid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select All From User

    public static DataTable SelectAllUser()
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_UserProfile where User_UserProfile.DelState =0 and User_UserProfile.ActiveState =1 ";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_UserProfile where User_UserProfile.DelState = 0 and User_UserProfile.ActiveState =1 ";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکوردی یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                    dt = new DataSet();
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
                dt = new DataSet();
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
            dt = new DataSet();
        }
        return dt.Tables["Select"];
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// for Edit

    #region Select One User

    public static DataTable SelectOneUser(int userid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_UserProfile where User_UserProfile.DelState =0 and User_UserProfile.ID =" +
                    userid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_UserProfile where User_UserProfile.DelState = 0 and User_UserProfile.ID =" +
                        userid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select One Role

    public static DataTable SelectOneRole(int roleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_RoleProfile where User_RoleProfile.DelState =0 and User_RoleProfile.ID =" +
                    roleid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_RoleProfile where User_RoleProfile.DelState = 0 and User_RoleProfile.ID =" +
                        roleid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select One Permission

    public static DataTable SelectOnePermission(int permissionid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_PermissionProfile where User_PermissionProfile.DelState =0 and User_PermissionProfile.ID =" +
                    permissionid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_PermissionProfile where User_PermissionProfile.DelState = 0 and User_PermissionProfile.ID =" +
                        permissionid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select One Group

    public static DataTable SelectOneGroup(int groupid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_GroupsProfile where User_GroupsProfile.DelState =0 and User_GroupsProfile.ID =" +
                    groupid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_GroupsProfile where User_GroupsProfile.DelState = 0 and User_GroupsProfile.ID =" +
                        groupid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select One Group Role

    public static DataTable SelectOneGroupRole(int grouproleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_GroupRole where User_GroupRole.DelState =0 and User_GroupRole.ID =" +
                    grouproleid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_GroupRole where User_GroupRole.DelState = 0 and User_GroupRole.ID =" +
                        grouproleid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select One Role Permission

    public static DataTable SelectOneRolePermission(int rolepermissionid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_RolePermission where User_RolePermission.DelState =0 and User_RolePermission.ID =" +
                    rolepermissionid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_RolePermission where User_RolePermission.DelState = 0 and User_RolePermission.ID =" +
                        rolepermissionid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select One GroupUser

    public static DataTable SelectOneGroupUser(int groupuserid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_GroupUser where User_GroupUser.DelState =0 and User_GroupUser.ID =" +
                    groupuserid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_GroupUser where User_GroupUser.DelState = 0 and User_GroupUser.ID =" +
                        groupuserid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    #region Select One UserRole

    public static DataTable SelectOneUserRole(int userroleid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_UserRole where User_UserRole.DelState =0 and User_UserRole.ID =" + userroleid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_UserRole where User_UserRole.DelState = 0 and User_UserRole.ID =" +
                        userroleid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    _state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    _state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                _state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            _state = false;
        }
        return dt.Tables["Select"];
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region User IS Active

    public static Boolean UserIsActive(int userid)
    {
        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        Boolean state;

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_UserProfile where User_UserProfile.DelState =0 and User_UserProfile.ActiveState =1 and User_UserProfile.ID =" +
                    userid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_UserProfile where User_UserProfile.DelState = 0 and User_UserProfile.ActiveState =1  and User_UserProfile.ID =" +
                        userid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    _messageerror = @"رکورد یافت شد";
                    state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            state = false;
        }
        return state;
    }

    #endregion

    #region Login User In System

    public static Boolean LoginUser(string username, string password)
    {
        Boolean states;

        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select * from User_UserProfile where User_UserProfile.DelState =0 and User_UserProfile.UserName = N'" +
                    PreventSqlInjection(username) + "' and User_UserProfile.Password =N'" +
                    EncryptString(PreventSqlInjection(password)) + "'";
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count == 1)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select * from User_UserProfile where User_UserProfile.DelState =0 and User_UserProfile.ActiveState =1 and User_UserProfile.UserName = N'" +
                        PreventSqlInjection(username) + "' and User_UserProfile.Password =N'" +
                        EncryptString(PreventSqlInjection(password)) + "'";

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");

                    if (dt.Tables["Select"].Rows.Count == 1)
                    {
                        states = true;
                        _messageerror = "به سامانه خوش آمدید";
                        DataRow drRow = dt.Tables["Select"].Rows[0];

                        _useriD = int.Parse(drRow.ItemArray.GetValue(0).ToString());
                        _userName = drRow.ItemArray.GetValue(1).ToString();
                        _userpassword = DecryptString(drRow.ItemArray.GetValue(2).ToString());
                        _userfirstName = drRow.ItemArray.GetValue(3).ToString();
                        _userfamily = drRow.ItemArray.GetValue(4).ToString();
                       // _userlevel = byte.Parse(drRow.ItemArray.GetValue(25).ToString());
                    }
                    else
                    {
                        _messageerror = "نام کاربری مورد نظر غیر فعال می باشد";
                        _useriD = 0;
                        states = false;
                    }
                }
                else
                {
                    _messageerror = "نام کاربری و یا رمز عبور شما اشتباه است";
                    states = false;
                    _useriD = 0;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                states = false;
                _useriD = 0;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            states = false;
            _useriD = 0;
        }
        return states;
    }

    #endregion

    #region User Access

    public static Boolean UserAccess(string username, Operations permissionnameen)
    {
        SqlConnection scn = new SqlConnection(Connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        Boolean state;
        try
        {
            if (ConnectionState(Connection))
            {
                scm.Connection = scn;
                scm.CommandText = @"select per.ID,per.GroupID,per.DelState,per.PermissionDescription,per.PermissionNameEn,per.PermissionNameFa from User_UserProfile us
                                            join User_UserRole usr on us.ID = usr.UserID
                                            join User_RoleProfile rol on usr.RoleID = rol.ID
                                            join User_RolePermission rolp on rol.ID = rolp.RoleID
                                            join User_PermissionProfile per on rolp.PermissionID = per.ID
                                        where us.DelState =0 and usr.DelState =0 and rol.DelState =0 and rolp.DelState =0 and per.DelState =0
                                                 and us.UserName = N'" + PreventSqlInjection(username) +
                                  "' and per.PermissionNameEn =N'" + permissionnameen + "'";
                scn.Open();
                sda.SelectCommand = scm;
                scm.ExecuteNonQuery();
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    _messageerror = @"رکورد یافت شد";
                    state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            state = false;
        }
        return state;
    }

    #endregion

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// قسمت به دست آوردن منو های مربوط به هر یوزر

    #region Select All Menu For User

    public static Boolean SelectAllMenu(int userid)
    {
        Boolean state = false;

        SqlConnection scn = new SqlConnection(_connection);
        SqlCommand scm = new SqlCommand();

        SqlDataAdapter sda = new SqlDataAdapter();
        DataSet dt = new DataSet();

        try
        {
            if (ConnectionState(_connection))
            {
                scm.Connection = scn;
                scm.CommandText =
                    @"select m.MID from User_UserProfile
                                            join User_UserRole as ur on User_UserProfile.ID = ur.UserID
                                            join User_RelationRoleMenu as rrm on ur.RoleID=rrm.RoleID
                                            join User_Menu as m on rrm.MID= m.MID
                                            where User_UserProfile.Delstate=0 and ur.DelState=0 and User_UserProfile.ID=" +
                    userid;
                sda.SelectCommand = scm;
                sda.Fill(dt, "Insert");

                if (dt.Tables["Insert"].Rows.Count != 0)
                {
                    scm.Connection = scn;
                    scm.CommandText =
                        @"select m.MID from User_UserProfile
                                            join User_UserRole as ur on User_UserProfile.ID = ur.UserID
                                            join User_RelationRoleMenu as rrm on ur.RoleID=rrm.RoleID
                                            join User_Menu as m on rrm.MID= m.MID
                                            where User_UserProfile.Delstate=0 and ur.DelState=0 and User_UserProfile.ID=" +
                        userid;

                    scn.Open();
                    scm.ExecuteNonQuery();
                    scn.Close();

                    sda.Fill(dt, "Select");


                    DataRow[] rows = dt.Tables["Select"].Select();
                    MenuForUser = Array.ConvertAll(rows, row => long.Parse(row["MID"].ToString()));

                    _messageerror = @"رکورد یافت شد";
                    state = true;
                }
                else
                {
                    _messageerror = @"رکوردی یافت نشد";
                    state = false;
                }
            }
            else
            {
                _messageerror = @"ارتباط با دیتابیس برقرار نشد";
                state = false;
            }
        }
        catch (Exception messageException)
        {
            _messageerror = messageException.Message;
            state = false;
        }
        return state;
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////تعریف متغییر ها مورد استفاده

    #region Register

    private static string _userName;
    private static int _useriD;
    private static string _userfirstName;
    private static string _userfamily;
    private static string _userpassword;
    private static string _connection;
    private static string _messageerror;
    private static string _picurl;
    private static long[] _menuforuser;
    private static byte _userlevel;
    private static Boolean _state;

    public static string UserName
    {
        get { return _userName; }
        set { _userName = value; }
    }

    public static int UserId
    {
        get { return _useriD; }
        set { _useriD = value; }
    }

    public static string UserFamily
    {
        get { return _userfamily; }
        set { _userfamily = value; }
    }

    public static string UserPassword
    {
        get { return _userpassword; }
        set { _userpassword = value; }
    }

    public static string UserFirstName
    {
        get { return _userfirstName; }
        set { _userfirstName = value; }
    }

    public static string Connection
    {
        get { return _connection; }
        set { _connection = value; }
    }

    public static string MessageError
    {
        get { return _messageerror; }
        set { _messageerror = value; }
    }

    public static string PicUrl
    {
        get { return _picurl; }
        set { _picurl = value; }
    }

    public static long[] MenuForUser
    {
        get { return _menuforuser; }
        set { _menuforuser = value; }
    }

    public static byte UserLevel
    {
        get { return _userlevel; }
        set { _userlevel = value; }
    }

    public static Boolean State
    {
        get { return _state; }
        set { _state = value; }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////چک کردن درست بودن آی پی وارد شده 

    #region Check IP Valid

    public static Boolean CheckIPValid(string ip)
    {
        string[] parts = ip.Split('.');
        if (parts.Length < 4)
        {
            return false;
        }
        else
        {
            foreach (string part in parts)
            {
                byte checkPart = 0;
                if (!byte.TryParse(part, out checkPart))
                {
                    return false;
                }
            }
            return true;
        }
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////تعریف فعالیت ها

    #region Register Operation

    public enum Operations
    {
        ViewAdmin = 1,
        ViewMap = 2,
        ViewView = 3,
        ViewService = 4,
        ViewEventRule = 5,
        ViewAsp = 6,
        ViewTimeLine = 7,
        UserManegment = 8,
        DeviceManegment = 9,
        DriverManegment = 10,
        ConfigManegment = 11,
        VehicleManegment = 12,
        SettingManegment = 13,
        ViewMainTenance = 14,
        Voip = 15,
        ViewVTruck = 16,
        ViewVShovel = 17,
        ViewVLoader = 18,
        ViewVGrader = 19,
        ViewVSprinkler = 20,
        ViewVDriller = 21,
        ViewVBulldozer = 22,
        ViewVWheeldozer = 23,
        ViewDTruck = 24,
        ViewDShovel = 25,
        ViewDLoader = 26,
        ViewDGrader = 27,
        ViewDSprinkler = 28,
        ViewDDriller = 29,
        ViewDBulldozer = 30,
        ViewDWheeldozer = 31,
        ViewRTypeLoad = 32,
        ViewRLoader = 33,
        ViewRUnLoading = 34,
        ViewRRoad = 35,
        ViewRRoadProblem = 36,
        ViewRSprinkler = 37,
        ViewLayerRightClick = 38,
        ViewVehicleRightClick = 39,
        ViewMessages = 40,
        ViewEvent = 41,
        ViewState = 42,
        ImportMainLayer = 43,
        CreateBlock1 = 44,
        ImportBlock1 = 45,
        ListBlock1 = 46,
        CreateBlock2 = 47,
        ImportBlock2 = 48,
        ListBlock2 = 49,
        CreateBlock3 = 50,
        ImportBlock3 = 51,
        ListBlock3 = 52,
        CreateJeofans = 53,
        ImportJeofans = 54,
        ListJeofans = 55,
        CreateRoad = 56,
        ImportRoad = 57,
        ListRoad = 58,
        CreatePoint = 59,
        ImportPoint = 60,
        ListPoint = 61,
        EventRole = 62,
        ScheduleDrill = 63,
        ScheduleFire = 64,
        ScheduleLoading = 65,
        MapLayerRightClick = 66,
        MapVehicleRightClick = 67,
        CreateBlock = 68,
        ListBlock = 69,
        ImportBlock = 70,
        ListCloseLayer = 71,

        MN_DeviceProfile = 72,
        MN_DeviceState = 73,
        MN_AlarmShow = 74,
        MN_BreakState = 75,
        MN_TypeDeviceState = 76,
        MN_ChangeState = 77,
        MN_ChangeStateList = 78,
        MN_ReportVisitSycle = 79,
        MN_ReportServicePart = 80,
        MN_ReportBreak = 81,

        //AddHardware = 23,
        //DeleteHardware = 24,
        //EditHardware = 25,
        //ViewRelHardMachin = 26,
        //AddRelHardMachin = 27,
        //DeleteRelHardMachin = 28,
        //EditRelHardMachin = 29,
        //ViewDriver = 30,
        //AddDriver = 31,
        //DeleteDriver = 32,
        //EditDriver = 33,
        //ViewMachin = 34,
        //AddMachin = 35,
        //DeleteMachin = 36,
        //EditMachin = 37,
        //ViewTermShift = 38,
        //AddTermShift = 39,
        //DeleteTermShift = 40,
        //EditTermShift = 41,
        //ViewWorkShift = 42,
        //AddWorkShift = 43,
        //DeleteWorkShift = 44,
        //EditWorkShift = 45,
        //ViewTypeLoad = 46,
        //AddTypeLoad = 47,
        //DeleteTypeLoad = 48,
        //EditTypeLoad = 49,
        //SendMessage = 50,
        //ViewBrand = 51,
        //AddBrand = 52,
        //DeleteBrand = 53,
        //EditBrand = 54,
        //ViewModel = 55,
        //AddModel = 56,
        //DeleteModel = 57,
        //EditModel = 58,
        //ViewConfigAlarm = 59,
        //ViewConfigGeneral = 60,
        //ViewConfigPriority = 61,
        //ViewMessage = 62,
        //AddMessage = 63,
        //DeleteMessage = 64,
        //EditMessage = 65,
        //AddConfigAlarm = 66,
        //DeleteConfigAlarm = 67,
        //EditConfigAlarm = 68,
        //AddConfigGeneral = 69,
        //DeleteConfigGeneral = 70,
        //EditConfigGeneral = 80,
        //AddConfigPriority = 81,
        //DeleteConfigPriority = 82,
        //EditConfigPriority = 83,
    }

    #endregion
}
