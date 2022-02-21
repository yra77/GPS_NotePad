

using GPS_NotePad.Models;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services.MarkerService;
using GPS_NotePad.Services.SettingsManager;

using Acr.UserDialogs;

using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;


namespace GPS_NotePad.ViewModels
{
    class PinListViewViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly IMarkerService _markerService;
        private readonly INavigationService _navigationService;
        private readonly ISettingsManager _settingsManager;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private List<MarkerInfo> _listMarkersClone;
        private string _email;


        public PinListViewViewModel(INavigationService navigationService, 
                                        IMarkerService markerService, 
                                        ISettingsManager settingsManager)
        {

            _markerService = markerService;
            _navigationService = navigationService;
            _settingsManager = settingsManager;

            _verifyInput = new VerifyInput_Helper();
        }

       
        public event EventHandler IsActiveChanged;


        #region Public property


        private IList<MarkerInfo> _listMarkers;
        public IList<MarkerInfo> ListMarkers 
        { 
            get => _listMarkers; 
            set => SetProperty(ref _listMarkers, value); 
        }


        private int _id;
        public int Id 
        { 
            get => _id; 
            set => SetProperty(ref _id, value); 
        }


        private bool _isActive;
        public bool IsActive 
        { 
            get { return _isActive; } 
            set { SetProperty(ref _isActive, value, IsActiveTabAsync); } 
        }


        private string _likeImage;
        public string LikeImage 
        { 
            get => _likeImage; 
            set => SetProperty(ref _likeImage, value); 
        }


        private string _markerImage;
        public string ImagePath 
        { 
            get => _markerImage; 
            set { SetProperty(ref _markerImage, value); } 
        }


        private string _search;
        public string Search 
        { 
            get => _search; 
            set => SetProperty(ref _search, value); 
        }


        private string _markerLabel;
        public string Label 
        { 
            get => _markerLabel;
            set => SetProperty(ref _markerLabel, value); 
        }
        

        private string _markerAddress;
        public string Address 
        { 
            get => _markerAddress; 
            set => SetProperty(ref _markerAddress, value); 
        }


        public DelegateCommand UnfocusedCommand => new DelegateCommand(SearchUnfocus);
        public DelegateCommand<MarkerInfo> ClickToItem => new DelegateCommand<MarkerInfo>(ItemClickAsync);
        public DelegateCommand AddNewMarker => new DelegateCommand(AddNewMakerClickAsync);
        public DelegateCommand SearchBtn_Pressed => new DelegateCommand(SearchBtnPressed);
        public DelegateCommand<MarkerInfo> EditItem => new DelegateCommand<MarkerInfo>(EditItem_Click);
        public DelegateCommand<MarkerInfo> DeleteItem => new DelegateCommand<MarkerInfo>(DeleteItem_ClickAsync);
        public DelegateCommand<object> LikeImageBtn => new DelegateCommand<object>(LikeImage_Click);
        public DelegateCommand ExitBtn => new DelegateCommand(LogOutAsync);
        public DelegateCommand SettingsBtn => new DelegateCommand(Settings_ClickAsync);

        #endregion


        #region Private helpers

        private void CheckingSearch()
        {
            if (_search.Length > 0)
            {
                string temp = _search;
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
            {
                OnTextChanged();
            }
        }

        private void LikeImage_Click(object item)
        {

            var id = ((int)item)-1;

            if (_listMarkersClone[id].LikeImage == Constants.Constant.Like_Image_Blue)
            {
                _listMarkersClone[id].LikeImage = Constants.Constant.Like_Image_Gray;
            }
            else
            {
                _listMarkersClone[id].LikeImage = Constants.Constant.Like_Image_Blue;
            }

            MarkerInfo markerItem = new MarkerInfo();
            markerItem = _listMarkersClone[id];

            _markerService.UpdateAsync(markerItem);

            RefreshPins();
        }

        private async void DeleteItem_ClickAsync(MarkerInfo item)
        {
            var res = await UserDialogs.Instance.ConfirmAsync(Resources.Resx.Resource.Message_Delete 
                                             + " - " + item.Address + " ?", "Message", "Ok", "cancel");
            if (res)
            {
                await _markerService.DeleteAsync<MarkerInfo>(item.Id);
                ListPinAsync();
            }
        }

        private void EditItem_Click(MarkerInfo item)
        {
            Console.WriteLine("Edit item " + item.Address);
        }

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
            var arr = await _markerService.GetDataAsync<MarkerInfo>("MarkerInfo", _email);
            
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
                    Id = item.Id,
                    Address = item.Address,
                    Label = item.Label,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    ImagePath = item.ImagePath,
                    LikeImage = item.LikeImage,
                    email = item.email
                });
            }

            return temp;
        }

        private async void LogOutAsync()
        {
            _settingsManager.Email = null;
            await _navigationService.NavigateAsync("/MainPage");
        }

        private async void Settings_ClickAsync()
        {

            Tuple<string, string> tuple = new Tuple<string, string>("TabbedPageMy?selectedTab=PinListView", _email);

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "addressPage", tuple },
                                };
            await _navigationService.NavigateAsync("/SettingsView", navParameters);
        }


        // Search 
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


        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters){ }

        public void OnNavigatedTo(INavigationParameters parameters) 
        {
            if (parameters.ContainsKey("email"))
            {
                var e = parameters.GetValue<string>("email");
                _email = e;
                ListPinAsync();
            }
        }
        #endregion


        #region ---- Override ----

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            Console.WriteLine(args.PropertyName);

            switch (args.PropertyName)
            {
                case "Search":
                    CheckingSearch();
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}
