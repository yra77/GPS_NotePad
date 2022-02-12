﻿

using GPS_NotePad.iOS.Services;
using GPS_NotePad.Services;
using GPS_NotePad.iOS.Effects;

using Prism;
using Prism.Ioc;

using Xamarin.Forms;

using UIKit;
using Foundation;

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
            return base.FinishedLaunching(app, options);
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
