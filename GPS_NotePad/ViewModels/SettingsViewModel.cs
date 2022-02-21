using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.CommunityToolkit.Helpers;

namespace GPS_NotePad.ViewModels
{
    class SettingsViewModel : BindableBase, INavigatedAware
    {

        #region Private helpers

        private readonly INavigationService _navigationService;
        private string _addressPage;
        private string _email;

        #endregion


        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            if (App.Language == "en")
            {
                SelectEnglish = true;
            }
            else
            {
                SelectUkrainian = true;
            }
            Checked_EngCommand = new DelegateCommand<string>(Checked_English);
            Checked_UkrCommand = new DelegateCommand<string>(Checked_Ukrain);
            BackBtn = new DelegateCommand(BackClickAsync);
        }



        #region Public property

        private bool _selectEnglish;
        public bool SelectEnglish { get=> _selectEnglish; set { SetProperty(ref _selectEnglish, value); } }


        private bool _selectUkrainian;
        public bool SelectUkrainian { get => _selectUkrainian; set { SetProperty(ref _selectUkrainian, value); } }


        public DelegateCommand<string> Checked_EngCommand { get; }
        public DelegateCommand<string> Checked_UkrCommand { get; }
        public DelegateCommand BackBtn { get; }

        #endregion


        #region Private metod


        private void Checked_English(string obj)
        {
            if (SelectEnglish)
            {
                LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("en");
                App.Language = "en";
            }
        }

        private void Checked_Ukrain(string obj)
        {
            if (SelectUkrainian)
            {
                LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk");
                App.Language = "uk";
            }
        }

        private async void BackClickAsync()
        {
            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", _email },
                                };
            await _navigationService.NavigateAsync("/" + _addressPage, navParameters);
        }

        #endregion



        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters)
        {
           
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("addressPage"))
            {
                var e = parameters.GetValue<Tuple<string, string>>("addressPage");
                _addressPage = e.Item1;
                _email = e.Item2;
            }
        }
        #endregion
    }
}
