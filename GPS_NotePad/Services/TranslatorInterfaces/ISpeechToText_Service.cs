using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.Services.TranslatorInterfaces
{
    public interface ISpeechToText_Service
    {
        void SpeechToText();
        void StopListening();
    }
}
