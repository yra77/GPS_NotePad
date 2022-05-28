

using System;
using System.Net;
using Xamarin.Essentials;


namespace GPS_NotePad.Helpers
{

    public class CheckingDeviceProperty_Helper
    {

        public static bool CheckNetwork()
        {

            NetworkAccess current = Connectivity.NetworkAccess;

            if (current != NetworkAccess.Internet)//is access to network
            {
                return false;
            }
            else//check internet connection
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
