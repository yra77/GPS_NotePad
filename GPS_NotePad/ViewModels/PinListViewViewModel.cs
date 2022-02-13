
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
using System.Threading.Tasks;


namespace GPS_NotePad.ViewModels
{
    class PinListViewViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        #region Private helpers

        private readonly IMarkerService _markerService;
        private readonly INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private IList<MarkerInfo> _listMarkers;
        private List<MarkerInfo> _listMarkersClone;
        private string _markerImage;
        private string _markerLabel;
        private string _markerAddress;       
        private string _email;
        private string _search;
        private bool _isActive;

        #endregion

        public PinListViewViewModel(INavigationService navigationService, IMarkerService markerService)
        {

            _markerService = markerService;
            _navigationService = navigationService;

            AddNewMarker = new DelegateCommand(AddNewMakerClickAsync);
            ClickToItem = new DelegateCommand<MarkerInfo>(ItemClickAsync);
            SearchBtn_Pressed = new DelegateCommand(SearchBtnPressed);
            UnfocusedCommand = new DelegateCommand(SearchUnfocus);

            _verifyInput = new VerifyInput_Helper();
        }


        public event EventHandler IsActiveChanged;


        #region Public property

        public DelegateCommand UnfocusedCommand { get; }
        public DelegateCommand<MarkerInfo> ClickToItem { get; set; }
        public DelegateCommand AddNewMarker { get; }
        public DelegateCommand SearchBtn_Pressed { get; }

        public IList<MarkerInfo> ListMarkers { get => _listMarkers; set => SetProperty(ref _listMarkers, value); }
        public bool IsActive { get { return _isActive; } set { SetProperty(ref _isActive, value, IsActiveTabAsync); } }
        public string ImagePath { get => _markerImage; set { SetProperty(ref _markerImage, value); } }

        public string Search { get => _search; 
                               set { 
                                     SetProperty(ref _search, value);
                if (_search.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.NameVerify(ref temp))//Verify 
                    {
                        Search = temp;
                        UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Search, "Error", "Ok");
                    }
                    else
                    {
                        Search_List();
                    }
                }
                else
                    OnTextChanged();
            } 
        }
       
        public string Label { get => _markerLabel; 
                              set { SetProperty(ref _markerLabel, value);
                if (_markerLabel.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.NameVerify(ref temp))//Verify label
                    {
                        Label = temp;
                        UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Search, "Error", "Ok");
                    }
                }
            } 
        }
        public string Address { get => _markerAddress; 
                                set { SetProperty(ref _markerAddress, value);
                if (_markerAddress.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.NameVerify(ref temp))//Verify Address
                    {
                        Address = temp;
                        UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Search, "Error", "Ok");
                    }
                }
            } 
        }
        

        #endregion


        #region Private method

        private async void AddNewMakerClickAsync()
        {
            await _navigationService.NavigateAsync("/AddPin");
        }

        private async void ItemClickAsync(MarkerInfo item)
        {
            NavigationParameters navParameters = new NavigationParameters
            {
              { "item", item }
            };
           await _navigationService.NavigateAsync("/TabbedPageMy?selectedTab=MapView", navParameters);        
        }

        private async void IsActiveTabAsync()
        {
            _email = MapViewModel.Email;
            await Task.Delay(150);
            ListPinAsync();
        }

        void RefreshPins()
        {
            ListMarkers = new List<MarkerInfo>(_listMarkersClone);
        }

        async void ListPinAsync()
        {
            var arr = await _markerService.GetData<MarkerInfo>("MarkerInfo", _email);
            
            ListMarkers = new List<MarkerInfo>(ToMyPins(arr));
            _listMarkersClone = new List<MarkerInfo>(ListMarkers);
        }

        List<MarkerInfo> ToMyPins(List<MarkerInfo> arr)
        {
            List<MarkerInfo> temp = new List<MarkerInfo>(50);
            foreach(var item in arr)
            {
                temp.Add(new MarkerInfo
                {
                    Address = item.Address,
                    Label = item.Label,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    ImagePath = item.ImagePath,
                    email = item.email
                });
            }

            return temp;
        }

        #region Search    
        private void SearchUnfocus()
        {
            SearchList_Clear();
            Search = "";
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
                await Task.Delay(100);//не убирать
                ItemClickAsync(ListMarkers[0]);
            }
        }
       
        async void SearchList_Clear()
        {
            await Task.Delay(100);//не убирать
            ListMarkers.Clear();
            RefreshPins();
        }

        void Search_List()
        {
            if (Search == null || Search.Length == 0)
            {
                SearchList_Clear();
                return;
            }

            string temp = Search.ToLower();
            List<MarkerInfo> buf = new List<MarkerInfo>();
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
                string a = Search.Remove(_search.Length - 1, 1);
                Search = a;
            }
        }

        #endregion
        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters){ }

        public void OnNavigatedTo(INavigationParameters parameters) { }//ListPinAsync(); }
        #endregion
    }
}
