using HtmlAgilityPack;
using StockAnalysis.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StockAnalysis.CheckBaoCao
{
    public class VnDirect
    {

        public void GetBaocao()
        {
            string[] maCks = FileUtility.ReadAllMack();
            int length = maCks.Length;

            string startQuy = "Q" + Constants.QUY_HIEN_TAI;

            for (int i = 0; i < length; i++)
            {
                string mack = maCks[i];
                for (int j = 0; j < 3; j++) // Vòng for duyệt cho 3 năm
                {
                    string param1 = "searchObject.fiscalQuarter=" + startQuy;

                    string param2 = "&searchObject.fiscalYear=" + (Constants.NAM_HIEN_TAI - i);
                    string myParameters = param1 + param2 + "& searchObject.moneyRate=1,000,000&searchObject.numberTerm=4";


                    string url = "https://www.vndirect.com.vn/portal/bang-can-doi-ke-toan/" + mack + ".shtml";
                    string result = NetworkUtility.SendPostRequest(url, myParameters);
                    ReadBangCanDoiKeToan(result, mack);

                }
            }
        }

        private void ReadBangCanDoiKeToan(string htmlContent, string mack)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//b");
            List<StringBuilder> listResult = InidResult(collection, mack);
            if (listResult.Count != 4)
            {
                MessageBox.Show("Khởi tạo thiếu data, khởi tạo được được " + listResult.Count + " quý");
                Environment.Exit(0);
            }
            collection = document.DocumentNode.SelectNodes("//tr");
            foreach (HtmlNode node in collection)
            {
                if (node.Attributes["class"] != null)
                {
                    string value = node.Attributes["class"].Value;
                    string content = node.WriteContentTo().ToLower();
                    List<long> listValue = null;
                    switch (value)
                    {
                        case "lv1":
                            break;
                        case "lv2":
                            if (content.Contains("nợ ngắn hạn"))
                            {
                                listValue = ExtractData(node);
                                for (int i = 0; i < listValue.Count; i++)
                                {
                                    AppendValue(listResult[i], Fields.NoNganHan, listValue[i].ToString());
                                }
                            }
                            else if (content.Contains("nợ dài hạn"))
                            {
                                listValue = ExtractData(node);
                                for (int i = 0; i < listValue.Count; i++)
                                {
                                    AppendValue(listResult[i], Fields.NoDaiHan, listValue[i].ToString());
                                }
                            }
                            break;
                        case "lv3":
                            if (content.Contains("vốn đầu tư của chủ sở hữu"))
                            {
                                listValue = ExtractData(node);
                                for (int i = 0; i < listValue.Count; i++)
                                {
                                    AppendValue(listResult[i], Fields.VonGopCuaChuSoHuu, listValue[i].ToString());
                                }
                            }
                            break;
                    }
                }
            }
        }

        private List<long> ExtractData(HtmlNode node)
        {
            List<long> listValue = new List<long>();
            HtmlNodeCollection NodesLv1s = node.ChildNodes;
            foreach (HtmlNode NodesLv1 in NodesLv1s)
            {
                if (!NodesLv1.HasChildNodes)
                {
                    continue;
                }
                HtmlNodeCollection NodesLv2s = NodesLv1.ChildNodes;
                foreach (HtmlNode NodesLv2 in NodesLv2s)
                {
                    if (NodesLv2.Attributes["style"] != null)
                    {
                        string attvalue = NodesLv2.Attributes["style"].Value;
                        if (attvalue.Contains("text-align: right;"))
                        {
                            string content = NodesLv2.WriteContentTo();
                            content = content.Replace(" ", "");
                            try
                            {
                                if (content == "")
                                {
                                    long value = 0;
                                    listValue.Add(value);
                                }
                                else
                                {
                                    long value = Convert.ToInt64(content.Replace(",", ""));
                                    listValue.Add(value);
                                }
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("Có exception khi tách data từ html: " + e.Message);
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            if (listValue[4] == 0)
            {
                listValue.RemoveAt(4);
            }
            if (listValue.Count != 4)
            {
                MessageBox.Show("Tách data sai!, lấy ra được " + listValue.Count + " trường");
                Environment.Exit(0);
            }
            return listValue;
        }
        private List<StringBuilder> InidResult(HtmlNodeCollection collection, string mack)
        {
            List<StringBuilder> listResult = new List<StringBuilder>();
            foreach (HtmlNode node in collection)
            {
                string content = node.WriteContentTo();
                content = content.Trim();
                if (content == "")
                {
                    continue;
                }

                StringBuilder s = new StringBuilder();
                AppendValue(s, Fields.MaCk, mack);
                AppendValue(s, Fields.Quy, content.Substring(1, 1)); // Q1/2017
                AppendValue(s, Fields.Nam, content.Substring(3, 4)); // Q1/2017
                listResult.Add(s);
            }
            return listResult;
        }

        private void AppendValue(StringBuilder s, string field, string value)
        {
            s.Append(field + "_" + value + ";");
        }
    }
}
