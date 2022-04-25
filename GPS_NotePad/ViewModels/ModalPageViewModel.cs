
using GPS_NotePad.Models;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GPS_NotePad.ViewModels
{
    class ModalPageViewModel : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;


        public ModalPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ImagesCarousel = new ObservableCollection<MarkerInfo>();
        }


        #region private helper


        private ObservableCollection<MarkerInfo> _imagesCarousel;
        public ObservableCollection<MarkerInfo> ImagesCarousel { get => _imagesCarousel; set => SetProperty(ref _imagesCarousel, value); }


        private int _positionFoto;
        public int PositionFoto { get => _positionFoto; set => SetProperty(ref _positionFoto, value); }


        private string _markerImage;
        public string MarkerImage
        {
            get => _markerImage;
            set => SetProperty(ref _markerImage, value);
        }


        private string _markerLabel;
        public string MarkerLabel
        {
            get => _markerLabel;
            set => SetProperty(ref _markerLabel, value);
        }


        private string _markerAddress;
        public string MarkerAddress
        {
            get => _markerAddress;
            set => SetProperty(ref _markerAddress, value);
        }


        private string _markerPosition;
        public string MarkerPosition
        {
            get => _markerPosition;
            set => SetProperty(ref _markerPosition, value);
        }


        public DelegateCommand CloseMarkerInfo => new DelegateCommand(Close_MarkerInfo);
        public DelegateCommand<object> FotoClick => new DelegateCommand<object>(Click_Foto);

        #endregion


        #region --- Private Helper---

        private void Click_Foto(object obj)
        {
            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "fotoGalery", ImagesCarousel }
                                };
            _navigationService.NavigateAsync("FotoGaleryView", navParameters, useModalNavigation: true, animated: true);
        }

        private void Close_MarkerInfo()
        {
            _navigationService.GoBackAsync(useModalNavigation: true, animated: true);
        }

        private void ToPage(MarkerInfo item)
        {
            MarkerAddress = item.Address;
            MarkerLabel = item.Label;
            MarkerImage = item.ImagePath;
            MarkerPosition = item.Latitude + ",  " + item.Longitude;

            MarkerImage = item.ImagePath.Trim();

            List<string> imgs = MarkerImage.Split(' ').ToList();

            PositionFoto = imgs.Count == 1 ? 0 : 1;

            foreach (string img in imgs)
            {
                if (img != null)
                {
                    ImagesCarousel.Add(new MarkerInfo { ImagePath = img });
                }
            }
        }


        #endregion


        #region --- Interface InavigatedAword implementation ---

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            MarkerInfo e = parameters.GetValue<MarkerInfo>("modal");

            ToPage(e);
        }

        #endregion
    }
}
