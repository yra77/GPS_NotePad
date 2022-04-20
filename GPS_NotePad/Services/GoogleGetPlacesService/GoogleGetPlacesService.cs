
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
            var httpClient = new HttpClient
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
                        requestStr = $"api/directions/json?mode=transit&transit_mode=train|tram|subway|bus&alternatives=true&origin={originLatitude}," +
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

                        //foreach (Route a in googleDirection.Routes)
                        //{
                        //    foreach (Leg b in a.Legs)
                        //    {
                        //        Console.WriteLine(b.StartAddress + "\n" + b.EndAddress + "\n" + b.Duration.Text + "\n" + b.Distance.Text);
                        //        foreach (Step c in b.Steps)
                        //        {
                        //            Console.WriteLine(c.TravelMode + " - " + c.Distance.Text + "\n" + c.HtmlInstructions
                        //                      + "\n" + c.Duration.Text + " " + c.Distance.Text);
                        //            if (c.TransitDetails != null)
                        //            {
                        //                Console.WriteLine(c.TransitDetails.DepartureTime.Text
                        //                    + "\n" + c.TransitDetails.ArrivalTime.Text 
                        //                    + "\n" + c.TransitDetails.Line.Name
                        //                    + "\n" + c.TransitDetails.TripShortName 
                        //                    + "\n" + c.TransitDetails.Line.Vehicle.Type);
                        //            }
                        //        }
                        //        Console.WriteLine("--------------------------------------");
                        //    }
                        //}

                    }
                }
            }

            return googleDirection;
        }


        public async Task<GooglePlace> GetPlaceDetails(string placeId)
        {
            GooglePlace result = null;
            using (var httpClient = CreateClient())
            {
                var response = await httpClient.GetAsync($"api/place/details/json?placeid={Uri.EscapeUriString(placeId)}&key={Constant.GOOGLE_MAP_KEY}").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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

            using (var httpClient = CreateClient())
            {
                var response = await httpClient.GetAsync($"api/place/autocomplete/json?input={Uri.EscapeUriString(text)}&key={Constant.GOOGLE_MAP_KEY}").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
