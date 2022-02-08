

using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;

namespace GPS_NotePad.ViewModels
{
    class MapViewModel : BindableBase, INavigatedAware, IActiveAware
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
        private bool isVisible_SearchList;
        private static string email;

        ObservableCollection<MyPin> listPin;

        public event EventHandler IsActiveChanged;


        public MapViewModel(INavigationService _navigationService, ITo_RepositoryService _toRepository)
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
            //Map.IsShowingUser = true;
           // Map.MapClicked += Maps_Click;

            Map.My_Pins = new List<MyPin>(30);
           
            MarkerInfoVisible = false;
            IsVisible_SearchList = false;

            ListPin = new ObservableCollection<MyPin>();
            
            CloseMarkerInfo = new DelegateCommand(Close_MarkerInfo);
            Click_SearchListItem = new DelegateCommand<MyPin>(SearchListItem_Click);
            ClickToItem = new DelegateCommand<MyPin>(ItemClick);
            LoadListMarkers();
           // ListPin = Map.My_Pins;
        }

        private void ItemClick(MyPin obj)
        {
            Console.WriteLine("Gggggggggggggggggggggg" + obj.Address);
        }

        MyPin myPinItem;
        public MyPin MyPinItem { get => myPinItem; set { SetProperty(ref myPinItem, value); } }

        public ObservableCollection<MyPin> ListPin { get => listPin; set => SetProperty(ref listPin, value); }


        public DelegateCommand<MyPin> ClickToItem { get; set; }
        public DelegateCommand CloseMarkerInfo { get; }
        public DelegateCommand<MyPin> Click_SearchListItem { get; }

        public static string Email { get => email; set => email = value; }
        public MyMap Map1 { get; private set; }
        public MyMap Map { get; private set; }
        public SearchBar SearchView { get; private set; }
        public List<MyPin> ListMarkers { get => listMarkers; set => SetProperty(ref listMarkers, value); }
        public bool MarkerInfoVisible { get => markerInfoVisible; set { SetProperty(ref markerInfoVisible, value); } }
        public string MarkerImage { get => markerImage; set { SetProperty(ref markerImage, value); } }
        public string MarkerLabel { get => markerLabel; set { SetProperty(ref markerLabel, value); } }
        public string MarkerAddress { get => markerAddress; set { SetProperty(ref markerAddress, value); } }
        public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value, IsActiveTab); } }
        public bool IsVisible_SearchList { get { return isVisible_SearchList; } set { SetProperty(ref isVisible_SearchList, value, IsActiveTab); } }
        public string Search { get => search; 
                               set { SetProperty(ref search, value);
                if (search.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.NameVerify(ref temp))//Verify
                    {
                        Search = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                    else
                        Search_Markers();
                }
            } 
        }

        public int Ids { get; set; }

        private void IsActiveTab()
        {
            // Console.WriteLine("Page1" + navigationService.GetNavigationUriPath());
            LoadListMarkers();
        }

        private void Close_MarkerInfo()
        {
            MarkerInfoVisible = false;
        }

        public void MarkerClicked(Position pos, int id, string ImagePath, string Label, string Address)
        {
            MarkerInfoVisible = true;
            // Move(pos.Latitude, pos.Longitude, 50);
            MarkerImage = ImagePath;// ListPin[id].ImagePath;
            MarkerLabel = Label;
            MarkerAddress = Address;

            MyPinItem = new MyPin { Address = Address, Position = pos, Label = Label, ImagePath = ImagePath, Ids = 0 };
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
        }

        private async void LoadListMarkers()
        {
            var arr = await toRepository.GetData<MarkerInfo>("MarkerInfo", Email);
             ToMyPins(arr);
           // RefreshPins();
        }

        void ToMyPins(List<MarkerInfo> arr)
        {
            foreach (var item in arr)
            {
                ListPin.Add(new MyPin
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
            if (SearchView.Text.Length == 0)
                SearchList_Clear();
        }
        private void SearchBtnPressed(object sender, EventArgs e)
        {
            if(ListMarkers != null && ListMarkers.Count > 0)
                      SearchListItem_Click(ListMarkers[0]); 
            SearchList_Clear();
        }
        private void SearchUnfocus(object sender, FocusEventArgs e)
        {
            SearchList_Clear();
        }
        private void SearchListItem_Click(MyPin val)
        {
            MarkerClicked(val.Position, val.Ids, val.ImagePath, val.Label, val.Address);
            SearchList_Clear();
        }
        async void SearchList_Clear()
        {
            await Task.Delay(100);
            ListMarkers = new List<MyPin>();
            ListMarkers.Clear();
            IsVisible_SearchList = false;
            SearchView.Text = "";
            Search = "";
        }
        void Search_Markers()
        {
            if (Search == null || Search.Length < 1)
            {
                ListMarkers = new List<MyPin>();
                IsVisible_SearchList = false;
                return;
            }
            if (Search.Length > 0)
            {
                ListMarkers = new List<MyPin>(Map.My_Pins);
                IsVisible_SearchList = true;
            }

            List<MyPin> buf = new List<MyPin>();
            string temp = search.ToLower();

            int m = Search.Length < 2 ? 0 : Search.Length - 1;

            for (int i = 0; i < ListMarkers.Count; i++)
            {
                string s = ListMarkers[i].Label.ToLower();
                string ss = ListMarkers[i].Address.ToLower();

                for (int j = m; j < temp.Length; j++)
                {

                    if (s != null && temp.Length <= s.Length && s[j] == temp[j])
                    {
                        bool isThat = false;
                        foreach (var item in buf)
                        {
                            if (item.Label == ListMarkers[i].Label)
                            {
                                isThat = true;
                                break;
                            }
                        }
                        if(!isThat)
                            buf.Add(ListMarkers[i]);
                    }
                    if (ss != null && temp.Length <= ss.Length && ss[j] == temp[j])
                    {
                        bool isThat = false;
                        foreach (var item in buf)
                        {
                            if (item.Address == ListMarkers[i].Address)
                            {
                                isThat = true;
                                break;
                            }
                        }
                        if (!isThat)
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
                Search = a;
                SearchView.Text = a;
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
