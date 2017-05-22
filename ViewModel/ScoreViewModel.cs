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
     * Doanh thu
     * Lợi nhuận sau thuế
     * Lợi nhuận gộp
     * Chi phí quản lý doanh nghiệp
     * Chi phí lãi vay
     * Vốn góp của chủ sở hữu
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
                item.Diem = TinhToanDiem(bctcDic[listCongty[i].mack]);
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
        private double TinhToanDiem(List<bctc> listBctc)
        {
            double diem = 0;
            diem += TinhDiemDoanhThu(listBctc);
            diem += TinhDiemLoiNhuan(listBctc);
            diem += TinhDiemChiPhiQuanLyDn(listBctc);
            diem += TinhDiemLoiNhuanTrenDoanhThu(listBctc);
            diem += TinhDiemChiPhiLaiVayTrenLoiNhuanGop(listBctc);
            diem += TinhDiem_LoiNhuanGop_DoanhThu(listBctc);
            diem += TinhDiem_TangTruongEPS(listBctc);
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
                if (bcQuyTruoc.C18_LoiNhuanSauThueTNDN == 0 ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN == 0 ||
                    bcQuyHienTai.C1_1_VonGopCuaChuSoHuu == 0 ||
                    bcQuyTruoc.C1_1_VonGopCuaChuSoHuu == 0)
                {
                    break;
                }
                long loiNhuanHienTai = (long)bcQuyHienTai.C18_LoiNhuanSauThueTNDN;
                long soCpHienTai = (long)bcQuyHienTai.C1_1_VonGopCuaChuSoHuu / 10000;
                double peHienTai = loiNhuanHienTai / soCpHienTai;

                long loiNhuanQuyTruoc = (long)bcQuyTruoc.C18_LoiNhuanSauThueTNDN;
                long soCpQuyTruoc = (long)bcQuyTruoc.C1_1_VonGopCuaChuSoHuu / 10000;
                double peQuyTruoc = loiNhuanQuyTruoc / soCpQuyTruoc;

                if (loiNhuanQuyTruoc != 0)
                {
                    diem += (peHienTai - peQuyTruoc) * heSo * 100 / peQuyTruoc;
                }
                LayQuyTruoc(ref currentQuy, ref currentYear);
            }
            return diem;
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
                loiNhuanGop += (long)bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV;
                doanhThuThuan += (long)bcQuyHienTai.C1_DoanhThuBanHangVaCungcapDV;
                LayQuyTruoc(ref currentQuy, ref currentYear); // Get quý trước đó
            }

            if (loiNhuanGop != 0)
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
                loiNhuanGop += (long)bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV;
                chiPhiLaiVay += (long)bcQuyHienTai.C7_1_ChiPhiLaiVay;
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo); // Get quý trước đó
            }

            if (loiNhuanGop != 0)
            {
                diem += (100 - (chiPhiLaiVay) * 100 / loiNhuanGop) * heSo;
            }
            return diem;

        }

        private double TinhDiemLoiNhuanTrenDoanhThu(List<bctc> listBctc)
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
                loiNhuanGop += (long)bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV;
                chiPhiQuanLyDN += (long)bcQuyHienTai.C10_ChiPhiQuanLyDoanhNghiep;
                chiPhiBanHang += (long)bcQuyHienTai.C9_ChiPhiBanHang;
                LayQuyTruoc(ref currentQuy, ref currentYear,ref heSo); // Get quý trước đó
            }

            if (loiNhuanGop != 0)
            {
                diem += (100 - (chiPhiQuanLyDN + chiPhiBanHang) * 100 / loiNhuanGop) * heSo;
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
                loiNhuanGop += (long)bcQuyHienTai.C5_LoiNhuanGopVeBanHangVaCungCapDV;
                chiPhiQuanLyDN += (long)bcQuyHienTai.C10_ChiPhiQuanLyDoanhNghiep;
                chiPhiBanHang += (long)bcQuyHienTai.C9_ChiPhiBanHang;
                LayQuyTruoc(ref currentQuy, ref currentYear, ref heSo); // Get quý trước đó
            }

            if (loiNhuanGop != 0)
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
                if (bcQuyTruoc.C18_LoiNhuanSauThueTNDN == 0 ||
                    bcQuyHienTai.C18_LoiNhuanSauThueTNDN == 0)
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
                if (bcQuyTruoc.C3_DoanhThuThuanVeBanHangVaCungCapDV == 0 ||
                    bcQuyHienTai.C3_DoanhThuThuanVeBanHangVaCungCapDV == 0)
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
