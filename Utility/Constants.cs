using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.Utility
{
    public static class Constants
    {
        public static int QUY_HIEN_TAI { get; set; }
        public static int NAM_HIEN_TAI { get; set; }
        public const int HESO_DOANH_THU = 10;
        public const int HESO_LOI_NHUAN = 11;
        // chỉ số chi phí bán hàng và quản lý doanh nghiệp / lợi nhuận gộp
        public const int HESO_ChiPhiBanHangVaQuanLyDn_LoiNhuanGop = 8;
        // chỉ số lợi nhuận sau thuế / doanh thu thuần
        public const int HESO_LoiNhuanSauThue_DoanhThu = 10;
    }
}
