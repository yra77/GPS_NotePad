
using GPS_NotePad.Constants;
using GPS_NotePad.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.GoogleGetPlacesService
{
    class GoogleGetPlacesService : IGoogleGetPlacesService
    {

        private const string ApiBaseAddress = "https://maps.googleapis.com/maps/";


        private HttpClient CreateClient()
        {
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }


        public async Task<GoogleDirection> GetDirections(string whatTheRoute, string originLatitude, string originLongitude,
                                                         string destinationLatitude, string destinationLongitude)
        {
            originLatitude = new string(originLatitude.Select(x => x == ',' ? '.' : x).ToArray());
            originLongitude = new string(originLongitude.Select(x => x == ',' ? '.' : x).ToArray());
            destinationLatitude = new string(destinationLatitude.Select(x => x == ',' ? '.' : x).ToArray());
            destinationLongitude = new string(destinationLongitude.Select(x => x == ',' ? '.' : x).ToArray());

            GoogleDirection googleDirection = new GoogleDirection();

            using (HttpClient httpClient = CreateClient())
            {
                string requestStr = "";

                switch (whatTheRoute)
                {
                    case "walk":
                        requestStr = $"api/directions/json?mode=walking&origin={originLatitude},{originLongitude}&destination={destinationLatitude}," +
                          $"{destinationLongitude}&key={Constant.GOOGLE_MAP_KEY}";
                        break;

                    case "busTrain":

                        string language = App.Language == "uk" ? "ru" : "en";

                        requestStr = $"api/directions/json?language={language}&mode=transit&transit_mode=train|tram|subway|bus&alternatives=true&origin={originLatitude}," +
                            $"{originLongitude}&destination={destinationLatitude},{destinationLongitude}&key={Constant.GOOGLE_MAP_KEY}";
                        break;

                    case "car":
                        requestStr = $"api/directions/json?mode=driving&transit_routing_preference=less_driving&origin={originLatitude}," +
                           $"{originLongitude}&destination={destinationLatitude},{destinationLongitude}&key={Constant.GOOGLE_MAP_KEY}";
                        break;
                    default:
                        break;
                }

                HttpResponseMessage response = await httpClient.GetAsync(requestStr).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                   // Console.WriteLine(json);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        googleDirection = await Task.Run(() =>
                           JsonConvert.DeserializeObject<GoogleDirection>(json)
                        ).ConfigureAwait(false);
                    }
                }
            }

            return googleDirection;
        }


        public async Task<GooglePlace> GetPlaceDetails(string placeId)
        {
            GooglePlace result = null;

            using (HttpClient httpClient = CreateClient())
            {
                var response = await httpClient.GetAsync($"api/place/details/json?placeid={Uri.EscapeUriString(placeId)}" +
                                                         $"&key={Constant.GOOGLE_MAP_KEY}").ConfigureAwait(false);
                
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(json) && json != "ERROR")
                    {
                        result = new GooglePlace(JObject.Parse(json));
                    }
                }
            }

            return result;
        }

        public async Task<GooglePlaceAutoCompleteResult> GetPlaces(string text)
        {
            GooglePlaceAutoCompleteResult results = null;

            using (HttpClient httpClient = CreateClient())
            {
                var response = await httpClient.GetAsync($"api/place/autocomplete/json?input={Uri.EscapeUriString(text)}" +
                                                         $"&key={Constant.GOOGLE_MAP_KEY}").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(json) && json != "ERROR")
                    {
                        results = await Task.Run(() =>
                           JsonConvert.DeserializeObject<GooglePlaceAutoCompleteResult>(json)
                        ).ConfigureAwait(false);

                    }
                }
            }

            return results;
        }

    }
}
