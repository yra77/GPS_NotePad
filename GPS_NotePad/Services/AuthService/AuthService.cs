

using GPS_NotePad.Constants;
using GPS_NotePad.Services.VerifyService;
using GPS_NotePad.Models;
using GPS_NotePad.Services.Repository;
using GPS_NotePad.ViewModels;
using GPS_NotePad.Helpers;

using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Auth;
using Xamarin.Forms;


namespace GPS_NotePad.Services.AuthService
{

    class AuthService : IAuthService
    {

        private GoogleAuthCallBack googleAuthCallBack;

        private readonly IRepository _repository;
        private Account _account;
        private readonly AccountStore _store;
        private readonly IVerifyInputService _verifyInput;


        public AuthService(IRepository repository,
                              IVerifyInputService verifyInputService)
        {
            _verifyInput = verifyInputService;
            _repository = repository;
            _store = AccountStore.Create();
        }


        #region Private helper

        async void OnAuthCompletedAsync(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompletedAsync;
                authenticator.Error -= OnAuthError;
            }
            
            GoogleUser user = null;

            if (e.IsAuthenticated)
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(Constant_Auth.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    var userJson = await response.GetResponseTextAsync();

                    user = JsonConvert.DeserializeObject<GoogleUser>(userJson); 
                }

                if (user != null && user.Email != null)
                {              

                    var res = await _repository.GetDataAsync<Loginin>("Loginin", user.Email);

                    if (res.Any())
                    {
                        //  if (res.First().password == "GoogleUser1")
                        //  {
                        googleAuthCallBack(user.Email, true);
                        return;
                        //}
                    }
                    else
                    {
                        googleAuthCallBack(user.Email, false);
                        return;
                    }

                }

            }
           // UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_SavePin, "Error", "Ok");
            return;
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompletedAsync;
                authenticator.Error -= OnAuthError;
            }

           // Console.WriteLine("Authentication error: " + e.Message);
        }

        #endregion


        #region Public method,  Intarface IAuthService implementation

        public async Task<(bool, string)> AuthAsync(string password, string email)
        {
            string str = "";

            if (_verifyInput.IsValidEmail(email))
            {
                if (_verifyInput.PasswordVerify(password))
                {
                    var res = await _repository.GetDataAsync<Loginin>("Loginin", email);

                    if (res.Any())
                    {
                        if (res.First().password == password)
                        {
                            return (true, "Ok");
                        }
                        else
                        {
                            str = Resources.Resx.Resource.Alert_Password1;
                        }
                    }
                    else
                    {
                        str = Resources.Resx.Resource.Alert_Email2;
                    }
                }
                else
                {
                    str = Resources.Resx.Resource.Alert_Password2;
                }
            }
            else
            {
                str = Resources.Resx.Resource.Alert_Email3;
            }

            return (false, str);
        }

        public void GoogleAuth(GoogleAuthCallBack myDel)
        {

            googleAuthCallBack = myDel;
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

            _account = _store.FindAccountsForService(Constant_Auth.AppName).FirstOrDefault();

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Constant_Auth.Scope,
                new Uri(Constant_Auth.AuthorizeUrl),
                new Uri(redirectUri),
                new Uri(Constant_Auth.AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthCompletedAsync;
            authenticator.Error += OnAuthError;

            AuthenticationState_Helper.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        #endregion

    }
}
