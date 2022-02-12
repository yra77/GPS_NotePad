using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GPS_NotePad.Helpers
{
    public interface IVerifyInputLogPas_Helper
    {
        bool IsValidEmail(string email);
        bool EmailVerify(ref string str);
        bool NameVerify(ref string str);
        bool PasswordCheckin(ref string str);
        bool PasswordVerify(string str);
    }

    class VerifyInput_Helper : IVerifyInputLogPas_Helper
    {
        public bool IsValidEmail(string email)
        {
                Regex regex =
             new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
             RegexOptions.CultureInvariant | RegexOptions.Singleline);
                
               // Console.WriteLine($"The email is {email}");
               
                bool isValidEmail = regex.IsMatch(email);
               
                //if (!isValidEmail)
                //{
                //    Console.WriteLine($"The email is invalid");
                //}
                //else
                //{
                //    Console.WriteLine($"The email is valid");
                //}
                return isValidEmail;

        }
    
        public bool EmailVerify(ref string str)
        {
            bool flag = true;
            string temp = str;
            int codeASCII = 0;

            for (int i = 0; i < temp.Length; i++)
            {
                codeASCII = Char.ConvertToUtf32(temp[i].ToString(), 0);
                if ((codeASCII == 45) || (codeASCII == 95) || (codeASCII == 64) || (codeASCII == 46) || (codeASCII == 38) || 
                    (codeASCII == 36) || (codeASCII > 47 && codeASCII < 58) || (codeASCII > 64 && codeASCII < 91) || 
                    (codeASCII > 96 && codeASCII < 123))
                {
                    continue;
                    // Console.WriteLine(Char.ConvertToUtf32(str[i].ToString(), 0));
                }
                else
                {
                    temp = temp.Remove(i, 1);
                    flag = false;
                }

            }
            str = temp;
            return flag;
        }
        public bool NameVerify(ref string str)
        {
            bool flag = true;
            string temp = str;
            int codeASCII = 0;

            for (int i = 0; i < temp.Length; i++)
            {
                codeASCII = Char.ConvertToUtf32(temp[i].ToString(), 0);
                if ((codeASCII > 64 && codeASCII < 91) || (codeASCII > 96 && codeASCII < 123))
                {
                    continue;
                    // Console.WriteLine(Char.ConvertToUtf32(str[i].ToString(), 0));
                }
                else
                {
                    temp = temp.Remove(i, 1);
                    flag = false;
                }

            }
            str = temp;
            return flag;
        }

        public bool PasswordCheckin(ref string str)
        {
            bool flag = true;
            string temp = str;
            int codeASCII = 0;

            for (int i = 0; i < temp.Length; i++)
            {
                codeASCII = Char.ConvertToUtf32(temp[i].ToString(), 0);
                if ((codeASCII > 47 && codeASCII < 58) || (codeASCII > 64 && codeASCII < 91) || (codeASCII > 96 && codeASCII < 123))
                {
                    continue;
                }
                else
                {
                    temp = temp.Remove(i, 1);
                    flag = false;
                }

            }
            str = temp;
            return flag;
        }

        public bool PasswordVerify(string str)
        {

            bool isUpper = false;
            bool isLower = false;
            bool isDigit = false;

            for (int i = 0; i < str.Length; i++)
            {
                int codeASCII = Char.ConvertToUtf32(str[i].ToString(), 0);

                if (codeASCII > 47 && codeASCII < 58)
                {
                    isDigit = true;
                }
                if (codeASCII > 64 && codeASCII < 91)
                {
                    isUpper = true;
                }
                if (codeASCII > 96 && codeASCII < 123)
                {
                    isLower = true;
                }
            }

            return (isDigit && isUpper && isLower);
        }
    }
}
