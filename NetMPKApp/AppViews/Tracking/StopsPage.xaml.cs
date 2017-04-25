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
using NetMPKApp.Infrastructure;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Tracking
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StopsPage : Page
    {

        private List<string> stopsList;

        public StopsPage()
        {
            this.InitializeComponent();
            FillStopsList();
        }


        private async void FillStopsList()
        {
            var client = ServiceConnection.GetInstance().client;
            stopsList = await client.GetStopsNamesAsync();
            _stopsSource.Source = stopsList.Select(s => new { stopName = s}).GroupBy(g => g.stopName.First());
        }

        private void _stopsContainer_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                sender.ItemsSource = stopsList.Where(w => w.StartsWith(sender.Text));
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                //DO przystanku
                //(Parent as Frame).Navigate()
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _letterSelector.IsOpen = true;
        }
    }
}
