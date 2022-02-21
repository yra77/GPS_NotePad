

using GPS_NotePad.Helpers;
using GPS_NotePad.Models;
using GPS_NotePad.Services.AuthService;
using GPS_NotePad.Services.RegistrService;
using GPS_NotePad.Services.SettingsManager;

using Acr.UserDialogs;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System;


namespace GPS_NotePad.ViewModels
{

    public delegate void GoogleAuthCallBack(string email, bool isReg);

    class RegistrViewModel2 : BindableBase, INavigatedAware
    {

        private readonly INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private readonly IRegistrService _registrService;
        private readonly IAuthService _authService;
        private readonly ISettingsManager _settingsManager;
        private string _name;
        private string _email;


        public RegistrViewModel2(IRegistrService registrService, 
                                    IAuthService authService, 
                                    INavigationService navigationService, 
                                    ISettingsManager settingsManager)
        {

            _navigationService = navigationService;
            _settingsManager = settingsManager;
            _verifyInput = new VerifyInput_Helper();
            _registrService = registrService;
            _authService = authService;

            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;

            PasswordConfirm = "";
            Password = "";
            IsEnabled = false;

            ImagePasConfirm = Constants.Constant_Auth.EYE_OFF;
            ImagePassword = Constants.Constant_Auth.EYE_OFF;
            IsVisiblePasConfirm = true;
            IsVisiblePassword = true;

            Color_RegistrBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            GoogleBtn = new DelegateCommand(GoogleClick);
            RegistrBtn = new DelegateCommand(Registr_Click, IsRegistrEnable).ObservesProperty(() => IsEnabled);
            Btn_IsVisiblePasConfirm = new DelegateCommand(Click_IsVisiblePasConfirm);
            Btn_IsVisiblePassword = new DelegateCommand(Click_IsVisiblePassword);
            BackBtn = new DelegateCommand(BackClickAsync);

        }


        #region Public Property

        private string _errorPassText;
        public string ErrorPassText { get => _errorPassText; set { SetProperty(ref _errorPassText, value); } }


        private string _errorConfPassText;
        public string ErrorPassConfText { get => _errorConfPassText; set { SetProperty(ref _errorConfPassText, value); } }


        private string _passBorderColor;
        public string PassBorderColor { get => _passBorderColor; set { SetProperty(ref _passBorderColor, value); } }


        private string _passConfBorderColor;
        public string PassConfBorderColor { get => _passConfBorderColor; set { SetProperty(ref _passConfBorderColor, value); } }


        private string _color_RegistrBtn;
        public string Color_RegistrBtn { get => _color_RegistrBtn; set => SetProperty(ref _color_RegistrBtn, value); }


        private string _imagePasConfirm;
        public string ImagePasConfirm { get => _imagePasConfirm; set => SetProperty(ref _imagePasConfirm, value); }


        private string _imagePassword;
        public string ImagePassword { get => _imagePassword; set => SetProperty(ref _imagePassword, value); }


        private string _password;
        public string Password
        {
            get { IsRegistrEnable(); return _password; }
            set
            {
                SetProperty(ref _password, value);
                if (_password.Length > 0) { CheckPassword(value); }
            }
        }


        private string _passwordConfirm;
        public string PasswordConfirm
        {
            get { IsRegistrEnable(); return _passwordConfirm; }
            set
            {
                SetProperty(ref _passwordConfirm, value);
                if (_passwordConfirm.Length > 0) { CheckPassConfirm(value); }
            }
        }


        private bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }


        private bool _isVisiblePasConfirm;
        public bool IsVisiblePasConfirm { get => _isVisiblePasConfirm; set => SetProperty(ref _isVisiblePasConfirm, value); }


        private bool _isVisiblePassword;
        public bool IsVisiblePassword { get => _isVisiblePassword; set => SetProperty(ref _isVisiblePassword, value); }


        public DelegateCommand RegistrBtn { get; }
        public DelegateCommand GoogleBtn { get; }
        public DelegateCommand Btn_IsVisiblePasConfirm { get; }
        public DelegateCommand Btn_IsVisiblePassword { get; }
        public DelegateCommand BackBtn { get; }

