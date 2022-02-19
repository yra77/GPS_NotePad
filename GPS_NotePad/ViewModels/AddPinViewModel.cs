

using GPS_NotePad.Models;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services.Interfaces;

using Acr.UserDialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;

using System;


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
        private MarkerInfo _markerInfo;
        private Position _position;
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

            LatitudeBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;
            LongitudeBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;
            AddressBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;
            LabelBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;
        }



        #region Public property


        private string _labelBorderColor;
        public string LabelBorderColor { get => _labelBorderColor; set { SetProperty(ref _labelBorderColor, value); } }


        private string _addressBorderColor;
        public string AddressBorderColor { get => _addressBorderColor; set { SetProperty(ref _addressBorderColor, value); } }


        private string _longitudeBorderColor;
        public string LongitudeBorderColor { get => _longitudeBorderColor; set { SetProperty(ref _longitudeBorderColor, value); } }


        private string _latitudeBorderColor;
        public string LatitudeBorderColor { get => _latitudeBorderColor; set { SetProperty(ref _latitudeBorderColor, value); } }


        private string _markerImage;
        public string ImagePath { get => _markerImage; set { SetProperty(ref _markerImage, value); } }


        private MarkerInfo _moveTo;
        public MarkerInfo MoveTo { get => _moveTo; set { SetProperty(ref _moveTo, value); } }


        private string _longitude;
        public string Longitude 
        {
            get => _longitude; 
            set 
            { 
                SetProperty(ref _longitude, value);
                if (_longitude.Length > 0) LongitudeCheck(Longitude);
            } 
        }
    

        private string _latitude;
        public string Latitude 
        { 
            get => _latitude; 
            set 
            {
                SetProperty(ref _latitude, value);
                if (_latitude.Length > 0) LatitudeCheck(Latitude);
            } 
        }
    

        private Position _mapClicPosition;
        public Position MapClicPosition
        {
            get { return _mapClicPosition; }
            set { SetProperty(ref _mapClicPosition, value); MapClicked(); }
        }


        private string _markerLabel;
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
                        LabelBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
                    }
                    else
                    {
                        LabelBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    }
                }
            }
        }


        private string _markerAddress;
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
                        AddressBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
                    }
                    else
                    {
                        AddressBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    }
                }
            }
        }

        public DelegateCommand MyLocationBtn { get; }
        public DelegateCommand BackBtn { get; }
        public DelegateCommand SaveAddBtn { get; }
        public DelegateCommand GaleryBtn { get; }
        public DelegateCommand CameraBtn { get; }

        #endregion



        #region Private metod

        private void MyLocation_Click()
        {
            MoveTo = new MarkerInfo { Address = "ffffff", Latitude = 0, Longitude = 0, Label = "", ImagePath = "" };
        }

        private void MapClicked()
        {
            _position = new Position(MapClicPosition.Latitude, MapClicPosition.Longitude);

            Latitude = MapClicPosition.Latitude.ToString();
            Longitude = MapClicPosition.Longitude.ToString();
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
            if (Label != null && Label.Length > 2 && Address != null
                && Address.Length > 2 && Longitude?.Length > 0 && Latitude?.Length > 0
                && ImagePath != null && ImagePath.Length > 2)
            {

                _markerInfo = new MarkerInfo
                {
                    email = _email,
                    ImagePath = ImagePath == null ? Constants.Constant.DEFAULT_IMAGE : ImagePath,
                    Label = Label,
                    Address = Address,
                    Latitude = _position.Latitude,
                    Longitude = _position.Longitude,
                    LikeImage = Constants.Constant.Like_Image_Blue
                };

                if (await _markerService.InsertAsync(_markerInfo))
                {
                    BackClickAsync();
                }
                else
                {
                    UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_SavePin, "Error", "Ok");
                }
            }
            else
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_All_Field, "Error", "Ok");
            }
        }

        private void LatitudeCheck(string position)
        {

            LatitudeBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;

            if (!_verifyInput.PositionVerify(ref position))
            {
                Latitude = position;
            }
            else
            {
                LatitudeBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                MoveTo_Position();
            }
        }

        private void LongitudeCheck(string position)
        {
            LongitudeBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;

            if (!_verifyInput.PositionVerify(ref position))
            {
                Longitude = position;
            }
            else
            {
                LongitudeBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                MoveTo_Position();
            }
        }

        private void MoveTo_Position()
        {
            Double.TryParse(Latitude, out double latitude); 
            Double.TryParse(Longitude, out double longitude);
            
            _position = new Position(latitude, longitude);

            MoveTo = new MarkerInfo { Address = "fff", Latitude = latitude, Longitude = longitude, Label = "", ImagePath = "" };
        }

        private async void BackClickAsync()
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

            _currentLocation = await Geolocation.GetLocationAsync();
            MoveTo = new MarkerInfo
            {
                Address = "",
                Latitude = _currentLocation.Latitude,
                Longitude = _currentLocation.Longitude,
                Label = "",
                ImagePath = ""
            };
        }

        #endregion
    }
}
