
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.TranslatorInterfaces.TextTranslation_Service
{
    public class Azure_Auth_Translate
    {
        private string _subscriptionKey;
        private static string _token;
        private Timer _accessTokenRenewer;
        private const int _RefreshTokenDuration = 9;
        private HttpClient _httpClient;

        public Azure_Auth_Translate(string apiKey)
        {
            _subscriptionKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        }

        public async Task InitializeAsync()
        {
            _token = await FetchTokenAsync(Constants.Constant_AZURE.AuthenticationTokenEndpoint);
            _accessTokenRenewer = new Timer(new TimerCallback(OnTokenExpiredCallback), this, 
                TimeSpan.FromMinutes(_RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
        }

        public string GetAccessToken()
        {
            return _token;
        }

        private async Task RenewAccessToken()
        {
            _token = await FetchTokenAsync(Constants.Constant_AZURE.AuthenticationTokenEndpoint);
            Debug.WriteLine("Renewed token.");
        }

        private async void OnTokenExpiredCallback(object stateInfo)
        {
            try
            {
                await RenewAccessToken();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Failed to renew access token. Details: {0}", ex.Message));
            }
            finally
            {
                try
                {
                    _accessTokenRenewer.Change(TimeSpan.FromMinutes(_RefreshTokenDuration), TimeSpan.FromMilliseconds(-1));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Failed to reschedule the timer to renew access token. Details: {0}", ex.Message));
                }
            }
        }

        private async Task<string> FetchTokenAsync(string fetchUri)
        {
            UriBuilder uriBuilder = new UriBuilder(fetchUri);
            uriBuilder.Path += "/issueToken";

            HttpResponseMessage result = await _httpClient.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
            return await result.Content.ReadAsStringAsync();
        }
    }
}
