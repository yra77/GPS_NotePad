
using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.ViewModels.Helpers;
using GPS_NotePad.ViewModels.Services;
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

namespace GPS_NotePad.ViewModels
{
    class TabbedPage2ViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly ITo_RepositoryService toRepository;
        private IMediaService mediaService;
        private INavigationService navigationService;
        private readonly IVerifyInputLogPas_Helper verifyInput;
        private List<MyPin> listMarkers;
        MarkerInfo markerInfo;
        Location CurrentLocation;
        string markerImage;
        string markerLabel;
        string markerAddress;
        Position position;
        private string email;
        private bool _isActive;
        private bool modalVisible;
        private string search;

        public TabbedPage2ViewModel(INavigationService _navigationService, IMediaService _mediaService, ITo_RepositoryService _toRepository)
        {
            SearchView = new SearchBar { CancelButtonColor = Color.Red, Placeholder = "Search", PlaceholderColor = Color.Gray };
            SearchView.TextChanged += OnTextChanged;
            SearchView.Unfocused += SearchUnfocus;
            SearchView.SearchButtonPressed += SearchBtnPressed;

            toRepository = _toRepository;
            mediaService = _mediaService;
            navigationService = _navigationService;
            Map2 = new MyMap();
            Map2.My_Pins = new List<MyPin>(30);

            Map2.IsShowingUser = true;
            Map2.MapClicked += Maps_Click;

            CloseModal = new DelegateCommand(CloseModalClick);
            SaveAddBtn = new DelegateCommand(SaveAddClick);
            GaleryBtn = new DelegateCommand(GaleryClick);
            CameraBtn = new DelegateCommand(CameraClick);
            AddNewMarker = new DelegateCommand(AddNewMakerClick);
            ClickToItem = new DelegateCommand<MyPin>(ItemClick);

            ModalVisible = false;

            markerInfo = new MarkerInfo();
            verifyInput = new VerifyInput_Helper();

            Move();
        }


        public event EventHandler IsActiveChanged;

        public DelegateCommand<MyPin> ClickToItem { get; set; }
        public DelegateCommand AddNewMarker { get; }
        public DelegateCommand CloseModal { get; }
        public DelegateCommand SaveAddBtn { get; }
        public DelegateCommand GaleryBtn { get; }
        public DelegateCommand CameraBtn { get; }

