using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.ViewModel
{
    public class DbOperViewModel : BaseViewModel
    {
        #region fields
        private ObservableCollection<int> m_nam = new ObservableCollection<int>();
        private ObservableCollection<int> m_quy = new ObservableCollection<int>();
        private ObservableCollection<string> m_MaCks = new ObservableCollection<string>();
        
        private long m_doanhThuBanHang;
        private long m_LoiNhuanSauThue;
        private long m_LoiNhuanGop;
        private long m_ChiPhiBanHang;
        private long m_ChiPhiQuanLyDoanhNghiep;
        private long m_ChiPhiLaiVay;
        private long m_VonGopCuaChuSoHuu;
        private long m_NoNganHan;
        private long m_NoDaiHan;

        #endregion
        public DbOperViewModel()
        {
            
        }

        #region Properties
        public long NoDaiHan
        {
            get { return m_NoDaiHan; }
            set
            {
                m_NoDaiHan = value;
                OnPropertyChanged("NoDaiHan");
            }
        }
        public long NoNganHan
        {
            get { return m_NoNganHan; }
            set
            {
                m_NoNganHan = value;
                OnPropertyChanged("NoNganHan");
            }
        }
        public long VonGopCuaChuSoHuu
        {
            get { return m_VonGopCuaChuSoHuu; }
            set
            {
                m_VonGopCuaChuSoHuu = value;
                OnPropertyChanged("VonGopCuaChuSoHuu");
            }
        }
        public long ChiPhiLaiVay
        {
            get { return m_ChiPhiLaiVay; }
            set
            {
                m_ChiPhiLaiVay = value;
                OnPropertyChanged("ChiPhiLaiVay");
            }
        }
        public long ChiPhiQuanLyDoanhNghiep
        {
            get { return m_ChiPhiQuanLyDoanhNghiep; }
            set
            {
                m_ChiPhiQuanLyDoanhNghiep = value;
                OnPropertyChanged("ChiPhiQuanLyDoanhNghiep");
            }
        }
        public long ChiPhiBanHang
        {
            get { return m_ChiPhiBanHang; }
            set
            {
                m_ChiPhiBanHang = value;
                OnPropertyChanged("ChiPhiBanHang");
            }
        }
        public long LoiNhuanGop
        {
            get { return m_LoiNhuanGop; }
            set
            {
                m_LoiNhuanGop = value;
                OnPropertyChanged("LoiNhuanGop");
            }
        }
        public long LoiNhuanSauThue
        {
            get { return m_LoiNhuanSauThue; }
            set
            {
                m_LoiNhuanSauThue = value;
                OnPropertyChanged("LoiNhuanSauThue");
            }
        }
        public long doanhThuBanHang
        {
            get { return m_doanhThuBanHang; }
            set
            {
                m_doanhThuBanHang = value;
                OnPropertyChanged("doanhThuBanHang");
            }
        }

        public ObservableCollection<int> Nam
        {
            get { return m_nam; }
            set
            {
                m_nam = value;
                OnPropertyChanged("nam");
            }
        }
        public ObservableCollection<int> Quy
        {
            get { return m_quy; }
            set
            {
                m_quy = value;
                OnPropertyChanged("Quy");
            }
        }
        public ObservableCollection<string> MaCks
        {
            get { return m_MaCks; }
            set
            {
                m_MaCks = value;
                OnPropertyChanged("MaCks");
            }
        }
        #endregion

    }
}
