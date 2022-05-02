
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace GPS_NotePad.Services.TranslatorInterfaces.TextTranslation_Service
{
    class TextTranslationsService : ITextTranslationsService
    {

        private readonly Azure_Auth_Translate _authentication_Azure;
        private HttpClient _httpClient;


        public TextTranslationsService()
        {
            _authentication_Azure = new Azure_Auth_Translate(Constants.Constant_AZURE.TextTranslatorApiKey);
        }


        #region Private helpers

        private string GenerateRequestUri(string endpoint, string text, string from, string to)
        {
            string requestUri = endpoint;
            requestUri += string.Format("?text={0}", Uri.EscapeUriString(text));
            requestUri += string.Format("&from={0}", from);
            requestUri += string.Format("&to={0}", to);
            return requestUri;
        }

        private async Task<string> SendRequestAsync(string url, string bearerToken)
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            return await response.Content.ReadAsStringAsync();
        }

        #endregion


        #region ITextTranslationsService implementation

        public async Task<string> TranslateTextAsync(string text, string languageFrom, string languageTo)
        {
            if (string.IsNullOrWhiteSpace(_authentication_Azure.GetAccessToken()))
            {
                await _authentication_Azure.InitializeAsync();
            }

            string requestUri = GenerateRequestUri(Constants.Constant_AZURE.TextTranslatorEndpoint, text, languageFrom, languageTo);

            string accessToken = _authentication_Azure.GetAccessToken(); 
            
            string response = await SendRequestAsync(requestUri, accessToken);

            try
            {
                XDocument xml = XDocument.Parse(response);

                return xml.Root.Value;
            }
            catch(Exception)
            {
                return "Error";
            }
        }

        #endregion

    }
}
