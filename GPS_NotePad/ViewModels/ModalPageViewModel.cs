
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

        #endregion



        private void Close_MarkerInfo()
        {
            _navigationService.GoBackAsync(useModalNavigation: true, animated: true);
        }


        #region --- Interface InavigatedAword implementation ---

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var e = parameters.GetValue<MarkerInfo>("modal");

            MarkerAddress = e.Address;
            MarkerLabel = e.Label;
            MarkerImage = e.ImagePath;
            MarkerPosition = e.Latitude + ",  " + e.Longitude;
            
            List<string> imgs = e.ImagePath.Split(' ').ToList();

            foreach (var img in imgs)
            {
                ImagesCarousel.Add(new MarkerInfo { ImagePath = img.Trim() });
            }
        }

        #endregion
    }
}
