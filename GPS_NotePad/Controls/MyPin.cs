
using Xamarin.Forms.Maps;
using GPS_NotePad.ViewModels;
using Xamarin.Forms;
using System;

namespace GPS_NotePad.Controls
{
    class MyPin : Pin
    {

        public static MapViewModel TabbedPageMyViewModel { get; internal set; }
        public int Ids { get; set; }
        public string ImagePath { get=> (string)GetValue(ImagePathProperty); set=>SetValue(ImagePathProperty, value); }
        public MyPin():base()
        {
            MarkerClicked += (object sender, PinClickedEventArgs e) =>
            { e.HideInfoWindow = true; TabbedPageMyViewModel.MarkerClicked(Position, Ids, ImagePath, Label, Address); };
        }
     
        public static readonly BindableProperty ImagePathProperty =
    BindableProperty.Create("ImagePath", typeof(string), typeof(MyPin), defaultValue:null, BindingMode.TwoWay);

    }
}
