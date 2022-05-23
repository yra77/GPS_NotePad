

using System;
using System.Net;
using Xamarin.Essentials;


namespace GPS_NotePad.Helpers
{

    public class CheckingDeviceProperty_Helper
    {

        public static bool CheckNetwork()//check internet connection
        {

            NetworkAccess current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)
            {
                return false;
            }
            else
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://g.cn/generate_204");
                    request.UserAgent = "Android";
                    request.KeepAlive = false;
                    request.Timeout = 1500;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.ContentLength == 0 && response.StatusCode == HttpStatusCode.NoContent)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

    }
}
