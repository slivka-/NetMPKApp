using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Routes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportIncidentPage : Page
    {
        public ReportIncidentPage()
        {
            this.InitializeComponent();
        }

        private void Register_Incident(object sender, RoutedEventArgs e)
        {
            var content = (((sender as Button).Content as Grid).Children[1] as TextBlock).Text.ToIncidentType();
        }

        /*
        private async Task InitLocationService()
        {
            /*
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
         */
    }

    static class IncidentExtensions
    {
        public static string ToIncidentType(this string s)
        {
            string output = s;
            switch (s)
            {
                case "Wypadek":
                    output = "ACC";
                    break;
                case "Prace drogowe":
                    output = "REP";
                    break;
                case "Korek":
                    output = "JAM";
                    break;
                case "Inne zdarzenie":
                    output = "OTH";
                    break;
            }
            return output;
        }
    }
}
