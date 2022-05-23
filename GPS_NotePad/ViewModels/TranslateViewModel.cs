

using GPS_NotePad.Services.TranslatorInterfaces;
using GPS_NotePad.Services.SettingsManager;
using GPS_NotePad.Services.Interfaces;
using GPS_NotePad.Services.TranslatorInterfaces.TextTranslation_Service;

using Prism;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Commands;

using Xamarin.Forms;

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Acr.UserDialogs;
using GPS_NotePad.Helpers;

namespace GPS_NotePad.ViewModels
{
    class TranslateViewModel : BindableBase, INavigatedAware, IActiveAware
    {

        private readonly INavigationService _navigationService;
        private readonly ISettingsManager _settingsManager;
        private readonly IMicrophoneService _microphoneService;
        private readonly ISpeechToText_Service _speechToText_Service;
        private readonly ITextTranslationsService _textTranslatorsService;
        private bool _isTranscribing;
        private string _email;


        public TranslateViewModel(INavigationService navigationService,
                                IMicrophoneService microphoneService,
                                ISettingsManager settingsManager,
                                ISpeechToText_Service speechToText_Service,
                                ITextTranslationsService textTranslatorsService)
        {

            _speechToText_Service = speechToText_Service;
            _navigationService = navigationService;
            _microphoneService = microphoneService;
            _settingsManager = settingsManager;
            _textTranslatorsService = textTranslatorsService;

            _isTranscribing = false;
            IsTextTranslate = false;

            MicIcon = "micIconBlue.png";
            TxtIcon = "txtIconBlue.png";
            LanguagesList = new ObservableCollection<string>() { "english", "deutsch", "українська", "русский" };
        }


        public event EventHandler IsActiveChanged;


        #region Public Property

        private ObservableCollection<string> _languagesList;
        public ObservableCollection<string> LanguagesList
        {
            get => _languagesList;
            set => SetProperty(ref _languagesList, value);
        }


        private string _result;
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }


        private string _micIcon;
        public string MicIcon
        {
            get => _micIcon;
            set => SetProperty(ref _micIcon, value);
        }


        private string _txtIcon;
        public string TxtIcon
        {
            get => _txtIcon;
            set => SetProperty(ref _txtIcon, value);
        }


