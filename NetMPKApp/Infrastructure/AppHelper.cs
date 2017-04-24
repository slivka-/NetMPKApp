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
    class AppHelper
    {
        public static Task<ContentDialogResult> ShowErrorInfo(string title, string content)
        {
            var parent = Window.Current.Content as Frame;
            var dialog = new ContentDialog();
            dialog.MaxWidth = parent.ActualWidth;
            dialog.Title = title;
            dialog.PrimaryButtonText = "OK";
            dialog.Content = content;
            return dialog.ShowAsync().AsTask();
        }


    }
}
