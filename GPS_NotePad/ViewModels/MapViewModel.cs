

using GPS_NotePad.Models;
using GPS_NotePad.Services.MarkerService;
using GPS_NotePad.Services.SettingsManager;
using GPS_NotePad.Services.VerifyService;
using GPS_NotePad.Services.Interfaces;
using GPS_NotePad.Helpers;

using Acr.UserDialogs;

using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms;
using Xamarin.Essentials;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Threading;
using System.Collections.Concurrent;


namespace GPS_NotePad.ViewModels
{
    class MapViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly ILocationConnectService _locationConnectService;
        private readonly IMarkerService _markerService;
        private readonly INavigationService _navigationService;
        private readonly ISettingsManager _settingsManager;
        private readonly IVerifyInputService _verifyInput;

        private List<MarkerInfo> _listMarkersClone;
        private Position _locatePositions;
        private bool _isRoute;
        private object _lock;
        private ConcurrentQueue<Position> _positionsQueue;


        public MapViewModel(INavigationService navigationService,
                                IMarkerService markerService,
                                ISettingsManager settingsManager,
                              IVerifyInputService verifyInputService,
                              ILocationConnectService locationConnectService)
        {

            _locationConnectService = locationConnectService;
            _verifyInput = verifyInputService;
            _markerService = markerService;
            _navigationService = navigationService;
            _settingsManager = settingsManager;

            ListPin = new ObservableCollection<Pin>();
            _listMarkersClone = new List<MarkerInfo>();

            IsVisible_SearchList = false;
            IsStopRouteBtn_Visible = false;
            IsSearchRouteBtn_Visible = false;
            _isRoute = false;

            _positionsQueue = new ConcurrentQueue<Position>();
            _lock = new object();

        }


        public event EventHandler IsActiveChanged;//Interface IActiveAware implementation


        #region Public property


        private static string _email;
        public static string Email
        {
            get => _email;
            set => _email = value;
        }


        private bool _isSearchRouteBtn_Visible;
        public bool IsSearchRouteBtn_Visible
        {
            get => _isSearchRouteBtn_Visible;
            set => SetProperty(ref _isSearchRouteBtn_Visible, value);
        }


        private bool _isStopRouteBtn_Visible;
        public bool IsStopRouteBtn_Visible
        {
            get => _isStopRouteBtn_Visible;
            set => SetProperty(ref _isStopRouteBtn_Visible, value);
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


        public ICommand CalculateRouteCommand { get; set; }
        public ICommand UpdatePositionCommand { get; set; }

        public DelegateCommand MyLocationBtn => new DelegateCommand(MyLocation_Click);
        public DelegateCommand UnfocusedCommand => new DelegateCommand(SearchUnfocus);
        public DelegateCommand<MarkerInfo> Click_SearchListItem => new DelegateCommand<MarkerInfo>(SearchListItem_Click);
        public DelegateCommand SearchBtn_Pressed => new DelegateCommand(SearchBtnPressed);
        public DelegateCommand ExitBtn => new DelegateCommand(LogOutAsync);
        public DelegateCommand SettingsBtn => new DelegateCommand(Settings_ClickAsync);
        public DelegateCommand SearchRouteBtn => new DelegateCommand(SearchRouteClick);
        public DelegateCommand StopRouteBtn => new DelegateCommand(StopRouteBtn_Click);

        #endregion


        #region Private helpers

        private async void LocatePosition()
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium);
                Location position = await Geolocation.GetLocationAsync(request);
                _locatePositions = new Position(position.Latitude, position.Longitude);

