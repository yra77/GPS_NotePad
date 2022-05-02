

using GPS_NotePad.Helpers;
using GPS_NotePad.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Xamarin.Forms.GoogleMaps;


namespace GPS_NotePad.ViewModels
{
    class PublicTransportSelectViewModel : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;
        private GoogleDirection _googleDirection;


        public PublicTransportSelectViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            DirectionList = new ObservableCollection<PublicTransport>();
        }


        #region Public Property

        private ObservableCollection<PublicTransport> _directionList;
        public ObservableCollection<PublicTransport> DirectionList
        {
            get => _directionList;
            set => SetProperty(ref _directionList, value);
        }


        public DelegateCommand<object> ClickToItem => new DelegateCommand<object>(Item_ClickAsync);
        public DelegateCommand BackBtn => new DelegateCommand(BackClickAsync);

        #endregion


        #region Private helper

        private async void Item_ClickAsync(object item)
        {
            int itemNum = (int)item;

            List<Position> positions = Enumerable.ToList(PolylineHelper.Decode(_googleDirection.Routes[itemNum].OverviewPolyline.Points));

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "routeList", positions }
                                };
            _ = await _navigationService.GoBackAsync(parameters: navParameters, useModalNavigation: true, animated: true);
        }

        private void ToDirectionsList()
        {
            int id = 0;

            foreach (Route a in _googleDirection.Routes)
            {

                foreach (Leg b in a.Legs)
                {
                    PublicTransport pb = new PublicTransport()
                    {
                        Title = b.StartAddress + "\n" + b.EndAddress,
                        SubTitle = b.Distance.Text + "    " + b.Duration.Text,
                        DirectionNum = Resources.Resx.Resource.SearchGooglePlaces + " № " + (id + 1).ToString(),
                        Id = id
                    };

                    id++;
                    SubPublicTransport bufSubList = null;

                    foreach (Step c in b.Steps)
                    {
                        string temp = "";
                        string icon = "";

                        if (c.TravelMode != null)
                        {
                            icon = c.TravelMode == "WALKING" ? Constants.Constant.WalkHuman : Constants.Constant.Bus;

                            temp += "\n" + c.HtmlInstructions + "\n" + c.Duration.Text + "    " + c.Distance.Text;
                        }

                        if (c.TransitDetails != null)
                        {
                            if (c.TransitDetails.Line.Name != null)
                            {
                                temp += "\n" + c.TransitDetails.Line.Name;
                            }

                            temp += "\n" + Resources.Resx.Resource.departure + "  -  " + c.TransitDetails.DepartureTime.Text
                                    + "\n" + Resources.Resx.Resource.arrival + "  -  " + c.TransitDetails.ArrivalTime.Text
                                    + "\n";
                        }
                        else
                        {
                            temp += "\n";
                        }

                        bufSubList = new SubPublicTransport() { Text = temp, Icon = icon };
                        pb.Add(bufSubList);
                    }

                    DirectionList.Add(pb);
                }
            }
        }

        private async void BackClickAsync()
        {
            NavigationParameters navParameter = new NavigationParameters
                                {
                                    { "Direction", _googleDirection }
                                };
            _ = await _navigationService.GoBackAsync(parameters: navParameter, useModalNavigation: true, animated: true);
        }

        #endregion


        #region INavigatedAware Implementation

        public void OnNavigatedFrom(INavigationParameters parameters) {}

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Direction"))
            {
                _googleDirection = parameters.GetValue<GoogleDirection>("Direction");
                if (_googleDirection != null)
                {
                    ToDirectionsList();
                }
            }
        }

        #endregion


        #region ---- Override ----

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);
        }

        #endregion

    }
}
