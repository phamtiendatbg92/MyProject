using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static string SendAjaxRequestForVndirec(string url, string parameters)
        {
            WebProxy wp = new WebProxy("proxy1.tsdv.com.vn", 3128);
            wp.Credentials = new NetworkCredential("datpt", "tsdv2015");

            parameters = "searchObject.fiscalQuarter=Q1&searchObject.fiscalYear=2016&searchObject.moneyRate=1,000,000&searchObject.numberTerm=5";
            url = "https://www.vndirect.com.vn/portal/ajax/listed/SearchIncomeStatement.shtml";
            string url2 = "https://www.vndirect.com.vn/portal/bao-cao-ket-qua-kinh-doanh/aaa.shtml";


            CookieContainer cookies = new CookieContainer();
            HttpWebRequest webRequest1111 = (HttpWebRequest)WebRequest.Create(url2);
            webRequest1111.CookieContainer = cookies;
            webRequest1111.Proxy = wp;

            HttpWebResponse response = (HttpWebResponse)webRequest1111.GetResponse();
            StreamReader responseReader = new StreamReader(response.GetResponseStream());

            
            string json = "{\"searchObject.fiscalQuarter\": \"Q1\", \"searchObject.fiscalYear\": \"2016\", \"searchObject.moneyRate\": \"1,000,000\", \"searchObject.numberTerm\": \"5\"}";
            

            HttpWebRequest webRequest22222 = (HttpWebRequest)WebRequest.Create(url);
            webRequest22222.CookieContainer = cookies;
            webRequest22222.Proxy = wp;
            webRequest22222.Method = "POST";
            webRequest22222.ContentLength = json.Length;
            webRequest22222.ContentType = "application/json; charset=utf-8";
            webRequest22222.Proxy = wp;
            StreamWriter myWriter = null;
            try
            {
                myWriter = new StreamWriter(webRequest22222.GetRequestStream());
                myWriter.Write(json);
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



            HttpWebResponse response2222 = (HttpWebResponse)webRequest22222.GetResponse();
            string result;
            using (StreamReader sr =
               new StreamReader(response2222.GetResponseStream()))
            {

                result = sr.ReadToEnd();
                result = result.Replace("\t", string.Empty);
                result = result.Replace("\n", string.Empty);
                result = result.Replace("&nbsp;", string.Empty);
                result = ClearSpace(result);
                File.WriteAllText("tttttttttttttt.txt",result);
                // Close and clean up the StreamReader
                sr.Close();
            }

            return "";

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
