


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

        private readonly INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;
        private readonly ITo_RepositoryService _toRepository;
        private readonly IAuthGoogleService _authGoogleService;
        private bool _isVisibleEntry;
        private bool _isEnabled;
        private bool _isVisiblePasConfirm;
        private bool _isVisiblePassword;
        private string _name;
        private string _email;
        private string _password;
        private string _passwordConfirm;
        private string _imagePasConfirm;
        private string _imagePassword;
        private string _color_OkBtn;


        public MainPageViewModel(ITo_RepositoryService repository, INavigationService navigationService, IAuthGoogleService authGoogleService)
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
            _toRepository = repository;
            _authGoogleService = authGoogleService;

        }


        #region Public propertys

        public DelegateCommand OkBtn { get; private set; }
        public DelegateCommand LogininBtn { get; }
        public DelegateCommand RegistrBtn { get; }
        public DelegateCommand GoogleRegBtn { get; }
        public DelegateCommand Btn_IsVisiblePasConfirm { get; }
        public DelegateCommand Btn_IsVisiblePassword { get; }

        public string Color_OkBtn { get => _color_OkBtn; set => SetProperty(ref _color_OkBtn, value); }
        public string ImagePasConfirm { get => _imagePasConfirm; set => SetProperty(ref _imagePasConfirm, value); }
        public string ImagePassword { get => _imagePassword; set => SetProperty(ref _imagePassword, value); }
        public string Name
        {
            get { IsOkEnable(); return _name; }
            set
            {
                SetProperty(ref _name, value);
                if (_name.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.NameVerify(ref temp))//Verify name
                    {
                        Name = temp;
                        UserDialogs.Instance.Alert("Name must have from 2 to 16 symbols, A-Z, a-z", "Error", "Ok");
                    }
                }
            }
        }
        public string Email
        {
            get { IsOkEnable(); return _email; }
            set
            {
                SetProperty(ref _email, value);
                if (_email.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.EmailVerify(ref temp))
                    {
                        Email = temp;
                        UserDialogs.Instance.Alert("Email error", "Error", "Ok");
                    }
                }
            }
        }
        public string Password
        {
            get { IsOkEnable(); return _password; }
            set
            {
                SetProperty(ref _password, value);
                if (_password.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.PasswordCheckin(ref temp))
                    {
                        Password = temp;
                        UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A-Z, a-z, 1-9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
                    }
                }
            }
        }
        public string PasswordConfirm
        {
            get { IsOkEnable(); return _passwordConfirm; }
            set
            {
                SetProperty(ref _passwordConfirm, value);
                if (_passwordConfirm.Length > 0)
                {
                    string temp = value;
                    if (!_verifyInput.PasswordCheckin(ref temp))
                    {
                        PasswordConfirm = temp;
                        UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A-Z, a-z, 1-9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
                    }
                }
            }
        }
        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }
        public bool IsVisibleEntry { get => _isVisibleEntry; set { SetProperty(ref _isVisibleEntry, value); } }
        public bool IsVisiblePasConfirm { get => _isVisiblePasConfirm; set => SetProperty(ref _isVisiblePasConfirm, value); }
        public bool IsVisiblePassword { get => _isVisiblePassword; set => SetProperty(ref _isVisiblePassword, value); }

        #endregion

        #region Private method

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
            //check internet connection
            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
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
                        var res = await _toRepository.GetData<Loginin>("Loginin", _email);

                        if (res.Any())
                        {
                            if (res.First().password == Password)
                            {
                                NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", Email }
                                };
                                await _navigationService.NavigateAsync("/TabbedPageMy", navParameters, animated: true);

                                Email = "";
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
                    if (_verifyInput.PasswordVerify(Password) && _verifyInput.PasswordVerify(PasswordConfirm))
                    {
                        if (Password.Equals(PasswordConfirm))
                        {
                            Loginin log = new Loginin();
                            log.name = Name;
                            log.email = Email;
                            log.password = Password;
                            log.DateCreated = DateTime.Now;

                            if (await _toRepository.Insert(log))
                            {
                                LogininClick();
                                UserDialogs.Instance.Alert("OK", "", "Ok");
                            }
                            else
                            {
                                UserDialogs.Instance.Alert("Email already exist", "Error", "Ok");
                            }
                        }
                        else
                        {
                            UserDialogs.Instance.Alert("Passwords must be equal", "Error", "Ok");
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A-Z, a-z, 1-9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
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
            _authGoogleService.StartAuth();
        }

        private void RegistrClick()
        {
            IsVisibleEntry = true;
            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
        }
        private void LogininClick()
        {
            IsVisibleEntry = false;
            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
        }

        private void ChangeColor_OkBtn()
        {
            if (IsEnabled)
                Color_OkBtn = "Green";
            else
                Color_OkBtn = "LavenderBlush";
        }

        private bool IsOkEnable()
        {
            if (!IsVisibleEntry)
                IsEnabled = ((_email.Length > 6) && (_password.Length > 8 && _password.Length < 17)) ? IsEnabled = true : IsEnabled = false;
            else
                IsEnabled = ((_passwordConfirm.Length > 8 && _passwordConfirm.Length < 17) && (_email.Length > 6)
                            && (_password.Length > 8 && _password.Length < 17) && _passwordConfirm == _password) ?
                            IsEnabled = true : IsEnabled = false;
            ChangeColor_OkBtn();
            return IsEnabled;
        }

        #endregion

        #region Interface InavigatedAword implementation
        public void OnNavigatedFrom(INavigationParameters parameters)  { }
        public void OnNavigatedTo(INavigationParameters parameters)  { }
        #endregion
    }
}
