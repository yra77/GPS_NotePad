

using Prism.Mvvm;
using Prism.Navigation;

namespace GPS_NotePad.ViewModels
{

    class TabbedPageMyViewModel : BindableBase, INavigatedAware
    {
        private INavigationService navigationService;

        public TabbedPageMyViewModel(INavigationService _navigationService)
        {
            navigationService = _navigationService;
        }


        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
