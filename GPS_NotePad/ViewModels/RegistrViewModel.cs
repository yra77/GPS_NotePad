

using GPS_NotePad.Models;
using GPS_NotePad.Services.VerifyService;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System.ComponentModel;


namespace GPS_NotePad.ViewModels
{
    class RegistrViewModel : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;
        private readonly IVerifyInputService _verifyInput;


        public RegistrViewModel(INavigationService navigationService,
                              IVerifyInputService verifyInputService)
        {
            _navigationService = navigationService;
            _verifyInput = verifyInputService;

            Name = "";
            Email = "";
            NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;

            Color_NextBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            IsEnabled = false;
        }


        #region Public Property

        private string _errorEmailText;
        public string ErrorEmailText
        {
            get => _errorEmailText;
            set => SetProperty(ref _errorEmailText, value);
        }

        private string _emailBorderColor;
        public string EmailBorderColor
        {
            get => _emailBorderColor;
            set => SetProperty(ref _emailBorderColor, value);
        }


        private string _errorNameText;
        public string ErrorNameText
        {
            get => _errorNameText;
            set => SetProperty(ref _errorNameText, value);
        }


        private string _nameBorderColor;
        public string NameBorderColor
        {
            get => _nameBorderColor;
            set => SetProperty(ref _nameBorderColor, value);
        }


        private string _color_NextBtn;
        public string Color_NextBtn
        {
            get => _color_NextBtn;
            set => SetProperty(ref _color_NextBtn, value);
        }


        private string _name;
        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value);
        }


        private string _email;
        public string Email
        {
            get => _email; 
            set => SetProperty(ref _email, value);
        }


        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }


        public DelegateCommand NextBtn => new DelegateCommand(Next_ClickAsync, IsNextEnable).ObservesProperty(() => IsEnabled);
        public DelegateCommand GoogleBtn => new DelegateCommand(GoogleClickAsync);
        public DelegateCommand BackBtn => new DelegateCommand(BackClickAsync);

        #endregion


        #region Private helpers

        private void CheckName()
        {
            if (_name.Length > 0)
            {
                NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
                ErrorNameText = Resources.Resx.Resource.ErrorText_Name;

                string name = Name;

                if (!_verifyInput.NameVerify(ref name))
                {
                    Name = name;
                }
                else if (Name.Length >= 2)
                {
                    ErrorNameText = "Ok! " + Resources.Resx.Resource.ErrorText_Name;
                    NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                }
                IsNextEnable();
            }
        }

        private void CheckEmail()
        {
            if (_email.Length > 0)
            {
                EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
                ErrorEmailText = Resources.Resx.Resource.ErrorText_Email;

                string email = Email;

                if (!_verifyInput.EmailVerify(ref email))
                {
                    Email = email;
                }
                else
                {
                    if (_verifyInput.IsValidEmail(Email))
                    {
                        ErrorEmailText = "Ok!";
                        EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    }
                }
                IsNextEnable();
            }
        }

        private async void GoogleClickAsync()
        {
            NavigationParameters navParameters = new NavigationParameters { { "google", "google" } };
            await _navigationService.NavigateAsync("/RegistrView2", navParameters, animated: true);
        }

        private void Input_ErrorColor()
        {
            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            ErrorEmailText = "";
            ErrorNameText = "";
            IsNextEnable();
        }

        private bool IsNextEnable()//Enable disable "Ok" Button
        {
            IsEnabled = (NameBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN
                         && EmailBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN)
                         ? IsEnabled = true : IsEnabled = false;

            if (IsEnabled)
            {
                Color_NextBtn = Constants.Constant_Auth.OK_BTN_COLOR_OK;
            }
            else
            {
                Color_NextBtn = Constants.Constant_Auth.OK_BTN_COLOR;
            }

            return IsEnabled;
        }

        private async void Next_ClickAsync()
        {
            if (_verifyInput.IsValidEmail(Email) && Name.Length >= 2)
            {
                Loginin log = new Loginin();
                log.name = Name;
                log.email = Email;

                NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", log }
                                };
                await _navigationService.NavigateAsync("/RegistrView2", navParameters, animated: true);
            }
            else
            {
                Name = "";
                Email = "";
                Input_ErrorColor();
            }

        }

        private async void BackClickAsync()
        {
            Name = "";
            Email = "";
            await _navigationService.NavigateAsync("/MainPage");
        }

        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters) { }
        #endregion


        #region ---- Override ----

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case "Email":
                    CheckEmail();
                    break;
                case "Name":
                    CheckName();
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}
