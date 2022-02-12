
using Acr.UserDialogs;
using GPS_NotePad.Constants;
using GPS_NotePad.Helpers;
using GPS_NotePad.Models;
using GPS_NotePad.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Auth;
using Xamarin.Forms;


namespace GPS_NotePad.Services
{

    public interface IAuthService
    {
        Task<List<T>> GetData<T>(string table, string email) where T : class, new();
        Task<bool> Insert(Loginin profile);
        void GoogleAuth();
    }

    class AuthService : IAuthService
    {

        private readonly IRepository _repository;
        private Account _account;
        private readonly AccountStore _store;

        public AuthService(IRepository repository)
        {
            _repository = repository;
            _repository.CreateTable<Loginin>();
            _store = AccountStore.Create();
        }


        #region Public method,  Intarface IAuthService implementation

        public async Task<List<T>> GetData<T>(string table, string email) where T : class, new()
        {
            return await _repository.GetData<T>(table, email);
        }

        public async Task<bool> Insert(Loginin profile)
        {
            var res = await _repository.GetData<Loginin>("Loginin", profile.email);
            if (!res.Any())
            {
                return await _repository.Insert<Loginin>(profile);
            }
            else
                return false;
        }

        public void GoogleAuth()
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


        #region Private methods
        async void OnAuthCompletedAsync(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompletedAsync;
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
                authenticator.Completed -= OnAuthCompletedAsync;
                authenticator.Error -= OnAuthError;
            }

            Console.WriteLine("Authentication error: " + e.Message);
        }

        #endregion

    }
}
