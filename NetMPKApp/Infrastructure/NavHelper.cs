using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NetMPKApp.Infrastructure
{
    class NavHelper
    {
        public static void BackRequestHandler(object sender, BackRequestedEventArgs e, UIElement origin)
        {
            Frame rootFrame = origin as Frame;
            if (rootFrame == null)
                return;
            if (rootFrame.CanGoBack && !e.Handled)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        public static async Task<ContentDialogResult> ShowErrorInfo(FrameworkElement parent, string title, string content)
        {
            var dialog = new ContentDialog();
            dialog.MaxWidth = parent.ActualWidth;
            dialog.Title = title;
            dialog.PrimaryButtonText = "OK";
            dialog.Content = content;
            return await dialog.ShowAsync();
        }

    }
}
