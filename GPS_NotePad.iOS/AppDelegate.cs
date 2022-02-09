

using GPS_NotePad.iOS.Services;
using GPS_NotePad.Services;

using Prism;
using Prism.Ioc;

using UIKit;
using Foundation;


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
            Xamarin.FormsMaps.Init();
            return base.FinishedLaunching(app, options);
        }
    }
    public class IOSPlatformInitializer : IPlatformInitializer
    {
        void IPlatformInitializer.RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ISQLiteAsyncConnectionProvider, SQLiteAsyncConnectionProvider>();
            containerRegistry.Register<IResizeImageService, ResizeImageService>();
        }
    }

}
