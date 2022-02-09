
using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.Helpers;
using GPS_NotePad.Controls;
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

namespace GPS_NotePad.ViewModels
{
    class PinListViewViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly ITo_RepositoryService toRepository;
        private IMediaService mediaService;
        private INavigationService navigationService;
        private readonly IVerifyInputLogPas_Helper verifyInput;
        private List<MyPin> listMarkers;
        private List<MyPin> listMarkersClone;
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
        Position mapClicPosition;


        public PinListViewViewModel(INavigationService _navigationService, IMediaService _mediaService, ITo_RepositoryService _toRepository)
        {

            toRepository = _toRepository;
            mediaService = _mediaService;
            navigationService = _navigationService;


            CloseModal = new DelegateCommand(CloseModalClick);
            SaveAddBtn = new DelegateCommand(SaveAddClick);
            GaleryBtn = new DelegateCommand(GaleryClick);
            CameraBtn = new DelegateCommand(CameraClick);
            AddNewMarker = new DelegateCommand(AddNewMakerClick);
            ClickToItem = new DelegateCommand<MyPin>(ItemClick);
            SearchBtn_Pressed = new DelegateCommand(SearchBtnPressed);
            UnfocusedCommand = new DelegateCommand(SearchUnfocus);

            ModalVisible = false;

            markerInfo = new MarkerInfo();
            verifyInput = new VerifyInput_Helper();

        }


        public event EventHandler IsActiveChanged;

        public DelegateCommand UnfocusedCommand { get; }
        public DelegateCommand<MyPin> ClickToItem { get; set; }
        public DelegateCommand AddNewMarker { get; }
        public DelegateCommand CloseModal { get; }
        public DelegateCommand SaveAddBtn { get; }
        public DelegateCommand GaleryBtn { get; }
        public DelegateCommand CameraBtn { get; }
        public DelegateCommand SearchBtn_Pressed { get; }


        public List<MyPin> ListMarkers { get => listMarkers; set => SetProperty(ref listMarkers, value); }
        public Position MapClicPosition
        {
            get { return mapClicPosition; }
            set { SetProperty(ref mapClicPosition, value); MapClicked(); }
        }
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
                    {
                        OnTextChanged();
                        Search_List();
                    }
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



        private void MapClicked()
        {
            position = new Position(MapClicPosition.Latitude, MapClicPosition.Longitude);
        }
        private void AddNewMakerClick()
        {
            ModalVisible = true;
            Label = "";
            Address = "";
            ImagePath = "";
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

                listMarkers.Add(new MyPin
                {
                    Ids = listMarkersClone.Count,
                    ImagePath = this.ImagePath == null ? "paris.jpg" : ImagePath,
                    Label = this.Label,
                    Address = this.Address,
                    Position = new Position(position.Latitude, position.Longitude)
                });              
                
                ToMarkerInfo();

                if (await toRepository.Insert(markerInfo))
                {
                    ModalVisible = false;
                    ListPin();
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

        private async void ItemClick(MyPin item)
        {
            NavigationParameters navParameters = new NavigationParameters
            {
              { "item", item }
            };
           await navigationService.NavigateAsync("/TabbedPageMy?selectedTab=Tabbed_Page1", navParameters, animated: true);
            // await navigationService.NavigateAsync("/TabbedPageMy?selectedTab=Tabbed_Page1");
          
        }

        private void CloseModalClick()
        {
            ModalVisible = false;
        }

        private void IsActiveTab()
        {
            email = MapViewModel.Email;
            Label = "";
            Address = "";
            ImagePath = "";
            position = new Position(0,0);
            ListPin();
        }

        void RefreshPins()
        {
            ListMarkers = new List<MyPin>(listMarkersClone);
        }

        async void ListPin()
        {
            var arr = await toRepository.GetData<MarkerInfo>("MarkerInfo", email);

            listMarkersClone = new List<MyPin>();
            ListMarkers = ToMyPins(arr);
            listMarkersClone = new List<MyPin>(ListMarkers);
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
        
        private void SearchUnfocus()
        {
            SearchList_Clear();
        }

        private void OnTextChanged()
        {
            if (ListMarkers == null || Search == null || Search.Length == 0)
                SearchList_Clear();
        }
        private async void SearchBtnPressed()
        {
            if (ListMarkers != null && ListMarkers.Count > 0)
            {
                var a = ListMarkers[0];
                await Task.Delay(100);//не убирать
                ItemClick(ListMarkers[0]);
            }
        }
       
        async void SearchList_Clear()
        {
            await Task.Delay(100);//не убирать
            ListMarkers.Clear();
            RefreshPins();
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
                string a = Search.Remove(search.Length - 1, 1);
                Search = a;
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
