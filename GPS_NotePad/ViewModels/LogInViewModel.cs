

using Acr.UserDialogs;
using GPS_NotePad.Helpers;
using GPS_NotePad.Services.AuthService;
using GPS_NotePad.Services.SettingsManager;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;


namespace GPS_NotePad.ViewModels
{

    class LogInViewModel : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private readonly IAuthService _authService;
        private readonly ISettingsManager _settingsManager;

        public LogInViewModel(IAuthService authService, 
                              INavigationService navigationService, 
                              ISettingsManager settingsManager)
        {

            Email = "";
            Password = "";
            IsEnabled = false;

            ImagePassword = Constants.Constant_Auth.EYE_OFF;
            IsVisiblePassword = true;

            Color_OkBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            GoogleBtn = new DelegateCommand(GoogleClickAsync);
            LogInBtn = new DelegateCommand(LogIn_Click, IsLogInEnable).ObservesProperty(() => IsEnabled);
            Btn_IsVisiblePassword = new DelegateCommand(Click_IsVisiblePassword);
            BackBtn = new DelegateCommand(BackClickAsync);

            _navigationService = navigationService;
            _verifyInput = new VerifyInput_Helper();
            _authService = authService;
            _settingsManager = settingsManager;

            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
        }

       

        #region Public Property

        private string _errorEmailText;
        public string ErrorEmailText { get => _errorEmailText; set { SetProperty(ref _errorEmailText, value); } }


        private string _errorPassText;
        public string ErrorPassText { get => _errorPassText; set { SetProperty(ref _errorPassText, value); } }


        private string _emailBorderColor;
        public string EmailBorderColor { get => _emailBorderColor; set { SetProperty(ref _emailBorderColor, value); } }


        private string _passBorderColor;
        public string PassBorderColor { get => _passBorderColor; set { SetProperty(ref _passBorderColor, value); } }


        private string _color_OkBtn;
        public string Color_OkBtn { get => _color_OkBtn; set => SetProperty(ref _color_OkBtn, value); }


        private string _imagePassword;
        public string ImagePassword { get => _imagePassword; set => SetProperty(ref _imagePassword, value); }


        private bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }


        private bool _isVisiblePassword;
        public bool IsVisiblePassword { get => _isVisiblePassword; set => SetProperty(ref _isVisiblePassword, value); }


        private string _email;
        public string Email
        {
            get { IsLogInEnable(); return _email; }
            set
            {
                SetProperty(ref _email, value);
                if (_email.Length > 0) { CheckEmail(_email); }
            }
        }


        private string _password;
        public string Password
        {
            get { IsLogInEnable(); return _password; }
            set
            {
                SetProperty(ref _password, value);
                if (_password.Length > 0) { CheckPassword(value); }
            }
        }


        public DelegateCommand LogInBtn { get; private set; }
        public DelegateCommand GoogleBtn { get; }
        public DelegateCommand Btn_IsVisiblePassword { get; }
        public DelegateCommand BackBtn { get; }

        #endregion



        #region Private method

        private async void LogIn_Click()
        {

           var result = await _authService.AuthAsync(Password, Email);

            if (result.Item1)
            {
                _settingsManager.Email = Email;
                NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", Email }
                                };
                await _navigationService.NavigateAsync("/TabbedPageMy", navParameters, animated: true);

                Password = "";
            }
            else
            {
                UserDialogs.Instance.Alert(result.Item2, "", "Ok");
                Password = "";
                Email = "";
                Input_ErrorColor();
            }

        }

        private void CheckEmail(string email)
        {
            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorEmailText = Resources.Resx.Resource.ErrorText_Email;

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
                    IsLogInEnable();
                }
            }
        }

        private void CheckPassword(string password)
        {
            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorPassText = Resources.Resx.Resource.ErrorText_Password;
            if (!_verifyInput.PasswordCheckin(ref password))
            {
                Password = password;
            }
            else
            {
                if (_verifyInput.PasswordVerify(Password) && (Password.Length > 7 && Password.Length < 17))
                {
                    ErrorPassText = "Ok!";
                    PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    IsLogInEnable();
                }
            }
        }

        private void Click_IsVisiblePassword()
        {
            if (IsVisiblePassword)
            {
                IsVisiblePassword = false;
                ImagePassword = Constants.Constant_Auth.EYE;
            }
            else
            {
                IsVisiblePassword = true;
                ImagePassword = Constants.Constant_Auth.EYE_OFF;
            }
        }

        private void Input_ErrorColor()
        {
            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            ErrorEmailText = "";
            ErrorPassText = "";
            IsLogInEnable();
        }

        private async void GoogleClickAsync()
        {
            NavigationParameters navParameters = new NavigationParameters { { "google", "google" } };
            await _navigationService.NavigateAsync("/RegistrView2", navParameters, animated: true);
        }

        private async void BackClickAsync()
        {
            await _navigationService.NavigateAsync("/MainPage");
        }

        private bool IsLogInEnable()//Enable disable "Log in" Button
        {
            IsEnabled = (PassBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN
                && EmailBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN) 
                ? IsEnabled = true : IsEnabled = false;

            if (IsEnabled)
                Color_OkBtn = Constants.Constant_Auth.OK_BTN_COLOR_OK;
            else
                Color_OkBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            return IsEnabled;
        }


        #endregion


        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("email"))
            {
                var e = parameters.GetValue<string>("email");
                Email = e;
            }
        }

        #endregion

    }
}
