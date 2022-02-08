
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Map = Xamarin.Forms.Maps.Map;

namespace GPS_NotePad.Helpers
{
    class MyMap : Map
    {
        Location CurrentLocation;
        public List<MyPin> My_Pins { get; set; }
        public ObservableCollection<MyPin> ItemList { get => (ObservableCollection<MyPin>)GetValue(ItemListProperty);
                                                      set { SetValue(ItemListProperty, value); UpdateList(); } }

        public static readonly BindableProperty ItemListProperty =
                    BindableProperty.Create("ItemList", typeof(ObservableCollection<MyPin>),typeof(MyMap));
       
        
        
        
        public static BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(MyPin),
        typeof(MyMap), null, defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);
        public MyPin SelectedItem
        {
            get { return (MyPin)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        
        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var b = (MyMap)bindable;
            var a = (MyPin)newValue;
            b.Move(a.Position.Latitude, a.Position.Longitude, 50);
        }


        private void UpdateList()
        {
            foreach (var pin in ItemList)
            {
                Pins.Add(pin);
            }
        }

        public MyMap()
        {
            this.SizeChanged += MyMap_Focused;    
            MapClicked += MyMap_MapClicked;
            Move();
        }

        private void MyMap_Focused(object sender, EventArgs e)
        {
           // ItemList.CollectionChanged += ItemListChanged;
        }

        
        private void ItemListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateList();
        }

        private static void MarkerClick(object sender, PinClickedEventArgs e)
        {
            Console.WriteLine("DDDDDDDDDDDDDDDDD");
            var a = (MyPin)sender;
           // e.HideInfoWindow = true;
            //Move(a.Position.Latitude, a.Position.Longitude, 50);
        }

        private void MyMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            Move(e.Position.Latitude, e.Position.Longitude, 50);
        }

        private async void Move(double latitude = 0, double longitude = 0, double distance = 1400)
        {
            if (latitude == 0)
            {
                CurrentLocation = await Geolocation.GetLocationAsync();
                latitude = CurrentLocation.Latitude;
                longitude = CurrentLocation.Longitude;
            }
            MoveToRegion(MapSpan.FromCenterAndRadius(new
               Position(latitude, longitude),
               Distance.FromMiles(distance)));
        }


    }
}
