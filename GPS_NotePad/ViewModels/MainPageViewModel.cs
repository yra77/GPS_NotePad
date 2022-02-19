
using GPS_NotePad.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;


namespace GPS_NotePad.ViewModels
{
    //486491599259-tco80iivdbvvj6ohmbhjb3tgn4ibfg5c.apps.googleusercontent.com//ios
    //486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk.apps.googleusercontent.com//android
    //AIzaSyDcSVIjErUsWelFNRTTqKJimfh9lDj7JJ0   -  maps google android
    //AIzaSyDzt_zSeQ_rK0TR2ClYHraBm7Yrg83JhDU - ios

    class MainPageViewModel : BindableBase, INavigatedAware
    {

        #region Private helpers

        private readonly INavigationService _navigationService;
        private readonly ICheckingDeviceProperty_Helper _checkingDeviceProperty;

        #endregion


        public MainPageViewModel(INavigationService navigationService)
        {

            _checkingDeviceProperty = new CheckingDeviceProperty_Helper();
            IsNetworkGeoLocalAsync();

            LogInBtn = new DelegateCommand(LogininClick);
            RegistrBtn = new DelegateCommand(RegistrClick);
           
            _navigationService = navigationService;
        }


        #region Public propertys

        public DelegateCommand LogInBtn { get; }
        public DelegateCommand RegistrBtn { get; }

        #endregion


        #region Private Methods

        private async void IsNetworkGeoLocalAsync() //check network and geo location
        {
            await _checkingDeviceProperty.CheckingDeviceProperty();
        }

        private async void RegistrClick()
        {
            await _navigationService.NavigateAsync("/RegistrView", animated: true);
        }

        private async void LogininClick()
        {
            await _navigationService.NavigateAsync("/LogInView", animated: true);
        }

        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters) { }
        #endregion
    }
}
