
using GPS_NotePad.Helpers;
using GPS_NotePad.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;


namespace GPS_NotePad.ViewModels
{
    class RegistrViewModel : BindableBase, INavigatedAware
    {

        INavigationService _navigationService;
        private readonly IVerifyInputLogPas_Helper _verifyInput;


        public RegistrViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _verifyInput = new VerifyInput_Helper();

            Name = "";
            Email = "";
            NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;
            EmailBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_White;

            Color_NextBtn = Constants.Constant_Auth.OK_BTN_COLOR;

            IsEnabled = false;

            BackBtn = new DelegateCommand(BackClickAsync);
            GoogleBtn = new DelegateCommand(GoogleClickAsync);
            NextBtn = new DelegateCommand(Next_ClickAsync, IsNextEnable).ObservesProperty(() => IsEnabled);
        }


        #region Public Property

        private string _errorEmailText;
        public string ErrorEmailText { get => _errorEmailText; set { SetProperty(ref _errorEmailText, value); } }


        private string _errorNameText;
        public string ErrorNameText { get => _errorNameText; set { SetProperty(ref _errorNameText, value); } }

        
        private string _emailBorderColor;
        public string EmailBorderColor { get => _emailBorderColor; set { SetProperty(ref _emailBorderColor, value); } }


        private string _nameBorderColor;
        public string NameBorderColor { get => _nameBorderColor; set { SetProperty(ref _nameBorderColor, value); } }


        private string _color_NextBtn;
        public string Color_NextBtn { get => _color_NextBtn; set => SetProperty(ref _color_NextBtn, value); }


        private string _name;
        public string Name
        {
            get { IsNextEnable(); return _name; }
            set
            {
                SetProperty(ref _name, value);
                if (_name.Length > 0) { CheckName(value); }
            }
        }


        private string _email;
        public string Email
        {
            get { IsNextEnable(); return _email; }
            set
            {
                SetProperty(ref _email, value);
                if (_email.Length > 0) { CheckEmail(_email); }
            }
        }


        private bool _isEnabled;
        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }

        public DelegateCommand NextBtn { get; private set; }
        public DelegateCommand GoogleBtn { get; }
        public DelegateCommand BackBtn { get; }

        #endregion


        #region Private method

        private void CheckName(string name)
        {
            NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_RED;
            ErrorNameText = Resources.Resx.Resource.ErrorText_Name;

            if (!_verifyInput.NameVerify(ref name))
            {
                Name = name;
            }
            else if (Name.Length >= 2)
            {
                ErrorNameText = "Ok! " + Resources.Resx.Resource.ErrorText_Name;
                NameBorderColor = Constants.Constant_Auth.ENTRY_BORDER_COLOR_GREEN;
                IsNextEnable();
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
                    IsNextEnable();
                }
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
                Color_NextBtn = Constants.Constant_Auth.OK_BTN_COLOR_OK;
            else
                Color_NextBtn = Constants.Constant_Auth.OK_BTN_COLOR;

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
    }
}
