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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using NetMPKApp.Infrastructure;
using Windows.Storage;
using Windows.Devices.Geolocation;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Basic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndexPage : Page
    {
        public IndexPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += BackRequestHandler;
            _mainNavigationFrame.Navigate(typeof(StartPage));
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null && (e.Parameter as bool?) == true)
                (Window.Current.Content as Frame).BackStack.Clear();
            if (!LocalizationService.isLocationAvailable)
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                LocalizationService.InitLocationService(accessStatus);
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySpliView.IsPaneOpen = !MySpliView.IsPaneOpen;
        }

        private void _LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            UserInfo.ClearUser();
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove("userLogin");
            localSettings.Values.Remove("userEncryptedPassword");
            (Window.Current.Content as Frame).Navigate(typeof(WelcomeScreen));
        }

        private void _LinesButton_Click(object sender, RoutedEventArgs e)
        {
            _mainNavigationFrame.Navigate(typeof(Tracking.LinesPage));
            MySpliView.IsPaneOpen = false;
        }

        private void _StopsButton_Click(object sender, RoutedEventArgs e)
        {
            _mainNavigationFrame.Navigate(typeof(Tracking.StopsPage));
            MySpliView.IsPaneOpen = false;
        }

        private void BackRequestHandler(object sender, BackRequestedEventArgs e)
        {
            if (_mainNavigationFrame.CanGoBack)
            {
                e.Handled = true;
                _mainNavigationFrame.GoBack();
            }
        }

        private void _SearchTrackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainNavigationFrame.Navigate(typeof(Routes.RouteSearchPage));
            MySpliView.IsPaneOpen = false;
        }

        private void _ReportIncident_Click(object sender, RoutedEventArgs e)
        {
            _mainNavigationFrame.Navigate(typeof(Routes.ReportIncidentPage));
            MySpliView.IsPaneOpen = false;
        }

        private void _MainPage_Click(object sender, RoutedEventArgs e)
        {
            _mainNavigationFrame.Navigate(typeof(StartPage));
            MySpliView.IsPaneOpen = false;
        }

        private void _AccountButton_Click(object sender, RoutedEventArgs e)
        {
            _mainNavigationFrame.Navigate(typeof(User.UserAccountPage));
            MySpliView.IsPaneOpen = false;
        }

        private void _NearestStop_Click(object sender, RoutedEventArgs e)
        {
            _mainNavigationFrame.Navigate(typeof(Routes.StopCompassPage));
            MySpliView.IsPaneOpen = false;
        }
    }
}
