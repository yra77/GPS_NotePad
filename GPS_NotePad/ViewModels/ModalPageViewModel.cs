
using GPS_NotePad.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;


namespace GPS_NotePad.ViewModels
{
    class ModalPageViewModel : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;


        public ModalPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }


        #region private helper

        private string _markerImage;
        public string MarkerImage
        {
            get => _markerImage;
            set => SetProperty(ref _markerImage, value);
        }


        private string _markerLabel;
        public string MarkerLabel
        {
            get => _markerLabel;
            set => SetProperty(ref _markerLabel, value);
        }


        private string _markerAddress;
        public string MarkerAddress
        {
            get => _markerAddress;
            set => SetProperty(ref _markerAddress, value);
        }


        private string _markerPosition;
        public string MarkerPosition
        {
            get => _markerPosition;
            set => SetProperty(ref _markerPosition, value);
        }


        public DelegateCommand CloseMarkerInfo => new DelegateCommand(Close_MarkerInfo);

        #endregion



        private void Close_MarkerInfo()
        {
            _navigationService.GoBackAsync(useModalNavigation: true, animated: true);
        }


        #region --- Interface InavigatedAword implementation ---

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var e = parameters.GetValue<MarkerInfo>("modal");
            MarkerAddress = e.Address;
            MarkerLabel = e.Label;
            MarkerImage = e.ImagePath;
            MarkerPosition = e.Latitude + ",  " + e.Longitude;
        }

        #endregion
    }
}
