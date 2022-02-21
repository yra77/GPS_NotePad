﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace GPS_NotePad.Services.SettingsManager
{
    class SettingsManager : ISettingsManager
    {
        public string Email 
        { 
            get => Preferences.Get(nameof(Email), null); 
            set => Preferences.Set(nameof(Email), value); 
        }



    }
}