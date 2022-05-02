
using GPS_NotePad.Helpers;
using GPS_NotePad.Models;
using GPS_NotePad.Services.GoogleGetPlacesService;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms.GoogleMaps;


namespace GPS_NotePad.ViewModels
{
    class SearchRouteViewModel : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;
        private readonly IGoogleGetPlacesService _googleGetPlacesService;
        private string _originLatitud;
        private string _originLongitud;
        private string _destinationLatitud;
        private string _destinationLongitud;
        private bool _isPickupFocused;
        private string _whatTheRoute;
        private bool _isFullFields;
        private Position _location;


        public SearchRouteViewModel(INavigationService navigationService, 
                                    IGoogleGetPlacesService googleGetPlacesService)
        {
            _navigationService = navigationService;
            _googleGetPlacesService = googleGetPlacesService;

            _isPickupFocused = true;
            _isFullFields = false;

            RecentPlaces = new ObservableCollection<GooglePlaceAutoCompletePrediction>();
            Places = new ObservableCollection<GooglePlaceAutoCompletePrediction>();
        }


        #region Public property

        public bool HasRouteRunning { get; set; }
        public bool ShowRecentPlaces { get; set; }


        GooglePlaceAutoCompletePrediction _placeSelected;
        public GooglePlaceAutoCompletePrediction PlaceSelected
        {
            get => _placeSelected;
            set => SetProperty(ref _placeSelected, value);
        }


        private string _pickupText;
        public string PickupText
        {
            get => _pickupText;
            set => SetProperty(ref _pickupText, value);
        }


        private string _originText;
        public string OriginText
        {
            get => _originText;
            set => SetProperty(ref _originText, value);
        }


        public ObservableCollection<GooglePlaceAutoCompletePrediction> Places { get; set; }
        public ObservableCollection<GooglePlaceAutoCompletePrediction> RecentPlaces { get; set; }


        public DelegateCommand BackBtn => new DelegateCommand(BackClickAsync);
        public DelegateCommand MyLocation => new DelegateCommand(MyLocation_ClickAsync);
        public DelegateCommand Walk_Btn => new DelegateCommand(WalkClickAsync);
        public DelegateCommand BusTrain_Btn => new DelegateCommand(BusTrainClickAsync);
        public DelegateCommand Car_Btn => new DelegateCommand(CarClickAsync);
        public DelegateCommand FocusedCommand => new DelegateCommand(Focus_Origin);
        public DelegateCommand UnfocusedOrigin => new DelegateCommand(Unfocused_Origin);
        public DelegateCommand CloseOriginKeyboard => new DelegateCommand(CloseOriginKeyboard_Click);
        public DelegateCommand ClosePickupKeyboard => new DelegateCommand(ClosePickupKeyboard_Click);

        #endregion


        #region Private helpers

        private void CloseOriginKeyboard_Click()
        {
            Focus_Origin();
        }

        private void ClosePickupKeyboard_Click()
        {
            Focus_Origin();
        }

        private void Focus_Origin()
        {
            Places.Clear();// = new ObservableCollection<GooglePlaceAutoCompletePrediction>();
           // ShowRecentPlaces = false;

        }

        private void Unfocused_Origin()
        {
           // Focus_Origin();
        }



        private async void MyLocation_ClickAsync()
        {
            _originLatitud = _location.Latitude.ToString();
            _originLongitud = _location.Longitude.ToString();

            await GetLocationNameAsync(_location);
        }

        private async void WalkClickAsync()
        {
            _whatTheRoute = "walk";
            await LoadRouteAsync();
        }

        private async void BusTrainClickAsync()
        {
            _whatTheRoute = "busTrain";
            await LoadRouteAsync();
        }

        private async void CarClickAsync()
        {
            _whatTheRoute = "car";
            await LoadRouteAsync();
        }

