﻿
using GPS_NotePad.Models;
using System.Threading.Tasks;

namespace GPS_NotePad.Services.RegistrService
{
    public interface IRegistrService
    {
        Task<(bool, string)> RegistrAsync(Loginin profile, string passConfirm);
        Task<bool> RegistrGoogleAsync(Loginin profile);
    }
}
