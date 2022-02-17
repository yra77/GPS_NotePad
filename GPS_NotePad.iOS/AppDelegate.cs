

using GPS_NotePad.iOS.Services;
using GPS_NotePad.iOS.Effects;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services.Interfaces;

using Prism;
using Prism.Ioc;

using Xamarin.Forms;

using UIKit;
using Foundation;
using System;


[assembly: ResolutionGroupName("GPS_NotePad")]
[assembly: ExportEffect(typeof(EntryUnderlineColor_Effect), "PlainEntryEffect")]

namespace GPS_NotePad.iOS
{

    [Register("AppDelegate")]

    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new IOSPlatformInitializer()));
            Xamarin.FormsGoogleMaps.Init("AIzaSyDzt_zSeQ_rK0TR2ClYHraBm7Yrg83JhDU");
            global::Xamarin.Auth.Presenters.XamarinIOS.AuthenticationConfiguration.Init();
            return base.FinishedLaunching(app, options);
        }

        //Google Auth
        public override bool OpenUrl ( UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
           #if DEBUG
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("OpenURL Called");
            sb.Append("     url         = ").AppendLine(url.AbsoluteUrl.ToString());
            sb.Append("     application = ").AppendLine(sourceApplication);
            sb.Append("     annotation  = ").AppendLine(annotation?.ToString());
            System.Diagnostics.Debug.WriteLine(sb.ToString());
           #endif

            // Convert iOS NSUrl to C#/netxf/BCL System.Uri - common API
            Uri uri_netfx = new Uri(url.AbsoluteString);

            // load redirect_url Page
            AuthenticationState_Helper.Authenticator.OnPageLoading(uri_netfx);

            return true;
        }
    }
    public class IOSPlatformInitializer : IPlatformInitializer
    {
        void IPlatformInitializer.RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISQLiteAsyncConnectionProvider, SQLiteAsyncConnectionProvider>();
            containerRegistry.Register<WorkingToImagesService, ResizeImageService>();
        }
    }

}
