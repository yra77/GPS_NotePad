using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.Constants
{
    public class Constant_Auth
    {
        public static string AppName = "GPS_NotePad";

        // 486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk.apps.googleusercontent.com
        // For Google login, configure at https://console.developers.google.com/
        public static string iOSClientId = "844060696235-gtoiepn6u6trvaoh5s6uo1a1a3hrcrnq.apps.googleusercontent.com";
        public static string AndroidClientId = "486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk.apps.googleusercontent.com";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "com.googleusercontent.apps.844060696235-gtoiepn6u6trvaoh5s6uo1a1a3hrcrnq:/oauth2redirect";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.486491599259-9qkacj16rv2j7rkpoe1upa1ou0gg65nk:/oauth2redirect";

        //Images
        public const string EYE = "eye.png";
        public const string EYE_OFF = "eyeoff.png";


        //Color string
        public const string OK_BTN_COLOR = "Gray";
        public const string OK_BTN_COLOR_OK = "#7485FB";
        public const string ENTRY_BORDER_COLOR_GRAY = "Gray";
        public const string ENTRY_BORDER_COLOR_GREEN = "Green";
        public const string ENTRY_BORDER_COLOR_RED = "Red";

    }
}