        public List<MyPin> ListMarkers { get => listMarkers; set => SetProperty(ref listMarkers, value); }
        public MyMap Map2 { get; private set; }
        public SearchBar SearchView { get; private set; }
        public string Search { get => search; 
                               set { 
                                     SetProperty(ref search, value);
                if (search.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.NameVerify(ref temp))//Verify 
                    {
                        Search = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                    else
                        Search_List();
                }
            } 
        }
        public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value, IsActiveTab); } }
        public bool ModalVisible { get => modalVisible; set { SetProperty(ref modalVisible, value); } }
        public string Label { get => markerLabel; 
                              set { SetProperty(ref markerLabel, value);
                if (markerLabel.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.NameVerify(ref temp))//Verify label
                    {
                        Label = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                }
            } 
        }
        public string Address { get => markerAddress; 
                                set { SetProperty(ref markerAddress, value);
                if (markerAddress.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.NameVerify(ref temp))//Verify Address
                    {
                        Address = temp;
                        UserDialogs.Instance.Alert("A-Z, a-z symbols only", "Error", "Ok");
                    }
                }
            } 
        }
        public string ImagePath { get => markerImage; set { SetProperty(ref markerImage, value); } }



        private void AddNewMakerClick()
        {
            ModalVisible = true;
        }

        private async void CameraClick()
        {
            string camera = await mediaService.OpenCamera();
            ImagePath = camera;
        }

        private async void GaleryClick()
        {
            string galery = await mediaService.OpenGalery();
            ImagePath = galery;
        }

        private async void SaveAddClick()
        {
            if (Label != null && Address != null && position.Latitude > 0)
            {
                Map2.My_Pins.Add(new MyPin
                {
                    Ids = Map2.My_Pins.Count,
                    ImagePath = this.ImagePath == null?"paris.jpg":ImagePath,
                    Label = this.Label,
                    Address = this.Address,
                    Position = new Position(position.Latitude, position.Longitude)
                });

                ToMarkerInfo();

                if (await toRepository.Insert(markerInfo))
                {
                    ModalVisible = false;
                    RefreshPins();
                }
                else
                    UserDialogs.Instance.Alert("Error saved data", "Error", "Ok");
            }
            else
                UserDialogs.Instance.Alert("All fields must be filled", "Error", "Ok");
        }

        void ToMarkerInfo()
        {
            markerInfo.email = email;
            markerInfo.Address = Address;
            markerInfo.Label = Label;
            markerInfo.ImagePath = this.ImagePath == null ? "paris.jpg" : ImagePath;
            markerInfo.Latitude = position.Latitude;
            markerInfo.Longitude = position.Longitude;
        }

        private void ItemClick(MyPin item)
        {
            NavigationParameters navParameters = new NavigationParameters
            {
              { "item", item }
            };
            navigationService.NavigateAsync("/TabbedPageMy?selectedTab=Tabbed_Page1", navParameters, animated: true);
            // await navigationService.NavigateAsync("/TabbedPageMy?selectedTab=Tabbed_Page1");
          
        }

        private void CloseModalClick()
        {
            ModalVisible = false;
        }

        private void IsActiveTab()
        {
            email = TabbedPage1ViewModel.Email;
            Label = "";
            Address = "";
            ImagePath = "";
            position = new Position(0,0);
            ListPin();
        }

        private void Maps_Click(object sender, MapClickedEventArgs e)
        {
            position = new Position(e.Position.Latitude, e.Position.Longitude);
            Move(e.Position.Latitude, e.Position.Longitude, 10);
        }

        private async void Move(double latitude = 0, double longitude = 0, double distance = 1400)
        {
            if (latitude == 0)
            {
                CurrentLocation = await Geolocation.GetLocationAsync();
                latitude = CurrentLocation.Latitude; longitude = CurrentLocation.Longitude;
            }
            Map2.MoveToRegion(MapSpan.FromCenterAndRadius(new
               Position(latitude, longitude),
               Distance.FromMiles(distance)));
        }  
        
        void RefreshPins()
        {
            ListMarkers = new List<MyPin>(Map2.My_Pins);
        }

        async void ListPin()
        {
            var arr = await toRepository.GetData<MarkerInfo>("MarkerInfo", email);
            Map2.My_Pins = ToMyPins(arr);
            RefreshPins();
        }

        List<MyPin> ToMyPins(List<MarkerInfo> arr)
        {
            List<MyPin> temp = new List<MyPin>(50);
            foreach(var item in arr)
            {
                temp.Add(new MyPin {Ids = item.Id, Address = item.Address, Label = item.Label, 
                                     Position = new Position(item.Latitude, item.Longitude), ImagePath = item.ImagePath });
            }
            return temp;
        }


        #region Search

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Search = SearchView.Text;
            if (SearchView.Text.Length == 0)
                SearchList_Clear();
        }
        private void SearchBtnPressed(object sender, EventArgs e)
        {
            if (ListMarkers != null && ListMarkers.Count > 0)
                ItemClick(ListMarkers[0]);
            SearchList_Clear();
        }
        private void SearchUnfocus(object sender, FocusEventArgs e)
        {
            SearchList_Clear();
        }
        async void SearchList_Clear()
        {
            await Task.Delay(100);
            ListMarkers.Clear();
            RefreshPins();
            SearchView.Text = "";
            Search = "";
        }
        void Search_List()
        {
            if (Search == null || Search.Length < 1)
            {
                ListMarkers.Clear();
                RefreshPins();
                return;
            }

            string temp = Search.ToLower();
            List<MyPin> buf = new List<MyPin>();
            int m = Search.Length < 2 ? 0 : Search.Length - 1;

            for (int i = 0; i < ListMarkers.Count; i++)
            {
                string s = ListMarkers[i].Label.ToLower();
                string ss = ListMarkers[i].Address.ToLower();

                for (int j = m; j < temp.Length; j++)
                {
                    //if (s == null || temp.Length > s.Length)
                    //{
                    //    buf.Clear();
                    //    ListMarkers = new List<MyPin>();
                    //    IsVisible_SearchList = false;
                    //    break;
                    //}
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
                        if (!isThat)
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
                string S = Search.Remove(search.Length - 1, 1);
                Console.WriteLine(S);
                Search = S;
            }
        }

        #endregion

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
           
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            ListPin();
        }
    }
}
