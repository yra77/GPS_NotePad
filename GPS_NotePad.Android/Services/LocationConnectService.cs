
using Android.App;
using Android.Content;

using GPS_NotePad.Services.Interfaces;


namespace GPS_NotePad.Droid.Services
{

    class LocationConnectService : ILocationConnectService
    {
        public bool IsGpsAvailable()
        {

            Android.Locations.LocationManager manager = 
                (Android.Locations.LocationManager)Application.Context.GetSystemService(Context.LocationService);
            
            if (!manager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider))
            {
                //gps disable
                return false;
            }
            else
            {
                //Gps enable
                return true;
            }        
        }

        public void OpenSettings()
        {
            Intent intent = new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
    }
}