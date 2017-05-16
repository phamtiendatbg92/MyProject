using StockAnalysis.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.ViewModel
{
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

            return diem;
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
                if (bcQuyTruoc.C3_DoanhThuThuanVeBanHangVaCungCapDV == null || bcQuyHienTai.C3_DoanhThuThuanVeBanHangVaCungCapDV == null)
                {
                    break;
                }

                long doanhThuHienTai = (long)bcQuyHienTai.C3_DoanhThuThuanVeBanHangVaCungCapDV;
                long doanhThuQuyTruoc = (long)bcQuyTruoc.C3_DoanhThuThuanVeBanHangVaCungCapDV;
                if (doanhThuQuyTruoc != 0)
                {
                    diem += (doanhThuHienTai - doanhThuQuyTruoc) * heSo * 100 / doanhThuQuyTruoc;
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
            return diem;
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
