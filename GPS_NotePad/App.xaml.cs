

using GPS_NotePad.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Prism;
using Prism.Ioc;
using Prism.Unity;

using System;
using GPS_NotePad.Views;
using GPS_NotePad.Repository;
using GPS_NotePad.ViewModels.Services;

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
            containerRegistry.RegisterForNavigation<Tabbed_Page1, TabbedPageMyViewModel>();
            containerRegistry.RegisterForNavigation<Tabbed_Page2, TabbedPageMyViewModel>();

            //Services
            containerRegistry.RegisterSingleton<ITo_RepositoryService, To_RepositoryService>();
            containerRegistry.RegisterSingleton<IRepository, Repository.Repository>();
            containerRegistry.RegisterSingleton<IAuthGoogleService, AuthGoogleService>();
        }

    }
}
