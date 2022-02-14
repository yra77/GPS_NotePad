
using GPS_NotePad.Helpers;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

using System;
using Xamarin.Auth;

namespace GPS_NotePad.Droid
{
    [Activity(Label = "CustomUrlSchemeInterceptorActivity", NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataSchemes = new[] { "com.googleusercontent.apps.486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk" },
    DataPath = "/oauth2redirect", AutoVerify = true)]

    public class CustomUrlSchemeInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Convert Android.Net.Url to Uri
            Uri uri = new Uri(Intent.Data.ToString());

            // Load redirectUrl page
            AuthenticationState_Helper.Authenticator.OnPageLoading(uri);

            //закрывает уведомление в браузере
            CustomTabsConfiguration.CustomTabsClosingMessage = null;
            //закрывает браузер
            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);
           
             Finish();
        }
    }
}