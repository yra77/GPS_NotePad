
using Acr.UserDialogs;
using GPS_NotePad.Helpers;
using GPS_NotePad.Models;
using GPS_NotePad.Repository;
using System.Linq;
using System.Threading.Tasks;


namespace GPS_NotePad.Services
{

    public interface IRegistrService
    {
        Task<bool> Registr(Loginin profile, string passConfirm);
        Task<bool> RegistrGoogle(Loginin profile);
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
                if (profile.password.Equals(passConfirm) && _verifyInput.PasswordVerify(profile.password)
                    && _verifyInput.PasswordVerify(passConfirm))
                {

                    if (await Insert(profile))
                    {
                        UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Ok_Reg, "", "Ok");

                        return true;
                    }
                    else
                    {
                        UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Email1, "Error", "Ok");
                    }
                }
                else
                {
                    UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Password2, "Error", "Ok");
                }
            }
            else
            {
                UserDialogs.Instance.Alert(Resources.Resx.Resource.Alert_Email3, "Error", "Ok");
            }

            return false;
        }

        public async Task<bool> RegistrGoogle(Loginin profile)
        {
            if (_verifyInput.IsValidEmail(profile.email))
            {
                return await _repository.Insert<Loginin>(profile);
            }
            else
                return false;
        }

    }
}
