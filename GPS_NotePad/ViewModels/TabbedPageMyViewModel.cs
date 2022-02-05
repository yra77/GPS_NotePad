using Acr.UserDialogs;
using GPS_NotePad.ViewModels.Helpers;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.TabbedPages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace GPS_NotePad.ViewModels
{
    
    class TabbedPageMyViewModel : BindableBase, INavigatedAware, IActiveAware
    {
        private INavigationService navigationService;
        private List<MyPin> listMarkers;
        Location CurrentLocation;
        bool markerInfoVisible;
        string markerImage;
        string markerLabel;
        string markerAddress;
        private bool _isActive;


        public event EventHandler IsActiveChanged;

        public TabbedPageMyViewModel(INavigationService _navigationService)
        {

            navigationService = _navigationService;
            MyPin.TabbedPageMyViewModel = this;
            Map = new MyMap();
           
            Map.IsShowingUser = true;
            Map.MapClicked += Maps_Click;

            Map.My_Pins = new List<MyPin>(30);
            Map.My_Pins.Add(new MyPin { Ids = 0, ImagePath = "paris.jpg", Label = "Запоріжжя", Address = "Ukraine", Position = new Position(47.8498730637859, 35.1050033792853) });
            Map.My_Pins.Add(new MyPin { Ids = 1, ImagePath = "paris.jpg", Label = "Львів", Address = "Ukraine", Position = new Position(49.8414421172141, 24.0277795493603) });
            Map.My_Pins.Add(new MyPin { Ids = 2, ImagePath = "paris.jpg", Label = "Київ", Address = "Ukraine", Position = new Position(50.4549828519957, 30.5185034126043) });
            Map.My_Pins.Add(new MyPin { Ids = 3, ImagePath = "paris.jpg", Label = "Харків", Address = "Ukraine", Position = new Position(49.9786805356518, 36.1922988295555) });
            Map.My_Pins.Add(new MyPin { Ids = 4, ImagePath = "paris.jpg", Label = "Париж", Address = "France", Position = new Position(48.8652635065471, 2.32298579066992) });
            Map.My_Pins.Add(new MyPin { Ids = 5, ImagePath = "rome.jpg", Label = "Рим", Address = "Italy", Position = new Position(41.8911033645807, 12.4073222279549) });
            Map.My_Pins.Add(new MyPin { Ids = 6, ImagePath = "paris.jpg", Label = "Нікоссія", Address = "Cyprus", Position = new Position(35.0660329616338, 33.007184676826) });
            Map.My_Pins.Add(new MyPin { Ids = 7, ImagePath = "paris.jpg", Label = "Анкара", Address = "Turkey", Position = new Position(39.9862060707397, 32.7424878627062) });

            MarkerInfoVisible = false;
            CloseMarkerInfo = new DelegateCommand(Close_MarkerInfo);
            
            Move();
            RefreshPins();
            ListMarkers = Map.My_Pins;
            //SelectTab();
        }


        public DelegateCommand CloseMarkerInfo { get; }
        public MyMap Map { get; private set; }
        public List<MyPin> ListMarkers { get => listMarkers; set => SetProperty(ref listMarkers, value); }
        public bool MarkerInfoVisible { get=>markerInfoVisible; set { SetProperty(ref markerInfoVisible, value); } }
        public string MarkerImage { get => markerImage; set { SetProperty(ref markerImage, value); } }
        public string MarkerLabel { get => markerLabel; set { SetProperty(ref markerLabel, value); } }
        public string MarkerAddress { get => markerAddress; set { SetProperty(ref markerAddress, value); } }
        public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value, RaiseIsActiveChanged); } }


        async void SelectTab()
        {
            await navigationService.NavigateAsync("Tabbed_Page2");
            //var result = await navigationService.SelectTabAsync("Tabbed_Page2");
        }


        int g = 0;
        private void RaiseIsActiveChanged()
        {
            g++;
            if (g == 2)
            {
                var page = navigationService.GetNavigationUriPath();
                
                switch (page)
                {
                    case "/TabbedPageMy?selectedTab=Tabbed_Page1":
                        Console.WriteLine(2);
                        break;

                    case "/TabbedPageMy?selectedTab=Tabbed_Page2":
                        Console.WriteLine(1);
                        break;
                    default:
                        break;
                }
                g = 0;
            }          
        }
        
        private void IndexTab()
        {
            Console.WriteLine("DDDDDDDDDDDDDDDDDDD  " + IsActive);
        }

        private void Close_MarkerInfo()
        {
            MarkerInfoVisible = false;
        }

        public void MarkerClicked(Position pos, int id, string ImagePath, string Label, string Address)
        {
            MarkerInfoVisible = true;
            Move(pos.Latitude, pos.Longitude, 50);
            MarkerImage = Map.My_Pins[id].ImagePath;
            MarkerLabel = Map.My_Pins[id].Label;
            MarkerAddress = Map.My_Pins[id].Address;
        }

        private async void Move(double latitude = 0, double longitude = 0, double distance = 1400)
        {
            if (latitude == 0)
            {
                CurrentLocation = await Geolocation.GetLocationAsync();
                latitude = CurrentLocation.Latitude; longitude = CurrentLocation.Longitude;
            }
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new
               Position(latitude, longitude),
               Distance.FromMiles(distance)));
        }

        private void Maps_Click(object sender, MapClickedEventArgs e)
        {
            Move(e.Position.Latitude, e.Position.Longitude, 10);
            AddPin(e.Position.Latitude, e.Position.Longitude);
        }

        private void AddPin(double latitude, double longitude)
        {
            Map.My_Pins.Add(new MyPin { Ids = Map.My_Pins.Count, ImagePath = "paris.jpg", Label = "gggggg", Address = "rrrr nnn", 
                                        Position = new Position(latitude, longitude) });
            //foreach (var item in Map.My_Pins)
            //{
            //    Console.WriteLine(item.Position.Latitude + " " + item.Position.Longitude);
            //}
            RefreshPins();
        }

        void RefreshPins()
        {
            foreach (var item in Map.My_Pins)
            {
                Map.Pins.Add(item);
            }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
