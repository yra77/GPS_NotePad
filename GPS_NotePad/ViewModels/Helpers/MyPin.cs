using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;
using GPS_NotePad.ViewModels;

namespace GPS_NotePad.ViewModels.Helpers
{
    class MyPin : Pin
    {
        public int Ids { get; set; }
        public string ImagePath { get; set; }
        public static TabbedPage1ViewModel TabbedPageMyViewModel { get; internal set; }
        public MyPin()
        {
            MarkerClicked += (object sender, PinClickedEventArgs e) => 
            { e.HideInfoWindow = true; TabbedPageMyViewModel.MarkerClicked(Position, Ids, ImagePath, Label, Address); };
        }

    }
}
