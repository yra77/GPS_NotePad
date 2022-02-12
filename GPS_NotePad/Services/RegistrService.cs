
using Acr.UserDialogs;
using GPS_NotePad.Helpers;
using GPS_NotePad.Models;
using GPS_NotePad.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GPS_NotePad.Services
{

    public interface IRegistrService
    {
        Task<bool> Registr(Loginin profile, string passConfirm);
    }

    class RegistrService : IRegistrService
    {

        private readonly IRepository _repository;
        private readonly IVerifyInputLogPas_Helper _verifyInput;

        public RegistrService(IRepository repository)
        {
            _verifyInput = new VerifyInput_Helper();
            _repository = repository;
            _repository.CreateTable<Loginin>();
        }

        private async Task<bool> Insert(Loginin profile)
        {
            var res = await _repository.GetData<Loginin>("Loginin", profile.email);
            if (!res.Any())
            {
                return await _repository.Insert<Loginin>(profile);
            }
            else
                return false;
        }

        public async Task<bool> Registr(Loginin profile, string passConfirm)
        {

            if (_verifyInput.IsValidEmail(profile.email))
            {
                if (profile.password.Equals(passConfirm) && _verifyInput.PasswordVerify(profile.password) && _verifyInput.PasswordVerify(passConfirm))
                {

                    if (await Insert(profile))
                    {
                        UserDialogs.Instance.Alert("OK! Are you registered.", "", "Ok");

                        return true;
                    }
                    else
                    {
                        UserDialogs.Instance.Alert("Email already exist", "Error", "Ok");
                    }
                }
                else
                {
                    UserDialogs.Instance.Alert("Passwords must be equals and have from 8 to 16 symbols, must contain at least one capital letter, one lowercase letter and one digit", "Error", "Ok");
                }
            }
            else
            {
                UserDialogs.Instance.Alert("Email is not valid", "Error", "Ok");
            }

            return false;
        }

    }
}
