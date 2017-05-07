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
    public sealed partial class RouteSearchPage : Page
    {
        private List<string> stopsList;
        private string firstStop;
        private string lastStop;

        private List<SingleRoute> routesList;

        public RouteSearchPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var client = Infrastructure.ServiceConnection.GetInstance().client;
            stopsList = await client.GetStopsNamesAsync();
            Tuple<string, string> favInfo = e.Parameter as Tuple<string, string>;
            if (favInfo != null)
            {
                firstStop = favInfo.Item1;
                _firstStop.Text = firstStop;
                lastStop = favInfo.Item2;
                _lastStop.Text = lastStop;
                SearchForRoute();
            }
        }

        private async void SearchForRoute()
        {
            if (firstStop != null && lastStop != null)
            {
                var client = Infrastructure.ServiceConnection.GetInstance().client;
                var result = await client.GetRoutesAsync(firstStop, lastStop);
                routesList = result.Select(s => new SingleRoute(s)).ToList();
                _routesSource.Source = routesList;
                _routesContainer.SelectedItem = null;
                _routesContainer.SelectionChanged += _routesContainer_SelectionChanged;
            }
        }

        private void _routesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceItem = (sender as ListView).SelectedItem as SingleRoute;

        }

        private void _searchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchForRoute();
        }

        private void _firstStop_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var x = stopsList.Where(w => w.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase));
                if (x != null)
                    sender.ItemsSource = x;
            }
        }

        private void _firstStop_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private void _firstStop_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                firstStop = args.ChosenSuggestion.ToString();
            }
        }

        private void _lastStop_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var x = stopsList.Where(w => w.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase));
                if (x != null)
                    sender.ItemsSource = x;
            }
        }

        private void _lastStop_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private void _lastStop_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                lastStop = args.ChosenSuggestion.ToString();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (Parent as Frame).Navigate(typeof(FavoriteRoutesPage));
        }

        private class SingleRoute
        {

            public int firstLine { get; }
            public int lastLine { get; }
            public string startTime { get; }
            public string finishTime { get; }
            public string firstLineSymbol { get; }
            public string lastLineSymbol { get; }
            public string initialDelay { get; }
            public int initialDelayInt { get; }
            public List<Tuple<int,string,string, string, string,int>> details { get; }

            public SingleRoute(List<Tuple<int, string, string, string, string, int>> r)
            {
                details = r;
                firstLine = r.First().Item1;
                lastLine = r.Last().Item1;
                startTime = r.First().Item3;
                finishTime = r.Last().Item5;
                firstLineSymbol = (firstLine < 100) ? char.ConvertFromUtf32(60237) : char.ConvertFromUtf32(60231);
                lastLineSymbol = (lastLine < 100) ? char.ConvertFromUtf32(60237) : char.ConvertFromUtf32(60231);
                initialDelay = r.First().Item6.ToString() +" min.";
                initialDelayInt = r.First().Item6;
            }
        }
    }
}
