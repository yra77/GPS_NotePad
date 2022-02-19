﻿
using GPS_NotePad.ViewModels;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.Interfaces
{
    public interface IAuthService
    {
        Task<(bool, string)> AuthAsync(string password, string email);
        void GoogleAuth(GoogleAuthCallBack myDel);
    }

}
