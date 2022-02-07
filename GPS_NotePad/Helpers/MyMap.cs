using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace GPS_NotePad.Helpers
{
    class MyMap:Map
    {
        public List<MyPin> My_Pins { get; set; }
    }
}
