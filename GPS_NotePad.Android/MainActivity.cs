

using GPS_NotePad.Droid.Services;
using GPS_NotePad.Droid.Effects;
using GPS_NotePad.Services.Interfaces;

using Acr.UserDialogs;

using Prism;
using Prism.Ioc;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps.Android;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android;


//[assembly: Dependency(typeof(LocationConnectService))]
[assembly: ResolutionGroupName("GPS_NotePad")]
[assembly: ExportEffect(typeof(EntryUnderlineColor_Effect), "PlainEntryEffect")]

namespace GPS_NotePad.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "GPS NotePad", Icon = "@drawable/logotip", Theme = "@style/MainTheme", 
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation | 
        ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        const int RequestLocationId = 0;


        readonly string[] LocationPermissions =
        {
           Manifest.Permission.AccessCoarseLocation,
           Manifest.Permission.AccessFineLocation
         };


        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            global::Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            LoadApplication(new App(new AndroidPlatformInitializer()));

            var platformConfig = new PlatformConfig
            {
                BitmapDescriptorFactory = new Icon_GoogleMap_Service()
            };

            global::Xamarin.FormsGoogleMaps.Init(this, savedInstanceState, platformConfig);

            UserDialogs.Init(this);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(LocationPermissions, RequestLocationId);
                }
                else
                {
                    // Permissions already granted - display a message.
                }
            }
        }

    }

    public class AndroidPlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISQLiteAsyncConnectionProvider, SQLiteAsyncConnectionProvider>();
            containerRegistry.Register<IWorkingToImagesService, ResizeImageService>();
            containerRegistry.Register<ILocationConnectService, LocationConnectService>();
        }

    }

}