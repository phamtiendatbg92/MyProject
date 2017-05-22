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
        // Chỉ số tăng trưởng doanh thu
        public const int HESO_DOANH_THU = 10;
        // Chỉ số tăng trưởng lợi nhuận
        public const int HESO_LOI_NHUAN = 11;
        // chỉ số chi phí bán hàng và quản lý doanh nghiệp / lợi nhuận gộp
        public const int HESO_ChiPhiBanHangVaQuanLyDn_LoiNhuanGop = 30;
        // Chỉ số chi phí lãi vay / lợi nhuận gộp
        public const int HESO_ChiPhiLaiVay_LoiNhuanGop = 30;
        // chỉ số lợi nhuận sau thuế / doanh thu thuần
        public const int HESO_LoiNhuanSauThue_DoanhThu = 30;
        // chỉ số thu nhập thuần / doanh thu thuần
        public const int HESO_ThuNhap_DoanhThu = 25;
        // Chỉ số Lợi nhuận gộp / doanh thu thuần
        public const int HESO_LoiNhuanGop_DoanhThuThuan = 30;

        // Chỉ số P/E
        public const int HESO_PE = 13;
    }
}
