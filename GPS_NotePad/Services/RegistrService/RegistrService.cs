

using GPS_NotePad.Models;
using GPS_NotePad.Services.Repository;
using GPS_NotePad.Services.VerifyService;

using System.Linq;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.RegistrService
{

    class RegistrService : IRegistrService
    {

        private readonly IRepository _repository;
        private readonly IVerifyInputService _verifyInput;

        public RegistrService(IRepository repository,
                              IVerifyInputService verifyInputService)
        {
            _verifyInput = verifyInputService;
            _repository = repository;
        }

        private async Task<bool> InsertAsync(Loginin profile)
        {
            var res = await _repository.GetDataAsync<Loginin>("Loginin", profile.email);
            if (!res.Any())
            {
                return await _repository.InsertAsync<Loginin>(profile);
            }
            else
                return false;
        }

        public async Task<(bool, string)> RegistrAsync(Loginin profile, string passConfirm)
        {

            string str = "";

            if (_verifyInput.IsValidEmail(profile.email))
            {
                if (profile.password.Equals(passConfirm) && _verifyInput.PasswordVerify(profile.password)
                    && _verifyInput.PasswordVerify(passConfirm))
                {

                    if (await InsertAsync(profile))
                    {
                        str = Resources.Resx.Resource.Alert_Ok_Reg;
                        return (true, str);
                    }
                    else
                    {
                        str = Resources.Resx.Resource.Alert_Email1;
                    }
                }
                else
                {
                    str = Resources.Resx.Resource.Alert_Password2;
                }
            }
            else
            {
                str = Resources.Resx.Resource.Alert_Email3;
            }

            return (false, str);
        }

        public async Task<bool> RegistrGoogleAsync(Loginin profile)
        {
            if (_verifyInput.IsValidEmail(profile.email))
            {
                return await _repository.InsertAsync<Loginin>(profile);
            }
            else
                return false;
        }

    }
}
