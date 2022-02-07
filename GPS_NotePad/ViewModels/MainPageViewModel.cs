


using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Auth;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using GPS_NotePad.ViewModels.Helpers;
using Acr.UserDialogs;
using GPS_NotePad.Models;
using GPS_NotePad.ViewModels.Services;
using System.Threading.Tasks;

namespace GPS_NotePad.ViewModels
{
    //486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk.apps.googleusercontent.com
    //AIzaSyDcSVIjErUsWelFNRTTqKJimfh9lDj7JJ0   -  maps google
    class MainPageViewModel : BindableBase, INavigatedAware
    {
        private readonly INavigationService navigationService;
        private readonly IVerifyInputLogPas_Helper verifyInput;
        private readonly ITo_RepositoryService toRepository;
        private readonly IAuthGoogleService authGoogleService;
        bool entryIsVisible;
        bool _isEnabled;
        string name;
        string email;
        string password;
        string passwordConfirm;


        public MainPageViewModel(ITo_RepositoryService _repository, INavigationService _navigationService, IAuthGoogleService _authGoogleService)
        {
           
            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
            EntryIsVisible = false;
            IsEnabled = false;
            LogininBtn = new DelegateCommand(LogininClick);
            RegistrBtn = new DelegateCommand(RegistrClick);
            GoogleRegBtn = new DelegateCommand(GoogleClick);
            OkBtn = new DelegateCommand(Ok_Click, IsOkEnable).ObservesProperty(() => IsEnabled);

            navigationService = _navigationService;
            verifyInput = new VerifyInput_Helper();
            toRepository = _repository;
            authGoogleService = _authGoogleService;
            
        }

        public DelegateCommand OkBtn { get; private set; }
        public DelegateCommand LogininBtn { get; }
        public DelegateCommand RegistrBtn { get; }
        public DelegateCommand GoogleRegBtn { get; }


        public string Name { get { IsOkEnable(); return name; } 
                             set 
                                 { 
                                   SetProperty(ref name, value);
                if (name.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.NameVerify(ref temp))//Verify name
                    {
                        Name = temp;
                        UserDialogs.Instance.Alert("Name must have from 2 to 16 symbols, A-Z, a-z", "Error", "Ok");
                    }
                }
            }
         }

        public string Email { get { IsOkEnable(); return email; } 
                              set { 
                                    SetProperty(ref email, value);
                if (email.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.EmailVerify(ref temp))
                    {
                        Email = temp;
                        UserDialogs.Instance.Alert("Email error", "Error", "Ok");
                    }
                }
            } 
        }
        public string Password { get { IsOkEnable(); return password; } 
                                 set 
                                     { 
                                       SetProperty(ref password, value);
                if (password.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.PasswordCheckin(ref temp))
                    {
                        Password = temp;
                        UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A-Z, a-z, 1-9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
                    }
                }
            } 
        }
        public string PasswordConfirm { get { IsOkEnable(); return passwordConfirm; }
                                        set 
                                            {  
                                              SetProperty(ref passwordConfirm, value);
                if (passwordConfirm.Length > 0)
                {
                    string temp = value;
                    if (!verifyInput.PasswordCheckin(ref temp))
                    {
                        PasswordConfirm = temp;
                        UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A-Z, a-z, 1-9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
                    }
                }
            }
        }
        public bool IsEnabled { get { return _isEnabled; } set { SetProperty(ref _isEnabled, value); } }
        public bool EntryIsVisible { get => entryIsVisible; set { SetProperty(ref entryIsVisible, value); } }


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


            if (verifyInput.IsValidEmail(Email))
            if(!EntryIsVisible)
            {
                    if (verifyInput.PasswordVerify(Password))
                    {
                        var res = await toRepository.GetData<Loginin>("Loginin", email);
                        
                        if (res.Any())
                        {
                            if (res.First().password == Password)
                            {
                                NavigationParameters navParameters = new NavigationParameters
                                {
                                    { "email", Email }
                                };
                                await navigationService.NavigateAsync("/TabbedPageMy", navParameters, animated: true);

                                Email = "";
                                Password = "";
                            }
                            else
                            {
                                UserDialogs.Instance.Alert("Password do not exist", "Error", "Ok");
                                Password = "";
                            }
                        }
                        else
                        {
                            UserDialogs.Instance.Alert("Email do not exist", "Error", "Ok");
                            Password = "";
                            Email = "";
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A - Z, a - z, 1 - 9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
                        Password = "";
                    }
            }
            else
            {
                if (verifyInput.PasswordVerify(Password) && verifyInput.PasswordVerify(PasswordConfirm))
                {
                    if (Password.Equals(PasswordConfirm))
                    {
                            Loginin log = new Loginin();
                            log.name = Name;
                            log.email = Email;
                            log.password = Password;
                            log.DateCreated = DateTime.Now;

                            if (await toRepository.Insert(log))
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
                        Password = "";
                        PasswordConfirm = "";
                    }
                }
                else
                {
                    UserDialogs.Instance.Alert("Password must have from 8 to 16 symbols, A-Z, a-z, 1-9.Password must contain at least one capital letter, one lowercase letter and one number", "Error", "Ok");
                    Password = "";
                    PasswordConfirm = "";
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Email is not valid", "Error", "Ok");
                Email = "";
            }

        }

        private void GoogleClick()
        {
            authGoogleService.StartAuth();
        }
        private void RegistrClick()
        {
            EntryIsVisible = true;
            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
        }
        private void LogininClick()
        {
            EntryIsVisible = false;
            Name = "";
            PasswordConfirm = "";
            Email = "";
            Password = "";
        }
        private bool IsOkEnable()
        {
            if(!EntryIsVisible)
                IsEnabled = ((email.Length > 6) && (password.Length > 8 && password.Length < 17)) ? IsEnabled = true : IsEnabled = false;
            else
                IsEnabled = ((passwordConfirm.Length > 8 && passwordConfirm.Length < 17) && (email.Length > 6) 
                            && (password.Length > 8 && password.Length < 17) && passwordConfirm == password) ? 
                            IsEnabled = true : IsEnabled = false;
            
            return IsEnabled;
        }


        public void OnNavigatedFrom(INavigationParameters parameters)
        {
          
        }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
           
        }
    }
}
