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
        private double m_DoanhThu1;
        private double m_DoanhThu2;
        private double m_DoanhThu3;
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

        public double DoanhThu1
        {
            get { return m_DoanhThu1; }
            set
            {
                m_DoanhThu1 = value;
                OnPropertyChanged("DoanhThu1");
            }
        }
        public double DoanhThu2
        {
            get { return m_DoanhThu2; }
            set
            {
                m_DoanhThu2 = value;
                OnPropertyChanged("DoanhThu2");
            }
        }
        public double DoanhThu3
        {
            get { return m_DoanhThu3; }
            set
            {
                m_DoanhThu3 = value;
                OnPropertyChanged("DoanhThu3");
            }
        }
        #endregion
    }
}