        #endregion


        #region Private Method

        private async void Registr_Click()
        {
            Loginin log = new Loginin();
            log.name = _name;
            log.email = _email;
            log.password = Password;
            log.DateCreated = DateTime.Now;

            var result = await _registrService.RegistrAsync(log, PasswordConfirm);

            if (result.Item1)
            {
                UserDialogs.Instance.Alert(result.Item2, "Info", "Ok");

                Password = "";
                PasswordConfirm = "";
                NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", _email }
                                };
                await _navigationService.NavigateAsync("/LogInView", navParameters, animated: true);
            }
            else
            {
                Input_ErrorColor();
                UserDialogs.Instance.Alert(result.Item2, "Error", "Ok");
            }

            Password = "";
            PasswordConfirm = "";
        }

        private void CheckPassConfirm(string passConfirm)
        {
            PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorPassConfText = Resources.Resx.Resource.ErrorText_Password;
            if (!_verifyInput.PasswordCheckin(ref passConfirm))
            {
                PasswordConfirm = passConfirm;
            }
            else
            {
                if (_verifyInput.PasswordVerify(PasswordConfirm) && Password == PasswordConfirm)
                {
                    ErrorPassConfText = "Ok! " + Resources.Resx.Resource.ErrorText_Name;
                    PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    IsRegistrEnable();
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
                    IsRegistrEnable();
                }
            }
        }

        private void Click_IsVisiblePasConfirm()
        {
            if (IsVisiblePasConfirm)
            {
                IsVisiblePasConfirm = false;
                ImagePasConfirm = Constants.Constant_Auth.EYE;
            }
            else
            {
                IsVisiblePasConfirm = true;
                ImagePasConfirm = Constants.Constant_Auth.EYE_OFF;
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
            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            ErrorPassText = "";
            ErrorPassConfText = "";
            IsRegistrEnable();
        }

        private async void Google_HandlerAsync(string email, bool isReg)
        {
            _email = email;
            if (!isReg)
            {
                Loginin log = new Loginin();
                log.name = "Google_User";
                log.email = _email;
                log.password = Constants.Constant_Auth.GOOGLE_PASSWORD_USER;
                log.DateCreated = DateTime.Now;

                if (!await _registrService.RegistrGoogleAsync(log))
                {
                    UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_SavePin, "Error", "Ok");
                    return;
                }
                else
                    UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Ok_Reg + " Your password is "
                                               + Constants.Constant_Auth.GOOGLE_PASSWORD_USER, "Message", "Ok");
            }

            _settingsManager.Email = _email;
            NavigationParameters navParameters = new NavigationParameters { { "email", _email } };

            await _navigationService.NavigateAsync("/TabbedPageMy", navParameters, animated: true);

        }

        private void GoogleClick()
        {
            GoogleAuthCallBack googleAuthCallBack;
            googleAuthCallBack = Google_HandlerAsync;

            _authService.GoogleAuth(googleAuthCallBack);
        }

        private async void BackClickAsync()
        {
            await _navigationService.NavigateAsync("/RegistrView");
        }

        private bool IsRegistrEnable()//Enable disable "Registr" Button
        {

                IsEnabled = (PassBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN
                    && _name.Length >= 2
                    && _verifyInput.IsValidEmail(_email)
                    && PassConfBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN) 
                    ? IsEnabled = true : IsEnabled = false;

            if (IsEnabled)
                Color_RegistrBtn = Constants.Constant_Auth.OK_BTN_COLOR_OK;
            else
                Color_RegistrBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            return IsEnabled;
        }

        #endregion


        #region Interface InavigatedAword implementation

        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("email"))
            {
                var e = parameters.GetValue<Loginin>("email");
                _email = e.email;
                _name = e.name;
            }
            if (parameters.ContainsKey("google"))
            {
                GoogleClick();
            }
        }

        #endregion
    }
}
