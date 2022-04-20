

using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.Android.Factories;
using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
using AndroidBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;


namespace GPS_NotePad.Droid.Services
{
    public sealed class Icon_GoogleMap_Service : IBitmapDescriptorFactory
    {
        public AndroidBitmapDescriptor ToNative(BitmapDescriptor descriptor)
        {
            int iconId = 0;
            switch (descriptor.Id)
            {
                case "pin":
                    iconId = Resource.Drawable.pin;
                    break;
                case "taxi":
                    iconId = Resource.Drawable.taxi;
                    break;
            }
            return AndroidBitmapDescriptorFactory.FromResource(iconId);
        }

    }
}