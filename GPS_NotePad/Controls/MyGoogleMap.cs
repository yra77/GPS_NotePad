

using GPS_NotePad.Models;
using GPS_NotePad.Views;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Essentials;
using Map = Xamarin.Forms.GoogleMaps.Map;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Android.App;

namespace GPS_NotePad.Controls
{
    class MyGoogleMap : Map
    {

        private Location _currentLocation;


        public MyGoogleMap()
        {
            PinsSource = new ObservableCollection<Pin>();
            MyLocationEnabled = true;

            //Assembly assembly = typeof(MyGoogleMap).GetTypeInfo().Assembly;
            //Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{"BlueStyleMap.json"}");
            //string json;
            //using (StreamReader r = new StreamReader(stream))
            //{
            //    json = r.ReadToEnd();
            //}
            //stream.Close();
            //Console.WriteLine(json);
            //MapStyle = MapStyle.FromJson(json.ToString());

            PinClicked += MyGoogleMap_PinClicked;
            MapClicked += MyGoogleMap_MapClicked;
            SizeChanged += MyGoogleMap_SizeChanged;
        }

        public event EventHandler OnCalculate = delegate { };


        #region Public property


        public static readonly BindableProperty CalculateCommandProperty =
            BindableProperty.Create(nameof(CalculateCommand), typeof(ICommand), typeof(MapView), null, BindingMode.TwoWay);
        public ICommand CalculateCommand
        {
            get { return (ICommand)GetValue(CalculateCommandProperty); }
            set { SetValue(CalculateCommandProperty, value); }
        }


        public static readonly BindableProperty UpdateCommandProperty =
          BindableProperty.Create(nameof(UpdateCommand), typeof(ICommand), typeof(MapView), null, BindingMode.TwoWay);
        public ICommand UpdateCommand
        {
            get { return (ICommand)GetValue(UpdateCommandProperty); }
            set { SetValue(UpdateCommandProperty, value); }
        }



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

        //name of location transfer to MapViewModel
        public static BindableProperty LocationNameProperty =
                BindableProperty.Create(nameof(LocationName),
                    returnType: typeof(Position),
                    declaringType: typeof(MyGoogleMap),
                    defaultValue: null,
                    defaultBindingMode: BindingMode.TwoWay);

        public Position LocationName
        {
            get { return (Position)GetValue(LocationNameProperty); }
            set { SetValue(LocationNameProperty, value); }
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
                try
                {
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High);
                    _currentLocation = await Geolocation.GetLocationAsync(request);
                    latitude = _currentLocation.Latitude;
                    longitude = _currentLocation.Longitude;
                    LocationName = new Position(latitude, longitude);
                }
                catch(Exception e)
                {
                    Console.WriteLine("Move myGoogleMap " + e.Message);
                   // await App.Current.MainPage.DisplayAlert("Error", "Enable GEO location", "Ok");
                    return;
                }
            }
          
            MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromMiles(distance)));
        }

        private void MyGoogleMap_SizeChanged(object sender, EventArgs e)
        {
            if (PinsSource != null)
            {
                PinsSource.CollectionChanged += PinsSourceOnCollectionChanged;
            }
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

        private async void Update(Position position)
        {
            if(position.Latitude == 0 && position.Longitude == 0)
            {
                Polylines.Clear();
                return;
            }
            if(Pins.Count == 1 && Polylines != null && Polylines?.Count > 1)
                return;

            Pin cPin = Pins.FirstOrDefault();

            if (cPin != null)
            {
                cPin.Position = new Position(position.Latitude, position.Longitude);
                //cPin.Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("taxi") : 
                //             BitmapDescriptorFactory.FromView(new Image() { Source = "taxi.png", WidthRequest = 25, HeightRequest = 25 });
              
                await MoveCamera(CameraUpdateFactory.NewPosition(new Position(position.Latitude, position.Longitude)));
                Position? previousPosition = Polylines?.FirstOrDefault()?.Positions?.FirstOrDefault();
                Polylines?.FirstOrDefault()?.Positions?.Remove(previousPosition.Value);
            }
            else
            {
                //END TRIP
                Polylines?.FirstOrDefault()?.Positions?.Clear();
                PinsSource.Clear();
            }
        }

        private void Calculate(List<Position> list)
        {
            OnCalculate?.Invoke(this, default(EventArgs));
            Polylines.Clear();
            var polyline = new Xamarin.Forms.GoogleMaps.Polyline();
            foreach (var p in list)
            {
                polyline.Positions.Add(p);
                polyline.StrokeWidth = 6;
            } 

            MoveToRegion(MapSpan.FromCenterAndRadius(new Position(polyline.Positions[0].Latitude, polyline.Positions[0].Longitude),
                                                        Distance.FromMiles(50f)));

            Polylines.Add(polyline);

            Pin pin = new Pin
            {
                Type = PinType.Place,
                Position = new Position(polyline.Positions.First().Latitude, polyline.Positions.First().Longitude),
                Label = "First",
                Address = "First",
                //Icon = (Device.RuntimePlatform == Device.Android) ? BitmapDescriptorFactory.FromBundle("taxi") : 
                //            BitmapDescriptorFactory.FromView(new Image() { Source = "taxi.png", WidthRequest = 25, HeightRequest = 25 })

            };

            PinsSource.Add(pin);

            Pin pin1 = new Pin
            {
                Type = PinType.Place,
                Position = new Position(polyline.Positions.Last().Latitude, polyline.Positions.Last().Longitude),
                Label = "Last",
                Address = "Last",
                Icon = BitmapDescriptorFactory.FromBundle("pin")
            };

            PinsSource.Add(pin1);

        }



        #endregion


        #region Override

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext != null)
            {
                CalculateCommand = new Command<List<Position>>(Calculate);
                UpdateCommand = new Command<Position>(Update);
            }
        }

        #endregion

    }
}
