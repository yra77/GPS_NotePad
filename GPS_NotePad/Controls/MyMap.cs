
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace GPS_NotePad.Controls
{
    class MyMap : Map
    {

        Location currentLocation;


        public MyMap()
        {
            this.SizeChanged += MyMap_Focused;
            MapClicked += MyMap_MapClicked;
           // Move();
        }


        public List<MyPin> My_Pins { get; set; }
        public MyPin SelectedItem { get { return (MyPin)GetValue(SelectedItemProperty); } set { SetValue(SelectedItemProperty, value); } }
        public Position MapClicPosition { get { return (Position)GetValue(MapClicPositionProperty); } set { SetValue(MapClicPositionProperty, value); } }

        public static BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(MyPin),
        typeof(MyMap), null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        public static BindableProperty MapClicPositionProperty = BindableProperty.Create("MapClicPosition", typeof(Position),
         typeof(MyMap), null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelected);

        private static void OnSelected(BindableObject bindable, object oldValue, object newValue)
        {
            var a = (MyMap)bindable; newValue = new Position(0,0);
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var b = (MyMap)bindable;
            var a = (MyPin)newValue;
            if(a.Address == " ")
                b.Move(a.Position.Latitude, a.Position.Longitude, 1400);
            else
            b.Move(a.Position.Latitude, a.Position.Longitude, 50);
        }


        private void MyMap_Focused(object sender, EventArgs e)
        {
          
        }

        private void MyMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            MapClicPosition = new Position(e.Position.Latitude, e.Position.Longitude);
            Move(e.Position.Latitude, e.Position.Longitude, 50);
        }

        private async void Move(double latitude = 0, double longitude = 0, double distance = 1400)
        {
            if (latitude == 0)
            {
                currentLocation = await Geolocation.GetLocationAsync();
                latitude = currentLocation.Latitude;
                longitude = currentLocation.Longitude;
            }
            MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitude, longitude), Distance.FromMiles(distance)));
        }


    }
}
