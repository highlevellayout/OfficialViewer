using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace HLL
{
    public static class Networking
    {
        static WebClient client = new WebClient();


        public static string DownloadPage(string page)
        {
            if (IsValid(page))
                return client.DownloadString(page);
            else if(IsValid(page + "/index.hll"))
            {
                return client.DownloadString(page + "/index.hll");
            }
            return "[font, 20];\n[text, 5, 5, \"404 page " + page + " not found\"];";
        }

        static bool IsValid(string url)
        {
            WebRequest webRequest = WebRequest.Create(url);
            WebResponse webResponse;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch //If exception thrown then couldn't get response from address
            {
                return false;
            }
            return true;
        }
    }
}
