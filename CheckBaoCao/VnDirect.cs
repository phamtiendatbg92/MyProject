using HtmlAgilityPack;
using StockAnalysis.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.CheckBaoCao
{
    public class VnDirect
    {

        public void GetBaocao()
        {
            string[] maCks = FileUtility.ReadAllMack();
            int length = maCks.Length;
            string url = "https://www.vndirect.com.vn/portal/bang-can-doi-ke-toan/aaa.shtml";
            string startQuy = "Q";
            if (Constants.QUY_HIEN_TAI == 4)
            {
                startQuy += 1;
            }
            else
            {
                startQuy += Constants.QUY_HIEN_TAI + 1;
            }


            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < 3; j++) // Vòng for duyệt cho 3 năm
                {
                    string param1 = "searchObject.fiscalQuarter=" + startQuy;

                    string param2 = "&searchObject.fiscalYear=" + (Constants.NAM_HIEN_TAI - i);
                    if (Constants.QUY_HIEN_TAI == 4)
                    {
                        param2 += Constants.NAM_HIEN_TAI - i;
                    }
                    else
                    {
                        param2 += Constants.NAM_HIEN_TAI - i - 1;
                    }
                    string myParameters = param1 + param2 + "& searchObject.moneyRate=1,000,000&searchObject.numberTerm=4";
                    string result = NetworkUtility.SendPostRequest(url, myParameters);
                    ReadBaoCao(result);
                }
            }
        }

        private void ReadBaoCao(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//tr");
            foreach (HtmlNode node in collection)
            {
                if (node.Attributes["id"] != null)
                {
                }
            }
        }
    }
}
