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
using maintenance.classes;


namespace maintenance
{
    /// <summary>
    /// Interaction logic for setting.xaml
    /// helllllo
    /// </summary>
    public partial class Setting : Window
    {
        private static IniFile ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
        public static string cnn = ini.IniReadValue("appSettings", "cnn");
        public static string Status;
        public Setting()
        {
            InitializeComponent();
        }

        private void canExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {

                e.Handled = true;
            }

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query = from c in db.MN_Plans select c;

                if (query.Count() >= 1)
                {
                    Text_UpdateDevice.Text = query.Single().VehicleThreadUpdate.ToString();
                    Text_UpdateHardWare.Text = query.Single().DeviceThreadUpdate.ToString();
                    Text_UpdateProgram.Text = query.Single().ScheduleThreadUpdate.ToString();
                    Text_UpdateAlarm.Text = query.Single().AlarmThreadUpdate.ToString();
                }
                create_Status();
            }


        }

        private void Text_value_kilometr_service_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Text_value_time_service_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Text_AlarmService_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Text_WarningService_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Text_ValuePart_Kilometer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Text_ValuePart_Time_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Text_AlarmPart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Text_WarningPart_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
        }

        private void Btn_RegisterSetting_Click(object sender, RoutedEventArgs e)
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {

                if (Text_UpdateDevice.Text == "" || Text_UpdateHardWare.Text == "" || Text_UpdateProgram.Text == "" ||
                    Text_UpdateAlarm.Text == "")
                {
                    MessageBox.Show("لطفا تمام فیل ها را تکمیل نمایید", "خطا", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                else
                {


                    if (int.Parse(Text_UpdateProgram.Text) < 1)
                        MessageBox.Show("زمان بروزرسانی بر حسب دقیقه باید بزرگتر از 1 باشد", "خطا", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else if ((int.Parse(Text_UpdateAlarm.Text) < 3 || (int.Parse(Text_UpdateHardWare.Text) < 3) || (int.Parse(Text_UpdateDevice.Text) < 3 )))
                        MessageBox.Show("زمان بروزرسانی بر حسب ثانیه باید بزرگتر از 3 باشد", "خطا", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    else
                    {
                        try
                        {

                            var query = from c in db.MN_Plans select c;
                            if (query.Count() >= 1)
                            {
                                var q = query.Single();
                                
                                q.VehicleThreadUpdate = int.Parse(Text_UpdateDevice.Text);
                                q.DeviceThreadUpdate = int.Parse(Text_UpdateHardWare.Text);
                                q.ScheduleThreadUpdate = int.Parse(Text_UpdateProgram.Text);
                                q.AlarmThreadUpdate = int.Parse(Text_UpdateAlarm.Text);
                                db.SubmitChanges();
                            }
                            else if (query.Count() == 0)
                            {
                                MN_Plan tb = new MN_Plan();
                                {
                                    tb.VehicleThreadUpdate = int.Parse(Text_UpdateDevice.Text);
                                    tb.DeviceThreadUpdate = int.Parse(Text_UpdateHardWare.Text);
                                    tb.ScheduleThreadUpdate = int.Parse(Text_UpdateProgram.Text);
                                    tb.AlarmThreadUpdate = int.Parse(Text_UpdateAlarm.Text);
                                }
                                db.MN_Plans.InsertOnSubmit(tb);
                                db.SubmitChanges();
                            }
                            string[] Array = new string[] {};
                            Array = Status.Split(',');

                            var query2 = from c in db.MN_States select c;
                            foreach (var FalseState in query2)
                            {
                                FalseState.Show = false;
                                db.SubmitChanges();
                            }

                            foreach (var TrueState in Array)
                            {
                                if (TrueState != "" && TrueState != null)
                                {
                                    var query3 =
                                        (from c in db.MN_States where c.ID == int.Parse(TrueState) select c).Single();
                                    query3.Show = true;
                                    db.SubmitChanges();
                                }
                            }


                            MessageBox.Show("اطلاعات با موفقیت ثبت شد", "موفقيت", MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
                            create_Status();


                        }

                        catch (Exception)
                        {
                            MessageBox.Show("خطا در ثبت اطلاعات", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);

                        }

                    }

                }
            }
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var CH = sender as CheckBox;
            string tag = CH.Tag.ToString();
            string name = "ch" + CH.Tag.ToString();

            if (CH.IsChecked == true)
                Status = Status + tag + ",";
            if (CH.IsChecked == false)
                Status = Status.Replace(tag,"");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Status);
        }


        private void create_Status()
        {
            var db = new DataClasses1DataContext(cnn);
            if (db.DatabaseExists())
            {
                var query2 = from c in db.MN_States where c.Parent == 0 && c.Code <= 2 select c;
                StackPanel Panel1 = new StackPanel();
                Panel1.Orientation = Orientation.Vertical;
                Panel1.VerticalAlignment = VerticalAlignment.Stretch;
                Panel1.HorizontalAlignment = HorizontalAlignment.Stretch;
                Panel1.Margin = new Thickness(20);
                StackPanel Panel2 = new StackPanel();
                Panel2.Orientation = Orientation.Vertical;
                Panel2.VerticalAlignment = VerticalAlignment.Stretch;
                Panel2.HorizontalAlignment = HorizontalAlignment.Stretch;

                foreach (var Stat in query2)
                {


                    TextBlock txt = new TextBlock();
                    txt.Text = Stat.Title;
                    Panel2.Children.Add(txt);
                    var query3 = from c in db.MN_States where c.Parent == Stat.Code select c;

                    StackPanel Panel3 = new StackPanel();
                    Panel3.Orientation = Orientation.Horizontal;
                    Panel3.VerticalAlignment = VerticalAlignment.Stretch;
                    Panel3.HorizontalAlignment = HorizontalAlignment.Stretch;
                    Panel3.Margin = new Thickness(10);
                    foreach (var SubStat in query3)
                    {


                        CheckBox CH = new CheckBox();
                        CH.Content = SubStat.Title;
                        CH.Margin = new Thickness(10);
                        CH.Tag = SubStat.ID;
                        CH.Click += CheckBox_Click;
                        CH.Name = "ch" + SubStat.ID.ToString();


                        if (SubStat.Show == true)
                        {
                            CH.IsChecked = true;
                            Status = Status + SubStat.ID + ",";
                        }
                        Panel3.Children.Add(CH);


                    }
                    Panel2.Children.Add(Panel3);

                }

                Panel1.Children.Add(Panel2);

                ConfigStatus.Children.Add(Panel1);

            }
        }
    }
}
