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
using NetMPKApp.Infrastructure;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Routes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StopCompassPage : Page
    {
        private Compass compass;

        private string acquiredStopName;

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
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            acquiredStopName = e.Parameter as string;
            if (LocalizationService.isLocationAvailable)
            {
                LocalizationService.LocationChanged += LocalizationService_LocationChanged;
                InitCompass();
            }
            else
            {
                await AppHelper.ShowErrorInfo("Błąd", "Brak dostępu do lokalizacji!");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (LocalizationService.isLocationAvailable)
            {
                LocalizationService.LocationChanged -= LocalizationService_LocationChanged;
            }
        }

        private  void SetTartgetToNearestStop()
        {
            var client = ServiceConnection.GetInstance().client;
            var result = client.GetNearestStopAsync(currentLatitude, currentLongitude).Result;
            targetStopName = result.Item1;
            targetStopLatitude = result.Item3;
            targetStopLongitude = result.Item2;
        }

        private void SetTargetStop(string stopName)
        {
            var client = ServiceConnection.GetInstance().client;
            var result = client.GetStopByNameAsync(stopName).Result;
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
                var t = AppHelper.ShowErrorInfo("Błąd", "Nie znaleziono kompasu");
                t.Wait();
            }
        }

        #endregion

        #region UpdateHandlers

        private void LocalizationService_LocationChanged(object sender, EventArgs e)
        {
            var args = e as LocationChangedEventArgs;
            currentLatitude = args.latitude;
            currentLongitude = args.longitude;

            if (targetStopName == null)
            {
                if (acquiredStopName == null)
                    SetTartgetToNearestStop();
                else
                    SetTargetStop(acquiredStopName);
            }
            RefreshUI();
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
                    var bearing = AppHelper.CalculateAngleFromNorth(currentLatitude, currentLongitude, targetStopLatitude, targetStopLongitude);
                    _arrow.RenderTransform = new RotateTransform() { Angle = (currentCompassReading+bearing)%360 };
                }
            });
        }

        private async void RefreshUI()
        {
            var distance = AppHelper.CalculateDistance(currentLatitude, currentLongitude, targetStopLatitude, targetStopLongitude);
            if (distance < 20000)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    _stopText.Text = targetStopName;
                    _NotFound.Visibility = Visibility.Collapsed;
                    _arrow.Visibility = Visibility.Visible;
                    _distanceText.Text = string.Format("Odległość: {0}m", distance.ToString());
                    var bearing = AppHelper.CalculateAngleFromNorth(currentLatitude, currentLongitude, targetStopLatitude, targetStopLongitude);
                    _arrow.RenderTransform = new RotateTransform() { Angle = (currentCompassReading + bearing) % 360 };
                });
            }
        }

        #endregion


    }
}
