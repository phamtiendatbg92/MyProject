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
using StockAnalysis.Views;
using StockAnalysis.ViewModel;
using StockAnalysis.Utility;

namespace StockAnalysis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScoreView scoreView = new ScoreView();
        private DbOperView dbOperView = new DbOperView();
        public MainWindow()
        {
            Constants.QUY_HIEN_TAI = 1;
            Constants.NAM_HIEN_TAI = 2017;
            InitializeComponent();
            scoreView.DataContext = new ScoreViewModel();
            dbOperView.SetDataContext(new DbOperViewModel());
            NetworkUtility.SettingProxy();
        }
        private void ShowScore_Button_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Add(scoreView);
        }
        private void DabaBaseOperation_Button_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            mainGrid.Children.Add(dbOperView);
        }
    }
}
