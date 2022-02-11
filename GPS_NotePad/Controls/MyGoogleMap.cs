

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
            this.SizeChanged += MyGoogleMap_SizeChanged;
        }
        

        public static readonly BindableProperty PinsSourceProperty = BindableProperty.Create(propertyName: "PinsSource",
                                 returnType: typeof(ObservableCollection<Pin>),declaringType: typeof(MyGoogleMap),
                                  defaultBindingMode: BindingMode.TwoWay,propertyChanged: PinsSourcePropertyChanged);

        public static BindableProperty MoveToProperty = BindableProperty.Create("MoveTo", typeof(MarkerInfo),
                   typeof(MyGoogleMap), null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: MoveTo_FromViewModel);

        public static BindableProperty MarkerInfoClickProperty = BindableProperty.Create("MarkerInfoClick", typeof(MarkerInfo),
                   typeof(MyGoogleMap), null, defaultBindingMode: BindingMode.TwoWay);

        public static BindableProperty MapClicPositionProperty = BindableProperty.Create("MapClicPosition", typeof(Position),
                    typeof(MyGoogleMap), null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelected);


        #region Public property

        public MarkerInfo MoveTo { get { return (MarkerInfo)GetValue(MoveToProperty); } 
                                    set { SetValue(MoveToProperty, value); } }
        public MarkerInfo MarkerInfoClick { get { return (MarkerInfo)GetValue(MarkerInfoClickProperty); }
                                   set { SetValue(MarkerInfoClickProperty, value); } }
        public Position MapClicPosition { get { return (Position)GetValue(MapClicPositionProperty); } 
                                          set { SetValue(MapClicPositionProperty, value); } }
        public ObservableCollection<Pin> PinsSource{get { return (ObservableCollection<Pin>)GetValue(PinsSourceProperty); }
                                                    set { SetValue(PinsSourceProperty, value); } }

        #endregion

        #region Private static method

        private static void PinsSourcePropertyChanged(BindableObject bindable, object oldvalue, object newValue)
        {
            var thisInstance = (MyGoogleMap)bindable;
            var newPinsSource = (ObservableCollection<Pin>)newValue;

            if (thisInstance == null ||
                newPinsSource == null)
                return;

            UpdatePinsSource(thisInstance, newPinsSource);
        }

        private static void UpdatePinsSource(MyGoogleMap bindableMap, ObservableCollection<Pin> newSource)
        {
            bindableMap.Pins.Clear();

            foreach (var pin in newSource)
                bindableMap.Pins.Add(pin);
        }

        private static void MoveTo_FromViewModel(BindableObject bindable, object oldValue, object newValue)
        {
            var b = (MyGoogleMap)bindable;
            var a = (MarkerInfo)newValue;
            if (a.Address == " ")
                  b.Move(a.Latitude, a.Longitude, 1400);
            else
                  b.Move(a.Latitude, a.Longitude, distance:50);
        }

        private static void OnSelected(BindableObject bindable, object oldValue, object newValue)
        {
            var a = (MyGoogleMap)bindable; 
            newValue = new Position(0, 0);
        }

        #endregion

        #region Private method

        private void MyGoogleMap_PinClicked(object sender, PinClickedEventArgs e)
        {
            e.Handled = true;
            MarkerInfoClick = new MarkerInfo { Latitude = e.Pin.Position.Latitude, Longitude = e.Pin.Position.Longitude, 
                                               Address = e.Pin.Address, Label = e.Pin.Label };
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
            ObservableCollection<Pin> newSource = (ObservableCollection<Pin>)sender;
            
            UpdatePinsSource(this, sender as ObservableCollection<Pin>);
        }

        #endregion

    }
}
