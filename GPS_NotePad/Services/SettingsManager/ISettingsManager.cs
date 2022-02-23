using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.Services.SettingsManager
{
    interface ISettingsManager
    {
        string Email { get; set; }
        string Language { get; set; }
    }
}