                if (_locatePositions.Latitude > 0 && _locatePositions.Longitude > 0 && !IsStopRouteBtn_Visible)
                {
                    IsSearchRouteBtn_Visible = true;
                }
            }
            catch (Exception e)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("No GPS Connection", Resources.Resx.Resource.NoGPSConnection, "OK");
                   
                  //  if (!_locationConnectService.IsGpsAvailable())
                   // {
                        _locationConnectService.OpenSettings();
                 //   }
                    while (true)
                    {
                        if (_locationConnectService.IsGpsAvailable())
                        {
                            LocatePosition();
                            break;
                        }
                        await Task.Delay(1000);
                    }
                });
                Console.WriteLine("LocatePosition " + e.Message);
            }
        }

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
            MoveTo = new Tuple<Position, double>(new Position(_locatePositions.Latitude, _locatePositions.Longitude), 5.0f);
        }

        private void StopRouteBtn_Click()
        {
            IsSearchRouteBtn_Visible = true;
            IsStopRouteBtn_Visible = false;
            _isRoute = false;
            LoadListMarkersAsync();
        }

        private void SearchRouteClick()
        {
            _isRoute = true;
            IsSearchRouteBtn_Visible = false;
            IsStopRouteBtn_Visible = true;

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "Location", _locatePositions }
                                };
            _ = _navigationService.NavigateAsync("SearchRoute", navParameters, useModalNavigation: true, animated: true);
        }

        private async void LogOutAsync()
        {
            _settingsManager.Email = null;
            _ = await _navigationService.NavigateAsync("/MainPage");
        }

        private async void Settings_ClickAsync()
        {
            Tuple<string, string> tuple = new Tuple<string, string>("TabbedPageMy", _email);

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "addressPage", tuple }
                                };
            _ = await _navigationService.NavigateAsync("/SettingsView", navParameters);
        }

        private async void LoadListMarkersAsync()
        {
            List<MarkerInfo> arr = await _markerService.GetDataAsync<MarkerInfo>("MarkerInfo", Email);

            if (!_isRoute)
            {
                ListPin.Clear();
                ToMyPins(arr);
            }
        }

        private void ToMyPins(List<MarkerInfo> arr)
        {
            _listMarkersClone.Clear();
            ListPin.Clear();

            foreach (MarkerInfo item in arr)
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
            ListMarkers = new List<MarkerInfo>(_listMarkersClone);
        }

        private void MarkerClicked(Position pos, string ImagePath, string Label, string Address)
        {
            foreach (MarkerInfo item in _listMarkersClone)
            {
                if (item.Address == Address)
                {
                    ImagePath = item.ImagePath;
                }
            }

            MarkerInfo marker = new MarkerInfo
            {
                Address = Address,
                Label = Label,
                ImagePath = ImagePath,
                Latitude = pos.Latitude,
                Longitude = pos.Longitude
            };

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "modal", marker }
                                };
            _ = _navigationService.NavigateAsync("ModalPageView", navParameters, useModalNavigation: true, animated: true);
        }


        //Google trakcing
        private void UpdatePosition(object callback)
        {
            lock (_lock)
            {
                try
                {
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(2));
                    CancellationTokenSource cts = new CancellationTokenSource();
                    Location location = Geolocation.GetLocationAsync(request, cts.Token).Result;

                    if (location != null)
                    {
                        _locatePositions = new Position(location.Latitude, location.Longitude);
                        _positionsQueue.Enqueue(_locatePositions);

                        // Console.WriteLine(_locatePositions.Latitude + " " + _locatePositions.Longitude);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("UpdatePosition " + e.Message);
                    _isRoute = false;
                }
            }
        }

        private async void LoadRoute(List<Position> positions)
        {

            //check distance from gps point to line route
            DistanceToPoint distanceToPoint = new DistanceToPoint();
            int positionIndex = 1;

            ListPin.Clear();
            CalculateRouteCommand.Execute(positions);


            while (positions.Count > positionIndex && _isRoute)
            {

                if (!ThreadPool.QueueUserWorkItem(new WaitCallback(UpdatePosition)))
                {
                    break;
                }

                await Task.Delay(2000);

                if (!_positionsQueue.IsEmpty && _positionsQueue.TryDequeue(out _locatePositions))
                {
                    //check distance from point to line route               
                    int resDistance = (int)(distanceToPoint.DistanceFromLineSegmentToPoint
                                           (new Helpers.Vec2(positions[positionIndex].Latitude, positions[positionIndex].Longitude),
                                            new Helpers.Vec2(positions[positionIndex + 1].Latitude, positions[positionIndex + 1].Longitude),
                                            new Helpers.Vec2(_locatePositions.Latitude, _locatePositions.Longitude)) * 100000);

                    if (resDistance / 2 <= 20 || _locatePositions.Latitude == positions[positionIndex].Latitude
                                              || _locatePositions.Longitude == positions[positionIndex].Longitude)
                    {
                        UpdatePositionCommand.Execute(positions[positionIndex]);
                        positionIndex++;
                    }
                }
            };

            UpdatePositionCommand.Execute(new Position(0, 0));//sending 0,0 for stoping
            _isRoute = false;
            IsSearchRouteBtn_Visible = true;
            IsStopRouteBtn_Visible = false;
            LoadListMarkersAsync();

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
            {
                SearchListItem_Click(ListMarkers[0]);
            }

            SearchList_Clear();
        }

        private void OnTextChanged()
        {
            if (Search == null || Search.Length == 0)
            {
                SearchList_Clear();
            }
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

            List<MarkerInfo> res = ListMarkers.Where(item => item.Label.ToLower().Contains(Search.ToLower())
                                   || item.Address.ToLower().Contains(Search.ToLower())).ToList();

            if (res.Count > 0)
            {
                ListMarkers.Clear();
                ListMarkers = res;
            }
            else
            {
                ListMarkers.Clear();
            }
        }


        #endregion



        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {

            Thread workerThread = new Thread(new ThreadStart(LocatePosition));
            workerThread.Start();

            if (parameters.ContainsKey("item"))
            {
                MarkerInfo e = parameters.GetValue<MarkerInfo>("item");
                MarkerClicked(new Position(e.Latitude, e.Longitude), e.ImagePath, e.Label, e.Address);
                MoveTo = new Tuple<Position, double>(new Position(e.Latitude, e.Longitude), 50.0);
            }
            else if (parameters.ContainsKey("email"))
            {
                string e = parameters.GetValue<string>("email");
                Email = e;
                MoveTo = new Tuple<Position, double>(new Position(0, 0), 1400.0);
            }
            else if (parameters.ContainsKey("routeList"))
            {
                List<Position> posList = parameters.GetValue<List<Position>>("routeList");
                LoadRoute(posList);
            }
            else
            {
                IsSearchRouteBtn_Visible = true;
                IsStopRouteBtn_Visible = false;
                _isRoute = false;
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
