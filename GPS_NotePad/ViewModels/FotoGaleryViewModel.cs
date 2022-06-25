
using GPS_NotePad.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System.Collections.ObjectModel;


namespace GPS_NotePad.ViewModels
{
    class FotoGaleryViewModel : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;


        public FotoGaleryViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Scaled = true;
        }


        #region --- Public property ---

        private ObservableCollection<MarkerInfo> _imagesCarousel;
        public ObservableCollection<MarkerInfo> ImagesCarousel { get => _imagesCarousel; set => SetProperty(ref _imagesCarousel, value); }


        private int _positionFoto;
        public int PositionFoto { get => _positionFoto; set => SetProperty(ref _positionFoto, value); }


        private string _numOfImg;
        public string NumOfImg { get => _numOfImg; set => SetProperty(ref _numOfImg, value); }


        private bool _scaled;
        public bool Scaled { get => _scaled; set => SetProperty(ref _scaled, value); }

        public DelegateCommand BackBtn => new DelegateCommand(BackClickAsync);
        public DelegateCommand PositionChangedCommand => new DelegateCommand(PositionChanged);

        #endregion



        #region --- Private Helper---
  
        private void PositionChanged()
        {
            NumOfImg = (PositionFoto + 1).ToString() + " - " + ImagesCarousel.Count.ToString();
        }

        private async void BackClickAsync()
        {
           await _navigationService.GoBackAsync(useModalNavigation: true, animated: true);
        }

        #endregion


        #region Interface InavigatedAword implementation

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            var e = parameters.GetValue<ObservableCollection<MarkerInfo>>("fotoGalery");
            ImagesCarousel = new ObservableCollection<MarkerInfo>(e);
            PositionFoto = 0;
            NumOfImg = "1 - " + ImagesCarousel.Count.ToString();
        }

        #endregion
    }
}
