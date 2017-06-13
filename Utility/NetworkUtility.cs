using System;
using System.Collections.Generic;
using System.IO;
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

        public static string SendPostRequest(string url, string parameters)
        {
            WebClient client = new WebClient();
            WebProxy wp = new WebProxy("proxy1.tsdv.com.vn", 3128);
            wp.Credentials = new NetworkCredential("datpt", "tsdv2015");
            client.Encoding = Encoding.UTF8;
            client.Proxy = wp;

            string result = "";
            StreamWriter myWriter = null;

            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Method = "POST";
            objRequest.ContentLength = parameters.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            objRequest.Proxy = wp;
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(parameters);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                myWriter.Close();
            }

            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
               new StreamReader(objResponse.GetResponseStream()))
            {

                result = sr.ReadToEnd();
                result = result.Replace("\t", string.Empty);
                result = result.Replace("\n", string.Empty);
                result = result.Replace("&nbsp;", string.Empty);
                result = ClearSpace(result);
                // Close and clean up the StreamReader
                sr.Close();
            }
            return result;
        }
        private static string ClearSpace(string s)
        {
            StringBuilder s2 = new StringBuilder(s);

            int lenght = s.Length;

            for (int i = lenght - 2; i > 0; i--)
            {
                if (s[i] == ' ' && s[i - 1] == ' ')
                {
                    s2.Remove(i, 1);
                }
            }
            return s2.ToString();
        }
    }
}
