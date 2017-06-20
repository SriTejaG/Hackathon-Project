using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;

namespace HackathonProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        String sURL = AppDomain.CurrentDomain.BaseDirectory + "WebData/map.html";
        //String sURL = "C:/Users/H223763/Documents/visual studio 2013/Projects/HackathonProjectFinal/HackathonProject/WebData/map.html";
        public string DefaultReviewsFileLocation = @"reviews.json";
        public string DefaultFileHistoryFile = @"Loc.json";
        //public string PathOfPrevPathJSON = @"PrevPath.json";
        
        List<Location> StandardLocationHistory = new List<Location>();
        
        
        
        
        
        public MainWindow()
        {
            InitializeComponent();
            Uri uri = new Uri(sURL);

            //Defaults
            System.TimeSpan _5Days = new TimeSpan(5,0,0,0,0);
            FromDatePicker.Value = DateTime.Now.Subtract(_5Days);
            ToDatePicker.Value = DateTime.Now;
            UploadErrorLabel.Visibility = System.Windows.Visibility.Hidden;
            PathLocationTextBox.Text = AppDomain.CurrentDomain.BaseDirectory + "Loc.json";


            //Initial Conditions
            webBrowser1.Navigate(uri);
            StandardLocationHistory = GetLocationHistoryDataFromJson(DefaultFileHistoryFile);
            
            FillData.ItemsSource = StandardLocationHistory;
            

        }


        public void GetReviewsDataFromJson()
        {
            using (StreamReader file = File.OpenText(DefaultReviewsFileLocation))
            {
                string printer = "";
                string json = file.ReadToEnd();
                //List<List<string>> dynObj = JsonConvert.DeserializeObject<List<List<string>>>(json);
                Reviews dynObj = JsonConvert.DeserializeObject<Reviews>(json);
                //TestTextBox.Text = dynObj.ToString();
                foreach (Feature i in dynObj.features)
                {
                    List<double> req = i.geometry.coordinates ;
                    printer += req[0].ToString() + ",";
                    printer += req[1].ToString() + Environment.NewLine;
                }
                //TestTextBox.Text = printer;
            }
        }

        public List<Location> GetLocationHistoryDataFromJson(string PathForJsonFile)
        {

            try
            {
                using (StreamReader file = File.OpenText(PathForJsonFile))
                {
                    string json = file.ReadToEnd();
                    LocationHistory dynObj = JsonConvert.DeserializeObject<LocationHistory>(json);
                    dynObj.locations = ObtainMyCustomVariables(dynObj.locations);

                    if (dynObj.locations.Count > 0)
                    {
                        UploadErrorLabel.Visibility = System.Windows.Visibility.Hidden;
                        return dynObj.locations;
                    }
                    else
                    {
                        UploadErrorLabel.Visibility = System.Windows.Visibility.Visible;
                        return StandardLocationHistory;
                    }
                }
            }
            catch
            {
                UploadErrorLabel.Visibility = System.Windows.Visibility.Visible;
                return StandardLocationHistory;
            }
        }

        public List<Location> ObtainMyCustomVariables(List<Location> ListofLocations)
        {
            List<Location> ConditionedList = new List<Location>();
            foreach (Location i in ListofLocations)
                    {
                        try
                        {
                            i.timeNormal = UnixTimeStampToDateTime(Convert.ToDouble(i.timestampMs));
                            i.latitudeDec = i.latitudeE7 / 10000000.0;
                            i.longitudeDec = i.longitudeE7 / 10000000.0;
                            
                            Activity2 DefaultActivity2 = new Activity2("UNKNOWN", 100);
                            List<Activity2> DefaultActivity2List = new List<Activity2>();
                            DefaultActivity2List.Add(DefaultActivity2);
                            List<Activity> DefaultList = new List<Activity>();
                            Activity DefaultActivity = new Activity(i.timestampMs, DefaultActivity2List);
                            DefaultList.Add(DefaultActivity);


                            i.activity = i.activity != null ? i.activity : new List<Activity>();
                            i.activity = i.activity.Count > 0 ? i.activity : DefaultList;
                            i.ActivityMain = i.activity[0].activity[0].type.ToString();
                            if (i.activity.Count > 0) i.ActivityAvailable = true;
                            else i.ActivityAvailable = false;

                            ConditionedList.Add(i);
                        }
                        catch { }
                        
                    }
          
            return ConditionedList;
        }

        //Debug Functions... delete later
        private void PrintTestData(List<Location> LocList)
        {
            string printer = "";
            foreach (Location i in LocList)
            {

                printer += i.latitudeE7.ToString() + "   ,";
                printer += i.longitudeE7.ToString() + "   ,";
                printer += Convert.ToInt64(i.timestampMs).ToString() + Environment.NewLine;
            }
            TestTextBox.Text = printer;
            
        }
        private void PrintTestString(string test)
        {
            TestTextBox.Text += test;
        }
        //Debug Functions End

        private void setupObjectForScripting(object sender, RoutedEventArgs e)
        {
            ((WebBrowser)sender).ObjectForScripting = new HtmlTestClass();
        }

        [System.Runtime.InteropServices.ComVisibleAttribute(true)]
        public class HtmlTestClass
        {
            public void endDragMarkerCS(Decimal Lat, Decimal Lng)
            {
                ((MainWindow)Application.Current.MainWindow).TestTextBox.Text = Math.Round(Lat, 5) + "," + Math.Round(Lng, 5);
            }
        }

        private void AddMarkerButton_Click(object sender, RoutedEventArgs e)
        {
       
            webBrowser1.InvokeScript("clearMarkers");
            long AverageLatitude = 0;
            long AverageLongitude = 0;
            int totalLocations = 0;
            try
            {
                foreach (Location loc in StandardLocationHistory)
                {
                    AverageLatitude += loc.latitudeE7;
                    AverageLongitude += loc.longitudeE7;
                    totalLocations++;
                }
                AverageLatitude = AverageLatitude / totalLocations;
                AverageLongitude = AverageLongitude / totalLocations;
                webBrowser1.InvokeScript("addMap", new Object[] { (AverageLatitude / 10000000.0), (AverageLongitude / 10000000.0), 6 });

                foreach (Location loc in StandardLocationHistory)
                {
                    webBrowser1.InvokeScript("addMarker", new Object[] { (loc.latitudeE7 / 10000000.0), (loc.longitudeE7 / 10000000.0), loc.timeNormal.ToString(), (loc.latitudeDec.ToString() + "  ,  " + loc.longitudeDec.ToString()), loc.activity[0].activity[0].type });
                }

            }
            catch
            {
                webBrowser1.InvokeScript("addMap", new Object[] { 20, 78, 5 });
            }
            
        }



        private List<Location> RemoveReplicatingLocations(List<Location> ActualList, double Accuracy)
        {
            
            List<Location> FilteredList = new List<Location>();
            List<Location> TempList = new List<Location>();
            foreach(Location loc in ActualList)
            {
                TempList.Add(loc);
            }
             foreach( Location loc in TempList)
            {
                Location temp = new Location();
               
                temp = loc;
                temp.latitudeE7 = temp.latitudeE7 - (temp.latitudeE7 % Convert.ToInt64(Math.Pow(10,Accuracy)));
                temp.longitudeE7 = temp.longitudeE7 - (temp.longitudeE7 % Convert.ToInt64(Math.Pow(10, Accuracy)));
                bool Detected = false;
                foreach(Location roll in FilteredList)
                {
                    if ((temp.latitudeE7 == roll.latitudeE7) && (temp.longitudeE7 == roll.longitudeE7))
                    {
                        Detected = true;
                       
                    }
                }
                if(Detected == false)
                {
                    FilteredList.Add(temp);
                }

            }
            return FilteredList;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
                PathLocationTextBox.Text = openFileDialog.FileName;
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            StandardLocationHistory = GetLocationHistoryDataFromJson(PathLocationTextBox.Text);
           
            FillData.ItemsSource = StandardLocationHistory;
        }

        private void AddToExistingButton_Click(object sender, RoutedEventArgs e)
        {
            List<Location> AppendedData = new List<Location>();
            List<Location> LocationData = new List<Location>();
            
            LocationData.AddRange(StandardLocationHistory);
            AppendedData = GetLocationHistoryDataFromJson(PathLocationTextBox.Text);
            LocationData.AddRange(AppendedData);
            StandardLocationHistory = LocationData;
            FillData.ItemsSource = StandardLocationHistory;

        }

        private void FillData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            Location SelectedLocation = (Location) FillData.SelectedItem;
            
            try
            {
                webBrowser1.InvokeScript("clearMarkers");
                webBrowser1.InvokeScript("addMap", new Object[] { (SelectedLocation.latitudeE7 / 10000000.0), (SelectedLocation.longitudeE7 / 10000000.0), 18 });
                webBrowser1.InvokeScript("addMarker", new Object[] { (SelectedLocation.latitudeE7 / 10000000.0), (SelectedLocation.longitudeE7 / 10000000.0), SelectedLocation.timeNormal.ToString(), (SelectedLocation.latitudeDec.ToString() + "  ,  " + SelectedLocation.longitudeDec.ToString()), SelectedLocation.activity[0].activity[0].type.ToString() });

            }
            catch { }
            
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {
            StandardLocationHistory = GetLocationHistoryDataFromJson(PathLocationTextBox.Text);
           
            FillData.ItemsSource = StandardLocationHistory;
        }

        private void ResolveReplicationsButton_Click(object sender, RoutedEventArgs e)
        {
            
            StandardLocationHistory = RemoveReplicatingLocations(StandardLocationHistory, Convert.ToDouble(Math.Floor(AccuracySlider.HigherValue)));
            PrintTestString((Convert.ToDouble(Math.Floor(AccuracySlider.HigherValue)).ToString()));
            FillData.ItemsSource = StandardLocationHistory;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestampMs is milliseconds past epoch
            System.DateTime dtDateTime2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime2 = dtDateTime2.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime2;
        }

        public List<Location> FilterWRTTime(List<Location> ActualList, long inputfrom, long inputto)
        {

            List<Location> FilteredList = new List<Location>();
            foreach (Location temp in ActualList)
            {
                if ((   Convert.ToInt64(temp.timestampMs) >= inputfrom) && (Convert.ToInt64(temp.timestampMs) <= inputto))
                    {
                        FilteredList.Add(temp);
                       
                    }
             }
            return FilteredList;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan t = FromDatePicker.Value.Value - new DateTime(1970, 1, 1);
            long FromTimeMS = Convert.ToInt64(t.TotalMilliseconds);
            t = ToDatePicker.Value.Value - new DateTime(1970, 1, 1);
            long ToTimeMS = Convert.ToInt64(t.TotalMilliseconds);

            StandardLocationHistory = FilterWRTTime(StandardLocationHistory, FromTimeMS, ToTimeMS);
            PrintTestString(FromTimeMS.ToString()+ "   ");
            PrintTestString(ToTimeMS.ToString()+ "\n");
            FillData.ItemsSource = StandardLocationHistory;
        }

        private void ResetFilterLocationsButton_Click(object sender, RoutedEventArgs e)
        {
            StandardLocationHistory = GetLocationHistoryDataFromJson(PathLocationTextBox.Text);
            //PrintTestData(StandardLocationHistory);
            FillData.ItemsSource = StandardLocationHistory;
        }

        private void ActivityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ActivityTextBox.Text.Length > 0)
            {
                List<Location> FilteredList = new List<Location>();
                foreach (Location loc in StandardLocationHistory)
                {

                    if (loc.activity[0].activity[0].type.Contains(ActivityTextBox.Text.ToUpper()))
                    {
                        FilteredList.Add(loc);
                    }
                }
                if (FilteredList.Count > 0)
                {
                    StandardLocationHistory = FilteredList;
                    FillData.ItemsSource = StandardLocationHistory;
                    ActivitySearchErrorLabel.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    StandardLocationHistory = GetLocationHistoryDataFromJson(PathLocationTextBox.Text);
                    FillData.ItemsSource = StandardLocationHistory;
                    ActivitySearchErrorLabel.Visibility = System.Windows.Visibility.Visible;

                }
            }
            else
            {
                StandardLocationHistory = GetLocationHistoryDataFromJson(PathLocationTextBox.Text);
                FillData.ItemsSource = StandardLocationHistory;
                ActivitySearchErrorLabel.Visibility = System.Windows.Visibility.Hidden;
            }
        }

    }


}
