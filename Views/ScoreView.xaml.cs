using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StockAnalysis.ViewModel;
namespace StockAnalysis.Views
{
    /// <summary>
    /// Interaction logic for ScoreView.xaml
    /// </summary>
    public partial class ScoreView : UserControl
    {
        public ScoreView()
        {
            InitializeComponent();
        }

        private void tinhDiemBtn_Click(object sender, RoutedEventArgs e)
        {
            ScoreViewModel vm = this.DataContext as ScoreViewModel;
            vm.TinhDiem();
        }

        private void KiemTraDuLieu_Click(object sender, RoutedEventArgs e)
        {
            ScoreViewModel vm = this.DataContext as ScoreViewModel;
            vm.KiemTraDuLieu();
        }
    }
}
