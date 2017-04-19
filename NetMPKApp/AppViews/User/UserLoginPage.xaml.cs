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

namespace NetMPKApp.AppViews.User
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserLoginPage : Page
    {
        public UserLoginPage()
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += ((s, e) => NavHelper.BackRequestHandler(s, e, Window.Current.Content));
            InitializeComponent();
        }

        private async void _LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_LoginBox.Text == "" || _PasswordBox.Password == "")
            {
                var dialog = new ContentDialog();
                dialog.MaxWidth = this.ActualWidth;
                dialog.Title = "Uzupełnij dane";
                dialog.PrimaryButtonText = "OK";
                dialog.Content = "Login i hasło nie mogą być puste!";
                await dialog.ShowAsync();
                return;
            }
            else
            {
                //var client = ServiceConnection.GetInstance().client;
                var client = new NetMPKService.MPKServiceClient();
                var loginRequest = new NetMPKService.LoginUserRequest() { login = _LoginBox.Text.Trim(), password = _PasswordBox.Password.Trim() };
                NetMPKService.LoginUserResponse loginResponse = new NetMPKService.LoginUserResponse();
                try
                {
                    var x = await client.GetStopsNamesAsync();
                    loginResponse = await client.LoginUserAsync(loginRequest);
                }
                catch (Exception ex)
                {
                    var dialog = new ContentDialog();
                    dialog.MaxWidth = this.ActualWidth;
                    dialog.Title = "Błąd";
                    dialog.PrimaryButtonText = "OK";
                    //dialog.Content = "Wystąpił błąd.\n Spróbuj ponownie później.";
                    dialog.Content = ex.ToString();
                    await dialog.ShowAsync();
                    return;
                }
                if (loginResponse.LoginUserResult)
                {
                    //redirectAndLogin
                }
                else
                {
                    var dialog = new ContentDialog();
                    dialog.MaxWidth = this.ActualWidth;
                    dialog.Title = "Błędne dane";
                    dialog.PrimaryButtonText = "OK";
                    dialog.Content = "Nazwa użytkownika lub hasło są niepoprawne!\nSpróbuj ponownie.";
                    await dialog.ShowAsync();
                    return;
                }
            }
        }


    }
}
