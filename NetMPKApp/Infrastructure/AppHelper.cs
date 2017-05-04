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
        private static readonly double R = 6378.137;

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

        public static int CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var baseRad = lat1.ToRadians();
            var targetRad = lat2.ToRadians();
            var theta = lon1 - lon2;
            var thetaRad = theta.ToRadians();

            double dist = Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) * Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);
            dist = dist.FromRadians() * 60 * 1.1515;
            dist = dist * 1.609344;

            return (int)(dist * 1000);
        }

        public static double CalculateAngleFromNorth(double lat1, double lon1, double lat2, double lon2)
        {
            lat1 = lat1.ToRadians();
            lon1 = lon1.ToRadians();
            lat2 = lat2.ToRadians();
            lon2 = lon2.ToRadians();

            double dLon = (lon2 - lon1);

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

            double brng = Math.Atan2(y, x);

            return  Math.Abs(360-brng.ToBearing());
        }
    }

    public static class StaticExtensions
    {
        public static double ToRadians(this double d)
        {
            return (Math.PI * d) / 180;
        }

        public static double FromRadians(this double d)
        {
            return (d * 180) / Math.PI;
        }

        public static double ToBearing(this double d)
        {
            return (d.FromRadians() + 360) % 360;
        }
    }
}
