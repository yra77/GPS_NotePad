
using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
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
                await Task.Delay(10000);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        private bool CheckNetwork()//check internet connection
        {
            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                UserDialogs.Instance.Alert("Error! No connection to the internet", "Error", "Ok");
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
            catch (Exception ex)
            {
             UserDialogs.Instance.Alert("Error! Geo location disable.", "Error", "Ok");
                return false;
            }
            return true;
        }
    }
}
