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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Routes
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoriteRoutesPage : Page
    {
        private List<FavouriteRoute> favRoutesList;

        public FavoriteRoutesPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            int userId = 0;
            if (int.TryParse(Infrastructure.UserInfo.GetInstance()._userId, out userId))
            {
                var result = await client.GetSavedRoutesForUserAsync(userId);
                favRoutesList = result.Select(s => new FavouriteRoute(s)).ToList();
                _favRoutesSource.Source = favRoutesList;
                _favRoutesContainer.SelectedItem = null;
                _favRoutesContainer.SelectionChanged += _favRoutesContainer_SelectionChanged;
            }
        }

        private void _favRoutesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceItem = (sender as ListView).SelectedItem as FavouriteRoute;
            (Parent as Frame).Navigate(typeof(RouteSearchPage),Tuple.Create(sourceItem.stopFrom,sourceItem.stopTo));
        }

        private class FavouriteRoute
        {
            public string stopFrom { get; }
            public string stopTo { get; }

            public FavouriteRoute(Tuple<string, string> s)
            {
                stopFrom = s.Item1;
                stopTo = s.Item2;
            }
        }
    }
}
