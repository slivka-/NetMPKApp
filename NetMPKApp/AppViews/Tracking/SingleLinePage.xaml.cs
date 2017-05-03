using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class SingleLinePage : Page
    {
        private int _lineNo;
        private string _selectedDirection;

        private List<SingleStopItem> _dir1Stops;
        private List<SingleStopItem> _dir2Stops;

        public SingleLinePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            int? lineNo = e.Parameter as int?;
            if (lineNo != null)
            {
                _lineNo = (int)lineNo;
                SetLineInfo((int)lineNo);
            }
        }

        private async void SetLineInfo(int lineNo)
        {
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            var lineInfo = await client.GetLineRoutesAsync(lineNo);

            _lineBlock.Text = "Linia: " + lineNo;
            _image.Text = (lineNo < 100) ? char.ConvertFromUtf32(60237) : char.ConvertFromUtf32(60231) ;

            _dirButton1.Content = lineInfo.First().Key;
            _dir1Stops = lineInfo.First().Value.Select(s => new SingleStopItem { stopName = s }).ToList();
            _singleStopsSource.Source = _dir1Stops;
            _selectedDirection = lineInfo.First().Key;
            _dirButton1.IsChecked = true;

            _dirButton2.Content = lineInfo.Last().Key;
            _dir2Stops = lineInfo.Last().Value.Select(s => new SingleStopItem { stopName = s }).ToList();

            _stopsContainer.SelectedItem = null;
            _stopsContainer.SelectionChanged += _stopsContainer_SelectionChanged; 
        }

        private void _stopsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceItem = (sender as ListView).SelectedItem as SingleStopItem;
            if (sourceItem != null)
            {
                Tuple<string, int, string> TimetableInfo = Tuple.Create<string, int, string>(sourceItem.stopName, _lineNo, _selectedDirection);
                (Parent as Frame).Navigate(typeof(TimetablePage), TimetableInfo);
            }
        }

        private void _dirButton1_Click(object sender, RoutedEventArgs e)
        {
            _dirButton2.IsChecked = false;
            _selectedDirection = _dirButton1.Content.ToString();
            _stopsContainer.SelectionChanged -= _stopsContainer_SelectionChanged;
            _singleStopsSource.Source = _dir1Stops;
            _stopsContainer.SelectedItem = null;
            _stopsContainer.SelectionChanged += _stopsContainer_SelectionChanged;
        }

        private void _dirButton2_Click(object sender, RoutedEventArgs e)
        {
            _dirButton1.IsChecked = false;
            _selectedDirection = _dirButton2.Content.ToString();
            _stopsContainer.SelectionChanged -= _stopsContainer_SelectionChanged;
            _singleStopsSource.Source = _dir2Stops;
            _stopsContainer.SelectedItem = null;
            _stopsContainer.SelectionChanged += _stopsContainer_SelectionChanged;
        }

        private class SingleStopItem
        {
            public string stopName { get; set; }
        }
    }
}
