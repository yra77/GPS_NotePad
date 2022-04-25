
using GPS_NotePad.Models;
using GPS_NotePad.Services.VerifyService;
using GPS_NotePad.Services.MarkerService;
using GPS_NotePad.Services.MediaService;

using Acr.UserDialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Xamarin.Forms.GoogleMaps;

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace GPS_NotePad.ViewModels
{
    class AddPinViewModel : BindableBase, INavigatedAware
    {


        private readonly IMarkerService _markerService;
        private readonly IMediaService _mediaService;
        private readonly INavigationService _navigationService;
        private readonly IVerifyInputService _verifyInput;
        private MarkerInfo _markerInfo;
        private Position _position;
        private string _email;
        private bool _isEdit;


        public AddPinViewModel(INavigationService navigationService,
                                    IMediaService mediaService,
                                    IMarkerService markerService,
                                    IVerifyInputService verifyInputService)
        {
            _markerService = markerService;
            _mediaService = mediaService;
            _navigationService = navigationService;
            _verifyInput = verifyInputService;

            ListPin = new ObservableCollection<Pin>();
            ListImage = new ObservableCollection<MarkerInfo>();

            LatitudeBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;
            LongitudeBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;
            AddressBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;
            LabelBorderColor = Constants.Constant.BORDER_COLOR_ADDPIN;

            IsVisibleFotoList = false;
        }



        #region Public property


        private ObservableCollection<MarkerInfo> _listImage;
        public ObservableCollection<MarkerInfo> ListImage
        {
            get => _listImage;
            set => SetProperty(ref _listImage, value);
        }


        private ObservableCollection<Pin> _listPin;
        public ObservableCollection<Pin> ListPin
        {
            get => _listPin;
            set => SetProperty(ref _listPin, value);
        }


        private string _labelBorderColor;
        public string LabelBorderColor
        {
            get => _labelBorderColor;
            set => SetProperty(ref _labelBorderColor, value);
        }


        private string _addressBorderColor;
        public string AddressBorderColor
        {
            get => _addressBorderColor;
            set => SetProperty(ref _addressBorderColor, value);
        }


        private string _longitudeBorderColor;
        public string LongitudeBorderColor
        {
            get => _longitudeBorderColor;
            set => SetProperty(ref _longitudeBorderColor, value);
        }


        private string _latitudeBorderColor;
        public string LatitudeBorderColor
        {
            get => _latitudeBorderColor;
            set => SetProperty(ref _latitudeBorderColor, value);
        }


        private string _markerImage;
        public string ImagePath
        {
            get => _markerImage;
            set => SetProperty(ref _markerImage, value);
        }


        private Tuple<Position, double> _moveTo;
        public Tuple<Position, double> MoveTo
        {
            get => _moveTo;
            set => SetProperty(ref _moveTo, value);
        }


        private string _longitude;
        public string Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }


        private string _latitude;
        public string Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }


        private Position _mapClicPosition;
        public Position MapClicPosition
        {
            get => _mapClicPosition;
            set => SetProperty(ref _mapClicPosition, value);
        }


        private string _markerLabel;
        public string Label
        {
            get => _markerLabel;
            set => SetProperty(ref _markerLabel, value);
        }


        private string _markerAddress;
        public string Address
        {
            get => _markerAddress;
            set => SetProperty(ref _markerAddress, value);
        }

        private bool _isVisibleFotoList;
        public bool IsVisibleFotoList { get => _isVisibleFotoList; set => SetProperty(ref _isVisibleFotoList, value); }


        public DelegateCommand MyLocationBtn => new DelegateCommand(MyLocation_Click);
        public DelegateCommand BackBtn => new DelegateCommand(BackClickAsync);
        public DelegateCommand SaveAddBtn => new DelegateCommand(SaveAddClickAsync);
        public DelegateCommand GaleryBtn => new DelegateCommand(GaleryClickAsync);
        public DelegateCommand CameraBtn => new DelegateCommand(CameraClickAsync);
        public DelegateCommand<object> DeleteImageBtn => new DelegateCommand<object>(Click_DeleteImage);

        #endregion



        #region Private helpers

        private void Click_DeleteImage(object item)
        {
            string str = (string)item;

            for (int i = 0; i < ListImage.Count; i++)
            {
                if (ListImage[i].ImagePath == str)
                {
                    _ = ListImage.Remove(ListImage[i]);
                }
            }

            if (ListImage.Count == 0)
            {
                IsVisibleFotoList = false;
            }
        }

        private void CheckedLabel()
        {
            if (_markerLabel.Length > 0)
            {
                string temp = _markerLabel;

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

        private void CheckedAddress()
        {
            if (_markerAddress.Length > 0)
            {
                string temp = _markerAddress;

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

        private void MyLocation_Click()
        {
            MoveTo = new Tuple<Position, double>(new Position(0, 0), 50.0);
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

            if (camera != null)
            {
                ListImage.Add(new MarkerInfo { ImagePath = camera });
                IsVisibleFotoList = true;
            }
        }

        private async void GaleryClickAsync()
        {
            string galery = await _mediaService.OpenGalery();

            if (galery != null)
            {
                ListImage.Add(new MarkerInfo { ImagePath = galery });
                IsVisibleFotoList = true;
            }
        }

        private async void SaveAddClickAsync()
        {
            if (Label != null && Label.Length > 2 && Address != null
                && Address.Length > 2 && Longitude?.Length > 0 && Latitude?.Length > 0)
            {

                if (ListImage.Count > 0)
                {
                    ImagePath = "";

                    foreach (MarkerInfo item in ListImage)
                    {
                        ImagePath += item.ImagePath;
                        ImagePath += " ";//записываем все пути фото через пробел
                    }
                }

                _markerInfo = new MarkerInfo
                {
                    Id = _isEdit ? _markerInfo.Id : 0,
                    email = _email,
                    ImagePath = ImagePath,
                    Label = Label,
                    Address = Address,
                    Latitude = _position.Latitude,
                    Longitude = _position.Longitude,
                    LikeImage = Constants.Constant.Like_Image_Blue
                };
                
                bool result = _isEdit ? await _markerService.UpdateAsync(_markerInfo) : await _markerService.InsertAsync(_markerInfo);

                if (result)
                {
                    IsVisibleFotoList = false;
                    BackClickAsync();
                }
                else
                {
                    _ = UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_SavePin, "Error", "Ok");
                }
            }
            else
            {
                _ = UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_All_Field, "Error", "Ok");
            }
        }

        private void EditItem()
        {

            Latitude = _markerInfo.Latitude.ToString();
            Longitude = _markerInfo.Longitude.ToString();
            Address = _markerInfo.Address;
            Label = _markerInfo.Label;

            if (_markerInfo.ImagePath != null && _markerInfo.ImagePath.Length > 0)
            {
                ImagePath = _markerInfo.ImagePath.Trim();

                string[] strList = ImagePath.Split(' ').ToArray();
                IsVisibleFotoList = true;

                for (int i = 0; i < strList.Length; i++)
                {
                    ListImage.Add(new MarkerInfo() { ImagePath = strList[i] });
                }
            }

            MoveTo_Position();
        }

        private void MoveTo_Position()
        {
            _ = double.TryParse(Latitude, out double latitude);
            _ = double.TryParse(Longitude, out double longitude);

            _position = new Position(latitude, longitude);
            ListPin.Clear();

            ListPin.Add(new Pin
            {
                Type = PinType.Place,
                Address = " ",
                Label = " ",
                Position = new Position(latitude, longitude),
                Icon = BitmapDescriptorFactory.FromBundle("pin")
            });

            MoveTo = new Tuple<Position, double>(new Position(latitude, longitude), 50.0);
        }

        private async void BackClickAsync()
        {
            _ = await _navigationService.NavigateAsync("/TabbedPageMy?selectedTab=PinListView");
        }

        #endregion


        #region Interface InavigatedAword implementation

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            _email = MapViewModel.Email;
            Label = "";
            Address = "";

            if (parameters.ContainsKey("itemEdit"))
            {
                _markerInfo = parameters.GetValue<MarkerInfo>("itemEdit");
                _isEdit = true;
                EditItem();
            }
            else
            {
                MoveTo = new Tuple<Position, double>(new Position(0, 0), 1400.0);
            }
        }

        #endregion


        #region ---- Override ----

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case "Longitude":
                    LongitudeCheck(_longitude);
                    break;
                case "Latitude":
                    LatitudeCheck(_latitude);
                    break;
                case "MapClicPosition":
                    MapClicked();
                    break;
                case "Label":
                    CheckedLabel();
                    break;
                case "Address":
                    CheckedAddress();
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}
