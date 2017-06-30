using HtmlAgilityPack;
using Newtonsoft.Json;
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
        public delegate void ReportProgressHandler(int percent);
        public event ReportProgressHandler ReportProgressCallback;
        public void ReportProgress(int percent)
        {
            ReportProgressCallback?.Invoke(percent);
        }

        public void DownLoadData()
        {
            string[] maCks = FileUtility.ReadAllMack();
            int length = maCks.Length;
            string startQuy = "Q" + Constants.QUY_HIEN_TAI;
            totalResultVndirect = new List<StringBuilder>();
            for (int i = 0; i < length; i++)
            {
                string mack = maCks[i];
                for (int j = 0; j < 3; j++) // Vòng for duyệt cho 3 năm
                {
                    string param1 = "searchObject.fiscalQuarter=" + startQuy;

                    string param2 = "&searchObject.fiscalYear=" + (Constants.NAM_HIEN_TAI - j);
                    string myParameters = param1 + param2 + "& searchObject.moneyRate=1&searchObject.numberTerm=4";

                    string url = "https://www.vndirect.com.vn/portal/bang-can-doi-ke-toan/" + mack + ".shtml";
                    string resultCanDoiKeToan = NetworkUtility.SendPostRequest(url, myParameters);
                    ReadBangCanDoiKeToan(resultCanDoiKeToan, mack);
                    string url2 = "https://www.vndirect.com.vn/portal/bao-cao-ket-qua-kinh-doanh/" + mack + ".shtml";
                    string resultKQKD = NetworkUtility.SendAjaxRequestForVndirec(url2, myParameters);
                    ReadKQKD(resultKQKD, mack);
                    Console.WriteLine("{0}", i * 3 + j);
                    // write result to total result
                    for (int k = 0; k < listResult.Count; k++)
                    {
                        totalResultVndirect.Add(listResult[k]);
                    }
                }
                ReportProgress(i * 100 / length);
            }
            // write result to text file
            FileUtility.WriteResultToTextFile(totalResultVndirect, "VndirectResult.txt");
        }
        private List<StringBuilder> listResult;
        private List<StringBuilder> totalResultVndirect;
        private void ReadBangCanDoiKeToan(string htmlContent, string mack)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//b");
            listResult = InidResult(collection, mack);
            if (listResult.Count != 4)
            {
                MessageBox.Show("Khởi tạo thiếu data, khởi tạo được được " + listResult.Count + " quý!!!" + mack);
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
                                    FileUtility.AppendValue(listResult[i], Fields.NoNganHan, listValue[i].ToString());
                                }
                            }
                            else if (content.Contains("nợ dài hạn"))
                            {
                                listValue = ExtractData(node);
                                for (int i = 0; i < listValue.Count; i++)
                                {
                                    FileUtility.AppendValue(listResult[i], Fields.NoDaiHan, listValue[i].ToString());
                                }
                            }
                            break;
                        case "lv3":
                            if (content.Contains("vốn đầu tư của chủ sở hữu"))
                            {
                                listValue = ExtractData(node);
                                for (int i = 0; i < listValue.Count; i++)
                                {
                                    FileUtility.AppendValue(listResult[i], Fields.VonGopCuaChuSoHuu, listValue[i].ToString());
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void ReadKQKD(string htmlContent, string mack)
        {
            RootObject rootObject = new RootObject();
            var ab = JsonConvert.DeserializeObject<RootObject>(htmlContent);
            List<FinanceInfoList> financeInfoList = ab.model.financeInfoList;

            int count = financeInfoList.Count;
            for (int i = 0; i < financeInfoList.Count; i++)
            {
                string item = financeInfoList[i].itemName;
                switch (item)
                {
                    case "Tổng doanh thu hoạt động kinh doanh":
                        AppendValueFromJsonObject(financeInfoList[i], Fields.DoanhThuBanHangVaCungCapDichVu);
                        break;
                    case "Lợi nhuận sau thuế thu nhập doanh nghiệp":
                        AppendValueFromJsonObject(financeInfoList[i], Fields.LoiNhuanSauThue);
                        break;
                    case "Lợi nhuận gộp":
                        AppendValueFromJsonObject(financeInfoList[i], Fields.LoiNhuanGop);
                        break;
                    case "Chi phí bán hàng":
                        AppendValueFromJsonObject(financeInfoList[i], Fields.ChiPhiBanHang);
                        break;
                    case "Chi phí quản lý doanh nghiệp":
                        AppendValueFromJsonObject(financeInfoList[i], Fields.ChiPhiQuanLyDoanhNghiep);
                        break;
                    case "Trong đó: Chi phí lãi vay":
                        AppendValueFromJsonObject(financeInfoList[i], Fields.ChiPhiLaiVay);
                        break;
                }
            }
        }
        private void AppendValueFromJsonObject(FinanceInfoList financeObject, string field)
        {
            FileUtility.AppendValue(listResult[0], field, financeObject.strNumericValue1.Replace(",", ""));
            FileUtility.AppendValue(listResult[1], field, financeObject.strNumericValue2.Replace(",", ""));
            FileUtility.AppendValue(listResult[2], field, financeObject.strNumericValue3.Replace(",", ""));
            FileUtility.AppendValue(listResult[3], field, financeObject.strNumericValue4.Replace(",", ""));
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
                FileUtility.AppendValue(s, Fields.MaCk, mack);
                FileUtility.AppendValue(s, Fields.Quy, content.Substring(1, 1)); // Q1/2017
                FileUtility.AppendValue(s, Fields.Nam, content.Substring(3, 4)); // Q1/2017
                listResult.Add(s);
            }
            return listResult;
        }


    }
}
