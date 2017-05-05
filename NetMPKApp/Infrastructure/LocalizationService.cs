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


namespace NetMPKApp.Infrastructure
{
    public static class LocalizationService
    {
        public static bool isLocationAvailable { get; private set; }
        public static double currentLatitude { get; private set; }
        public static double currentLongitude { get; private set; }

        public static event EventHandler LocationChanged;

        private static GeolocationAccessStatus accessStatus;

        public static void InitLocationService(GeolocationAccessStatus status)
        {
            accessStatus = status;
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 5 };
                isLocationAvailable = true;
                geolocator.PositionChanged += Geolocator_PositionChanged;
            }
            else
            {
                isLocationAvailable = false;
            }
        }

        private static void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            if (args.Position.Coordinate.Point.Position.Latitude != 0 && args.Position.Coordinate.Point.Position.Longitude != 0)
            {
                currentLatitude = args.Position.Coordinate.Point.Position.Latitude;
                currentLongitude = args.Position.Coordinate.Point.Position.Longitude;
                OnLocationChanged(new LocationChangedEventArgs() { latitude = currentLatitude, longitude = currentLongitude });
            }
        }

        public static void OnLocationChanged(LocationChangedEventArgs e)
        {
            LocationChanged?.Invoke(null, e);
        }
    }

    public class LocationChangedEventArgs : EventArgs
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}
