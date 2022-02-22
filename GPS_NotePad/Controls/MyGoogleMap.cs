

using GPS_NotePad.Models;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using Map = Xamarin.Forms.GoogleMaps.Map;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;


namespace GPS_NotePad.Controls
{
    class MyGoogleMap : Map
    {

        private Location _currentLocation;


        public MyGoogleMap()
        {
            PinsSource = new ObservableCollection<Pin>();
            MyLocationEnabled = true;

            PinClicked += MyGoogleMap_PinClicked;
            MapClicked += MyGoogleMap_MapClicked;
            SizeChanged += MyGoogleMap_SizeChanged;
        }


        #region Public property

        public static readonly BindableProperty PinsSourceProperty =
            BindableProperty.Create(propertyName: nameof(PinsSource),
                                 returnType: typeof(ObservableCollection<Pin>),
                                 declaringType: typeof(MyGoogleMap),
                                  defaultBindingMode: BindingMode.TwoWay,
                                  propertyChanged: PinsSourcePropertyChanged);

        public ObservableCollection<Pin> PinsSource
        {
            get { return (ObservableCollection<Pin>)GetValue(PinsSourceProperty); }
            set { SetValue(PinsSourceProperty, value); }
        }

        public static BindableProperty MoveToProperty =
                           BindableProperty.Create(nameof(MoveTo),
                             returnType: typeof(Tuple<Position, double>),
                             declaringType: typeof(MyGoogleMap),
                             defaultValue: null,
                             defaultBindingMode: BindingMode.TwoWay,
                             propertyChanged: MoveTo_FromViewModel);

        public Tuple<Position, double> MoveTo
        {
            get { return (Tuple<Position, double>)GetValue(MoveToProperty); }
            set { SetValue(MoveToProperty, value); }
        }

        public static BindableProperty MarkerInfoClickProperty =
                BindableProperty.Create(nameof(MarkerInfoClick),
                         returnType: typeof(MarkerInfo),
                         declaringType: typeof(MyGoogleMap),
                         defaultValue: null,
                         defaultBindingMode: BindingMode.TwoWay);

        public MarkerInfo MarkerInfoClick
        {
            get { return (MarkerInfo)GetValue(MarkerInfoClickProperty); }
            set { SetValue(MarkerInfoClickProperty, value); }
        }

        public static BindableProperty MapClicPositionProperty =
                BindableProperty.Create(nameof(MapClicPosition),
                    returnType: typeof(Position),
                    declaringType: typeof(MyGoogleMap),
                    defaultValue: null,
                    defaultBindingMode: BindingMode.TwoWay);

        public Position MapClicPosition
        {
            get { return (Position)GetValue(MapClicPositionProperty); }
            set { SetValue(MapClicPositionProperty, value); }
        }

        #endregion


        #region Private static method

        private static void PinsSourcePropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            if (bindable is MyGoogleMap map && newValue is ObservableCollection<Pin> pin)
            {
                UpdatePinsSource(map, pin);
            }
        }

        private static void UpdatePinsSource(MyGoogleMap bindableMap, ObservableCollection<Pin> newSource)
        {
            bindableMap.Pins.Clear();

            foreach (var pin in newSource)
                bindableMap.Pins.Add(pin);
        }

        private static void MoveTo_FromViewModel(BindableObject bindable, object oldValue, object newValue)
        {

            if (bindable is MyGoogleMap map && newValue is Tuple<Position, double> marker)
            {
                map.Move(marker.Item1.Latitude, marker.Item1.Longitude, marker.Item2);
            }
        }


        #endregion


        #region Private method

        private void MyGoogleMap_PinClicked(object sender, PinClickedEventArgs e)
        {
            e.Handled = true;
            MarkerInfoClick = new MarkerInfo
            {
                Latitude = e.Pin.Position.Latitude,
                Longitude = e.Pin.Position.Longitude,
                Address = e.Pin.Address,
                Label = e.Pin.Label
            };
            Move(e.Pin.Position.Latitude, e.Pin.Position.Longitude, 50);
        }

        private async void Move(double latitude = 0, double longitude = 0, double distance = 1400)
        {
            if (latitude == 0)
            {
                _currentLocation = await Geolocation.GetLocationAsync();
                latitude = _currentLocation.Latitude;
                longitude = _currentLocation.Longitude;
            }
            MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromMiles(distance)));
        }

        private void MyGoogleMap_SizeChanged(object sender, EventArgs e)
        {
            PinsSource.CollectionChanged += PinsSourceOnCollectionChanged;
        }

        private void MyGoogleMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            MapClicPosition = new Position(e.Point.Latitude, e.Point.Longitude);
        }

        private void PinsSourceOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (sender is ObservableCollection<Pin> newSource)
            {
                UpdatePinsSource(this, sender as ObservableCollection<Pin>);
            }
        }

        #endregion

    }
}
