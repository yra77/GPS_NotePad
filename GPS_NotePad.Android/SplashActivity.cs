using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;
using System.Threading.Tasks;

namespace GPS_NotePad.Droid
{
    [Activity(ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, 
        Label = "GPS NotePad", MainLauncher = true, Theme = "@style/MyTheme.Splash", Icon = "@drawable/logotip", NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        async void SimulateStartup()
        {
            await Task.Delay(200);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
        public override void OnBackPressed() { }
    }
}