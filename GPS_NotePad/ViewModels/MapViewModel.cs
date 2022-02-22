﻿

using GPS_NotePad.Models;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services.MarkerService;
using GPS_NotePad.Services.SettingsManager;

using Acr.UserDialogs;

using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace GPS_NotePad.ViewModels
{
    class MapViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly IMarkerService _markerService;
        private readonly INavigationService _navigationService;
        private readonly ISettingsManager _settingsManager;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private Location _currentLocation;
        private List<MarkerInfo> _listMarkersClone;


        public MapViewModel(INavigationService navigationService, 
                                IMarkerService markerService, 
                                ISettingsManager settingsManager)
        {

            _verifyInput = new VerifyInput_Helper();
            _markerService = markerService;
            _navigationService = navigationService;
            _settingsManager = settingsManager;

            ListPin = new ObservableCollection<Pin>();
            _listMarkersClone = new List<MarkerInfo>();

            MarkerInfoVisible = false;
            IsVisible_SearchList = false;
        }


        public event EventHandler IsActiveChanged;


        #region Public property


        private static string _email;
        public static string Email 
        { 
            get => _email; 
            set => _email = value; 
        }


        private MarkerInfo _markerInfoClick;
        public MarkerInfo MarkerInfoClick
        {
            get => _markerInfoClick;
            set => SetProperty(ref _markerInfoClick, value);
        }


        private Tuple<Position, double> _moveTo;
        public Tuple<Position, double> MoveTo 
        { 
            get => _moveTo; 
            set => SetProperty(ref _moveTo, value); 
        }


        private ObservableCollection<Pin> _listPin;
        public ObservableCollection<Pin> ListPin 
        {
            get => _listPin; 
            set => SetProperty(ref _listPin, value); 
        }


        private List<MarkerInfo> _listMarkers;
        public List<MarkerInfo> ListMarkers 
        { 
            get => _listMarkers; 
            set => SetProperty(ref _listMarkers, value);
        }


        private bool _isMarkerInfoVisible;
        public bool MarkerInfoVisible 
        { 
            get => _isMarkerInfoVisible; 
            set => SetProperty(ref _isMarkerInfoVisible, value); 
        }


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


        private bool _isActive;
        public bool IsActive 
        { 
            get => _isActive; 
            set => SetProperty(ref _isActive, value, IsActiveTabAsync); 
        }


        private bool _isVisible_SearchList;
        public bool IsVisible_SearchList 
        { 
            get => _isVisible_SearchList;  
            set => SetProperty(ref _isVisible_SearchList, value, IsActiveTabAsync);  
        }


        private string _search;
        public string Search
        {
            get => _search;
            set => SetProperty(ref _search, value);
        }

        
        public DelegateCommand MyLocationBtn => new DelegateCommand(MyLocation_Click);
        public DelegateCommand UnfocusedCommand => new DelegateCommand(SearchUnfocus);
        public DelegateCommand CloseMarkerInfo => new DelegateCommand(Close_MarkerInfo);
        public DelegateCommand<MarkerInfo> Click_SearchListItem => new DelegateCommand<MarkerInfo>(SearchListItem_Click);
        public DelegateCommand SearchBtn_Pressed => new DelegateCommand(SearchBtnPressed);
        public DelegateCommand ExitBtn => new DelegateCommand(LogOutAsync);
        public DelegateCommand SettingsBtn => new DelegateCommand(Settings_ClickAsync);

        #endregion


        #region Private helpers

        private void CheckingSearch()
        {
            if (_search.Length > 0)
            {
                string temp = _search;
                if (!_verifyInput.NameVerify(ref temp))//Verify 
                {
                    Search = temp;
                    UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Search, "Error", "Ok");
                }
                else
                {
                    Search_Markers();
                }
            }
            else
            {
                OnTextChanged();
            }
        }

        private async void IsActiveTabAsync()
        {
            await Task.Delay(150);
            LoadListMarkersAsync();
        }

        private void MyLocation_Click()
        {
            MoveTo = new Tuple<Position, double>(new Position(0, 0), 50.0);
        }

        private void Close_MarkerInfo()
        {
            MarkerInfoVisible = false;
        }

        private async void LogOutAsync()
        {
            _settingsManager.Email = null;
            await _navigationService.NavigateAsync("/MainPage");
        }

        private async void Settings_ClickAsync()
        {
            Tuple<string, string> tuple = new Tuple<string, string>("TabbedPageMy", _email);

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "addressPage", tuple }
                                };
            await _navigationService.NavigateAsync("/SettingsView", navParameters);
        }

        private async void LoadListMarkersAsync()
        {
            var arr = await _markerService.GetDataAsync<MarkerInfo>("MarkerInfo", Email);
            ListPin.Clear();
            ToMyPins(arr);
        }

        private void ToMyPins(List<MarkerInfo> arr)
        {
            foreach (var item in arr)
            {
                if (item.LikeImage == Constants.Constant.Like_Image_Blue)
                {
                    ListPin.Add(new Pin
                    {
                        Type = PinType.Place,
                        Address = item.Address,
                        Label = item.Label,
                        Position = new Position(item.Latitude, item.Longitude),
                        Icon = BitmapDescriptorFactory.FromBundle("pin")
                    });
                }

                _listMarkersClone.Add(new MarkerInfo
                {
                    Address = item.Address,
                    Label = item.Label,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    ImagePath = item.ImagePath,
                    email = item.email
                });
            }
        }

        private void Obser_ToList()
        {
            ListMarkers = new List<MarkerInfo>();
            for (int i = 0; i < _listMarkersClone.Count; i++)
            {
                ListMarkers.Add(new MarkerInfo
                {
                    Address = _listMarkersClone[i].Address,
                    ImagePath = _listMarkersClone[i].ImagePath,
                    Label = _listMarkersClone[i].Label,
                    Latitude = _listMarkersClone[i].Latitude,
                    Longitude = _listMarkersClone[i].Longitude
                });
            }
        }

        private void MarkerClicked(Position pos, string ImagePath, string Label, string Address)
        {
            MarkerInfoVisible = true;

            foreach (var item in _listMarkersClone)
            {
                if (item.Address == Address)
                    ImagePath = item.ImagePath;
            }

            MarkerImage = ImagePath;
            MarkerLabel = Label;
            MarkerAddress = Address;
            MarkerPosition = pos.Latitude + ",  " + pos.Longitude;
        }


        //Search
        private void SearchUnfocus()
        {
            SearchList_Clear();
            Search = "";
        }

        private void SearchBtnPressed()
        {
            if (ListMarkers != null && ListMarkers.Count > 0)
                SearchListItem_Click(ListMarkers[0]);
            SearchList_Clear();
        }

        private void OnTextChanged()
        {
            if (Search == null || Search.Length == 0)
                SearchList_Clear();
        }

        private void SearchListItem_Click(MarkerInfo val)
        {
            MarkerClicked(new Position(val.Latitude, val.Longitude), val.ImagePath, val.Label, val.Address);
            MoveTo = new Tuple<Position, double>(new Position(val.Latitude, val.Longitude), 50.0);
            SearchList_Clear();
        }

        private async void SearchList_Clear()
        {
            await Task.Delay(100);
            ListMarkers = new List<MarkerInfo>();
            ListMarkers.Clear();
            IsVisible_SearchList = false;
        }

        private void Search_Markers()
        {
            if (Search == null || Search.Length < 1)
            {
                ListMarkers = new List<MarkerInfo>();
                IsVisible_SearchList = false;
                return;
            }
            if (Search.Length > 0)
            {
                Obser_ToList();
                IsVisible_SearchList = true;
            }

            List<MarkerInfo> buf = new List<MarkerInfo>();
            string temp = _search.ToLower();

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
       


        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("item"))
            {
                var e = parameters.GetValue<MarkerInfo>("item");
                MarkerClicked(new Position(e.Latitude, e.Longitude), e.ImagePath, e.Label, e.Address);
                MoveTo = new Tuple<Position, double>(new Position(e.Latitude, e.Longitude), 50.0);
            }
            if (parameters.ContainsKey("email"))
            {
                var e = parameters.GetValue<string>("email");
                Email = e;
                MoveTo = new Tuple<Position, double>(new Position(0, 0), 1400.0);
            }

            LoadListMarkersAsync();
        }
        #endregion


        #region ---- Override ----

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case "Search":
                    CheckingSearch();
                    break;

                case "MarkerInfoClick":
                    MarkerClicked(new Position(MarkerInfoClick.Latitude, MarkerInfoClick.Longitude),
                                  MarkerInfoClick.ImagePath, MarkerInfoClick.Label, MarkerInfoClick.Address);
                    break;
                default:
                    break;
            }
        }

        #endregion


    }
}
