
using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;

namespace GPS_NotePad.ViewModels
{
    class AddPinViewModel : BindableBase, INavigatedAware
    {

        #region Private helpers

        private readonly IMarkerService _markerService;
        private readonly IMediaService _mediaService;
        private readonly INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private Location _currentLocation;
        private MarkerInfo _moveTo;
        private MarkerInfo _markerInfo;
        private Position _mapClicPosition;
        private Position _position;
        private string _markerImage;
        private string _markerLabel;
        private string _markerAddress;
        private string _email;

        #endregion

        public AddPinViewModel(INavigationService navigationService, IMediaService mediaService, IMarkerService markerService)
        {
            _markerService = markerService;
            _mediaService = mediaService;
            _navigationService = navigationService;
            _verifyInput = new VerifyInput_Helper();

            BackBtn = new DelegateCommand(BackClickAsync);
            SaveAddBtn = new DelegateCommand(SaveAddClickAsync);
            GaleryBtn = new DelegateCommand(GaleryClickAsync);
            CameraBtn = new DelegateCommand(CameraClickAsync);
            MyLocationBtn = new DelegateCommand(MyLocation_Click);

        }

        #region Public property

        public DelegateCommand MyLocationBtn { get; }
        public DelegateCommand BackBtn { get; }
        public DelegateCommand SaveAddBtn { get; }
        public DelegateCommand GaleryBtn { get; }
        public DelegateCommand CameraBtn { get; }

        public MarkerInfo MoveTo { get => _moveTo; set { SetProperty(ref _moveTo, value); } }
        public string ImagePath { get => _markerImage; set { SetProperty(ref _markerImage, value); } }
        public Position MapClicPosition
        {
            get { return _mapClicPosition; }
            set { SetProperty(ref _mapClicPosition, value); MapClicked(); }
        }

        public string Label
        {
            get => _markerLabel;
            set
            {
                SetProperty(ref _markerLabel, value);
                if (_markerLabel.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.NameVerify(ref temp))//Verify label
                    {
                        Label = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                }
            }
        }

        public string Address
        {
            get => _markerAddress;
            set
            {
                SetProperty(ref _markerAddress, value);
                if (_markerAddress.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.NameVerify(ref temp))//Verify Address
                    {
                        Address = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                }
            }
        }
        #endregion

        #region Private metod

        private void MyLocation_Click()
        {
            MoveTo = new MarkerInfo { Address = "ffffff", Latitude = 0, Longitude = 0, Label = " ", ImagePath = " " };
        }

        private void MapClicked()
        {
            _position = new Position(MapClicPosition.Latitude, MapClicPosition.Longitude);
            MoveTo = new MarkerInfo
            {
                Address = "ffffff",
                Latitude = MapClicPosition.Latitude,
                Longitude = MapClicPosition.Longitude,
                Label = " ",
                ImagePath = " "
            };
        }

        private async void CameraClickAsync()
        {
            string camera = await _mediaService.OpenCamera();
            ImagePath = camera;
        }

        private async void GaleryClickAsync()
        {
            string galery = await _mediaService.OpenGalery();
            ImagePath = galery;
        }

        private async void SaveAddClickAsync()
        {
            if (Label != null && Address != null && _position.Latitude > 0)
            {

                _markerInfo = new MarkerInfo
                {
                    email = _email,
                    ImagePath = this.ImagePath == null ? "paris.jpg" : ImagePath,
                    Label = this.Label,
                    Address = this.Address,
                    Latitude = _position.Latitude,
                    Longitude = _position.Longitude
                };

                if (await _markerService.Insert(_markerInfo))
                {
                    BackClickAsync();
                }
                else
                    UserDialogs.Instance.Alert("Error saved data", "Error", "Ok");
            }
            else
                UserDialogs.Instance.Alert("All fields must be filled", "Error", "Ok");
        }

        async void BackClickAsync()
        {
            await _navigationService.NavigateAsync("/TabbedPageMy?selectedTab=PinListView");
        }

        #endregion


        #region Interface InavigatedAword implementation

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            _email = MapViewModel.Email;
            Label = "";
            Address = "";
            ImagePath = "";
            _position = new Position(0, 0);

            _currentLocation = await Geolocation.GetLocationAsync();
            MoveTo = new MarkerInfo
            {
                Address = " ",
                Latitude = _currentLocation.Latitude,
                Longitude = _currentLocation.Longitude,
                Label = " ",
                ImagePath = " "
            };
        }

        #endregion
    }
}
