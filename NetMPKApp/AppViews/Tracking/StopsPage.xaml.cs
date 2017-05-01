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
using Windows.UI.Xaml.Documents;
using Windows.UI.Text;

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
            Init();
        }


        private async void Init()
        {
            var client = ServiceConnection.GetInstance().client;
            stopsList = await client.GetStopsNamesAsync();
            _stopsSource.Source = stopsList.Select(s => new StopItem{ stopName = s}).GroupBy(g => g.stopName.First());

            var firstLetters = stopsList.Select(s => s.First().ToString()).Distinct().ToList();
            for (int i = 0; i < firstLetters.Count; i++)
            {
                Button b = new Button();
                b.Width = 50;
                b.Height = 50;
                b.Content = firstLetters[i];
                b.Margin = new Thickness() { Left = 10, Right = 10 };
                //b.Background = Brush.
                b.VerticalAlignment = VerticalAlignment.Center;
                b.HorizontalAlignment = HorizontalAlignment.Center;
                b.FontSize = 30;
                b.FontWeight = FontWeights.ExtraBold;
                b.Click += letterPickerClick;
                _letterPicker.Children.Add(b);
                Grid.SetRow(b, i / 4);
                if(i>=firstLetters.Count-2)
                    Grid.SetColumn(b, (i % 4)+1);
                else
                    Grid.SetColumn(b, i % 4);
            }
            _stopsContainer.SelectedItem = null;
            _stopsContainer.SelectionChanged += _stopsContainer_SelectionChanged;
        }

        private void letterPickerClick(object sender, RoutedEventArgs e)
        {
            _letterSelector.IsOpen = false;
            var clickedLetter = (e.OriginalSource as Button).Content.ToString().First();
            var x = _stopsContainer.Items.Where(w => (w as StopItem).stopName.First().Equals(clickedLetter)).First();
            _stopsContainer.ScrollIntoView(x, ScrollIntoViewAlignment.Leading);
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
                (Parent as Frame).Navigate(typeof(SingleStopPage), args.ChosenSuggestion.ToString());
            }
        }

        private void _stopsContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceItem = (sender as ListView).SelectedItem as StopItem;
            (Parent as Frame).Navigate(typeof(SingleStopPage), sourceItem.stopName);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _letterSelector.IsOpen = true;
        }

        private class StopItem
        {
            public string stopName { get; set; }
        }


    }
}
