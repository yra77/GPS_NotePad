

using GPS_NotePad.Services.TranslatorInterfaces;
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
using GPS_NotePad.Services.MediaService;
using Android.Content;


//[assembly: Dependency(typeof(LocationConnectService))]
[assembly: ResolutionGroupName("GPS_NotePad")]
[assembly: ExportEffect(typeof(EntryUnderlineColor_Effect), "PlainEntryEffect")]
[assembly: ExportEffect(typeof(EditorUnderlineColorEffect), "PlainEditorEffect")]

namespace GPS_NotePad.Droid
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait, Label = "GPS NotePad", Icon = "@drawable/logotip", Theme = "@style/MainTheme", 
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation | 
        ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        const int RequestLocationId = 0;
        private const int RECORD_AUDIO = 1;

        private IMicrophoneService micService;
        private readonly string[] Permissions =
        {
           Manifest.Permission.AccessCoarseLocation,
           Manifest.Permission.AccessFineLocation
         };

        internal static MainActivity Instance { get; private set; }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            Instance = this;
            base.OnCreate(savedInstanceState);

            Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            LoadApplication(new App(new AndroidPlatformInitializer()));

            //чтобы редактор не заходил за открытую клавиатуру
            //App.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().
              //          UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            //

            PlatformConfig platformConfig = new PlatformConfig
            {
                BitmapDescriptorFactory = new Icon_GoogleMap_Service()
            };

            micService = DependencyService.Get<IMicrophoneService>();

            Xamarin.FormsGoogleMaps.Init(this, savedInstanceState, platformConfig);

            UserDialogs.Init(this);

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            MultiMediaPickerService.SharedInstance.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, 
                                                        [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            //switch (requestCode)
            //{
            //    case RECORD_AUDIO:
            //        {
            //            if (grantResults[0] == Permission.Granted)
            //            {
            //                micService.OnRequestPermissionResult(true);
            //            }
            //            else
            //            {
            //                micService.OnRequestPermissionResult(false);
            //            }
            //        }
            //        break;
            //}
        }

        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(Permissions, RequestLocationId);
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
            _ = containerRegistry.Register<ISQLiteAsyncConnectionProvider, SQLiteAsyncConnectionProvider>();
            _ = containerRegistry.Register<IWorkingToImagesService, ResizeImageService>();
            _ = containerRegistry.Register<ILocationConnectService, LocationConnectService>();
            _ = containerRegistry.Register<IMicrophoneService, MicrophoneService>();
            _ = containerRegistry.Register<ISpeechToText_Service, SpeechToText_Service>();
            _ = containerRegistry.Register <IMultiMediaPicker, MultiMediaPickerService>();
        }

    }

}