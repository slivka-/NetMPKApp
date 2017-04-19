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
    }
}
