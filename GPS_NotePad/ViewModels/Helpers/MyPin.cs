using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;
using static GPS_NotePad.ViewModels.TabbedPageMyViewModel;

namespace GPS_NotePad.ViewModels.Helpers
{
    public interface IPin { }
    class MyPin : Pin,IPin
    {
        public int Ids { get; set; }
        public string ImagePath { get; set; }
        public static TabbedPageMyViewModel TabbedPageMyViewModel { get; internal set; }
        public MyPin()
        {
            MarkerClicked += (object sender, PinClickedEventArgs e) => 
            { e.HideInfoWindow = true; TabbedPageMyViewModel.MarkerClicked(Position, Ids, ImagePath, Label, Address); };
        }

    }
}
