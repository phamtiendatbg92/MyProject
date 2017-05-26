using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.Utility
{
    public static class NetworkUtility
    {
        public static string GetHtmlSource(string url)
        {
            WebClient client = new WebClient();
            WebProxy wp = new WebProxy("proxy1.tsdv.com.vn", 3128);
            wp.Credentials = new NetworkCredential("datpt", "tsdv2015");
            client.Encoding = Encoding.UTF8;
            client.Proxy = wp;
            string downloadString = client.DownloadString(url);
            return downloadString;
        }
    }
}
