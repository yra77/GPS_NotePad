using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.ViewModels
{
    class SettingsViewModel : BindableBase, INavigatedAware
    {

        #region Private helpers

        private readonly INavigationService _navigationService;

        #endregion


        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

        }


        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
           
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            
        }
        #endregion
    }
}
