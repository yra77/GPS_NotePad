
using GPS_NotePad.Models;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.GoogleGetPlacesService
{
    interface IGoogleGetPlacesService
    {
        Task<GoogleDirection> GetDirections(string whatTheRoute, string originLatitude, string originLongitude, 
                                            string destinationLatitude, string destinationLongitude);
        Task<GooglePlaceAutoCompleteResult> GetPlaces(string text);
        Task<GooglePlace> GetPlaceDetails(string placeId);
    }
}
