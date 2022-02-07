
using Xamarin.Forms.Maps;
using GPS_NotePad.ViewModels;

namespace GPS_NotePad.Helpers
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
