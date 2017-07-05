using HtmlAgilityPack;
using StockAnalysis.CheckBaoCao;
using StockAnalysis.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.CheckBaoCao
{
    public class CompareData
    {
        private List<string> DataErrorLog = new List<string>();


        public delegate void ReportProgressHandler(int percent);
        public event ReportProgressHandler ReportProgressCallback;
        public void ReportProgress(int percent)
        {
            ReportProgressCallback?.Invoke(percent);
        }

        public void DoCompare()
        {
            Dictionary<string, List<string>> m_vietstockResult = ReadContentResult(Constants.CONTENT_RESULT_VIETSTOCK_FILE_NAME);
            Dictionary<string, List<string>> m_vndirectResult = ReadContentResult(Constants.CONTENT_RESULT_VNDIRECT_FILE_NAME);

            List<bctc> listBaoCao = DatabaseUtility.GetAllBctc2();
            int count = listBaoCao.Count;
            for (int i = 0; i < count; i++)
            {
                bctc baoCao = listBaoCao[i];
                string contentVietStock = FindResultString(m_vietstockResult[baoCao.mack], baoCao.mack, baoCao.Nam.ToString(), baoCao.Quy.ToString());
                string contentVndirect = FindResultString(m_vndirectResult[baoCao.mack], baoCao.mack, baoCao.Nam.ToString(), baoCao.Quy.ToString());
                CompareValue(baoCao, contentVietStock, contentVndirect);
            }
        }
        private void CompareValue(bctc baoCao, string contentVietStock, string contentVndirect)
        {
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.NoNganHan);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.NoDaiHan);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.VonGopCuaChuSoHuu);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.DoanhThuBanHangVaCungCapDichVu);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.LoiNhuanGop);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.ChiPhiLaiVay);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.ChiPhiBanHang);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.ChiPhiQuanLyDoanhNghiep);
            CompareOneValue(baoCao, contentVietStock, contentVndirect, Fields.LoiNhuanSauThue);
        }

        private void CompareOneValue(bctc baoCao, string contentVietStock, string contentVndirect, string field)
        {
            long valueVietStock = 0;
            long valueVndirect = 0;
            valueVietStock = ExtractValue(contentVietStock, field);
            valueVndirect = ExtractValue(contentVndirect, field);
            long correctValue = 0;
            switch (field)
            {
                case Fields.NoNganHan:
                    correctValue = GetCorrectValue(baoCao.C1_NoNganHan, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C1_NoNganHan, baoCao, field);
                    baoCao.C1_NoNganHan = correctValue;
                    break;
                case Fields.NoDaiHan:
                    correctValue = GetCorrectValue((long)baoCao.C2_NoDaiHan, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C2_NoDaiHan, baoCao, field);
                    baoCao.C2_NoDaiHan = correctValue;
                    break;
                case Fields.VonGopCuaChuSoHuu:
                    correctValue = GetCorrectValue((long)baoCao.C1_1_VonGopCuaChuSoHuu, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C1_1_VonGopCuaChuSoHuu, baoCao, field);
                    baoCao.C1_1_VonGopCuaChuSoHuu = correctValue;
                    break;
                case Fields.DoanhThuBanHangVaCungCapDichVu:
                    correctValue = GetCorrectValue((long)baoCao.C1_DoanhThuBanHangVaCungcapDV, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C1_DoanhThuBanHangVaCungcapDV, baoCao, field);
                    baoCao.C1_DoanhThuBanHangVaCungcapDV = correctValue;
                    break;
                case Fields.LoiNhuanGop:
                    correctValue = GetCorrectValue((long)baoCao.C5_LoiNhuanGopVeBanHangVaCungCapDV, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C5_LoiNhuanGopVeBanHangVaCungCapDV, baoCao, field);
                    baoCao.C5_LoiNhuanGopVeBanHangVaCungCapDV = correctValue;
                    break;
                case Fields.ChiPhiLaiVay:
                    correctValue = GetCorrectValue((long)baoCao.C7_1_ChiPhiLaiVay, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C7_1_ChiPhiLaiVay, baoCao, field);
                    baoCao.C7_1_ChiPhiLaiVay = correctValue;
                    break;
                case Fields.ChiPhiBanHang:
                    correctValue = GetCorrectValue((long)baoCao.C9_ChiPhiBanHang, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C9_ChiPhiBanHang, baoCao, field);
                    baoCao.C9_ChiPhiBanHang = correctValue;
                    break;
                case Fields.ChiPhiQuanLyDoanhNghiep:
                    correctValue = GetCorrectValue((long)baoCao.C10_ChiPhiQuanLyDoanhNghiep, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C10_ChiPhiQuanLyDoanhNghiep, baoCao, field);
                    baoCao.C10_ChiPhiQuanLyDoanhNghiep = correctValue;
                    break;
                case Fields.LoiNhuanSauThue:
                    correctValue = GetCorrectValue((long)baoCao.C18_LoiNhuanSauThueTNDN, valueVietStock, valueVndirect);
                    SetErrorlog(correctValue, (long)baoCao.C18_LoiNhuanSauThueTNDN, baoCao, field);
                    baoCao.C18_LoiNhuanSauThueTNDN = correctValue;
                    break;
            }
        }
        private void SetErrorlog(long correctValue, long currentValue, bctc baoCao, string field)
        {
            if (correctValue != currentValue)
            {
                DataErrorLog.Add(baoCao.mack + "_" + baoCao.Quy + "_" + baoCao.Nam + "_" + field + "_" + correctValue);
            }
        }

        private long GetCorrectValue(long valueCafef, long valueVietStock, long valueVndirect)
        {
            if (valueCafef <= valueVndirect + 1000 && valueCafef >= valueVndirect - 1000) // cho phép nhập sai số 1000đ
            {
                return valueCafef;
            }
            else if (valueCafef <= valueVietStock + 1000 && valueCafef >= valueVietStock - 1000)
            {
                return valueCafef;
            }
            else
            {
                if (valueVndirect != 0)
                {
                    return valueVndirect;
                }
                else if (valueVietStock != 0)
                {
                    return valueVietStock;
                }
                else
                {
                    return valueCafef;
                }
            }
        }
        private long ExtractValue(string content, string fieldName)
        {
            long result = 0;
            string[] contentArr = content.Split(';');
            for (int i = 0; i < contentArr.Length; i++)
            {
                if (contentArr[i].Contains(fieldName))
                {
                    string value = contentArr[i].Split('_')[1];
                    if (value != "")
                    {
                        result = Convert.ToInt64(contentArr[i].Split('_')[1]);
                    }
                    break;
                }
            }
            return result;
        }

        private string FindResultString(List<string> listStringResult, string mack, string nam, string quy)
        {
            string result = "";
            for (int i = 0; i < listStringResult.Count; i++)
            {
                string[] temp = listStringResult[i].Split(';');
                string mack_temp = temp[0].Split('_')[1];
                string nam_temp = temp[2].Split('_')[1];
                string quy_temp = temp[1].Split('_')[1];
                if (mack == mack_temp &&
                    nam == nam_temp &&
                    quy == quy_temp)
                {
                    return result;
                }
            }
            return result;
        }

        private Dictionary<string, List<string>> ReadContentResult(string fileName)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            string[] contentArr = File.ReadAllLines(fileName);
            int length = contentArr.Length;
            for (int i = 0; i < length; i++)
            {
                string[] temp = contentArr[i].Split(';');
                string mack = temp[0].Split('_')[1];
                if (result.ContainsKey(mack))
                {
                    result[mack].Add(contentArr[i]);
                }
                else
                {
                    List<string> abc = new List<string>();
                    abc.Add(contentArr[i]);
                    result.Add(mack, abc);
                }
            }
            return result;
        }
    }
}
