using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StockAnalysis.CheckBaoCao;
using System.Runtime.Serialization.Json;

namespace StockAnalysis.Utility
{
    public static class NetworkUtility
    {
        private static WebProxy m_webProxy;

        public static void SettingProxy()
        {
            m_webProxy = new WebProxy("proxy1.tsdv.com.vn", 3128);
            m_webProxy.Credentials = new NetworkCredential("datpt", "tsdv2015");
        }
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
            WebProxy wp = new WebProxy("proxy1.tsdv.com.vn", 3128);
            wp.Credentials = new NetworkCredential("datpt", "tsdv2015");

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
                myWriter.Flush();
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


        private const string AJAX_URL = "https://www.vndirect.com.vn/portal/ajax/listed/SearchIncomeStatement.shtml";
        private static CookieContainer cookies;
        public static string SendAjaxRequestForVndirec(string url2, string parameters)
        {
            string result = "";

            cookies = new CookieContainer();

            Cookie id = JoinToWebPage(url2);
            result = SendAjaxRequest(id, parameters);

            return result;

        }
        private static string SendAjaxRequest(Cookie id, string parameters)
        {
            string result = "";
            try
            {
                HttpWebRequest webRequest22222 = (HttpWebRequest)WebRequest.Create(AJAX_URL);
                webRequest22222.CookieContainer = cookies;
                WebHeaderCollection header = new WebHeaderCollection();
                header.Add(id.Name, id.Value);
                webRequest22222.KeepAlive = false;
                webRequest22222.Headers = header;
                webRequest22222.Method = "POST";
                webRequest22222.ContentLength = parameters.Length;
                webRequest22222.ContentType = "application/x-www-form-urlencoded";
                webRequest22222.Proxy = m_webProxy;
                using (var reqStream = webRequest22222.GetRequestStream())
                {
                    if (reqStream != null)
                    {
                        StreamWriter myWriter = new StreamWriter(reqStream);
                        myWriter.Write(parameters);
                        myWriter.Flush();
                        myWriter.Close();
                    }
                }
                HttpWebResponse response2222 = (HttpWebResponse)webRequest22222.GetResponse();

                using (StreamReader sr =
                   new StreamReader(response2222.GetResponseStream()))
                {
                    result = sr.ReadToEnd();
                    result = result.Replace("\t", string.Empty);
                    result = result.Replace("\n", string.Empty);
                    result = result.Replace("&nbsp;", string.Empty);
                    result = ClearSpace(result);
                    sr.Close();
                }
                response2222.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("sleeppppppppppppppppppppppp {0}", e.Message);
                System.Threading.Thread.Sleep(1000);
            }
            return result;
        }
        private static Cookie JoinToWebPage(string url2)
        {
            HttpWebRequest webRequest1111 = (HttpWebRequest)WebRequest.Create(url2);
            webRequest1111.CookieContainer = cookies;
            webRequest1111.Proxy = m_webProxy;

            HttpWebResponse response = (HttpWebResponse)webRequest1111.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            CookieCollection cookieCollection = response.Cookies;
            Cookie id = cookieCollection[0];
            cookies.Add(id);
            response.Close();
            return id;
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
