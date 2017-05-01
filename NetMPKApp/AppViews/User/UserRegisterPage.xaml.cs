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
using Windows.UI.Core;
using Windows.Storage;
using System.Text.RegularExpressions;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NetMPKApp.AppViews.User
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserRegisterPage : Page
    {
        private bool isLoginOk = false;
        private bool isEmailOk = false;
        private bool isPasswOk = false;
        private bool isConfiOk = false;

        public UserRegisterPage()
        {
            isLoginOk = false;
            isEmailOk = false;
            isPasswOk = false;
            isConfiOk = false;
            InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += UserRegisterPage_BackRequested;
            _loginBox.LostFocus += _loginBox_LostFocus;
            _emailBox.LostFocus += _emailBox_LostFocus;
            _passwordBox.LostFocus += _passwordBox_LostFocus;
            _passwordConfirmBox.LostFocus += _passwordConfirmBox_LostFocus;
        }

        private void _passwordConfirmBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_passwordConfirmBox.Password.Trim() == _passwordBox.Password.Trim())
            {
                _passwordConfirmMessage.Text = "Ok";
                _passwordConfirmMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 0, G = 255, B = 0 });
                isConfiOk = true;
            }
            else
            {
                _passwordConfirmMessage.Text = "Hasło i potwierdzenie nie pasują do siebie!";
                _passwordConfirmMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, G = 0, B = 0 });
                isConfiOk = false;
            }
        }

        private void _passwordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_passwordBox.Password.Trim().Length >= 6)
            {
                _passwordMessage.Text = "Ok";
                _passwordMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 0, G = 255, B = 0 });
                isPasswOk = true;
            }
            else
            {
                _passwordMessage.Text = "Hasło musi mieć minimum 6 znaków!";
                _passwordMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, G = 0, B = 0 });
                isPasswOk = false;
            }
        }

        private async void _emailBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Regex r = new Regex(".+[@].+[.]...?", RegexOptions.IgnoreCase);
            if (r.Match(_emailBox.Text.Trim()).Success)
            {
                var client = ServiceConnection.GetInstance().client;
                var result = await client.EmailFreeAsync(_emailBox.Text.Trim());
                isEmailOk = result;
                if (result)
                {
                    _emailMessage.Text = "Ok";
                    _emailMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 0, G = 255, B = 0 });
                }
                else
                {
                    _emailMessage.Text = "Ten email jest zajęty!";
                    _emailMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, G = 0, B = 0 });
                }
            }
            else
            {
                _emailMessage.Text = "Niepoprawny adres email!";
                _emailMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, G = 0, B = 0 });
            }
        }

        private async void _loginBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_loginBox.Text.Trim().Length > 3)
            {
                var client = ServiceConnection.GetInstance().client;
                var result = await client.LoginFreeAsync(_loginBox.Text.Trim());
                isLoginOk = result;
                if (result)
                {
                    _loginMessage.Text = "Ok";
                    _loginMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 0, G = 255, B = 0 });
                }
                else
                {
                    _loginMessage.Text = "Ten login jest zajęty!";
                    _loginMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, G = 0, B = 0 });
                }
            }
            else
            {
                _loginMessage.Text = "Login musi składać się z minimum 3 znaków!";
                _loginMessage.Foreground = new SolidColorBrush(new Windows.UI.Color() { A = 255, R = 255, G = 0, B = 0 });
            }
        }

        private void UserRegisterPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private async void _registerButton_Click(object sender, RoutedEventArgs e)
        {
            if (isLoginOk && isEmailOk && isPasswOk && isConfiOk)
            {
                string _userLogin = _loginBox.Text.Trim();
                string _userPassword = _passwordBox.Password.Trim();
                var client = ServiceConnection.GetInstance().client;
                var result = await client.RegisterUserAsync(_userLogin, _userPassword, _emailBox.Text.Trim());
                if (result)
                {
                    ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                    localSettings.Values["userLogin"] = _loginBox.Text.Trim();
                    localSettings.Values["userEncryptedPassword"] = await client.GetEncryptedPasswordAsync(_userLogin, _userPassword);
                    (Window.Current.Content as Frame).Navigate(typeof(Basic.IndexPage),true);
                }
                else
                {
                    await AppHelper.ShowErrorInfo("Błąd","Wystąpił błąd podczas rejestracji.\nSpróbuj ponownie później.");
                }
            }
        }
    }
}
