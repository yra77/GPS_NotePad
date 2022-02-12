


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

        public MainPageViewModel(IAuthService authService, INavigationService navigationService)
        {

            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
            IsVisibleEntry = false;
            IsEnabled = false;

            ImagePasConfirm = "eyeoff.png";
            ImagePassword = "eyeoff.png";
            IsVisiblePasConfirm = true;
            IsVisiblePassword = true;

            Color_OkBtn = "LavenderBlush";

            LogininBtn = new DelegateCommand(LogininClick);
            RegistrBtn = new DelegateCommand(RegistrClick);
            GoogleRegBtn = new DelegateCommand(GoogleClick);
            OkBtn = new DelegateCommand(Ok_Click, IsOkEnable).ObservesProperty(() => IsEnabled);
            Btn_IsVisiblePasConfirm = new DelegateCommand(Click_IsVisiblePasConfirm);
            Btn_IsVisiblePassword = new DelegateCommand(Click_IsVisiblePassword);

            _navigationService = navigationService;
            _verifyInput = new VerifyInput_Helper();
            _authService = authService;

            EmailBorderColor = "Gray";
            PassBorderColor = "Gray";
            PassConfBorderColor = "Gray";
            NameBorderColor = "Gray";
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
            set { SetProperty(ref _name, value); 
                if (_name.Length > 0) { CheckName(value); } }
        }

        public string Email
        {
            get { IsOkEnable(); return _email; }
            set { SetProperty(ref _email, value);
                 if (_email.Length > 0) { CheckEmail(value); } }
        }

        public string Password
        {
            get { IsOkEnable(); return _password; }
            set { SetProperty(ref _password, value);
                  if (_password.Length > 0) { CheckPassword(value); } }
        }

        public string PasswordConfirm
        {
            get { IsOkEnable(); return _passwordConfirm; }
            set { SetProperty(ref _passwordConfirm, value);
                  if(_passwordConfirm.Length > 0) { CheckPassConfirm(value); } }
        }

        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }
        public bool IsVisibleEntry { get => _isVisibleEntry; set { SetProperty(ref _isVisibleEntry, value); } }
        public bool IsVisiblePasConfirm { get => _isVisiblePasConfirm; set => SetProperty(ref _isVisiblePasConfirm, value); }
        public bool IsVisiblePassword { get => _isVisiblePassword; set => SetProperty(ref _isVisiblePassword, value); }

        #endregion

        #region Private method

        void CheckName(string temp)
        {
            NameBorderColor = "Red";
            ErrorNameText = "Must have 2-16 symbols, A-Z, a-z";
            if (!_verifyInput.NameVerify(ref temp))
            {
                Name = temp;
            }
            else if (Name.Length >= 2)
            {
                ErrorNameText = "Ok! Must have 2-16 symbols, A-Z, a-z";
                NameBorderColor = "Green";
                IsOkEnable();
            }
        }

        void CheckEmail(string temp)
        {
            EmailBorderColor = "Red";
            ErrorEmailText = "Email. Must have - aaa@aa.aa";

            if (!_verifyInput.EmailVerify(ref temp))
            {
                Email = temp;
            }
            else
            {
                if (_verifyInput.IsValidEmail(Email))
                {
                    ErrorEmailText = "Ok!";
                    EmailBorderColor = "Green";
                    IsOkEnable();
                }
            }
        }

        void CheckPassConfirm(string temp)
        {
            PassConfBorderColor = "Red";
            ErrorPassConfText = " 8-16 symbols one capital letter, one lowercase letter, one digit";
            if (!_verifyInput.PasswordCheckin(ref temp))
            {
                PasswordConfirm = temp;
            }
            else
            {
                if (_verifyInput.PasswordVerify(PasswordConfirm) && Password == PasswordConfirm)
                {
                    ErrorPassConfText = "Ok! Must have from 8 to 16 symbols";
                    PassConfBorderColor = "Green";
                    IsOkEnable();
                }
            }
        }

        void CheckPassword(string temp)
        {
            PassBorderColor = "Red";
            ErrorPassText = " 8-16 symbols one capital letter, one lowercase letter, one digit";
            if (!_verifyInput.PasswordCheckin(ref temp))
            {
                Password = temp;
            }
            else
            {
                if (_verifyInput.PasswordVerify(Password) && (Password.Length > 7 && Password.Length < 17))
                {
                    ErrorPassText = "Ok!";
                    PassBorderColor = "Green";
                    IsOkEnable();
                }
            }
        }

        private void Click_IsVisiblePasConfirm()
        {
            if (IsVisiblePasConfirm)
            {
                IsVisiblePasConfirm = false;
                ImagePasConfirm = "eye.png";
            }
            else
            {
                IsVisiblePasConfirm = true;
                ImagePasConfirm = "eyeoff.png";
            }
        }
        private void Click_IsVisiblePassword()
        {
            if (IsVisiblePassword)
            {
                IsVisiblePassword = false;
                ImagePassword = "eye.png";
            }
            else
            {
                IsVisiblePassword = true;
                ImagePassword = "eyeoff.png";
            }
        }

        private async void Ok_Click()
        {
            
            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)//check internet connection
            {
                UserDialogs.Instance.Alert("No connection to the internet", "Error", "Ok");
                await Task.Delay(10000);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }

            if (_verifyInput.IsValidEmail(Email))
                if (!IsVisibleEntry)
                {
                    if (_verifyInput.PasswordVerify(Password))
                    {
                        var res = await _authService.GetData<Loginin>("Loginin", _email);

                        if (res.Any())
                        {
                            if (res.First().password == Password)
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
                                UserDialogs.Instance.Alert("Password do not exist", "Error", "Ok");
                            }
                        }
                        else
                        {
                            UserDialogs.Instance.Alert("Email do not exist", "Error", "Ok");
                            Email = "";
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A - Z, a - z, 1 - 9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
                    }
                    Password = "";
                }
                else
                {
                    if (Password.Equals(PasswordConfirm) && _verifyInput.PasswordVerify(Password) && _verifyInput.PasswordVerify(PasswordConfirm))
                    {
                            Loginin log = new Loginin();
                            log.name = Name;
                            log.email = Email;
                            log.password = Password;
                            log.DateCreated = DateTime.Now;

                            if (await _authService.Insert(log))
                            {
                                LogininClick();
                                EmailBorderColor = "Green";
                                UserDialogs.Instance.Alert("OK! Are you registered.", "", "Ok");
                            }
                            else
                            {
                                UserDialogs.Instance.Alert("Email already exist", "Error", "Ok");
                                Email = "";
                            }
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("Passwords must be equals and have from 8 to 16 symbols, must contain at least one capital letter, one lowercase letter and one digit", "Error", "Ok");
                    }
                    Password = "";
                    PasswordConfirm = "";
                }
            else
            {
                UserDialogs.Instance.Alert("Email is not valid", "Error", "Ok");
                Email = "";
            }
        }

        private void GoogleClick()
        {
            _authService.GoogleAuth();
        }
        private void Input_ErrorColor()
        {
            EmailBorderColor = "Gray";
            PassBorderColor = "Gray";
            PassConfBorderColor = "Gray";
            NameBorderColor = "Gray";
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
                IsEnabled = (PassBorderColor == "Green" && EmailBorderColor == "Green")? IsEnabled = true : IsEnabled = false;
            else
                IsEnabled = (PassBorderColor == "Green" && NameBorderColor == "Green" && EmailBorderColor == "Green" 
                             && PassConfBorderColor == "Green")? IsEnabled = true : IsEnabled = false;

            if (IsEnabled)
                Color_OkBtn = "Green";
            else
                Color_OkBtn = "LavenderBlush";

            return IsEnabled;
        }

        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters) { }
        public void OnNavigatedTo(INavigationParameters parameters) { }
        #endregion
    }
}
