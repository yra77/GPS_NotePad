

using GPS_NotePad.ViewModels;
using GPS_NotePad.Views;
using GPS_NotePad.Repository;
using GPS_NotePad.Services;

using Xamarin.Forms;

using Prism;
using Prism.Ioc;
using Prism.Unity;


namespace GPS_NotePad
{
    public partial class App: PrismApplication
    {
        public App(IPlatformInitializer platformInitializer = null) : base(platformInitializer) { }

        protected async override void OnInitialized()
        {
            InitializeComponent();
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<TabbedPageMy, TabbedPageMyViewModel>();
            containerRegistry.RegisterForNavigation<MapView, MapViewModel>();
            containerRegistry.RegisterForNavigation<PinListView, PinListViewViewModel>();
            containerRegistry.RegisterForNavigation<AddPin, AddPinViewModel>();

            //Services
            containerRegistry.RegisterSingleton<IRepository, Repository.Repository>();
            containerRegistry.RegisterSingleton<IAuthService, AuthService>();
            containerRegistry.RegisterSingleton<IRegistrService, RegistrService>();
            containerRegistry.RegisterSingleton<IMarkerService, MarkerService>();
            containerRegistry.RegisterSingleton<IMediaService, MediaService>();
        }

    }
}
