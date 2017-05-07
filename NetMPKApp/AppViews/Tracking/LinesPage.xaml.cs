using NetMPKApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.Tracking
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LinesPage : Page
    {
        private List<LineItem> linesList;
        private List<int> rawLines;

        public LinesPage()
        {
            this.InitializeComponent();
            Init();
        }


        private async void Init()
        {
            var client = ServiceConnection.GetInstance().client;
            var tempLines = await client.GetAllLinesAsync();
            linesList = tempLines.Select(s => new LineItem(s)).ToList();
            _linesSource.Source = linesList.GroupBy(g => g.groupText);
            rawLines = linesList.Select(s => s.lineNo).ToList();

            var linesGroups = linesList.Select(s => s.groupText).Distinct().ToList();

            for (int i = 0; i < linesGroups.Count; i++)
            {
                Button b = new Button();
                b.Height = 50;
                b.Content = linesGroups[i];
                b.Margin = new Thickness() { Left = 10, Right = 10 };
                //b.Background = Brush.
                b.VerticalAlignment = VerticalAlignment.Center;
                b.HorizontalAlignment = HorizontalAlignment.Stretch;
                b.FontSize = 15;
                b.FontWeight = FontWeights.ExtraBold;
                b.Click += lineTypePickerClick;
                _linePicker.Children.Add(b);
                Grid.SetRow(b, i);
            }
            _linesContainer.SelectedItem = null;
            _linesContainer.SelectionChanged += _linesContainer_SelectionChanged;
        }



        private void lineTypePickerClick(object sender, RoutedEventArgs e)
        {
            _lineSelector.IsOpen = false;
            var clickedGroup = (e.OriginalSource as Button).Content.ToString();
            var x = _linesContainer.Items.Where(w => (w as LineItem).groupText.Equals(clickedGroup)).First();
            _linesContainer.ScrollIntoView(x, ScrollIntoViewAlignment.Leading);
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem.ToString();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var x = rawLines.Where(w => w.ToString().StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase));
                if(x != null)
                    sender.ItemsSource = x;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                (Parent as Frame).Navigate(typeof(SingleLinePage), args.ChosenSuggestion.ToString());
            }
        }

        private void _linesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sourceItem = (sender as ListView).SelectedItem as LineItem;
            (Parent as Frame).Navigate(typeof(SingleLinePage), sourceItem.lineNo);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _lineSelector.IsOpen = true;
        }

        private class LineItem
        {
            public int lineNo { get; }
            public string groupTag { get;}
            public string groupText { get; }
            public string imgRef { get; }

            public LineItem(Tuple<int, string, string, string, string> input)
            {
                lineNo = input.Item1;

                imgRef = (lineNo < 100) ? char.ConvertFromUtf32(60237) : char.ConvertFromUtf32(60231);

                if (input.Item2 == "TRAM")
                {
                    if (input.Item4 == "DAY")
                    {
                        groupTag = "TD";
                        groupText = "Linie tramwajowe";
                    }
                    else if(input.Item4 == "NIGHT")
                    {
                        groupTag = "TN";
                        groupText = "Linie tramwajowe nocne";
                    }
                }
                else if (input.Item2 == "BUS")
                {
                    if (input.Item4 == "DAY")
                    {
                        if (input.Item3 == "CITY")
                        {
                            if (input.Item5 == "NORMAL")
                            {
                                groupTag = "BDN";
                                groupText = "Linie autobusowe miejskie";
                            }
                            else if (input.Item5 == "ACCEL")
                            {
                                groupTag = "BDAC";
                                groupText = "Linie autobusowe miejskie pośpieszne";
                            }
                            else if (input.Item5 == "AUX")
                            {
                                groupTag = "BDAX";
                                groupText = "Linie autobusowe miejskie pomocniczne";
                            }
                            else if (input.Item5 == "REPLACE")
                            {
                                groupTag = "BDR";
                                groupText = "Linie autobusowe miejskie zastępcze";
                            }
                        }
                        else if (input.Item3 == "AGGL")
                        {
                            groupTag = "BDA";
                            groupText = "Linie autobusowe aglomeracyjne";
                        }
                    }
                    else if (input.Item4 == "NIGHT")
                    {
                        if (input.Item3 == "CITY")
                        {
                            groupTag = "BNC";
                            groupText = "Linie autobusowe miejskie nocne";
                        }
                        else if (input.Item3 == "AGGL")
                        {
                            groupTag = "BNA";
                            groupText = "Linie autobusowe aglomeracyjne nocne";
                        }
                    }
                }
            }
        }
    }
}
