using HtmlAgilityPack;
using StockAnalysis.CheckBaoCao;
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

        public delegate void ReportProgressHandler(int percent);
        public event ReportProgressHandler ReportProgressCallback;
        public void ReportProgress(int percent)
        {
            ReportProgressCallback?.Invoke(percent);
        }

        public void DoCompare()
        {
                
        }
    }
}
