using Acr.UserDialogs;
using GPS_NotePad.ViewModels.Constants;
using GPS_NotePad.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Auth;
using Xamarin.Forms;

namespace GPS_NotePad.ViewModels.Services
{
    public interface IAuthGoogleService
    {
        void StartAuth();
    }
    class AuthGoogleService : IAuthGoogleService
    {
        Account account;
        AccountStore store;
        public AuthGoogleService()
        {
            store = AccountStore.Create();
        }

        public void StartAuth()
        {

            string clientId = null;
            string redirectUri = null;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    clientId = Constant_Auth.iOSClientId;
                    redirectUri = Constant_Auth.iOSRedirectUrl;
                    break;

                case Device.Android:
                    clientId = Constant_Auth.AndroidClientId;
                    redirectUri = Constant_Auth.AndroidRedirectUrl;
                    break;
            }

            account = store.FindAccountsForService(Constant_Auth.AppName).FirstOrDefault();

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Constant_Auth.Scope,
                new Uri(Constant_Auth.AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(Constant_Auth.AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            AuthenticationState_Helper.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);

        }
        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            //User user = null;
            if (e.IsAuthenticated)
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(Constant_Auth.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    string userJson = await response.GetResponseTextAsync();
                    UserDialogs.Instance.Alert(userJson, "Response", "Ok");
                    //user = JsonConvert.DeserializeObject<User>(userJson);
                }

                //if (user != null)
                //{
                //	//App.Current.MainPage = new NavigationPage(new MyDashBoardPage());

                //}

               // await store.SaveAsync(account = e.Account, AppConstant.Constants.AppName);
                //await DisplayAlert("Email address", user.Email, "OK");
            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            Console.WriteLine("Authentication error: " + e.Message);
        }
    }

}
