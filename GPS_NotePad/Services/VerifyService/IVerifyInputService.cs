

namespace GPS_NotePad.Services.VerifyService
{
    interface IVerifyInputService
    {
        bool IsValidEmail(string email);
        bool EmailVerify(ref string str);
        bool NameVerify(ref string str);
        bool PasswordCheckin(ref string str);
        bool PasswordVerify(string str);
        bool PositionVerify(ref string str);
    }
}
