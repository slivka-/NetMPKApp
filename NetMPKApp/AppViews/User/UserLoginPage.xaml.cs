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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            bool? isLoginBad = e.Parameter as bool?;
            if (isLoginBad != null)
            {
                if ((bool)isLoginBad)
                {
                    AppHelper.ShowErrorInfo("Błąd", "Niepoprawny login lub hasło.");
                }
                else
                {
                    AppHelper.ShowErrorInfo("Błąd", "Problem z połączeniem z usługą.\nSpróbuj ponownie.");
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    _LoginBox.Text = localSettings.Values["userLogin"] as string;
                    _autoLoginCheckBox.IsChecked = true;
                }
            }
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
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
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
                            localSettings.Values["userLogin"] = _userLogin;
                            localSettings.Values["userEncryptedPassword"] = await client.GetEncryptedPasswordAsync(_userLogin, _userPassword);
                        }

                        (Window.Current.Content as Frame).Navigate(typeof(Basic.IndexPage),true);
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
