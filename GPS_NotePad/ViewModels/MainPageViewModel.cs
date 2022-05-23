
using Acr.UserDialogs;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services.SettingsManager;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System.Globalization;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.Helpers;


namespace GPS_NotePad.ViewModels
{
    //486491599259-tco80iivdbvvj6ohmbhjb3tgn4ibfg5c.apps.googleusercontent.com//ios
    //486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk.apps.googleusercontent.com//android
    //AIzaSyDcSVIjErUsWelFNRTTqKJimfh9lDj7JJ0   -  maps google android
    //AIzaSyDzt_zSeQ_rK0TR2ClYHraBm7Yrg83JhDU - ios

    class MainPageViewModel : BindableBase, INavigatedAware
    {
 
        private readonly INavigationService _navigationService;
        private readonly ISettingsManager _settingsManager;


        public MainPageViewModel(INavigationService navigationService, 
                                 ISettingsManager settingsManager)
        {

            _navigationService = navigationService;
            _settingsManager = settingsManager;

            LogInBtn = new DelegateCommand(LogininClick);
            RegistrBtn = new DelegateCommand(RegistrClick);           
        }


        #region Public propertys

        public DelegateCommand LogInBtn { get; }
        public DelegateCommand RegistrBtn { get; }

        #endregion


        #region Private helpers

        private async void RegistrClick()
        {
            if (CheckInternetConect())
            {
                await _navigationService.NavigateAsync("/RegistrView", animated: true);
            }
        }

        private async void LogininClick()
        {
            if (CheckInternetConect())
            {
                await _navigationService.NavigateAsync("/LogInView", animated: true);
            }
        }

        private async void GoToMapAsync(string email)
        {
            App.Language = (_settingsManager.Language != null) ? _settingsManager.Language : "en";
            LocalizationResourceManager.Current.CurrentCulture = new CultureInfo(App.Language);

            await Task.Delay(100);

            if (CheckInternetConect())
            {
                NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", email }
                                };
                await _navigationService.NavigateAsync("/TabbedPageMy", navParameters, animated: true);
            }
        }

        private bool CheckInternetConect()
        {
            if (!CheckingDeviceProperty_Helper.CheckNetwork())
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Device_Internet, "Error Internet", "Ok");
                return false;
            }
            return true;
        }

        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (_settingsManager.Email != null)
            {
                GoToMapAsync(_settingsManager.Email);
            }
        }
        #endregion
    }
}
