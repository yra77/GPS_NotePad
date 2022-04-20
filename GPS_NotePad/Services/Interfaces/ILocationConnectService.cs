using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.Services.Interfaces
{
    public interface ILocationConnectService
    {
        void OpenSettings();

        bool IsGpsAvailable();
    }
}
