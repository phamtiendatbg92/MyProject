using HtmlAgilityPack;
using StockAnalysis.CheckBaoCao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.Utility
{
    public class CompareData
    {
        private Dictionary<string, List<bctc>> dataOfVietStock = new Dictionary<string, List<bctc>>();
        private Dictionary<string, List<bctc>> dataOfVndirect = new Dictionary<string, List<bctc>>();

        public delegate void ReportProgressHandler(int percent);
        public event ReportProgressHandler ReportProgressCallback;
        public void ReportProgress(int percent)
        {
            ReportProgressCallback?.Invoke(percent);
        }
        private void InitBCTC()
        {
            string[] maCks = GetAllMack();
            int length = maCks.Length;


            for (int i = 0; i < length; i++)
            {
                string mack = maCks[i];
                int quy = Constants.QUY_HIEN_TAI;
                int nam = Constants.NAM_HIEN_TAI;
                dataOfVietStock.Add(mack, new List<bctc>());
                dataOfVndirect.Add(mack, new List<bctc>());

                for (int j = 0; j < 12; j++) // Vòng for duyệt cho 3 năm
                {
                    bctc baoCao = new bctc();
                    baoCao.Nam = nam;
                    baoCao.Quy = quy;

                    bctc baoCao2 = new bctc();
                    baoCao2.Nam = nam;
                    baoCao2.Quy = quy;

                    dataOfVietStock[mack].Add(baoCao);
                    dataOfVndirect[mack].Add(baoCao2);

                    if (quy == 1)
                    {
                        quy = 4;
                        nam--;
                    }
                    else
                    {
                        quy--;
                    }
                }
            }
        }
        public CompareData()
        {
            //InitBCTC();
        }
        public void DoCompare(bool isHaveData)
        {
            // nếu chưa có file data thì phải download về trước
            if (!isHaveData)
            {
                DownLoadData();
                File.WriteAllText("VietStockData.txt", BctcVietStock.ToString());
            }
            ReadAndCompareData();
        }

        private void ReadAndCompareData()
        {
            string[] vietStockDataStringArr = File.ReadAllLines("VietStockData.txt");
        }
        private void DownLoadData()
        {
            CheckByVietStock checkByVietStock = new CheckByVietStock();
            // giá trị 1 bắt đầu từ quý 1 năm 2017 (Mới nhất)

            string[] maCks = GetAllMack();
            int length = maCks.Length;

            for (int i = 0; i < length; i++)
            {
                for (int j = 1; j <= 3; j++) // Vòng for duyệt cho 3 năm
                {
                    string htmlContent = checkByVietStock.GetBaoCao(maCks[i], j);
                    ReadHtmlContentOfVietStock(htmlContent, j, maCks[i]);
                }
                ReportProgress(i * 100 / length);
            }
        }

        private string[] GetAllMack()
        {
            string maCkString = File.ReadAllText("MaCK.txt");
            string[] maCks = maCkString.Split('_');
            return maCks;
        }

        private StringBuilder BctcVietStock = new StringBuilder();
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
                                StringBuilder s = InitResultData(mack, namTemp, quyTemp);
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
                                    data[i].Append(Fields.DoanhThuBanHangVaCungCapDichVu + "_" + listValues[i] + ";");
                            }
                            break;
                        case "2212":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    data[i].Append(Fields.LoiNhuanSauThue + "_" + listValues[i] + ";");
                            }
                            break;
                        case "2217":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    data[i].Append(Fields.LoiNhuanGop + "_" + listValues[i] + ";");
                            }
                            break;
                        case "2227":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    data[i].Append(Fields.ChiPhiBanHang + "_" + listValues[i] + ";");
                            }
                            break;
                        case "2223":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    data[i].Append(Fields.ChiPhiLaiVay + "_" + listValues[i] + ";");
                            }
                            break;
                        case "3063":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    data[i].Append(Fields.VonGopCuaChuSoHuu + "_" + listValues[i] + ";");
                            }
                            break;
                        case "3014":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    data[i].Append(Fields.NoNganHan + "_" + listValues[i] + ";");
                            }
                            break;
                        case "3017":
                            listValues = ExtractVietStockXmlData(node);
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (i < listValues.Count)
                                    data[i].Append(Fields.NoDaiHan + "_" + listValues[i] + ";");
                            }
                            break;
                            #endregion
                    }
                }
            }
            for (int i = 0; i < data.Count; i++)
            {
                BctcVietStock.Append(data[i] + "\n");
            }
        }
        private StringBuilder InitResultData(string mack, int nam, int quy)
        {

            StringBuilder s = new StringBuilder();
            s.Append(Fields.MaCk + "_" + mack + ";");
            s.Append(Fields.Quy + "_" + quy + ";");
            s.Append(Fields.Nam + "_" + nam + ";");

            return s;
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
