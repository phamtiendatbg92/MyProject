using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockAnalysis.Utility;
using HtmlAgilityPack;
using System.IO;

namespace StockAnalysis.CheckBaoCao
{
    public class VietStock
    {
        private List<StringBuilder> BctcVietStock;
        //http://finance.vietstock.vn/Controls/Report/Data/GetReport.ashx?rptType=KQKD&scode=AAA&bizType=1&rptUnit=1&rptTermTypeID=2&page=1

        public delegate void ReportProgressHandler(int percent);
        public event ReportProgressHandler ReportProgressCallback;
        public void ReportProgress(int percent)
        {
            ReportProgressCallback?.Invoke(percent);
        }
        
        public void DownLoadData()
        {
            BctcVietStock = new List<StringBuilder>();
            // giá trị 1 bắt đầu từ quý 1 năm 2017 (Mới nhất)

            string[] maCks = FileUtility.ReadAllMack();
            int length = maCks.Length;
            length = 1;
            for (int i = 0; i < length; i++)
            {
                for (int j = 1; j <= 3; j++) // Vòng for duyệt cho 3 năm
                {
                    string htmlContent = GetBaoCao(maCks[i], j);
                    ReadHtmlContentOfVietStock(htmlContent, j, maCks[i]);
                }
                ReportProgress(i * 100 / length);
            }
            FileUtility.WriteResultToTextFile(BctcVietStock, "resultVietStock.txt");
        }

        
        private void ReadHtmlContentOfVietStock(string htmlContent, int pageNo, string mack)
        {
            int nam = Constants.NAM_HIEN_TAI;
            int quy = Constants.QUY_HIEN_TAI;
            switch (pageNo)
            {
                case 1:
                    nam = Constants.NAM_HIEN_TAI;
                    break;
                case 2:
                    nam = Constants.NAM_HIEN_TAI - 1;
                    break;
                case 3:
                    nam = Constants.NAM_HIEN_TAI - 2;
                    break;
            }



            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//tr");
            if (collection == null)
            {
                return;
            }

            // Get header
            HtmlNodeCollection headerNode = document.DocumentNode.SelectNodes("//td");
            List<string> listQuyTemp = new List<string>();
            List<StringBuilder> data = new List<StringBuilder>();
            foreach (HtmlNode node in headerNode)
            {
                if (node.Attributes["class"] != null)
                {
                    string value = node.Attributes["class"].Value;
                    if (value == "BR_colHeader_Time")
                    {
                        try
                        {
                            string content = node.WriteContentTo();
                            int quyTemp = Convert.ToInt32(content.Substring(4, 1));
                            int namTemp = Convert.ToInt32(content.Substring(6, 4));
                            string abc = quyTemp + "_" + namTemp;
                            if (!listQuyTemp.Contains(abc))
                            {
                                listQuyTemp.Add(abc);
                                StringBuilder s = FileUtility.InitResultData(mack, namTemp.ToString(), quyTemp.ToString());
                                data.Add(s);
                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }

            foreach (HtmlNode node in collection)
            {
                if (node.Attributes["id"] != null)
                {
                    string value = node.Attributes["id"].Value;
                    List<long> listValues = null;
                    switch (value)
                    {
                        #region Read data
                        case "2206":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.DoanhThuBanHangVaCungCapDichVu, listValues[i].ToString());
                            }
                            break;
                        case "2212":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.LoiNhuanSauThue, listValues[i].ToString());
                            }
                            break;
                        case "2217":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.LoiNhuanGop, listValues[i].ToString());
                            }
                            break;
                        case "2227":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.ChiPhiBanHang, listValues[i].ToString());
                            }
                            break;
                        case "2223":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.ChiPhiLaiVay, listValues[i].ToString());
                            }
                            break;
                        case "3063":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.VonGopCuaChuSoHuu, listValues[i].ToString());
                            }
                            break;
                        case "3014":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.NoNganHan, listValues[i].ToString());
                            }
                            break;
                        case "3017":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    FileUtility.AppendValue(data[i], Fields.NoDaiHan, listValues[i].ToString());
                            }
                            break;
                            #endregion
                    }
                }
            }
            for (int i = 0; i < data.Count; i++)
            {
                BctcVietStock.Add(data[i]);
            }
        }

        private string GetBaoCao(string mack, int pageNo)
        {
            string s1 = "http://finance.vietstock.vn/Controls/Report/Data/GetReport.ashx?rptType=KQKD&scode=";
            string s2 = "&bizType=1&rptUnit=1&rptTermTypeID=2&page=";
            // giá trị 1 bắt đầu từ quý 1 năm 2017 (Mới nhất)
            string url = s1 + mack + s2 + pageNo.ToString();
            string content = NetworkUtility.GetHtmlSource(url);

            s1 = "http://finance.vietstock.vn/Controls/Report/Data/GetReport.ashx?rptType=CDKT&scode=";
            url = s1 + mack + s2 + pageNo.ToString();

            content += NetworkUtility.GetHtmlSource(url);

            return content;
        }
        private List<long> ExtractVietStockXmlData(HtmlNode link)
        {
            List<long> listValue = new List<long>();
            HtmlNodeCollection tdNodes = link.ChildNodes;
            foreach (HtmlNode node in tdNodes)
            {
                if (node.Attributes["align"] != null)
                {
                    if (node.Attributes["align"].Value == "right")
                    {
                        string content = node.WriteContentTo();
                        content = content.Replace(" ", "");
                        content = content.Replace("-", "");
                        content = content.Replace("(", "-");
                        content = content.Replace(")", "");
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
                        catch (Exception)
                        {
                            Console.WriteLine("=============={0}", content.Replace(",", ""));
                        }

                    }
                }
            }
            return listValue;
        }
    }
}