        private async Task LoadRouteAsync()
        {
            if(!_isFullFields)
            {
                await App.Current.MainPage.DisplayAlert("Error", Resources.Resx.Resource.Alert_All_Field, "Ok");
                return;
            }

            CleanFields();
            Places.Clear();
            ShowRecentPlaces = false;
            GoogleDirection googleDirection = null;

            try
            {
                googleDirection = await _googleGetPlacesService.GetDirections(_whatTheRoute, _originLatitud, _originLongitud,
                                                                         _destinationLatitud, _destinationLongitud);
            }
            catch(Exception)
            {
                await App.Current.MainPage.DisplayAlert("Error", Resources.Resx.Resource.Alert_All_Field, "Ok");
                return;
            }

            if (googleDirection.Routes != null && googleDirection.Routes.Count > 0)
            {
                if (_whatTheRoute == "busTrain")
                {
                    NavigationParameters navParameter = new NavigationParameters
                                {
                                    { "Direction", googleDirection }
                                };
                    _ = await _navigationService.NavigateAsync("PublicTransportSelect", parameters: navParameter, 
                                                                useModalNavigation: true, animated: true);                   
                }
                else
                {
                    RouteToMapAsync(googleDirection);
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(":(", Resources.Resx.Resource.NoRouteFound, "Ok");
            }

        }

        private async void RouteToMapAsync(GoogleDirection googleDirection)
        {
            await App.Current.MainPage.DisplayAlert(Resources.Resx.Resource.SearchGooglePlaces,
                                                       googleDirection.Routes[0].Legs[0].Distance.Text + " = "
                                                       + googleDirection.Routes[0].Legs[0].Duration.Text, "Ok");

            List<Position> positions = Enumerable.ToList(PolylineHelper.Decode(googleDirection.Routes.First().OverviewPolyline.Points));

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "routeList", positions }
                                };
            _ = await _navigationService.GoBackAsync(parameters: navParameters, useModalNavigation: true, animated: true);
        }


        public async Task GetPlacesByNameAsync(string placeText)
        {
            Places.Clear();
            var places = await _googleGetPlacesService.GetPlaces(placeText);
            var placeResult = places.AutoCompletePlaces;

            if (placeResult != null && placeResult.Count > 0)
            {
                foreach (GooglePlaceAutoCompletePrediction item in placeResult)
                {
                    Places.Add(item);
                }
            }

            ShowRecentPlaces = placeResult == null || placeResult.Count == 0;
        }

        public async Task GetPlacesDetailAsync(GooglePlaceAutoCompletePrediction placeA)
        {

            var place = await _googleGetPlacesService.GetPlaceDetails(placeA.PlaceId);
            
            if (place != null)
            {
                if (_isPickupFocused)
                {
                    PickupText = place.Name;
                    _originLatitud = $"{place.Latitude}";
                    _originLongitud = $"{place.Longitude}";
                    _isPickupFocused = false;
                    //  FocusOriginCommand.Execute(null);
                }
                else
                {
                    OriginText = place.Name;
                    _destinationLatitud = $"{place.Latitude}";
                    _destinationLongitud = $"{place.Longitude}";

                    RecentPlaces.Add(placeA);

                    if (_originLatitud == _destinationLatitud && _originLongitud == _destinationLongitud)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", Resources.Resx.Resource.ErrorRoute, "Ok");
                    }
                    else
                    {
                        _isFullFields = true;
                    }

                }
            }
        }

        void CleanFields()
        {
            PickupText = OriginText = string.Empty;
            ShowRecentPlaces = true;
            PlaceSelected = null;
        }

        
        //Get place 
        private async Task GetLocationNameAsync(Position position)
        {
            try
            {
                IEnumerable<Placemark> placemarks = await Geocoding.GetPlacemarksAsync(position.Latitude, position.Longitude);
                Placemark placemark = placemarks?.FirstOrDefault();

                PickupText = placemark != null ? placemark.Locality : string.Empty;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("SearchRoute ERROR " + ex.ToString());
            }
        }


        private async void BackClickAsync()
        {
            await _navigationService.GoBackAsync(useModalNavigation: true, animated: true);
        }

        #endregion


        #region Interface implementation
        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Location"))
            {
                _location = parameters.GetValue<Position>("Location");              
            }

            if (parameters.ContainsKey("routeList"))
            {
               List<Position> positions = parameters.GetValue<List<Position>>("routeList");
           
            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "routeList", positions }
                                };
               _ = await _navigationService.GoBackAsync(parameters: navParameters, useModalNavigation: true, animated: true);
            }
        }

        #endregion


        #region ---- Override ----

        protected override async void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case "PlaceSelected":
                    if (_placeSelected != null)
                        await GetPlacesDetailAsync(PlaceSelected);
                    break;

                case "PickupText":
                    if (!string.IsNullOrEmpty(_pickupText))
                    {
                        _isPickupFocused = true;
                        await GetPlacesByNameAsync(PickupText);
                    }
                    break;
                case "OriginText":
                    if (!string.IsNullOrEmpty(_originText))
                    {
                        _isPickupFocused = false;
                        await GetPlacesByNameAsync(OriginText);
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion


    }
}
