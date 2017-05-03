using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Tracking
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimetablePage : Page
    {
        private string _currentStop;
        private int _currentLine;
        private string _currentDirection;
        private DayOfWeek _currentDayOfWeek;

        private List<TimeTableItem> regularTT = new List<TimeTableItem>();
        private List<TimeTableItem> saturdayTT = new List<TimeTableItem>();
        private List<TimeTableItem> holidayTT = new List<TimeTableItem>();

        public TimetablePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Tuple<string, int, string> timetableInfo = e.Parameter as Tuple<string, int, string>;
            InitializeUI(timetableInfo); 
        }

        private async void InitializeUI(Tuple<string, int, string> timetableInfo)
        {
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            _currentStop = timetableInfo.Item1;
            _currentLine = timetableInfo.Item2;
            var directions = await client.GetDirectionsForLineAsync(_currentLine, _currentStop);
            _currentDirection = (timetableInfo.Item3 == null) ? directions.First() : timetableInfo.Item3;

            _lineBlock.Text = "Rozkład jazdy linii " + _currentLine;
            _stopBlock.Text = "Przystanek: " + _currentStop;

            _image.Text = (_currentLine < 100) ? char.ConvertFromUtf32(60237) : char.ConvertFromUtf32(60231);

            _dirButton1.Content = directions.First();
            _dirButton1.IsChecked = directions.First().Equals(_currentDirection);
            _dirButton2.Content = directions.Last();
            _dirButton2.IsChecked = directions.Last().Equals(_currentDirection);

            _currentDayOfWeek = DateTime.Now.DayOfWeek;

            await RefreshDirection();
        }

        private async void _dirButton1_Click(object sender, RoutedEventArgs e)
        {
            _dirButton2.IsChecked = false;
            _currentDirection = (sender as ToggleButton).Content.ToString();
            await RefreshDirection();
            
        }

        private async void _dirButton2_Click(object sender, RoutedEventArgs e)
        {
            _dirButton1.IsChecked = false;
            _currentDirection = (sender as ToggleButton).Content.ToString();
            await RefreshDirection();
        }

        private void _dayButton1_Click(object sender, RoutedEventArgs e)
        {
            _dayButton2.IsChecked = false;
            _dayButton3.IsChecked = false;
            _currentDayOfWeek = DayOfWeek.Monday;
            _timetableSource.Source = regularTT;
        }

        private void _dayButton2_Click(object sender, RoutedEventArgs e)
        {
            _dayButton1.IsChecked = false;
            _dayButton3.IsChecked = false;
            _currentDayOfWeek = DayOfWeek.Saturday;
            _timetableSource.Source = saturdayTT;
        }

        private void _dayButton3_Click(object sender, RoutedEventArgs e)
        {
            _dayButton1.IsChecked = false;
            _dayButton2.IsChecked = false;
            _currentDayOfWeek = DayOfWeek.Sunday;
            _timetableSource.Source = holidayTT;
        }

        private async Task RefreshDirection()
        {
            regularTT.Clear();
            saturdayTT.Clear();
            holidayTT.Clear();
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            var timetables = await client.GetTimeTableAsync(_currentLine, _currentStop, _currentDirection);
            for (int i = 0; i < 24; i++)
            {
                regularTT.Add(new TimeTableItem(i.ToString(), timetables[i][0] ));
                saturdayTT.Add(new TimeTableItem(i.ToString(),timetables[i][1] ));
                holidayTT.Add(new TimeTableItem(i.ToString(), timetables[i][2] ));
            }

            if (_currentDayOfWeek == DayOfWeek.Sunday)
            {
                _dayButton3.IsChecked = true;
                _timetableSource.Source = holidayTT;
            }
            else if (_currentDayOfWeek == DayOfWeek.Saturday)
            {
                _dayButton2.IsChecked = true;
                _timetableSource.Source = saturdayTT;
            }
            else
            {
                _dayButton1.IsChecked = true;
                _timetableSource.Source = regularTT;
            }
        }

        private class TimeTableItem
        {
            public string hours { get; }
            public string minutes { get; }

            public TimeTableItem(string hour, string minute)
            {
                hours = hour;
                minutes = (minute.Equals("EMPTY", StringComparison.OrdinalIgnoreCase)) ? "—" : minute;
            }
        }
    }
}
