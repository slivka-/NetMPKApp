﻿using System;
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
using Windows.Storage;
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
    }
}
