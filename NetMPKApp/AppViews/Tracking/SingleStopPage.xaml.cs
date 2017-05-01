using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Tracking
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SingleStopPage : Page
    {
        private string _stopName;

        public SingleStopPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _stopName = e.Parameter as string;
            if (_stopName != null)
                SetStopInfo(_stopName);
        }

        private async void SetStopInfo(string stopName)
        {
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            var stopInfo = await client.GetStopByNameAsync(stopName);

            _stopBlock.Text = "Przystanek: " + stopInfo.Item2;
            if (stopInfo.Item3 != "")
                _streetBlock.Text = "Ulica: " + stopInfo.Item3;

            _singleLineSource.Source = stopInfo.Item6.Select(s => new SingleLineItem(s)).ToList();

            _singleLinesContainer.SelectedItem = null;
            _singleLinesContainer.SelectionChanged += _singleLinesContainer_SelectionChanged;
        }

        private void _singleLinesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceItem = (sender as ListView).SelectedItem as SingleLineItem;
            Tuple<string, int, string> TimetableInfo = Tuple.Create<string, int, string>(_stopName,sourceItem.lineNo,null);
            (Parent as Frame).Navigate(typeof(TimetablePage), TimetableInfo);
        }

        private class SingleLineItem
        {
            public int lineNo { get; }
            public string imgRef { get; }
            public SingleLineItem(int _lineNo)
            {
                lineNo = _lineNo;
                imgRef = (_lineNo<100) ? "ms-appx:///Assets/tram.png" : "ms-appx:///Assets/bus.png";
            }
        }
    }
}
