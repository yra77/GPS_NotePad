


using GPS_NotePad.Helpers;
using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.Services;

using Xamarin.Essentials;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GPS_NotePad.ViewModels
{
    //486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk.apps.googleusercontent.com
    //AIzaSyDcSVIjErUsWelFNRTTqKJimfh9lDj7JJ0   -  maps google android
    //AIzaSyDzt_zSeQ_rK0TR2ClYHraBm7Yrg83JhDU - ios
    class MainPageViewModel : BindableBase, INavigatedAware
    {

        #region Private helpers

        private readonly INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private readonly IAuthService _authService;
        private readonly IRegistrService _registrService;
        private string _name;
        private string _email;
        private string _password;
        private string _passwordConfirm;
        private string _imagePasConfirm;
        private string _imagePassword;
        private string _color_OkBtn;

        private string _errorEmailText;
        private string _errorPassText;
        private string _errorNameText;
        private string _errorConfPassText;

        private string _nameBorderColor;
        private string _emailBorderColor;
        private string _passBorderColor;
        private string _passConfBorderColor;

        private bool _isVisibleEntry;
        private bool _isEnabled;
        private bool _isVisiblePasConfirm;
        private bool _isVisiblePassword;

        #endregion

        public MainPageViewModel(IAuthService authService, IRegistrService registrService, INavigationService navigationService)
        {

            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
            IsVisibleEntry = false;
            IsEnabled = false;

            ImagePasConfirm = Constants.Constant_Auth.EYE_OFF;
            ImagePassword = Constants.Constant_Auth.EYE_OFF;
            IsVisiblePasConfirm = true;
            IsVisiblePassword = true;

            Color_OkBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            LogininBtn = new DelegateCommand(LogininClick);
            RegistrBtn = new DelegateCommand(RegistrClick);
            GoogleRegBtn = new DelegateCommand(GoogleClick);
            OkBtn = new DelegateCommand(Ok_Click, IsOkEnable).ObservesProperty(() => IsEnabled);
            Btn_IsVisiblePasConfirm = new DelegateCommand(Click_IsVisiblePasConfirm);
            Btn_IsVisiblePassword = new DelegateCommand(Click_IsVisiblePassword);

            _navigationService = navigationService;
            _verifyInput = new VerifyInput_Helper();
            _authService = authService;
            _registrService = registrService;

            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
            PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
            NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
        }


        #region Public propertys

        public DelegateCommand OkBtn { get; private set; }
        public DelegateCommand LogininBtn { get; }
        public DelegateCommand RegistrBtn { get; }
        public DelegateCommand GoogleRegBtn { get; }
        public DelegateCommand Btn_IsVisiblePasConfirm { get; }
        public DelegateCommand Btn_IsVisiblePassword { get; }

        public string ErrorEmailText { get => _errorEmailText; set { SetProperty(ref _errorEmailText, value); } }
        public string ErrorPassText { get => _errorPassText; set { SetProperty(ref _errorPassText, value); } }
        public string ErrorPassConfText { get => _errorConfPassText; set { SetProperty(ref _errorConfPassText, value); } }
        public string ErrorNameText { get => _errorNameText; set { SetProperty(ref _errorNameText, value); } }
        public string EmailBorderColor { get => _emailBorderColor; set { SetProperty(ref _emailBorderColor, value); } }
        public string PassBorderColor { get => _passBorderColor; set { SetProperty(ref _passBorderColor, value); } }
        public string PassConfBorderColor { get => _passConfBorderColor; set { SetProperty(ref _passConfBorderColor, value); } }
        public string NameBorderColor { get => _nameBorderColor; set { SetProperty(ref _nameBorderColor, value); } }

        public string Color_OkBtn { get => _color_OkBtn; set => SetProperty(ref _color_OkBtn, value); }
        public string ImagePasConfirm { get => _imagePasConfirm; set => SetProperty(ref _imagePasConfirm, value); }
        public string ImagePassword { get => _imagePassword; set => SetProperty(ref _imagePassword, value); }
        public string Name
        {
            get { IsOkEnable(); return _name; }
            set
            {
                SetProperty(ref _name, value);
                if (_name.Length > 0) { CheckName(value); }
            }
        }

        public string Email
        {
            get { IsOkEnable(); return _email; }
            set
            {
                SetProperty(ref _email, value);
                if (_email.Length > 0) { CheckEmail(value); }
            }
        }

        public string Password
        {
            get { IsOkEnable(); return _password; }
            set
            {
                SetProperty(ref _password, value);
                if (_password.Length > 0) { CheckPassword(value); }
            }
        }

        public string PasswordConfirm
        {
            get { IsOkEnable(); return _passwordConfirm; }
            set
            {
                SetProperty(ref _passwordConfirm, value);
                if (_passwordConfirm.Length > 0) { CheckPassConfirm(value); }
            }
        }

        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }
        public bool IsVisibleEntry { get => _isVisibleEntry; set { SetProperty(ref _isVisibleEntry, value); } }
        public bool IsVisiblePasConfirm { get => _isVisiblePasConfirm; set => SetProperty(ref _isVisiblePasConfirm, value); }
        public bool IsVisiblePassword { get => _isVisiblePassword; set => SetProperty(ref _isVisiblePassword, value); }

        #endregion


        #region Private method

        void CheckName(string temp)
        {
            NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorNameText = Resources.Resx.Resource.ErrorText_Name;

            if (!_verifyInput.NameVerify(ref temp))
            {
                Name = temp;
            }
            else if (Name.Length >= 2)
            {
                ErrorNameText = "Ok! " + Resources.Resx.Resource.ErrorText_Name;
                NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                IsOkEnable();
            }
        }

        void CheckEmail(string temp)
        {
            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorEmailText = Resources.Resx.Resource.ErrorText_Email;

            if (!_verifyInput.EmailVerify(ref temp))
            {
                Email = temp;
            }
            else
            {
                if (_verifyInput.IsValidEmail(Email))
                {
                    ErrorEmailText = "Ok!";
                    EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    IsOkEnable();
                }
            }
        }

        void CheckPassConfirm(string temp)
        {
            PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorPassConfText = Resources.Resx.Resource.ErrorText_Password;
            if (!_verifyInput.PasswordCheckin(ref temp))
            {
                PasswordConfirm = temp;
            }
            else
            {
                if (_verifyInput.PasswordVerify(PasswordConfirm) && Password == PasswordConfirm)
                {
                    ErrorPassConfText = "Ok! " + Resources.Resx.Resource.ErrorText_Name;
                    PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    IsOkEnable();
                }
            }
        }

        void CheckPassword(string temp)
        {
            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorPassText = Resources.Resx.Resource.ErrorText_Password;
            if (!_verifyInput.PasswordCheckin(ref temp))
            {
                Password = temp;
            }
            else
            {
                if (_verifyInput.PasswordVerify(Password) && (Password.Length > 7 && Password.Length < 17))
                {
                    ErrorPassText = "Ok!";
                    PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                    IsOkEnable();
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

        private async void Ok_Click()
        {

            ICheckingDeviceProperty_Helper checkingDeviceProperty = new CheckingDeviceProperty_Helper();
            await checkingDeviceProperty.CheckingDeviceProperty();


            if (!IsVisibleEntry)
            {
                if (await _authService.Auth(Password, Email))
                {
                    NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", Email }
                                };
                    await _navigationService.NavigateAsync("/TabbedPageMy", navParameters, animated: true);

                    Password = "";
                }
                else
                {
                    Password = "";
                    Email = "";
                    Input_ErrorColor();
                }
            }
            else
            {

                Loginin log = new Loginin();
                log.name = Name;
                log.email = Email;
                log.password = Password;
                log.DateCreated = DateTime.Now;

                if (await _registrService.Registr(log, PasswordConfirm))
                {
                    LogininClick();
                    EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                }
                else
                {
                    UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Email1, "Error", "Ok");
                    Email = "";
                }

                Password = "";
                PasswordConfirm = "";
            }
        }

        private void GoogleClick()
        {
            _authService.GoogleAuth();
        }
        private void Input_ErrorColor()
        {
            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
            PassBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
            PassConfBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
            NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GRAY;
            ErrorEmailText = "";
            ErrorPassText = "";
            ErrorNameText = "";
            ErrorPassConfText = "";
        }

        private void RegistrClick()
        {
            Input_ErrorColor();
            IsVisibleEntry = true;
            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
        }
        private void LogininClick()
        {
            Input_ErrorColor();
            IsVisibleEntry = false;
            Name = "";
            PasswordConfirm = "";
            // Email = "";
            Password = "";
        }

        private bool IsOkEnable()//Enable disable "Ok" Button
        {
            if (!IsVisibleEntry)
                IsEnabled = (PassBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN 
                    && EmailBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN) ? IsEnabled = true : IsEnabled = false;
            else
                IsEnabled = (PassBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN 
                    && NameBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN
                    && EmailBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN
                    && PassConfBorderColor == Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN) ? IsEnabled = true : IsEnabled = false;

            if (IsEnabled)
                Color_OkBtn = Constants.Constant_Auth.OK_BTN_COLOR_GREEN;
            else
                Color_OkBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            return IsEnabled;
        }

        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters) { }
        #endregion
    }
}
