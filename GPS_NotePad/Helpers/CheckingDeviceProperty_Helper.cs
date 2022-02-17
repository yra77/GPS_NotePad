
using Acr.UserDialogs;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GPS_NotePad.Helpers
{

    public interface ICheckingDeviceProperty_Helper
    {
        Task CheckingDeviceProperty();
    }

    class CheckingDeviceProperty_Helper : ICheckingDeviceProperty_Helper
    {

        public async Task CheckingDeviceProperty()
        {
            if (!CheckNetwork() || !await CheckGeoLocation())
            {
                await Task.Delay(5000);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private bool CheckNetwork()//check internet connection
        {
            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Device_Internet, "Error", "Ok");
                return false;            
            }
            return true;
        }
        private async Task<bool> CheckGeoLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var userLocation = await Geolocation.GetLocationAsync(request);
            }
            catch (Exception)
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Device_GeoLocacia, "Error", "Ok");
                return false;
            }
            return true;
        }
    }
}
