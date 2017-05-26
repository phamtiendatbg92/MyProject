using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockAnalysis.Utility;
namespace StockAnalysis.CheckBaoCao
{
    public class CheckByVietStock
    {
        public string GetBaoCao(string mack, int pageNo)
        {
            string s1 = "http://finance.vietstock.vn/Controls/Report/Data/GetReport.ashx?rptType=BCTT&scode=";
            string s2 = "&bizType=1&rptUnit=1000000&rptTermTypeID=2&page=";
            // giá trị 1 bắt đầu từ quý 1 năm 2017 (Mới nhất)
            string url = s1 + mack + s2 + pageNo.ToString();
            return NetworkUtility.GetHtmlSource(url);
        }
        public void GetAllBc()
        {

        }
    }
}
