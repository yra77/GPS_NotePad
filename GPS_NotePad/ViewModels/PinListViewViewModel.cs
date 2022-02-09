
using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.Helpers;
using GPS_NotePad.Controls;
using GPS_NotePad.Services;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace GPS_NotePad.ViewModels
{
    class PinListViewViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly ITo_RepositoryService _toRepository;
        private IMediaService _mediaService;
        private INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private List<MyPin> _listMarkers;
        private List<MyPin> _listMarkersClone;
        private MarkerInfo _markerInfo;
        private Location _currentLocation;
        private string _markerImage;
        private string _markerLabel;
        private string _markerAddress;
        private Position _position;
        private string _email;
        private bool _isActive;
        private bool _isModalVisible;
        private string _search;
        private Position _mapClicPosition;


        public PinListViewViewModel(INavigationService navigationService, IMediaService mediaService, ITo_RepositoryService toRepository)
        {

            _toRepository = toRepository;
            _mediaService = mediaService;
            _navigationService = navigationService;

            CloseModal = new DelegateCommand(CloseModalClick);
            SaveAddBtn = new DelegateCommand(SaveAddClickAsync);
            GaleryBtn = new DelegateCommand(GaleryClickAsync);
            CameraBtn = new DelegateCommand(CameraClickAsync);
            AddNewMarker = new DelegateCommand(AddNewMakerClick);
            ClickToItem = new DelegateCommand<MyPin>(ItemClickAsync);
            SearchBtn_Pressed = new DelegateCommand(SearchBtnPressed);
            UnfocusedCommand = new DelegateCommand(SearchUnfocus);

            IsModalVisible = false;

            _markerInfo = new MarkerInfo();
            _verifyInput = new VerifyInput_Helper();
        }


        public event EventHandler IsActiveChanged;

        #region Public property
        public DelegateCommand UnfocusedCommand { get; }
        public DelegateCommand<MyPin> ClickToItem { get; set; }
        public DelegateCommand AddNewMarker { get; }
        public DelegateCommand CloseModal { get; }
        public DelegateCommand SaveAddBtn { get; }
        public DelegateCommand GaleryBtn { get; }
        public DelegateCommand CameraBtn { get; }
        public DelegateCommand SearchBtn_Pressed { get; }


        public List<MyPin> ListMarkers { get => _listMarkers; set => SetProperty(ref _listMarkers, value); }
        public Position MapClicPosition
        {
            get { return _mapClicPosition; }
            set { SetProperty(ref _mapClicPosition, value); MapClicked(); }
        }
        public string Search { get => _search; 
                               set { 
                                     SetProperty(ref _search, value);
                if (_search.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.NameVerify(ref temp))//Verify 
                    {
                        Search = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                    else
                    {
                        OnTextChanged();
                        Search_List();
                    }
                }
            } 
        }
        public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value, IsActiveTab); } }
        public bool IsModalVisible { get => _isModalVisible; set { SetProperty(ref _isModalVisible, value); } }
        public string Label { get => _markerLabel; 
                              set { SetProperty(ref _markerLabel, value);
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
        public string Address { get => _markerAddress; 
                                set { SetProperty(ref _markerAddress, value);
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
        public string ImagePath { get => _markerImage; set { SetProperty(ref _markerImage, value); } }
        #endregion

        #region Private method
        private void MapClicked()
        {
            _position = new Position(MapClicPosition.Latitude, MapClicPosition.Longitude);
        }
        private void AddNewMakerClick()
        {
            IsModalVisible = true;
            Label = "";
            Address = "";
            ImagePath = "";
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

                _listMarkers.Add(new MyPin
                {
                    Ids = _listMarkersClone.Count,
                    ImagePath = this.ImagePath == null ? "paris.jpg" : ImagePath,
                    Label = this.Label,
                    Address = this.Address,
                    Position = new Position(_position.Latitude, _position.Longitude)
                });              
                
                ToMarkerInfo();

                if (await _toRepository.Insert(_markerInfo))
                {
                    IsModalVisible = false;
                    ListPinAsync();
                }
                else
                    UserDialogs.Instance.Alert("Error saved data", "Error", "Ok");
            }
            else
                UserDialogs.Instance.Alert("All fields must be filled", "Error", "Ok");
        }

        void ToMarkerInfo()
        {
            _markerInfo.email = _email;
            _markerInfo.Address = Address;
            _markerInfo.Label = Label;
            _markerInfo.ImagePath = this.ImagePath == null ? "paris.jpg" : ImagePath;
            _markerInfo.Latitude = _position.Latitude;
            _markerInfo.Longitude = _position.Longitude;
        }

        private async void ItemClickAsync(MyPin item)
        {
            NavigationParameters navParameters = new NavigationParameters
            {
              { "item", item }
            };
           await _navigationService.NavigateAsync("/TabbedPageMy?selectedTab=Tabbed_Page1", navParameters, animated: true);
            // await navigationService.NavigateAsync("/TabbedPageMy?selectedTab=Tabbed_Page1");
          
        }

        private void CloseModalClick()
        {
            IsModalVisible = false;
        }

        private void IsActiveTab()
        {
            _email = MapViewModel.Email;
            Label = "";
            Address = "";
            ImagePath = "";
            _position = new Position(0,0);
            ListPinAsync();
        }

        void RefreshPins()
        {
            ListMarkers = new List<MyPin>(_listMarkersClone);
        }

        async void ListPinAsync()
        {
            var arr = await _toRepository.GetData<MarkerInfo>("MarkerInfo", _email);

            _listMarkersClone = new List<MyPin>();
            ListMarkers = ToMyPins(arr);
            _listMarkersClone = new List<MyPin>(ListMarkers);
        }

        List<MyPin> ToMyPins(List<MarkerInfo> arr)
        {
            List<MyPin> temp = new List<MyPin>(50);
            foreach(var item in arr)
            {
                temp.Add(new MyPin {Ids = item.Id, Address = item.Address, Label = item.Label, 
                                     Position = new Position(item.Latitude, item.Longitude), ImagePath = item.ImagePath });
            }

            return temp;
        }

        #region Search    
        private void SearchUnfocus()
        {
            SearchList_Clear();
        }

        private void OnTextChanged()
        {
            if (ListMarkers == null || Search == null || Search.Length == 0)
                SearchList_Clear();
        }
        private async void SearchBtnPressed()
        {
            if (ListMarkers != null && ListMarkers.Count > 0)
            {
                var a = ListMarkers[0];
                await Task.Delay(100);//не убирать
                ItemClickAsync(ListMarkers[0]);
            }
        }
       
        async void SearchList_Clear()
        {
            await Task.Delay(100);//не убирать
            ListMarkers.Clear();
            RefreshPins();
            Search = "";
        }

        void Search_List()
        {
            if (Search == null || Search.Length < 1)
            {
                ListMarkers.Clear();
                RefreshPins();
                return;
            }

            string temp = Search.ToLower();
            List<MyPin> buf = new List<MyPin>();
            int m = Search.Length < 2 ? 0 : Search.Length - 1;

            for (int i = 0; i < ListMarkers.Count; i++)
            {
                string s = ListMarkers[i].Label.ToLower();
                string ss = ListMarkers[i].Address.ToLower();

                for (int j = m; j < temp.Length; j++)
                {

                    if (s != null && temp.Length <= s.Length && s[j] == temp[j])
                    {
                        bool isThat = false;
                        foreach (var item in buf)
                        {
                            if (item.Label == ListMarkers[i].Label)
                            {
                                isThat = true;
                                break;
                            }
                        }
                        if (!isThat)
                            buf.Add(ListMarkers[i]);
                    }
                    if (ss != null && temp.Length <= ss.Length && ss[j] == temp[j])
                    {
                        bool isThat = false;
                        foreach (var item in buf)
                        {
                            if (item.Address == ListMarkers[i].Address)
                            {
                                isThat = true;
                                break;
                            }
                        }
                        if (!isThat)
                            buf.Add(ListMarkers[i]);
                    }
                }
            }

            if (buf.Count > 0)
            {
                ListMarkers.Clear();
                ListMarkers = buf;
            }
            else
            {
                string a = Search.Remove(_search.Length - 1, 1);
                Search = a;
            }
        }

        #endregion
        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters){ }

        public void OnNavigatedTo(INavigationParameters parameters) { ListPinAsync(); }
        #endregion
    }
}
