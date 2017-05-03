using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Routes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StopCompassPage : Page
    {
        private Compass _compass;
        private GeolocationAccessStatus accessStatus;

        private double _currentLatitude;
        private double _currentLongitude;

        private string targetStopName;
        private double targetStopLatitude;
        private double targetStopLongitude;

        public StopCompassPage()
        {
            this.InitializeComponent();
            /*
            _compass = Compass.GetDefault();

            if (_compass != null)
            {
                uint minReportInterval = _compass.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                _compass.ReportInterval = reportInterval;
                _compass.ReadingChanged += new TypedEventHandler<Compass, CompassReadingChangedEventArgs>(ReadingChanged);
            }
            */
        }

        private void SetAccessStatus()
        {
            var t = Geolocator.RequestAccessAsync().AsTask();
            t.Wait();
            accessStatus = t.Result;
        }


        private async Task InitLocationService()
        {
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 5 };
                geolocator.PositionChanged += Geolocator_PositionChanged;
                Geoposition pos = await geolocator.GetGeopositionAsync();
                _currentLatitude = pos.Coordinate.Point.Position.Latitude;
                _currentLongitude = pos.Coordinate.Point.Position.Longitude;
                RefreshUI();
            }
            else
            {
                await Infrastructure.AppHelper.ShowErrorInfo("Błąd", "Brak dostępu do lokalizacji!");
            }
        }

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _currentLatitude = args.Position.Coordinate.Point.Position.Latitude;
                _currentLongitude = args.Position.Coordinate.Point.Position.Longitude;
                RefreshUI();
            });
        }

        private void RefreshUI()
        {
            _latitudeText.Text = _currentLatitude.ToString() + " N";
            _LongitudeText.Text = _currentLongitude.ToString() + " E";
            _distance.Text = Infrastructure.AppHelper.CalculateDistance(_currentLatitude, _currentLongitude, targetStopLatitude, targetStopLongitude).ToString();
        }


        /*
        private async void ReadingChanged(object sender, CompassReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CompassReading reading = e.Reading;
                txtMagnetic.Text = String.Format("{0,5:0.00}", reading.HeadingMagneticNorth);
                if (reading.HeadingTrueNorth.HasValue)
                    txtNorth.Text = String.Format("{0,5:0.00}", reading.HeadingTrueNorth);
                else
                    txtNorth.Text = "No reading.";
            });
        }
        */

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string stopName = e.Parameter as string;

            SetAccessStatus();
            await InitLocationService();
            if (stopName == null)
                SetTartgetToNearestStop();
            else
                SetTargetStop(stopName);
        }

        private async void SetTartgetToNearestStop()
        {
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            var result = await client.GetNearestStopAsync(_currentLatitude, _currentLongitude);
            targetStopName = result.Item1;
            targetStopLatitude = result.Item2;
            targetStopLongitude = result.Item3;
        }

        private async void SetTargetStop(string stopName)
        {
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            var result = await client.GetStopByNameAsync(stopName);
            targetStopName = result.Item2;
            targetStopLatitude = result.Item5;
            targetStopLongitude = result.Item4;
        }
    }
}
