using StockAnalysis.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.ViewModel
{
    /*
     * Kiểm tra các giá trị:
     * Doanh thu ///////////
     * Lợi nhuận sau thuế //////
     * Lợi nhuận gộp  /////
     * Chi phí bán hàng  /////
     * Chi phí quản lý doanh nghiệp /////
     * Chi phí lãi vay ///
     * Vốn góp của chủ sở hữu
     * Nợ ngắn hạn
     * Nợ dài hạn
     */
    public class ScoreViewModel : BaseViewModel
    {
        private ObservableCollection<ScoreItemViewModel> m_listScoreItemVM = new ObservableCollection<ScoreItemViewModel>();

        public ScoreViewModel()
        {
        }

        #region public method
        public void TinhDiem()
        {
            List<congty> listCongty = DatabaseUtility.GetAllCty();
            Dictionary<string, List<bctc>> bctcDic = DatabaseUtility.GetAllBctc();
            int count = listCongty.Count;
            for (int i = 0; i < count; i++)
            {
                ScoreItemViewModel item = new ScoreItemViewModel();
                item.MaCK = listCongty[i].mack;
                TinhToanDiem(bctcDic[listCongty[i].mack], item);
                ListScoreItemVM.Add(item);
            }
            SapXep();
        }
        private void SapXep()
        {
            int count = ListScoreItemVM.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    if (ListScoreItemVM[i].Diem > ListScoreItemVM[j].Diem)
                    {
                        ScoreItemViewModel item = ListScoreItemVM[i];
                        ListScoreItemVM[i] = ListScoreItemVM[j];
                        ListScoreItemVM[j] = item;
                    }
                }
            }
        }
        public void KiemTraDuLieu()
        {
            List<congty> listCongty = DatabaseUtility.GetAllCty();
            Dictionary<string, List<bctc>> bctcDic = DatabaseUtility.GetAllBctc();
            int count = listCongty.Count;
            int number = 1;
            for (int i = 0; i < count; i++)
            {
                bool result = CheckBaoCao(bctcDic[listCongty[i].mack]);
                if (!result)
                {
                    Console.WriteLine("{0} Ma chung khoan: {1}", number, listCongty[i].mack);
                    number++;
                }
            }
        }

        #endregion

        #region private method

        private bool CheckBaoCao(List<bctc> listBctc)
        {
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            for (int i = 0; i < 8; i++)
            {
                bctc baocao = TimBaoCao(listBctc, currentQuy, currentYear);
                if (baocao.C3_DoanhThuThuanVeBanHangVaCungCapDV == null)
                {
                    return false;
                }
                if (currentQuy == 1)
                {
                    currentQuy = 4;
                    currentYear--;
                }
                else
                {
                    currentQuy--;
                }
            }
            return true;
        }
        private void TinhToanDiem(List<bctc> listBctc, ScoreItemViewModel item)
        {

            if (listBctc[0].mack == "FCM")
            {
                int x = 2;
            }
            item.DiemTangTruongDoanhThu = TinhDiemDoanhThu(listBctc);
            item.DiemTangTruongLoiNhuan = TinhDiemLoiNhuan(listBctc);
            item.DiemChiPhiQuanLyDN = TinhDiemChiPhiQuanLyDn(listBctc);
            item.DiemChiPhiLaiVayTrenLoiNhuanGop = TinhDiemChiPhiLaiVayTrenLoiNhuanGop(listBctc);
            item.DiemLoiNhuanGopTrenDoanhThu = TinhDiem_LoiNhuanGop_DoanhThu(listBctc);
            item.DiemTangTruongEPS = TinhDiem_TangTruongEPS(listBctc);
            item.DiemNoNganHanTrenNoDaiHan = TinhDiem_NoNganHan_NoDaiHan(listBctc);
            item.ROE = TinhDiem_ROE(listBctc);
            //////////////////////////////////////////////////////////////////////
            double diemLoiNhuanSauThue_NoDaiHan = TinhDiem_ROE(listBctc);
            item.Diem = item.DiemTangTruongDoanhThu + item.DiemTangTruongLoiNhuan
                + item.DiemChiPhiQuanLyDN + item.DiemChiPhiLaiVayTrenLoiNhuanGop +
                item.DiemLoiNhuanGopTrenDoanhThu + item.DiemTangTruongEPS +
                item.DiemNoNganHanTrenNoDaiHan + item.ROE;
        }
        private double TinhDiem_ROE(List<bctc> listBctc)
        {
            double diem = 0;

            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.ROE;
            long loiNhuanSauThueTNDN = 0;
            long vonChuSoHuu = 0;
            for (int i = 0; i < 4; i++)
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);
                if (bcQuyHienTai.II_VonChuSoHuu == null ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN == null)
                {
                    break;
                }
                if (bcQuyHienTai.II_VonChuSoHuu < 0 ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN < 0)
                {
                    break;
                }
                loiNhuanSauThueTNDN += (long)bcQuyHienTai.C18_LoiNhuanSauThueTNDN;

                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo); // Get quý trước đó
            }
            currentYear = Constants.NAM_HIEN_TAI;
            currentQuy = Constants.QUY_HIEN_TAI;
            bctc bcQuyHienTai2 = TimBaoCao(listBctc, currentQuy, currentYear);
            if (bcQuyHienTai2.II_VonChuSoHuu == null)
            {
                return 0;
            }
            vonChuSoHuu += (long)bcQuyHienTai2.II_VonChuSoHuu;
            if (loiNhuanSauThueTNDN != 0 && vonChuSoHuu != 0)
            {
                diem += (loiNhuanSauThueTNDN * 100 / vonChuSoHuu) * heSo;
            }
            return diem;
        }

        private double TinhDiem_NoNganHan_NoDaiHan(List<bctc> listBctc)
        {
            double diem = 0;
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.HESO_NoNganHan_NoDaiHan;
            long noNganHan = 0;
            long noDaiHan = 0;
            if (listBctc[0].mack == "BMP")
            {
                int x = 2;
            }
            for (int i = 0; i < 1; i++) // Nợ của quý hiện tại
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);
                if (bcQuyHienTai.C2_NoDaiHan == null)
                {
                    break;
                }
                if (bcQuyHienTai.C1_NoNganHan < 0 ||
                    bcQuyHienTai.C2_NoDaiHan < 0)
                {
                    break;
                }
                noNganHan += (long)bcQuyHienTai.C1_NoNganHan;
                noDaiHan += (long)bcQuyHienTai.C2_NoDaiHan;
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo); // Get quý trước đó
            }

            if (noNganHan != 0 && noDaiHan != 0)
            {
                diem += (100 - noNganHan * 100 / noDaiHan) * heSo;
            }
            else if (noDaiHan == 0)
            {
                diem += 100 * heSo;
            }
            else if (noDaiHan == 0)
            {
                diem -= 50 * heSo;
            }

            return diem;
        }

        private double TinhDiem_TangTruongEPS(List<bctc> listBctc)
        {
            double diem = 0;
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.HESO_PE;

            for (int i = 0; i < 4; i++)
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);

                bctc bcQuyTruoc = TimBaoCao(listBctc, currentQuy, currentYear - 1);
                if (bcQuyTruoc.C18_LoiNhuanSauThueTNDN == null ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN == null ||
                    bcQuyHienTai.C1_1_VonGopCuaChuSoHuu == null ||
                    bcQuyTruoc.C1_1_VonGopCuaChuSoHuu == null)
                {
                    break;
                }
                if (bcQuyTruoc.C18_LoiNhuanSauThueTNDN <= 0 ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN <= 0 ||
                    bcQuyHienTai.C1_1_VonGopCuaChuSoHuu <= 0 ||
                    bcQuyTruoc.C1_1_VonGopCuaChuSoHuu <= 0)
                {
                    break;
                }
                long loiNhuanHienTai = (long)bcQuyHienTai.C18_LoiNhuanSauThueTNDN;
                long soCpHienTai = (long)bcQuyHienTai.C1_1_VonGopCuaChuSoHuu / 10000;
                double peHienTai = loiNhuanHienTai / soCpHienTai;

                long loiNhuanQuyTruoc = (long)bcQuyTruoc.C18_LoiNhuanSauThueTNDN;
                long soCpQuyTruoc = (long)bcQuyTruoc.C1_1_VonGopCuaChuSoHuu / 10000;
                double peQuyTruoc = loiNhuanQuyTruoc / soCpQuyTruoc;
                if (peQuyTruoc > 0)
                {
                    diem += (peHienTai - peQuyTruoc) * heSo * 100 / Math.Abs(peQuyTruoc);
                }
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo);
            }
            return Math.Round(diem, 0);
        }
        private double TinhDiem_LoiNhuanGop_DoanhThu(List<bctc> listBctc)
        {
            double diem = 0;
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.HESO_LoiNhuanGop_DoanhThuThuan;
            long loiNhuanGop = 0;
            long doanhThuThuan = 0;
            for (int i = 0; i < 4; i++)
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);
                if (bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV == null ||
                    bcQuyHienTai.C1_DoanhThuBanHangVaCungcapDV == null)
                {
                    break;
                }
                if (bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV <= 0 ||
                    bcQuyHienTai.C1_DoanhThuBanHangVaCungcapDV <= 0)
                {
                    break;
                }
                loiNhuanGop += (long)bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV;
                doanhThuThuan += (long)bcQuyHienTai.C1_DoanhThuBanHangVaCungcapDV;
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo); // Get quý trước đó
            }

            if (loiNhuanGop > 0 && doanhThuThuan > 0)
            {
                diem += (loiNhuanGop * 100 / doanhThuThuan) * heSo;
            }
            return diem;
        }

        private double TinhDiemChiPhiLaiVayTrenLoiNhuanGop(List<bctc> listBctc)
        {
            double diem = 0;
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.HESO_ChiPhiLaiVay_LoiNhuanGop;
            long loiNhuanGop = 0;
            long chiPhiLaiVay = 0;
            for (int i = 0; i < 4; i++)
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);
                if (bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV == null ||
                    bcQuyHienTai.C7_1_ChiPhiLaiVay == null)
                {
                    break;
                }
                if (bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV <= 0)
                {
                    break;
                }
                loiNhuanGop += (long)bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV;
                chiPhiLaiVay += (long)bcQuyHienTai.C7_1_ChiPhiLaiVay;
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo); // Get quý trước đó
            }

            if (loiNhuanGop > 0)
            {
                diem += (100 - (chiPhiLaiVay) * 100 / loiNhuanGop) * heSo;
            }
            return diem;

        }

        private double TinhDiemChiPhiQuanLyDn(List<bctc> listBctc)
        {
            double diem = 0;
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.HESO_ChiPhiBanHangVaQuanLyDn_LoiNhuanGop;
            long loiNhuanGop = 0;
            long chiPhiQuanLyDN = 0;
            long chiPhiBanHang = 0;
            for (int i = 0; i < 4; i++)
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);
                if (bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV == null ||
                    bcQuyHienTai.C10_ChiPhiQuanLyDoanhNghiep == null ||
                    bcQuyHienTai.C9_ChiPhiBanHang == null)
                {
                    break;
                }
                if (bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV <= 0 ||
                    bcQuyHienTai.C10_ChiPhiQuanLyDoanhNghiep < 0 ||
                    bcQuyHienTai.C9_ChiPhiBanHang < 0)
                {
                    break;
                }
                loiNhuanGop += (long)bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV;
                chiPhiQuanLyDN += (long)bcQuyHienTai.C10_ChiPhiQuanLyDoanhNghiep;
                chiPhiBanHang += (long)bcQuyHienTai.C9_ChiPhiBanHang;
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo); // Get quý trước đó
            }

            if (loiNhuanGop > 0)
            {
                diem += (100 - (chiPhiQuanLyDN + chiPhiBanHang) * 100 / loiNhuanGop) * heSo;
            }
            return diem;
        }

        private double TinhDiemLoiNhuan(List<bctc> listBctc)
        {
            double diem = 0;
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.HESO_LOI_NHUAN;
            for (int i = 0; i < 4; i++)
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);

                bctc bcQuyTruoc = TimBaoCao(listBctc, currentQuy, currentYear - 1);
                if (bcQuyTruoc.C18_LoiNhuanSauThueTNDN <= 0 ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN <= 0)
                {
                    break;
                }
                if (bcQuyTruoc.C18_LoiNhuanSauThueTNDN == null ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN == null)
                {
                    break;
                }

                long loiNhuanHienTai = (long)bcQuyHienTai.C18_LoiNhuanSauThueTNDN;
                long loiNhuanQuyTruoc = (long)bcQuyTruoc.C18_LoiNhuanSauThueTNDN;
                if (loiNhuanQuyTruoc != 0)
                {
                    diem += (loiNhuanHienTai - loiNhuanQuyTruoc) * heSo * 100 / loiNhuanQuyTruoc;
                }
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo);
            }
            return diem;
        }

        private double TinhDiemDoanhThu(List<bctc> listBctc)
        {
            double diem = 0;
            int currentYear = Constants.NAM_HIEN_TAI;
            int currentQuy = Constants.QUY_HIEN_TAI;
            int heSo = Constants.HESO_DOANH_THU;
            for (int i = 0; i < 4; i++)
            {
                bctc bcQuyHienTai = TimBaoCao(listBctc, currentQuy, currentYear);

                bctc bcQuyTruoc = TimBaoCao(listBctc, currentQuy, currentYear - 1);
                if (bcQuyTruoc.C3_DoanhThuThuanVeBanHangVaCungCapDV == null ||
                    bcQuyHienTai.C3_DoanhThuThuanVeBanHangVaCungCapDV == null)
                {
                    break;
                }
                if (bcQuyTruoc.C3_DoanhThuThuanVeBanHangVaCungCapDV <= 0 ||
                    bcQuyHienTai.C3_DoanhThuThuanVeBanHangVaCungCapDV <= 0)
                {
                    break;
                }
                long doanhThuHienTai = (long)bcQuyHienTai.C3_DoanhThuThuanVeBanHangVaCungCapDV;
                long doanhThuQuyTruoc = (long)bcQuyTruoc.C3_DoanhThuThuanVeBanHangVaCungCapDV;
                if (doanhThuQuyTruoc != 0)
                {
                    diem += (doanhThuHienTai - doanhThuQuyTruoc) * heSo * 100 / doanhThuQuyTruoc;
                }
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo);
            }
            return diem;
        }

        private void LayQuyTruoc(ref int currentQuy, ref int currentYear, ref int heso)
        {
            heso--;
            if (currentQuy == 1)
            {
                currentQuy = 4;
                currentYear--;
            }
            else
            {
                currentQuy--;
            }
        }

        private bctc TimBaoCao(List<bctc> listBctc, int quy, int nam)
        {
            for (int i = 0; i < listBctc.Count; i++)
            {
                if (listBctc[i].Quy == quy && listBctc[i].Nam == nam)
                {
                    return listBctc[i];
                }
            }
            return null;

        }
        #endregion

        public ObservableCollection<ScoreItemViewModel> ListScoreItemVM
        {
            get { return m_listScoreItemVM; }
            set
            {
                m_listScoreItemVM = value;
                OnPropertyChanged("ListScoreItemVM");
            }
        }
    }
}