        private bool _isTextTranslate;
        public bool IsTextTranslate
        {
            get => _isTextTranslate;
            set => SetProperty(ref _isTextTranslate, value);
        }


        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value, IsActiveTab);
        }


        private string _languageFrom;
        public string LanguageFrom
        {
            get => _languageFrom;
            set => SetProperty(ref _languageFrom, value);
        }


        private string _languageTo;
        public string LanguageTo
        {
            get => _languageTo;
            set => SetProperty(ref _languageTo, value);
        }


        private string _textToTranslate;
        public string TextToTranslate
        {
            get => _textToTranslate;
            set => SetProperty(ref _textToTranslate, value);
        }


        public DelegateCommand TranscribeClicked => new DelegateCommand(Transcribe_ClickedAsync);
        public DelegateCommand TextTranslateClicked => new DelegateCommand(TextTranslate_ClickedAsync);
        public DelegateCommand OppositesClick => new DelegateCommand(Opposites_ClickAsync);
        public DelegateCommand ExitBtn => new DelegateCommand(LogOutAsync);
        public DelegateCommand SettingsBtn => new DelegateCommand(Settings_ClickAsync);
        public DelegateCommand CompletedCommand => new DelegateCommand(Completed_Command);

        #endregion


        #region Private helper

        private void Completed_Command()
        {
            Text_Traslate_Async(TextToTranslate);
        }

        private void IsActiveTab()
        {
            // await Task.Delay(150);
            if (!CheckingDeviceProperty_Helper.CheckNetwork())
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Device_Internet, "Error Internet", "Ok");
            }
            _email = _settingsManager.Email;
        }

        private async void Opposites_ClickAsync()
        {
            if (LanguageFrom == null || LanguageTo == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", Resources.Resx.Resource.Language, "OK");
                return;
            }

            string temp = LanguageFrom;
            LanguageFrom = LanguageTo;
            LanguageTo = temp;

            if (IsTextTranslate)
            {
                TextToTranslate = Result;
                Result = "";
            }
        }

        private async void TextTranslate_ClickedAsync()
        {
            if (CheckingDeviceProperty_Helper.CheckNetwork())
            {

                if (LanguageFrom == null || LanguageTo == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", Resources.Resx.Resource.Language, "OK");
                    return;
                }

                if (IsTextTranslate)
                {
                    IsTextTranslate = false;
                    TxtIcon = "txtIconBlue.png";
                }
                else
                {
                    IsTextTranslate = true;
                    TextToTranslate = "";
                    Result = "";
                    TxtIcon = "txtIconRed.png";
                }

            }
            else
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Device_Internet, "Error Internet", "Ok");
            }
        }

        private async void Transcribe_ClickedAsync()
        {
            if (CheckingDeviceProperty_Helper.CheckNetwork())
            {
                bool micAccessGranted = await _microphoneService.GetPermissionAsync();

                if (!micAccessGranted)
                {
                    await Application.Current.MainPage.DisplayAlert("Error access", Resources.Resx.Resource.AccessToMicrophone, "OK");
                    return;
                }

                if (LanguageFrom == null || LanguageTo == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", Resources.Resx.Resource.Language, "OK");
                    return;
                }

                if (_isTranscribing)
                {
                    _speechToText_Service.StopListening();
                    _isTranscribing = false;
                }
                else
                {
                    _speechToText_Service.SpeechToText();
                    Result = "";
                    _isTranscribing = true;
                }

                MessagingCenter.Subscribe<IMessageSender, string>(this, "STT", (sender, args) =>
                {
                    Mic_Traslate_Async(args);
                });

                Update_Mic_Button();
            }
            else
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Device_Internet, "Error Internet", "Ok");
            }
        }

        private async void Mic_Traslate_Async(string message)
        {
            if (message != null)
            {
                message = await _textTranslatorsService.TranslateTextAsync(message,
                                                               ToShortName(LanguageFrom),
                                                               ToShortName(LanguageTo));
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                Result += $"{message}" + " ";
            });
        }

        private async void Text_Traslate_Async(string message)
        {
            if ((TextToTranslate != null && TextToTranslate.Length > 1)
                || (TextToTranslate.Length > 1 && TextToTranslate[TextToTranslate.Length - 1] == ' '))
            {

                if (message != null)
                {
                    message = await _textTranslatorsService.TranslateTextAsync(message,
                                                                   ToShortName(LanguageFrom),
                                                                   ToShortName(LanguageTo));
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    Result = $"{message}" + " ";
                });
            }
            else
            {
                Result = "";
            }
        }

        private string ToShortName(string languageLong_Name)
        {
            switch (languageLong_Name)
            {
                case "deutsch":
                    languageLong_Name = "de";
                    break;
                case "english":
                    languageLong_Name = "en";
                    break;
                case "українська":
                    languageLong_Name = "uk";
                    break;
                case "русский":
                    languageLong_Name = "ru";
                    break;
                default:
                    languageLong_Name = "en";
                    break;
            }

            return languageLong_Name;
        }

        private void Update_Mic_Button()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MicIcon = _isTranscribing ? "micIconRed.png" : "micIconBlue.png";
            });
        }

        private async void LogOutAsync()
        {
            _settingsManager.Email = null;
            _ = await _navigationService.NavigateAsync("/MainPage");
        }

        private async void Settings_ClickAsync()
        {

            Tuple<string, string> tuple = new Tuple<string, string>("TabbedPageMy?selectedTab=TranslateView", _email);

            NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "addressPage", tuple },
                                };
            _ = await _navigationService.NavigateAsync("/SettingsView", navParameters);
        }

        #endregion


        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {

        }
        #endregion


        #region ---- Override ----

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case "TextToTranslate":
                    Text_Traslate_Async(TextToTranslate);
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
