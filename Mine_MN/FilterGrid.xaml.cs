using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// Interaction logic for FilterGrid.xaml
    /// </summary>
    public partial class FilterGrid : Window
    {
        public static string TypeSelect_Tab;
       
        public FilterGrid()
        {
            InitializeComponent();
            this.Text_StartDate.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            this.Text_EndDate.FlowDirection = System.Windows.FlowDirection.RightToLeft;
        }

        private void Btn_Filter_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.FilterGrid_StartDate = Text_StartDate.Text;
            MainWindow.FilterGrid_EndDate = Text_EndDate.Text;

            if (TypeSelect_Tab == "ProfileGrid_ProgramServiceTime")
            {
                 var myObject = this.Owner as MainWindow;
                 myObject.FillGridProfileServiceTime(sender, e);
            }
                
            else if (TypeSelect_Tab == "ProfileGrid_ProgramServiceKilometer")
            {
                var myObject = this.Owner as MainWindow;
                myObject.FillGridProfileServiceKilometer(sender, e);
                
            }
                
            else if (TypeSelect_Tab == "ProfileGrid_ProgramPart")
            {
                var myObject = this.Owner as MainWindow;
                myObject.FillGridProfileServicePart(sender, e); 
            }
               
            else if (TypeSelect_Tab == "ProfileGrid_BadPart")
            {
                 var myObject = this.Owner as MainWindow;
                 myObject.FillGridProfileBadPart(sender, e);
                
            }
                
            else if (TypeSelect_Tab == "ProfileGrid_EventRule")
            {
                  var myObject = this.Owner as MainWindow;
                  myObject.FillGridEventRule(sender, e);
             }
              
            else if (TypeSelect_Tab == "ProfileGrid_DeviceStatus")
            {
                 var myObject = this.Owner as MainWindow;
                 myObject.FillGridProfileStatusDevice(sender, e);
                
            }

            else if (TypeSelect_Tab == "ProfileGrid_Alarm")
            {
                 var myObject = this.Owner as MainWindow;
                 myObject.FillGridAlarm(sender, e);
                
            }

            
            
            this.Close();
        }

        private void Btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConvertDate Date = new ConvertDate();
            string DateNow = Date.GetDateNow();
            Text_StartDate.Text = DateNow;
            Text_EndDate.Text = DateNow;
        }
    }
}
