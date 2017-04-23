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
using NetMPKApp.AppViews;

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
                await NavHelper.ShowErrorInfo(this, "Uzupełnij dane", "Login i hasło nie mogą być puste!");
                return;
            }
            else
            {
                var client = ServiceConnection.GetInstance().client;
                Tuple<bool, string> loginResponse = null;

                try
                {
                    loginResponse = await client.LoginUserAsync(_LoginBox.Text.Trim(), _PasswordBox.Password.Trim());
                }
                catch (Exception)
                {
                    await NavHelper.ShowErrorInfo(this, "Błąd", "Wystąpił błąd.\n Sprawdź połączenie z internetem.");
                    return;
                }

                if (loginResponse != null)
                {
                    if (loginResponse.Item1 && loginResponse.Item2 != null)
                    {
                        var userInfo = UserInfo.GetInstance();
                        userInfo._userId = loginResponse.Item2;
                        userInfo._userLogin = _LoginBox.Text.Trim();
                        (Window.Current.Content as Frame).Navigate(typeof(Basic.IndexPage));
                        return;
                    }
                    else
                    {
                        await NavHelper.ShowErrorInfo(this, "Błędne dane", "Nazwa użytkownika lub hasło są niepoprawne!\nSpróbuj ponownie.");
                        return;
                    }
                }
                else
                {
                    await NavHelper.ShowErrorInfo(this, "Błąd", "Wystąpił błąd.\n Spróbuj ponownie później.");
                    return;
                }
            }
        }


    }
}
