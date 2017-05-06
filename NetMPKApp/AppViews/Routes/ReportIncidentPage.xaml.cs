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
using NetMPKApp.Infrastructure;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Routes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReportIncidentPage : Page
    {
        private double currentLongitude;
        private double currentLatitude;

        private string innerIncidentStopFrom;
        private string innerIncidentStopTo;

        private string incidentType;

        private List<string> stopsList;
        private List<Tuple<string, string>> neighbouringStops;

        private string manualStopFrom;
        private string manualStopTo;

        public ReportIncidentPage()
        {
            this.InitializeComponent();
        }

        private async void Register_Incident(object sender, RoutedEventArgs e)
        {
            if (currentLatitude != 0.0 && currentLongitude != 0.0)
            {
                incidentType = (((sender as Button).Content as Grid).Children[1] as TextBlock).Text;
                var client = ServiceConnection.GetInstance().client;
                var result = await client.GetProbableConnectionsAsync(currentLongitude, currentLatitude);
                _incName.Text = incidentType;
                _firstStop.Text = result.Item1;
                _secondStop.Text = result.Item2;
                innerIncidentStopFrom = result.Item1;
                innerIncidentStopTo = result.Item2;
                _incidentPopup.IsOpen = true;
                if (incidentType.ToIncidentType() == "JAM")
                {
                    _incGlyph.Visibility = Visibility.Collapsed;
                    _incImg.Visibility = Visibility.Visible;
                }
                else
                {
                    _incGlyph.Visibility = Visibility.Visible;
                    _incImg.Visibility = Visibility.Collapsed;
                    if (incidentType.ToIncidentType() == "ACC")
                        _incGlyph.Text = char.ConvertFromUtf32(59423);
                    else if (incidentType.ToIncidentType() == "REP")
                        _incGlyph.Text = char.ConvertFromUtf32(59426);
                    else
                        _incGlyph.Text = char.ConvertFromUtf32(59412);
                }
            }
            else
            {
                await AppHelper.ShowErrorInfo("Poczekaj", "Wyszukiwanie lokalizacji w toku.");
            }

        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (LocalizationService.isLocationAvailable)
            {
                LocalizationService.LocationChanged += LocalizationService_LocationChanged;
                currentLatitude = LocalizationService.currentLatitude;
                currentLongitude = LocalizationService.currentLongitude;
                var client = ServiceConnection.GetInstance().client;
                stopsList = await client.GetStopsNamesAsync();
                neighbouringStops = await client.GetStopsWithNeighboursAsync();
            }
            else
            {
                await AppHelper.ShowErrorInfo("Błąd", "Brak dostępu do lokalizacji!");
            }
        }

        private void LocalizationService_LocationChanged(object sender, EventArgs e)
        {
            var args = e as LocationChangedEventArgs;
            currentLatitude = args.latitude;
            currentLongitude = args.longitude;
        }

        private async void Confirm_Register(object sender, RoutedEventArgs e)
        {
            
            var client = ServiceConnection.GetInstance().client;
            bool result = await client.RegisterTrafficIncidentAsync(incidentType.ToIncidentType(), innerIncidentStopFrom, innerIncidentStopTo, UserInfo.GetInstance()._userId);
            if (result)
                await AppHelper.ShowErrorInfo("Zgłoszenie przyjęte", "Dziękujemy za zgłoszenie");
            else
                await AppHelper.ShowErrorInfo("Błąd", "Nastąpił błąd, zgłoszenie nie zostało zarejestrowane");
            _incidentPopup.IsOpen = false;
        }

        private void Manual_Connection(object sender, RoutedEventArgs e)
        {
            _manincName.Text = incidentType;
            if (incidentType.ToIncidentType() == "JAM")
            {
                _manincGlyph.Visibility = Visibility.Collapsed;
                _manincImg.Visibility = Visibility.Visible;
            }
            else
            {
                _manincGlyph.Visibility = Visibility.Visible;
                _manincImg.Visibility = Visibility.Collapsed;
                if (incidentType.ToIncidentType() == "ACC")
                    _manincGlyph.Text = char.ConvertFromUtf32(59423);
                else if (incidentType.ToIncidentType() == "REP")
                    _manincGlyph.Text = char.ConvertFromUtf32(59426);
                else
                    _manincGlyph.Text = char.ConvertFromUtf32(59412);
            }
            _incidentPopup.IsOpen = false;
            _manualStop.IsOpen = true;
            
        }

        private async void Manual_RegisterIncident(object sender, RoutedEventArgs e)
        {
            var client = ServiceConnection.GetInstance().client;
            bool result = await client.RegisterTrafficIncidentAsync(incidentType.ToIncidentType(), manualStopFrom, manualStopTo, UserInfo.GetInstance()._userId);
            if (result)
                await AppHelper.ShowErrorInfo("Zgłoszenie przyjęte", "Dziękujemy za zgłoszenie");
            else
                await AppHelper.ShowErrorInfo("Błąd", "Nastąpił błąd, zgłoszenie nie zostało zarejestrowane");
            _manualStop.IsOpen = false;
        }

        private void _manualFirstStop_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = stopsList.Where(w => w.StartsWith(sender.Text));
            }
        }

        private void _manualFirstStop_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private void _manualFirstStop_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && stopsList.Contains(args.ChosenSuggestion.ToString()))
            {
                manualStopFrom = args.ChosenSuggestion.ToString();
            }
        }

        private void _manualSecontStop_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = neighbouringStops.Where(w => w.Item1 == manualStopFrom).Where(w => w.Item2.StartsWith(sender.Text)).Select(s => s.Item2);
            }
        }

        private void _manualSecontStop_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private void _manualSecontStop_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && stopsList.Contains(args.ChosenSuggestion.ToString()))
            {
                manualStopTo = args.ChosenSuggestion.ToString();
            }
        }
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
