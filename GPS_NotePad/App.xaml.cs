

using GPS_NotePad.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Prism;
using Prism.Ioc;
using Prism.Unity;

using System;
using GPS_NotePad.Views;
using GPS_NotePad.Repository;
using GPS_NotePad.Services;

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
            containerRegistry.RegisterForNavigation<Tabbed_Page1, TabbedPage1ViewModel>();
            containerRegistry.RegisterForNavigation<Tabbed_Page2, TabbedPage2ViewModel>();

            //Services
            containerRegistry.RegisterSingleton<ITo_RepositoryService, To_RepositoryService>();
            containerRegistry.RegisterSingleton<IRepository, Repository.Repository>();
            containerRegistry.RegisterSingleton<IAuthGoogleService, AuthGoogleService>();
            containerRegistry.RegisterSingleton<IMediaService, MediaService>();
        }

    }
}
