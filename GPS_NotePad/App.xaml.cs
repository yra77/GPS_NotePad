

using GPS_NotePad.ViewModels;
using GPS_NotePad.Views;
using GPS_NotePad.Services.Repository;
using GPS_NotePad.Services.AuthService;
using GPS_NotePad.Services.RegistrService;
using GPS_NotePad.Services.MarkerService;
using GPS_NotePad.Services.MediaService;
using GPS_NotePad.Services.SettingsManager;
using GPS_NotePad.Resources.Resx;
using GPS_NotePad.Services.VerifyService;
using GPS_NotePad.Services.GoogleGetPlacesService;

using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

using Prism;
using Prism.Ioc;
using Prism.Unity;

using System.Globalization;


namespace GPS_NotePad
{
    public partial class App: PrismApplication
    {

        public static string Language { get; set; } = "en";

        public App(IPlatformInitializer platformInitializer = null) : base(platformInitializer) { }

        protected async override void OnInitialized()
        {
            InitializeComponent();

            LocalizationResourceManager.Current.PropertyChanged += (sender, e) => Resource.Culture = LocalizationResourceManager.Current.CurrentCulture;
            LocalizationResourceManager.Current.Init(Resource.ResourceManager);
            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("en");

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<LogInView, LogInViewModel>();
            containerRegistry.RegisterForNavigation<RegistrView, RegistrViewModel>();
            containerRegistry.RegisterForNavigation<RegistrView2, RegistrViewModel2>();
            containerRegistry.RegisterForNavigation<TabbedPageMy, TabbedPageMyViewModel>();
            containerRegistry.RegisterForNavigation<MapView, MapViewModel>();
            containerRegistry.RegisterForNavigation<ModalPageView, ModalPageViewModel>();
            containerRegistry.RegisterForNavigation<PinListView, PinListViewViewModel>();
            containerRegistry.RegisterForNavigation<AddPin, AddPinViewModel>();
            containerRegistry.RegisterForNavigation<FotoGaleryView, FotoGaleryViewModel>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModel>();
            containerRegistry.RegisterForNavigation<SearchRoute, SearchRouteViewModel>();
            containerRegistry.RegisterForNavigation<PublicTransportSelect, PublicTransportSelectViewModel>();

            //Services
            containerRegistry.RegisterSingleton<IRepository, Repository>();
            containerRegistry.RegisterSingleton<IAuthService, AuthService>();
            containerRegistry.RegisterSingleton<IRegistrService, RegistrService>();
            containerRegistry.RegisterSingleton<IMarkerService, MarkerService>();
            containerRegistry.RegisterSingleton<IMediaService, MediaService>();
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<IVerifyInputService, VerifyInputService>();
            containerRegistry.RegisterSingleton<IGoogleGetPlacesService, GoogleGetPlacesService>();

        }

    }
}
