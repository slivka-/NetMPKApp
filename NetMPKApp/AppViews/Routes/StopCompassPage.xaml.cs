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
        private Compass compass;
        private GeolocationAccessStatus accessStatus;

        private double currentLatitude;
        private double currentLongitude;

        private string targetStopName;
        private double targetStopLatitude;
        private double targetStopLongitude;

        private double currentCompassReading;

        public StopCompassPage()
        {
            this.InitializeComponent();           
        }

        #region Init
        private async Task InitLocationService()
        {
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 5 };
                geolocator.PositionChanged += Geolocator_PositionChanged;
                Geoposition pos = await geolocator.GetGeopositionAsync();
            }
            else
            {
                await Infrastructure.AppHelper.ShowErrorInfo("Błąd", "Brak dostępu do lokalizacji!");
            }
        }

        private void SetAccessStatus()
        {
            var t = Geolocator.RequestAccessAsync().AsTask();
            t.Wait();
            accessStatus = t.Result;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string stopName = e.Parameter as string;

            SetAccessStatus();
            await InitLocationService();
            if (stopName == null)
                await SetTartgetToNearestStop();
            else
                await SetTargetStop(stopName);
            _stopText.Text = targetStopName;
            InitCompass();
        }

        private async Task SetTartgetToNearestStop()
        {
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            var result = await client.GetNearestStopAsync(currentLatitude, currentLongitude);
            targetStopName = result.Item1;
            targetStopLatitude = result.Item2;
            targetStopLongitude = result.Item3;
        }

        private async Task SetTargetStop(string stopName)
        {
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            var result = await client.GetStopByNameAsync(stopName);
            targetStopName = result.Item2;
            targetStopLatitude = result.Item5;
            targetStopLongitude = result.Item4;
        }

        private void InitCompass()
        {
            compass = Compass.GetDefault();

            if (compass != null)
            {
                uint minReportInterval = compass.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                compass.ReportInterval = reportInterval;
                compass.ReadingChanged += new TypedEventHandler<Compass, CompassReadingChangedEventArgs>(Compass_ReadingChanged);
            }
            else
            {
                var t = Infrastructure.AppHelper.ShowErrorInfo("Błąd", "Nie znaleziono kompasu");
                t.Wait();
            }
        }
        #endregion

        #region UpdateHandlers

        private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                currentLatitude = args.Position.Coordinate.Point.Position.Latitude;
                currentLongitude = args.Position.Coordinate.Point.Position.Longitude;
                RefreshUI();
            });
        }
        
        private async void Compass_ReadingChanged(object sender, CompassReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                double newReading = 0.0;
                CompassReading reading = e.Reading;
                if (reading.HeadingTrueNorth.HasValue)
                    newReading = Math.Abs(360 - (double)reading.HeadingTrueNorth);
                else
                    newReading = Math.Abs(360 - reading.HeadingMagneticNorth);

                if (Math.Abs(currentCompassReading - newReading) >= 1)
                {
                    currentCompassReading = newReading;
                    var bearing = Infrastructure.AppHelper.CalculateAngleFromNorth(currentLatitude, currentLongitude, targetStopLatitude, targetStopLongitude);
                    _arrow.RenderTransform = new RotateTransform() { Angle = (currentCompassReading+bearing)%360 };
                }
            });
        }

        private void RefreshUI()
        {
            var distance = Infrastructure.AppHelper.CalculateDistance(currentLatitude, currentLongitude, targetStopLatitude, targetStopLongitude);
            if (distance < 20000)
            {
                _NotFound.Visibility = Visibility.Collapsed;
                _arrow.Visibility = Visibility.Visible;
                _distanceText.Text = String.Format("Odległość: {0}m", distance.ToString());
                var bearing = Infrastructure.AppHelper.CalculateAngleFromNorth(currentLatitude, currentLongitude, targetStopLatitude, targetStopLongitude);
                _arrow.RenderTransform = new RotateTransform() { Angle = (currentCompassReading + bearing) % 360 };
            }
        }

        #endregion


    }
}
