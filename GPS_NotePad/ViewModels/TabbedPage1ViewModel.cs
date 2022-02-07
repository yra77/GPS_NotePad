

using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.ViewModels.Helpers;
using GPS_NotePad.ViewModels.Services;
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
    class TabbedPage1ViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly ITo_RepositoryService toRepository;
        private INavigationService navigationService;
        private readonly IVerifyInputLogPas_Helper verifyInput;
        Location CurrentLocation;
        bool markerInfoVisible;
        string markerImage;
        string markerLabel;
        string markerAddress;
        private bool _isActive;
        private string search;
        private List<MyPin> listMarkers;

        public event EventHandler IsActiveChanged;
        public static string Email;


        public TabbedPage1ViewModel(INavigationService _navigationService, ITo_RepositoryService _toRepository)
        {
            SearchView = new SearchBar { CancelButtonColor = Color.Red, Placeholder = "Search", PlaceholderColor = Color.Gray };
            SearchView.TextChanged += OnTextChanged;
            SearchView.Unfocused += SearchUnfocus;
            SearchView.SearchButtonPressed += SearchBtnPressed;

            verifyInput = new VerifyInput_Helper();
            toRepository = _toRepository;
            navigationService = _navigationService;

            MyPin.TabbedPageMyViewModel = this;
            
            Map = new MyMap();
            Map.IsShowingUser = true;
            Map.MapClicked += Maps_Click;

            Map.My_Pins = new List<MyPin>(30);
           
            MarkerInfoVisible = false;

            CloseMarkerInfo = new DelegateCommand(Close_MarkerInfo);

            LoadListMarkers();
        }


        public DelegateCommand CloseMarkerInfo { get; }
        public MyMap Map { get; private set; }
        public SearchBar SearchView { get; private set; }
        public List<MyPin> ListMarkers { get => listMarkers; set => SetProperty(ref listMarkers, value); }
        public bool MarkerInfoVisible { get => markerInfoVisible; set { SetProperty(ref markerInfoVisible, value); } }
        public string MarkerImage { get => markerImage; set { SetProperty(ref markerImage, value); } }
        public string MarkerLabel { get => markerLabel; set { SetProperty(ref markerLabel, value); } }
        public string MarkerAddress { get => markerAddress; set { SetProperty(ref markerAddress, value); } }
        public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value, IsActiveTab); } }
        public string Search { get => search; 
                               set { SetProperty(ref search, value);
                if (search.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.NameVerify(ref temp))//Verify label
                    {
                        Search = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                    else
                        Search_Markers();
                }
            } 
        }


        private void IsActiveTab()
        {
            // Console.WriteLine("Page1" + navigationService.GetNavigationUriPath());
            LoadListMarkers();
           // Move();
        }

        private void Close_MarkerInfo()
        {
            MarkerInfoVisible = false;
        }

        public void MarkerClicked(Position pos, int id, string ImagePath, string Label, string Address)
        {
            MarkerInfoVisible = true;
            Move(pos.Latitude, pos.Longitude, 50);
            MarkerImage = ImagePath;// Map.My_Pins[id].ImagePath;
            MarkerLabel = Label;// Map.My_Pins[id].Label;
            MarkerAddress = Address;//Map.My_Pins[id].Address;
        }

        private async void Move(double latitude = 0, double longitude = 0, double distance = 1400)
        {
            if (latitude == 0)
            {
                CurrentLocation = await Geolocation.GetLocationAsync();
                latitude = CurrentLocation.Latitude; 
                longitude = CurrentLocation.Longitude;
            }
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new
               Position(latitude, longitude),
               Distance.FromMiles(distance)));
        }

        private void Maps_Click(object sender, MapClickedEventArgs e)
        {
            Move(e.Position.Latitude, e.Position.Longitude, 10);
           // AddPin(e.Position.Latitude, e.Position.Longitude);
        }

        //private void AddPin(double latitude, double longitude)
        //{
        //    Map.My_Pins.Add(new MyPin
        //    {
        //        Ids = Map.My_Pins.Count,
        //        ImagePath = "paris.jpg",
        //        Label = "gggggg",
        //        Address = "rrrr nnn",
        //        Position = new Position(latitude, longitude)
        //    });

        //    RefreshPins();
        //}

        
       

        private async void LoadListMarkers()
        {
            var arr = await toRepository.GetData<MarkerInfo>("MarkerInfo", Email);
           ToMyPins(arr);
            RefreshPins();
        }
        void ToMyPins(List<MarkerInfo> arr)
        {
            foreach (var item in arr)
            {
                Map.My_Pins.Add(new MyPin
                {
                    Ids = item.Id,
                    Address = item.Address,
                    Label = item.Label,
                    Position = new Position(item.Latitude, item.Longitude),
                    ImagePath = item.ImagePath
                });
            }
            
        }
        void RefreshPins()
        {
            foreach (var item in Map.My_Pins)
            {
                Map.Pins.Add(item);
            }
        }



        #region  SearchBar

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Search = SearchView.Text;
        }
        private void SearchBtnPressed(object sender, EventArgs e)
        {
            ListMarkers = new List<MyPin>();
        }
        private void SearchUnfocus(object sender, FocusEventArgs e)
        {
            ListMarkers = new List<MyPin>();
        }
        void Search_Markers()
        {
            if (Search == null)
            {
                ListMarkers = new List<MyPin>();
                return;
            }
            if (Search.Length > 0)
                ListMarkers = new List<MyPin>(Map.My_Pins);
            if (Search.Length < 1)
            {
                ListMarkers = new List<MyPin>();
                return;
            }

            List<MyPin> buf = new List<MyPin>();
            string temp = search.ToLower();

            int m = Search.Length < 2 ? 0 : Search.Length - 1;

            for (int i = 0; i < ListMarkers.Count; i++)
            {
                string s = ListMarkers[i].Label.ToLower();

                for (int j = m; j < temp.Length; j++)
                {
                    if (s == null || temp.Length > s.Length)
                    {
                        buf.Clear();
                        ListMarkers = new List<MyPin>();
                        break;
                    }
                    if (s[j] == temp[j])
                    {
                        buf.Add(ListMarkers[i]);
                    } 
                }
            }

            if (buf.Count > 0)
            {
                ListMarkers.Clear();
                ListMarkers = buf;
            }
            else
            {
                string a = Search.Remove(search.Length - 1, 1);
                Console.WriteLine(a);
                Search = a;
            }

        }


        #endregion



        public void OnNavigatedFrom(INavigationParameters parameters)
        {
           
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("item"))
            {
                var e = parameters.GetValue<MyPin>("item");
                MarkerClicked(e.Position, e.Ids, e.ImagePath, e.Label, e.Address);
            }
            if (parameters.ContainsKey("email"))
            {
                var e = parameters.GetValue<string>("email");
                Email = e; 
                LoadListMarkers();
                Move();
            }
        }
    }
}
