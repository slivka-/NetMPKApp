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
using Windows.Storage;

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
            InitializeComponent();
            _LoginBox.Loaded += _LoginBox_Loaded;
        }

        private async void _LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_LoginBox.Text == "" || _PasswordBox.Password == "")
            {
                await AppHelper.ShowErrorInfo("Uzupełnij dane", "Login i hasło nie mogą być puste!");
                return;
            }
            else
            {
                var client = ServiceConnection.GetInstance().client;
                Tuple<bool, string> loginResponse = null;
                string _userLogin = _LoginBox.Text.Trim();
                string _userPassword = _PasswordBox.Password.Trim();
                try
                {
                    loginResponse = await client.LoginUserAsync(_userLogin, _userPassword);
                }
                catch (Exception)
                {
                    await AppHelper.ShowErrorInfo("Błąd", "Wystąpił błąd.\n Sprawdź połączenie z internetem.");
                    return;
                }

                if (loginResponse != null)
                {
                    if (loginResponse.Item1 && loginResponse.Item2 != null)
                    {
                        var userInfo = UserInfo.GetInstance();
                        userInfo._userId = loginResponse.Item2;
                        userInfo._userLogin = _userLogin;
                        if (_autoLoginCheckBox.IsChecked == true)
                        {
                            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                            localSettings.Values["userLogin"] = _userLogin;
                            localSettings.Values["userEncryptedPassword"] = await client.GetEncryptedPasswordAsync(_userLogin, _userPassword);
                        }

                        (Window.Current.Content as Frame).Navigate(typeof(Basic.IndexPage));
                        return;
                    }
                    else
                    {
                        await AppHelper.ShowErrorInfo("Błędne dane", "Nazwa użytkownika lub hasło są niepoprawne!\nSpróbuj ponownie.");
                        return;
                    }
                }
                else
                {
                    await AppHelper.ShowErrorInfo("Błąd", "Wystąpił błąd.\n Spróbuj ponownie później.");
                    return;
                }
            }
        }

        private void _LoginBox_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).Focus(FocusState.Programmatic);
        }
    }
}
