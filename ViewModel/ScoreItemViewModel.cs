using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.ViewModel
{
    public class ScoreItemViewModel : BaseViewModel
    {
        #region fields
        private double m_DiemTangTruongDoanhThu;
        private double m_DiemTangTruongLoiNhuan;
        private double m_DiemChiPhiQuanLyDN;
        private double m_DiemChiPhiLaiVayTrenLoiNhuanGop;
        private double m_DiemLoiNhuanGopTrenDoanhThu;
        private double m_DiemTangTruongEPS;
        private double m_DiemNoNganHanTrenNoDaiHan;
        private double m_ROE = 0;
        private double m_diem;
        private string m_mack;
        #endregion

        #region Properties
        public string MaCK
        {
            get { return m_mack; }
            set
            {
                m_mack = value;
                OnPropertyChanged("MaCK");
            }
        }
        public double Diem
        {
            get { return m_diem; }
            set
            {
                m_diem = value;
                OnPropertyChanged("Diem");
            }
        }

        public double DiemTangTruongDoanhThu
        {
            get { return m_DiemTangTruongDoanhThu; }
            set
            {
                m_DiemTangTruongDoanhThu = value;
                OnPropertyChanged("DiemTangTruongDoanhThu");
            }
        }
        public double DiemTangTruongLoiNhuan
        {
            get { return m_DiemTangTruongLoiNhuan; }
            set
            {
                m_DiemTangTruongLoiNhuan = value;
                OnPropertyChanged("DiemTangTruongLoiNhuan");
            }
        }
        public double DiemChiPhiQuanLyDN
        {
            get { return m_DiemChiPhiQuanLyDN; }
            set
            {
                m_DiemChiPhiQuanLyDN = value;
                OnPropertyChanged("DiemChiPhiQuanLyDN");
            }
        }
        public double DiemChiPhiLaiVayTrenLoiNhuanGop
        {
            get { return m_DiemChiPhiLaiVayTrenLoiNhuanGop; }
            set
            {
                m_DiemChiPhiLaiVayTrenLoiNhuanGop = value;
                OnPropertyChanged("DiemChiPhiLaiVayTrenLoiNhuanGop");
            }
        }
        public double DiemLoiNhuanGopTrenDoanhThu
        {
            get { return m_DiemLoiNhuanGopTrenDoanhThu; }
            set
            {
                m_DiemLoiNhuanGopTrenDoanhThu = value;
                OnPropertyChanged("DiemLoiNhuanGopTrenDoanhThu");
            }
        }
        public double DiemTangTruongEPS
        {
            get { return m_DiemTangTruongEPS; }
            set
            {
                m_DiemTangTruongEPS = value;
                OnPropertyChanged("DiemTangTruongEPS");
            }
        }

        public double DiemNoNganHanTrenNoDaiHan
        {
            get { return m_DiemNoNganHanTrenNoDaiHan; }
            set
            {
                m_DiemNoNganHanTrenNoDaiHan = value;
                OnPropertyChanged("DiemNoNganHanTrenNoDaiHan");
            }
        }

        public double ROE
        {
            get { return m_ROE; }
            set
            {
                m_ROE = value;
                OnPropertyChanged("ROE");
            }
        }
        #endregion
    }
}
