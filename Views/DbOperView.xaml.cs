using HtmlAgilityPack;
using StockAnalysis.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace StockAnalysis.Views
{
    /// <summary>
    /// Interaction logic for DbOperView.xaml
    /// </summary>
    public partial class DbOperView : UserControl
    {
        public static stocksqlEntities entities = new stocksqlEntities();

        public DbOperView()
        {
            InitializeComponent();
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += LayDuLieu;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }
        private void LayDuLieu(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            string folderName = Directory.GetCurrentDirectory();
            folderName += "\\CanDoiKeToan\\";
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string maCkString = File.ReadAllText("MaCK.txt");
            string[] maCks = maCkString.Split('_');
            string s1 = "http://s.cafef.vn/bao-cao-tai-chinh/";
            string s2 = "/BSheet/"; //2016/1/0/0
            string s3 = "/bao-cao-tai-chinh-cong-ty-co-phan-dich-vu-va-xay-dung-dia-oc-dat-xanh.chn";



            string downloadString = "";
            int length = maCks.Length;
            for (int i = 0; i < length; i++)
            {
                int startYear = 2016;
                int startQuy = 4;
                int startIndex = -3;

                string mack = maCks[i];
                for (int j = 0; j < 3; j++)
                {
                    string thoigian = startYear.ToString() + "/" + startQuy.ToString() + "/" + startIndex.ToString() + "/0";
                    string url = s1 + mack + s2 + thoigian + s3;
                    try
                    {
                        downloadString = client.DownloadString(url);
                    }
                    catch (Exception ex)
                    {
                        System.Threading.Thread.Sleep(1000);
                        Console.WriteLine("exception: {0}", url);
                        while (true)
                        {
                            try
                            {
                                downloadString = client.DownloadString(url);
                                break;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("=================");
                            }
                        }
                    }
                    File.WriteAllText(folderName + mack + startYear.ToString() + ".txt", downloadString, Encoding.UTF8);
                    startYear--;
                    startIndex += 4;
                }
                bw.ReportProgress(i * 100 / length);
            }
        }
        private void bw_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void CapNhatDBWorker(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            string currentFolder = Directory.GetCurrentDirectory();
            string folderCanDoiKeToan = currentFolder + "\\CanDoiKeToan\\";
            string folderKetQuaHDKD = currentFolder + "\\KetQuaHDKD\\";
            string maCkString = File.ReadAllText("MaCK.txt");
            string[] maCks = maCkString.Split('_');

            for (int i = 0; i < maCks.Length; i++)
            {
                List<bctc> listBaoCao = InitBaoCao(maCks[i]);
                int temp = 2016;
                for (int j = 0; j < 3; j++)
                {
                    string fileName = maCks[i] + temp.ToString();
                    string contentCanDoiKT = File.ReadAllText(folderCanDoiKeToan + fileName + ".txt");
                    string contentKetQuaHDKD = File.ReadAllText(folderKetQuaHDKD + fileName + ".txt");
                    ReadCanDoiKeToan(contentCanDoiKT, listBaoCao, temp);
                    ReadCanDoiKeToan(contentKetQuaHDKD, listBaoCao, temp);
                    temp--;
                }
                for (int k = 0; k < listBaoCao.Count; k++)
                {
                    entities.bctcs.Add(listBaoCao[k]);
                }
                entities.SaveChanges();
                bw.ReportProgress(i * 100 / maCks.Length);
            }

        }


        private void CapNhatDBButton_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += CapNhatDBWorker;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }
        private List<bctc> InitBaoCao(string mack)
        {
            List<bctc> listBaoCao = new List<bctc>();
            int quy = 1;
            int nam = 2014;
            for (int k = 0; k < 12; k++)
            {
                bctc baocao = new bctc();
                baocao.mack = mack;
                baocao.Quy = quy;
                baocao.Nam = nam;
                if (quy == 4)
                {
                    nam++;
                    quy = 1;
                }
                else
                {
                    quy++;
                }
                listBaoCao.Add(baocao);
            }

            return listBaoCao;
        }
        private void ReadCanDoiKeToan(string htmlContent, List<bctc> listBaoCao, int year)
        {
            int index = 4 * (year - 2014);
            bctc baocao1 = listBaoCao[index];
            bctc baocao2 = listBaoCao[index + 1];
            bctc baocao3 = listBaoCao[index + 2];
            bctc baocao4 = listBaoCao[index + 3];
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//tr");
            List<long> listValues = new List<long>();
            foreach (HtmlNode link in collection)
            {
                if (link.Attributes["id"] != null)
                {
                    string value = link.Attributes["id"].Value;
                    #region cac chi so
                    switch (value)
                    {
                        #region bang can doi ke toan
                        case "100":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanNganHan = listValues[0];
                            baocao2.TaiSanNganHan = listValues[1];
                            baocao3.TaiSanNganHan = listValues[2];
                            baocao4.TaiSanNganHan = listValues[3];
                            break;
                        case "110":
                            listValues = ExtractXmlData(link);
                            baocao1.TienVaCacKhoanTuongDuongTien = listValues[0];
                            baocao2.TienVaCacKhoanTuongDuongTien = listValues[1];
                            baocao3.TienVaCacKhoanTuongDuongTien = listValues[2];
                            baocao4.TienVaCacKhoanTuongDuongTien = listValues[3];
                            break;
                        case "111":
                            listValues = ExtractXmlData(link);
                            baocao1.Tien = listValues[0];
                            baocao2.Tien = listValues[1];
                            baocao3.Tien = listValues[2];
                            baocao4.Tien = listValues[3];
                            break;
                        case "112":
                            listValues = ExtractXmlData(link);
                            baocao1.CacKhoanTuongDuongTien = listValues[0];
                            baocao2.CacKhoanTuongDuongTien = listValues[1];
                            baocao3.CacKhoanTuongDuongTien = listValues[2];
                            baocao4.CacKhoanTuongDuongTien = listValues[3];
                            break;
                        case "120":
                            listValues = ExtractXmlData(link);
                            baocao1.CacKhoanDauTuTaiChinhNganHan = listValues[0];
                            baocao2.CacKhoanDauTuTaiChinhNganHan = listValues[1];
                            baocao3.CacKhoanDauTuTaiChinhNganHan = listValues[2];
                            baocao4.CacKhoanDauTuTaiChinhNganHan = listValues[3];
                            break;
                        case "1201":
                            listValues = ExtractXmlData(link);
                            baocao1.ChungKhoanKinhDoanh = listValues[0];
                            baocao2.ChungKhoanKinhDoanh = listValues[1];
                            baocao3.ChungKhoanKinhDoanh = listValues[2];
                            baocao4.ChungKhoanKinhDoanh = listValues[3];
                            break;
                        case "1202":
                            listValues = ExtractXmlData(link);
                            baocao1.DuPhongGiamGiaChungKhoanKinhDoanh = listValues[0];
                            baocao2.DuPhongGiamGiaChungKhoanKinhDoanh = listValues[1];
                            baocao3.DuPhongGiamGiaChungKhoanKinhDoanh = listValues[2];
                            baocao4.DuPhongGiamGiaChungKhoanKinhDoanh = listValues[3];
                            break;
                        case "1203":
                            listValues = ExtractXmlData(link);
                            baocao1.DauTuNamGiuDenNgayDaoHan = listValues[0];
                            baocao2.DauTuNamGiuDenNgayDaoHan = listValues[1];
                            baocao3.DauTuNamGiuDenNgayDaoHan = listValues[2];
                            baocao4.DauTuNamGiuDenNgayDaoHan = listValues[3];
                            break;
                        case "130":
                            listValues = ExtractXmlData(link);
                            baocao1.CacKhoanPhaiThuNganHan = listValues[0];
                            baocao2.CacKhoanPhaiThuNganHan = listValues[1];
                            baocao3.CacKhoanPhaiThuNganHan = listValues[2];
                            baocao4.CacKhoanPhaiThuNganHan = listValues[3];
                            break;
                        case "131":
                            listValues = ExtractXmlData(link);
                            baocao1.PhaiThuKhachHang = listValues[0];
                            baocao2.PhaiThuKhachHang = listValues[1];
                            baocao3.PhaiThuKhachHang = listValues[2];
                            baocao4.PhaiThuKhachHang = listValues[3];
                            break;
                        case "132":
                            listValues = ExtractXmlData(link);
                            baocao1.TraTruocChoNguoiBan = listValues[0];
                            baocao2.TraTruocChoNguoiBan = listValues[1];
                            baocao3.TraTruocChoNguoiBan = listValues[2];
                            baocao4.TraTruocChoNguoiBan = listValues[3];
                            break;
                        case "133":
                            listValues = ExtractXmlData(link);
                            baocao1.PhaiThuNoiBoNganHan = listValues[0];
                            baocao2.PhaiThuNoiBoNganHan = listValues[1];
                            baocao3.PhaiThuNoiBoNganHan = listValues[2];
                            baocao4.PhaiThuNoiBoNganHan = listValues[3];
                            break;
                        case "134":
                            listValues = ExtractXmlData(link);
                            baocao1.PhaiThuTheoTienDoKeHoachHopDongXayDung = listValues[0];
                            baocao2.PhaiThuTheoTienDoKeHoachHopDongXayDung = listValues[1];
                            baocao3.PhaiThuTheoTienDoKeHoachHopDongXayDung = listValues[2];
                            baocao4.PhaiThuTheoTienDoKeHoachHopDongXayDung = listValues[3];
                            break;
                        case "135":
                            listValues = ExtractXmlData(link);
                            baocao1.PhaiThuVeChoVayNganHan = listValues[0];
                            baocao2.PhaiThuVeChoVayNganHan = listValues[1];
                            baocao3.PhaiThuVeChoVayNganHan = listValues[2];
                            baocao4.PhaiThuVeChoVayNganHan = listValues[3];
                            break;
                        case "136":
                            listValues = ExtractXmlData(link);
                            baocao1.CacKhoanPhaiThuKhac = listValues[0];
                            baocao2.CacKhoanPhaiThuKhac = listValues[1];
                            baocao3.CacKhoanPhaiThuKhac = listValues[2];
                            baocao4.CacKhoanPhaiThuKhac = listValues[3];
                            break;
                        case "139":
                            listValues = ExtractXmlData(link);
                            baocao1.DuPhongPhaiThuNganHanKhoDoi = listValues[0];
                            baocao2.DuPhongPhaiThuNganHanKhoDoi = listValues[1];
                            baocao3.DuPhongPhaiThuNganHanKhoDoi = listValues[2];
                            baocao4.DuPhongPhaiThuNganHanKhoDoi = listValues[3];
                            break;
                        case "1391":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanThieuChoXuLy = listValues[0];
                            baocao2.TaiSanThieuChoXuLy = listValues[1];
                            baocao3.TaiSanThieuChoXuLy = listValues[2];
                            baocao4.TaiSanThieuChoXuLy = listValues[3];
                            break;
                        case "140":
                            listValues = ExtractXmlData(link);
                            baocao1.HangTonKhoTotal = listValues[0];
                            baocao2.HangTonKhoTotal = listValues[1];
                            baocao3.HangTonKhoTotal = listValues[2];
                            baocao4.HangTonKhoTotal = listValues[3];
                            break;
                        case "141":
                            listValues = ExtractXmlData(link);
                            baocao1.HangTonKho = listValues[0];
                            baocao2.HangTonKho = listValues[1];
                            baocao3.HangTonKho = listValues[2];
                            baocao4.HangTonKho = listValues[3];
                            break;
                        case "149":
                            listValues = ExtractXmlData(link);
                            baocao1.DuPhongGiamGiaHangTonKho = listValues[0];
                            baocao2.DuPhongGiamGiaHangTonKho = listValues[1];
                            baocao3.DuPhongGiamGiaHangTonKho = listValues[2];
                            baocao4.DuPhongGiamGiaHangTonKho = listValues[3];
                            break;
                        case "150":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanNganHanKhacTotal = listValues[0];
                            baocao2.TaiSanNganHanKhacTotal = listValues[1];
                            baocao3.TaiSanNganHanKhacTotal = listValues[2];
                            baocao4.TaiSanNganHanKhacTotal = listValues[3];
                            break;
                        case "151":
                            listValues = ExtractXmlData(link);
                            baocao1.ChiPhiTraTruocNganHan = listValues[0];
                            baocao2.ChiPhiTraTruocNganHan = listValues[1];
                            baocao3.ChiPhiTraTruocNganHan = listValues[2];
                            baocao4.ChiPhiTraTruocNganHan = listValues[3];
                            break;
                        case "152":
                            listValues = ExtractXmlData(link);
                            baocao1.ThueGTGTDuocKhauTru = listValues[0];
                            baocao2.ThueGTGTDuocKhauTru = listValues[1];
                            baocao3.ThueGTGTDuocKhauTru = listValues[2];
                            baocao4.ThueGTGTDuocKhauTru = listValues[3];
                            break;
                        case "154":
                            listValues = ExtractXmlData(link);
                            baocao1.ThueVaCacKhoanKhacPhaiThuNhaNuoc = listValues[0];
                            baocao2.ThueVaCacKhoanKhacPhaiThuNhaNuoc = listValues[1];
                            baocao3.ThueVaCacKhoanKhacPhaiThuNhaNuoc = listValues[2];
                            baocao4.ThueVaCacKhoanKhacPhaiThuNhaNuoc = listValues[3];
                            break;
                        case "155":
                            listValues = ExtractXmlData(link);
                            baocao1.GiaoDichMuaBanLaiTraiPhieuChinhPhu = listValues[0];
                            baocao2.GiaoDichMuaBanLaiTraiPhieuChinhPhu = listValues[1];
                            baocao3.GiaoDichMuaBanLaiTraiPhieuChinhPhu = listValues[2];
                            baocao4.GiaoDichMuaBanLaiTraiPhieuChinhPhu = listValues[3];
                            break;
                        case "158":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanNganHanKhac = listValues[0];
                            baocao2.TaiSanNganHanKhac = listValues[1];
                            baocao3.TaiSanNganHanKhac = listValues[2];
                            baocao4.TaiSanNganHanKhac = listValues[3];
                            break;
                        case "200":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanDaiHan = listValues[0];
                            baocao2.TaiSanDaiHan = listValues[1];
                            baocao3.TaiSanDaiHan = listValues[2];
                            baocao4.TaiSanDaiHan = listValues[3];
                            break;
                        case "210":
                            listValues = ExtractXmlData(link);
                            baocao1.CacKhoanPhaiThuDaiHan = listValues[0];
                            baocao2.CacKhoanPhaiThuDaiHan = listValues[1];
                            baocao3.CacKhoanPhaiThuDaiHan = listValues[2];
                            baocao4.CacKhoanPhaiThuDaiHan = listValues[3];
                            break;
                        case "211":
                            listValues = ExtractXmlData(link);
                            baocao1.PhaiThuDaiHanCuaKhachHang = listValues[0];
                            baocao2.PhaiThuDaiHanCuaKhachHang = listValues[1];
                            baocao3.PhaiThuDaiHanCuaKhachHang = listValues[2];
                            baocao4.PhaiThuDaiHanCuaKhachHang = listValues[3];
                            break;
                        case "212":
                            listValues = ExtractXmlData(link);
                            baocao1.TraTruocChoNguoiBanDaiHan = listValues[0];
                            baocao2.TraTruocChoNguoiBanDaiHan = listValues[1];
                            baocao3.TraTruocChoNguoiBanDaiHan = listValues[2];
                            baocao4.TraTruocChoNguoiBanDaiHan = listValues[3];
                            break;
                        case "2131":
                            listValues = ExtractXmlData(link);
                            baocao1.PhaiThuVeChoVayDaiHan = listValues[0];
                            baocao2.PhaiThuVeChoVayDaiHan = listValues[1];
                            baocao3.PhaiThuVeChoVayDaiHan = listValues[2];
                            baocao4.PhaiThuVeChoVayDaiHan = listValues[3];
                            break;
                        case "218":
                            listValues = ExtractXmlData(link);
                            baocao1.PhaiThuDaiHanKhac = listValues[0];
                            baocao2.PhaiThuDaiHanKhac = listValues[1];
                            baocao3.PhaiThuDaiHanKhac = listValues[2];
                            baocao4.PhaiThuDaiHanKhac = listValues[3];
                            break;
                        case "219":
                            listValues = ExtractXmlData(link);
                            baocao1.DuPhongPhaiThuKhoDoi = listValues[0];
                            baocao2.DuPhongPhaiThuKhoDoi = listValues[1];
                            baocao3.DuPhongPhaiThuKhoDoi = listValues[2];
                            baocao4.DuPhongPhaiThuKhoDoi = listValues[3];
                            break;
                        case "220":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanCoDinh = listValues[0];
                            baocao2.TaiSanCoDinh = listValues[1];
                            baocao3.TaiSanCoDinh = listValues[2];
                            baocao4.TaiSanCoDinh = listValues[3];
                            break;
                        case "221":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanCoDinhHuuHinh = listValues[0];
                            baocao2.TaiSanCoDinhHuuHinh = listValues[1];
                            baocao3.TaiSanCoDinhHuuHinh = listValues[2];
                            baocao4.TaiSanCoDinhHuuHinh = listValues[3];
                            break;
                        case "2211":
                            listValues = ExtractXmlData(link);
                            baocao1.NguyenGiaTSHH = listValues[0];
                            baocao2.NguyenGiaTSHH = listValues[1];
                            baocao3.NguyenGiaTSHH = listValues[2];
                            baocao4.NguyenGiaTSHH = listValues[3];
                            break;
                        case "2212":
                            listValues = ExtractXmlData(link);
                            baocao1.GiaTriHaoMonLuyKeTSHH = listValues[0];
                            baocao2.GiaTriHaoMonLuyKeTSHH = listValues[1];
                            baocao3.GiaTriHaoMonLuyKeTSHH = listValues[2];
                            baocao4.GiaTriHaoMonLuyKeTSHH = listValues[3];
                            break;
                        case "222":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanCoDinhThueTaiChinh = listValues[0];
                            baocao2.TaiSanCoDinhThueTaiChinh = listValues[1];
                            baocao3.TaiSanCoDinhThueTaiChinh = listValues[2];
                            baocao4.TaiSanCoDinhThueTaiChinh = listValues[3];
                            break;
                        case "2221":
                            listValues = ExtractXmlData(link);
                            baocao1.NguyenGiaTSCDTTC = listValues[0];
                            baocao2.NguyenGiaTSCDTTC = listValues[1];
                            baocao3.NguyenGiaTSCDTTC = listValues[2];
                            baocao4.NguyenGiaTSCDTTC = listValues[3];
                            break;
                        case "2222":
                            listValues = ExtractXmlData(link);
                            baocao1.GiaTriHaoMonLuyKeTSCDTTC = listValues[0];
                            baocao2.GiaTriHaoMonLuyKeTSCDTTC = listValues[1];
                            baocao3.GiaTriHaoMonLuyKeTSCDTTC = listValues[2];
                            baocao4.GiaTriHaoMonLuyKeTSCDTTC = listValues[3];
                            break;
                        case "223":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanCoDinhVoHinh = listValues[0];
                            baocao2.TaiSanCoDinhVoHinh = listValues[1];
                            baocao3.TaiSanCoDinhVoHinh = listValues[2];
                            baocao4.TaiSanCoDinhVoHinh = listValues[3];
                            break;
                        case "2231":
                            listValues = ExtractXmlData(link);
                            baocao1.NguyenGiaTSCDVH = listValues[0];
                            baocao2.NguyenGiaTSCDVH = listValues[1];
                            baocao3.NguyenGiaTSCDVH = listValues[2];
                            baocao4.NguyenGiaTSCDVH = listValues[3];
                            break;
                        case "2232":
                            listValues = ExtractXmlData(link);
                            baocao1.GiaTriHaoMonLuyKeTSCDVH = listValues[0];
                            baocao2.GiaTriHaoMonLuyKeTSCDVH = listValues[1];
                            baocao3.GiaTriHaoMonLuyKeTSCDVH = listValues[2];
                            baocao4.GiaTriHaoMonLuyKeTSCDVH = listValues[3];
                            break;
                        case "230":
                            listValues = ExtractXmlData(link);
                            baocao1.BatDongSanDauTu = listValues[0];
                            baocao2.BatDongSanDauTu = listValues[1];
                            baocao3.BatDongSanDauTu = listValues[2];
                            baocao4.BatDongSanDauTu = listValues[3];
                            break;
                        case "2301":
                            listValues = ExtractXmlData(link);
                            baocao1.NguyenGiaBDSDT = listValues[0];
                            baocao2.NguyenGiaBDSDT = listValues[1];
                            baocao3.NguyenGiaBDSDT = listValues[2];
                            baocao4.NguyenGiaBDSDT = listValues[3];
                            break;
                        case "2302":
                            listValues = ExtractXmlData(link);
                            baocao1.GiaTriHaoMonLuyKeBDSDT = listValues[0];
                            baocao2.GiaTriHaoMonLuyKeBDSDT = listValues[1];
                            baocao3.GiaTriHaoMonLuyKeBDSDT = listValues[2];
                            baocao4.GiaTriHaoMonLuyKeBDSDT = listValues[3];
                            break;
                        case "240":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanDangDoDaiHan = listValues[0];
                            baocao2.TaiSanDangDoDaiHan = listValues[1];
                            baocao3.TaiSanDangDoDaiHan = listValues[2];
                            baocao4.TaiSanDangDoDaiHan = listValues[3];
                            break;
                        case "24021":
                            listValues = ExtractXmlData(link);
                            baocao1.ChiPhiSxkdDoDangDaiHan = listValues[0];
                            baocao2.ChiPhiSxkdDoDangDaiHan = listValues[1];
                            baocao3.ChiPhiSxkdDoDangDaiHan = listValues[2];
                            baocao4.ChiPhiSxkdDoDangDaiHan = listValues[3];
                            break;
                        case "24022":
                            listValues = ExtractXmlData(link);
                            baocao1.ChiPhiXayDungCoBanDoDang = listValues[0];
                            baocao2.ChiPhiXayDungCoBanDoDang = listValues[1];
                            baocao3.ChiPhiXayDungCoBanDoDang = listValues[2];
                            baocao4.ChiPhiXayDungCoBanDoDang = listValues[3];
                            break;
                        case "250":
                            listValues = ExtractXmlData(link);
                            baocao1.DauTuTaiChinhDaiHan = listValues[0];
                            baocao2.DauTuTaiChinhDaiHan = listValues[1];
                            baocao3.DauTuTaiChinhDaiHan = listValues[2];
                            baocao4.DauTuTaiChinhDaiHan = listValues[3];
                            break;
                        case "251":
                            listValues = ExtractXmlData(link);
                            baocao1.DauTuVaoCtyCon = listValues[0];
                            baocao2.DauTuVaoCtyCon = listValues[1];
                            baocao3.DauTuVaoCtyCon = listValues[2];
                            baocao4.DauTuVaoCtyCon = listValues[3];
                            break;
                        case "252":
                            listValues = ExtractXmlData(link);
                            baocao1.DauTuVaoCtyLienKetLienDoanh = listValues[0];
                            baocao2.DauTuVaoCtyLienKetLienDoanh = listValues[1];
                            baocao3.DauTuVaoCtyLienKetLienDoanh = listValues[2];
                            baocao4.DauTuVaoCtyLienKetLienDoanh = listValues[3];
                            break;
                        case "253":
                            listValues = ExtractXmlData(link);
                            baocao1.DauTuGopVonVaoDonViKhac = listValues[0];
                            baocao2.DauTuGopVonVaoDonViKhac = listValues[1];
                            baocao3.DauTuGopVonVaoDonViKhac = listValues[2];
                            baocao4.DauTuGopVonVaoDonViKhac = listValues[3];
                            break;
                        case "258":
                            listValues = ExtractXmlData(link);
                            baocao1.DuPhongGiamGiaDauTuDaiHan = listValues[0];
                            baocao2.DuPhongGiamGiaDauTuDaiHan = listValues[1];
                            baocao3.DuPhongGiamGiaDauTuDaiHan = listValues[2];
                            baocao4.DuPhongGiamGiaDauTuDaiHan = listValues[3];
                            break;
                        case "259":
                            listValues = ExtractXmlData(link);
                            baocao1.DauTuNamGiuDenNgayDaoHan = listValues[0];
                            baocao2.DauTuNamGiuDenNgayDaoHan = listValues[1];
                            baocao3.DauTuNamGiuDenNgayDaoHan = listValues[2];
                            baocao4.DauTuNamGiuDenNgayDaoHan = listValues[3];
                            break;
                        case "260":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanDaiHanKhac = listValues[0];
                            baocao2.TaiSanDaiHanKhac = listValues[1];
                            baocao3.TaiSanDaiHanKhac = listValues[2];
                            baocao4.TaiSanDaiHanKhac = listValues[3];
                            break;
                        case "261":
                            listValues = ExtractXmlData(link);
                            baocao1.ChiPhiTraTruocDaiHan = listValues[0];
                            baocao2.ChiPhiTraTruocDaiHan = listValues[1];
                            baocao3.ChiPhiTraTruocDaiHan = listValues[2];
                            baocao4.ChiPhiTraTruocDaiHan = listValues[3];
                            break;
                        case "2611":
                            listValues = ExtractXmlData(link);
                            baocao1.TaiSanThueThuNhapHoanLai = listValues[0];
                            baocao2.TaiSanThueThuNhapHoanLai = listValues[1];
                            baocao3.TaiSanThueThuNhapHoanLai = listValues[2];
                            baocao4.TaiSanThueThuNhapHoanLai = listValues[3];
                            break;
                        case "267":
                            listValues = ExtractXmlData(link);
                            baocao1.C6_4_TaiSanDaiHanKhac = listValues[0];
                            baocao2.C6_4_TaiSanDaiHanKhac = listValues[1];
                            baocao3.C6_4_TaiSanDaiHanKhac = listValues[2];
                            baocao4.C6_4_TaiSanDaiHanKhac = listValues[3];
                            break;
                        case "268":
                            listValues = ExtractXmlData(link);
                            baocao1.C6_5_KyQuyKyCuocDaiHan = listValues[0];
                            baocao2.C6_5_KyQuyKyCuocDaiHan = listValues[1];
                            baocao3.C6_5_KyQuyKyCuocDaiHan = listValues[2];
                            baocao4.C6_5_KyQuyKyCuocDaiHan = listValues[3];
                            break;
                        case "269":
                            listValues = ExtractXmlData(link);
                            baocao1.C6_6_LoiTheThuongMai = listValues[0];
                            baocao2.C6_6_LoiTheThuongMai = listValues[1];
                            baocao3.C6_6_LoiTheThuongMai = listValues[2];
                            baocao4.C6_6_LoiTheThuongMai = listValues[3];
                            break;
                        case "001":
                            listValues = ExtractXmlData(link);
                            baocao1.TongTaiSan = listValues[0];
                            baocao2.TongTaiSan = listValues[1];
                            baocao3.TongTaiSan = listValues[2];
                            baocao4.TongTaiSan = listValues[3];
                            break;
                        case "300":
                            listValues = ExtractXmlData(link);
                            baocao1.NoPhaiTra = listValues[0];
                            baocao2.NoPhaiTra = listValues[1];
                            baocao3.NoPhaiTra = listValues[2];
                            baocao4.NoPhaiTra = listValues[3];
                            break;
                        case "310":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_NoNganHan = listValues[0];
                            baocao2.C1_NoNganHan = listValues[1];
                            baocao3.C1_NoNganHan = listValues[2];
                            baocao4.C1_NoNganHan = listValues[3];
                            break;
                        case "312":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_1_PhaiTraNguoiBanNganHan = listValues[0];
                            baocao2.C1_1_PhaiTraNguoiBanNganHan = listValues[1];
                            baocao3.C1_1_PhaiTraNguoiBanNganHan = listValues[2];
                            baocao4.C1_1_PhaiTraNguoiBanNganHan = listValues[3];
                            break;
                        case "313":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_2_NguoiMuaTraTienTruocNganHan = listValues[0];
                            baocao2.C1_2_NguoiMuaTraTienTruocNganHan = listValues[1];
                            baocao3.C1_2_NguoiMuaTraTienTruocNganHan = listValues[2];
                            baocao4.C1_2_NguoiMuaTraTienTruocNganHan = listValues[3];
                            break;
                        case "314":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_3_ThueVaCacKhoanPhaiNopNhaNuoc = listValues[0];
                            baocao2.C1_3_ThueVaCacKhoanPhaiNopNhaNuoc = listValues[1];
                            baocao3.C1_3_ThueVaCacKhoanPhaiNopNhaNuoc = listValues[2];
                            baocao4.C1_3_ThueVaCacKhoanPhaiNopNhaNuoc = listValues[3];
                            break;
                        case "315":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_4_PhaiTraNguoiLaoDong = listValues[0];
                            baocao2.C1_4_PhaiTraNguoiLaoDong = listValues[1];
                            baocao3.C1_4_PhaiTraNguoiLaoDong = listValues[2];
                            baocao4.C1_4_PhaiTraNguoiLaoDong = listValues[3];
                            break;
                        case "316":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_5_ChiPhiPhaiTraNganHan = listValues[0];
                            baocao2.C1_5_ChiPhiPhaiTraNganHan = listValues[1];
                            baocao3.C1_5_ChiPhiPhaiTraNganHan = listValues[2];
                            baocao4.C1_5_ChiPhiPhaiTraNganHan = listValues[3];
                            break;
                        case "317":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_6_PhaiTraNoiBoNganHan = listValues[0];
                            baocao2.C1_6_PhaiTraNoiBoNganHan = listValues[1];
                            baocao3.C1_6_PhaiTraNoiBoNganHan = listValues[2];
                            baocao4.C1_6_PhaiTraNoiBoNganHan = listValues[3];
                            break;
                        case "318":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_7_PhaiTraTheoTienDoKeHoachHDXD = listValues[0];
                            baocao2.C1_7_PhaiTraTheoTienDoKeHoachHDXD = listValues[1];
                            baocao3.C1_7_PhaiTraTheoTienDoKeHoachHDXD = listValues[2];
                            baocao4.C1_7_PhaiTraTheoTienDoKeHoachHDXD = listValues[3];
                            break;
                        case "3181":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_8_DoanhThuChuaDuocThucHienNganHan = listValues[0];
                            baocao2.C1_8_DoanhThuChuaDuocThucHienNganHan = listValues[1];
                            baocao3.C1_8_DoanhThuChuaDuocThucHienNganHan = listValues[2];
                            baocao4.C1_8_DoanhThuChuaDuocThucHienNganHan = listValues[3];
                            break;
                        case "3182":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_9_PhaiTraNganHanKhac = listValues[0];
                            baocao2.C1_9_PhaiTraNganHanKhac = listValues[1];
                            baocao3.C1_9_PhaiTraNganHanKhac = listValues[2];
                            baocao4.C1_9_PhaiTraNganHanKhac = listValues[3];
                            break;
                        case "3183":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_10_VayVaNoThueTaichinhNganHan = listValues[0];
                            baocao2.C1_10_VayVaNoThueTaichinhNganHan = listValues[1];
                            baocao3.C1_10_VayVaNoThueTaichinhNganHan = listValues[2];
                            baocao4.C1_10_VayVaNoThueTaichinhNganHan = listValues[3];
                            break;
                        case "320":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_11_DuPhongPhaiTraNganHan = listValues[0];
                            baocao2.C1_11_DuPhongPhaiTraNganHan = listValues[1];
                            baocao3.C1_11_DuPhongPhaiTraNganHan = listValues[2];
                            baocao4.C1_11_DuPhongPhaiTraNganHan = listValues[3];
                            break;
                        case "3201":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_12_QuyKhenThuongPhucLoi = listValues[0];
                            baocao2.C1_12_QuyKhenThuongPhucLoi = listValues[1];
                            baocao3.C1_12_QuyKhenThuongPhucLoi = listValues[2];
                            baocao4.C1_12_QuyKhenThuongPhucLoi = listValues[3];
                            break;
                        case "322":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_13_QuyBinhOnGia = listValues[0];
                            baocao2.C1_13_QuyBinhOnGia = listValues[1];
                            baocao3.C1_13_QuyBinhOnGia = listValues[2];
                            baocao4.C1_13_QuyBinhOnGia = listValues[3];
                            break;
                        case "323":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_14_GiaoDichMuaBanTraiPhieuChinhPhu = listValues[0];
                            baocao2.C1_14_GiaoDichMuaBanTraiPhieuChinhPhu = listValues[1];
                            baocao3.C1_14_GiaoDichMuaBanTraiPhieuChinhPhu = listValues[2];
                            baocao4.C1_14_GiaoDichMuaBanTraiPhieuChinhPhu = listValues[3];
                            break;
                        case "330":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_NoDaiHan = listValues[0];
                            baocao2.C2_NoDaiHan = listValues[1];
                            baocao3.C2_NoDaiHan = listValues[2];
                            baocao4.C2_NoDaiHan = listValues[3];
                            break;
                        case "331":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_1_PhaiTraDaiHanNguoiBan = listValues[0];
                            baocao2.C2_1_PhaiTraDaiHanNguoiBan = listValues[1];
                            baocao3.C2_1_PhaiTraDaiHanNguoiBan = listValues[2];
                            baocao4.C2_1_PhaiTraDaiHanNguoiBan = listValues[3];
                            break;
                        case "3311":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_2_NguoiMuaTraTienTruocDaiHan = listValues[0];
                            baocao2.C2_2_NguoiMuaTraTienTruocDaiHan = listValues[1];
                            baocao3.C2_2_NguoiMuaTraTienTruocDaiHan = listValues[2];
                            baocao4.C2_2_NguoiMuaTraTienTruocDaiHan = listValues[3];
                            break;
                        case "3313":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_4_PhaiTraNoiBoVeVonKD = listValues[0];
                            baocao2.C2_4_PhaiTraNoiBoVeVonKD = listValues[1];
                            baocao3.C2_4_PhaiTraNoiBoVeVonKD = listValues[2];
                            baocao4.C2_4_PhaiTraNoiBoVeVonKD = listValues[3];
                            break;
                        case "332":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_5_PhaiTraDaiHanNoiBoDaiHan = listValues[0];
                            baocao2.C2_5_PhaiTraDaiHanNoiBoDaiHan = listValues[1];
                            baocao3.C2_5_PhaiTraDaiHanNoiBoDaiHan = listValues[2];
                            baocao4.C2_5_PhaiTraDaiHanNoiBoDaiHan = listValues[3];
                            break;
                        case "3321":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_6_DoanhThuChuaThucHienDaiHan = listValues[0];
                            baocao2.C2_6_DoanhThuChuaThucHienDaiHan = listValues[1];
                            baocao3.C2_6_DoanhThuChuaThucHienDaiHan = listValues[2];
                            baocao4.C2_6_DoanhThuChuaThucHienDaiHan = listValues[3];
                            break;
                        case "333":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_7_PhaiTraDaiHanKhac = listValues[0];
                            baocao2.C2_7_PhaiTraDaiHanKhac = listValues[1];
                            baocao3.C2_7_PhaiTraDaiHanKhac = listValues[2];
                            baocao4.C2_7_PhaiTraDaiHanKhac = listValues[3];
                            break;
                        case "334":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_8_VayVaNoThueTaiChinhDaiHan = listValues[0];
                            baocao2.C2_8_VayVaNoThueTaiChinhDaiHan = listValues[1];
                            baocao3.C2_8_VayVaNoThueTaiChinhDaiHan = listValues[2];
                            baocao4.C2_8_VayVaNoThueTaiChinhDaiHan = listValues[3];
                            break;
                        case "3341":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_9_TraiPhieuChuyenDoi = listValues[0];
                            baocao2.C2_9_TraiPhieuChuyenDoi = listValues[1];
                            baocao3.C2_9_TraiPhieuChuyenDoi = listValues[2];
                            baocao4.C2_9_TraiPhieuChuyenDoi = listValues[3];
                            break;
                        case "335":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_11_ThueThuNhapHoanLaiPhaiTra = listValues[0];
                            baocao2.C2_11_ThueThuNhapHoanLaiPhaiTra = listValues[1];
                            baocao3.C2_11_ThueThuNhapHoanLaiPhaiTra = listValues[2];
                            baocao4.C2_11_ThueThuNhapHoanLaiPhaiTra = listValues[3];
                            break;
                        case "337":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_12_DuPhongPhaiTraDaiHan = listValues[0];
                            baocao2.C2_12_DuPhongPhaiTraDaiHan = listValues[1];
                            baocao3.C2_12_DuPhongPhaiTraDaiHan = listValues[2];
                            baocao4.C2_12_DuPhongPhaiTraDaiHan = listValues[3];
                            break;
                        case "3371":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_13_QuyPhatTrienKhoaHocCongNghe = listValues[0];
                            baocao2.C2_13_QuyPhatTrienKhoaHocCongNghe = listValues[1];
                            baocao3.C2_13_QuyPhatTrienKhoaHocCongNghe = listValues[2];
                            baocao4.C2_13_QuyPhatTrienKhoaHocCongNghe = listValues[3];
                            break;
                        case "400":
                            listValues = ExtractXmlData(link);
                            baocao1.II_VonChuSoHuu = listValues[0];
                            baocao2.II_VonChuSoHuu = listValues[1];
                            baocao3.II_VonChuSoHuu = listValues[2];
                            baocao4.II_VonChuSoHuu = listValues[3];
                            break;
                        case "410":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_VonChuSoHuu = listValues[0];
                            baocao2.C1_VonChuSoHuu = listValues[1];
                            baocao3.C1_VonChuSoHuu = listValues[2];
                            baocao4.C1_VonChuSoHuu = listValues[3];
                            break;
                        case "4101":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_1_VonGopCuaChuSoHuu = listValues[0];
                            baocao2.C1_1_VonGopCuaChuSoHuu = listValues[1];
                            baocao3.C1_1_VonGopCuaChuSoHuu = listValues[2];
                            baocao4.C1_1_VonGopCuaChuSoHuu = listValues[3];
                            break;
                        case "4102":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_1_1_CoPhieuPhoThongCoQuyenBieuQuyet = listValues[0];
                            baocao2.C1_1_1_CoPhieuPhoThongCoQuyenBieuQuyet = listValues[1];
                            baocao3.C1_1_1_CoPhieuPhoThongCoQuyenBieuQuyet = listValues[2];
                            baocao4.C1_1_1_CoPhieuPhoThongCoQuyenBieuQuyet = listValues[3];
                            break;
                        case "4103":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_1_2_CoPhieuUuDai = listValues[0];
                            baocao2.C1_1_2_CoPhieuUuDai = listValues[1];
                            baocao3.C1_1_2_CoPhieuUuDai = listValues[2];
                            baocao4.C1_1_2_CoPhieuUuDai = listValues[3];
                            break;
                        case "412":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_2_ThangDuVonCoPhan = listValues[0];
                            baocao2.C1_2_ThangDuVonCoPhan = listValues[1];
                            baocao3.C1_2_ThangDuVonCoPhan = listValues[2];
                            baocao4.C1_2_ThangDuVonCoPhan = listValues[3];
                            break;
                        case "4121":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_3_QuyenChonChuyenDoiTraiPhieu = listValues[0];
                            baocao2.C1_3_QuyenChonChuyenDoiTraiPhieu = listValues[1];
                            baocao3.C1_3_QuyenChonChuyenDoiTraiPhieu = listValues[2];
                            baocao4.C1_3_QuyenChonChuyenDoiTraiPhieu = listValues[3];
                            break;
                        case "413":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_4_VonKhacCuaChuSoHuu = listValues[0];
                            baocao2.C1_4_VonKhacCuaChuSoHuu = listValues[1];
                            baocao3.C1_4_VonKhacCuaChuSoHuu = listValues[2];
                            baocao4.C1_4_VonKhacCuaChuSoHuu = listValues[3];
                            break;
                        case "414":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_5_CoPhieuQuy = listValues[0];
                            baocao2.C1_5_CoPhieuQuy = listValues[1];
                            baocao3.C1_5_CoPhieuQuy = listValues[2];
                            baocao4.C1_5_CoPhieuQuy = listValues[3];
                            break;
                        case "415":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_6_ChenhLechDanhGiaLaiTaiSan = listValues[0];
                            baocao2.C1_6_ChenhLechDanhGiaLaiTaiSan = listValues[1];
                            baocao3.C1_6_ChenhLechDanhGiaLaiTaiSan = listValues[2];
                            baocao4.C1_6_ChenhLechDanhGiaLaiTaiSan = listValues[3];
                            break;
                        case "416":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_7_ChenhLechTyGiaHoiDoai = listValues[0];
                            baocao2.C1_7_ChenhLechTyGiaHoiDoai = listValues[1];
                            baocao3.C1_7_ChenhLechTyGiaHoiDoai = listValues[2];
                            baocao4.C1_7_ChenhLechTyGiaHoiDoai = listValues[3];
                            break;
                        case "417":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_8_QuyDauTuPhatTrien = listValues[0];
                            baocao2.C1_8_QuyDauTuPhatTrien = listValues[1];
                            baocao3.C1_8_QuyDauTuPhatTrien = listValues[2];
                            baocao4.C1_8_QuyDauTuPhatTrien = listValues[3];
                            break;
                        case "4171":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_9_QuyHoTroSapXepDoanhNghiep = listValues[0];
                            baocao2.C1_9_QuyHoTroSapXepDoanhNghiep = listValues[1];
                            baocao3.C1_9_QuyHoTroSapXepDoanhNghiep = listValues[2];
                            baocao4.C1_9_QuyHoTroSapXepDoanhNghiep = listValues[3];
                            break;
                        case "419":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_10_QuyKhacThuocVonChuSoHuu = listValues[0];
                            baocao2.C1_10_QuyKhacThuocVonChuSoHuu = listValues[1];
                            baocao3.C1_10_QuyKhacThuocVonChuSoHuu = listValues[2];
                            baocao4.C1_10_QuyKhacThuocVonChuSoHuu = listValues[3];
                            break;
                        case "420":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_11_LoiNhuanSauThueChuaPhanPhoi = listValues[0];
                            baocao2.C1_11_LoiNhuanSauThueChuaPhanPhoi = listValues[1];
                            baocao3.C1_11_LoiNhuanSauThueChuaPhanPhoi = listValues[2];
                            baocao4.C1_11_LoiNhuanSauThueChuaPhanPhoi = listValues[3];
                            break;
                        case "4201":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_11_1_LNSTChuaPhanPhoiLuyKeDenCuoiKyTruoc = listValues[0];
                            baocao2.C1_11_1_LNSTChuaPhanPhoiLuyKeDenCuoiKyTruoc = listValues[1];
                            baocao3.C1_11_1_LNSTChuaPhanPhoiLuyKeDenCuoiKyTruoc = listValues[2];
                            baocao4.C1_11_1_LNSTChuaPhanPhoiLuyKeDenCuoiKyTruoc = listValues[3];
                            break;
                        case "4202":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_11_2_LNSTChuaPhanPhoiKyNay = listValues[0];
                            baocao2.C1_11_2_LNSTChuaPhanPhoiKyNay = listValues[1];
                            baocao3.C1_11_2_LNSTChuaPhanPhoiKyNay = listValues[2];
                            baocao4.C1_11_2_LNSTChuaPhanPhoiKyNay = listValues[3];
                            break;
                        case "421":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_12_NguonVonDauTuXDCB = listValues[0];
                            baocao2.C1_12_NguonVonDauTuXDCB = listValues[1];
                            baocao3.C1_12_NguonVonDauTuXDCB = listValues[2];
                            baocao4.C1_12_NguonVonDauTuXDCB = listValues[3];
                            break;
                        case "4211":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_13_LoiIchCoDongKhongKiemSoat = listValues[0];
                            baocao2.C1_13_LoiIchCoDongKhongKiemSoat = listValues[1];
                            baocao3.C1_13_LoiIchCoDongKhongKiemSoat = listValues[2];
                            baocao4.C1_13_LoiIchCoDongKhongKiemSoat = listValues[3];
                            break;
                        case "4212":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_14_PhuTroiHopNhatCongTyCon = listValues[0];
                            baocao2.C1_14_PhuTroiHopNhatCongTyCon = listValues[1];
                            baocao3.C1_14_PhuTroiHopNhatCongTyCon = listValues[2];
                            baocao4.C1_14_PhuTroiHopNhatCongTyCon = listValues[3];
                            break;
                        case "430":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_NguonKinhPhiVaCacQuyKhac = listValues[0];
                            baocao2.C2_NguonKinhPhiVaCacQuyKhac = listValues[1];
                            baocao3.C2_NguonKinhPhiVaCacQuyKhac = listValues[2];
                            baocao4.C2_NguonKinhPhiVaCacQuyKhac = listValues[3];
                            break;
                        case "432":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_1_NguonKinhPhi = listValues[0];
                            baocao2.C2_1_NguonKinhPhi = listValues[1];
                            baocao3.C2_1_NguonKinhPhi = listValues[2];
                            baocao4.C2_1_NguonKinhPhi = listValues[3];
                            break;
                        case "433":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_2_NguonKinhPhiDaHinhThanhTSCD = listValues[0];
                            baocao2.C2_2_NguonKinhPhiDaHinhThanhTSCD = listValues[1];
                            baocao3.C2_2_NguonKinhPhiDaHinhThanhTSCD = listValues[2];
                            baocao4.C2_2_NguonKinhPhiDaHinhThanhTSCD = listValues[3];
                            break;
                        case "500":
                            listValues = ExtractXmlData(link);
                            baocao1.III_LoiIchCuaCoDongThieuSo = listValues[0];
                            baocao2.III_LoiIchCuaCoDongThieuSo = listValues[1];
                            baocao3.III_LoiIchCuaCoDongThieuSo = listValues[2];
                            baocao4.III_LoiIchCuaCoDongThieuSo = listValues[3];
                            break;
                        #endregion
                        #region ket qua hoat dong kinh doanh
                        case "01":
                            listValues = ExtractXmlData(link);
                            baocao1.C1_DoanhThuBanHangVaCungcapDV = listValues[0];
                            baocao2.C1_DoanhThuBanHangVaCungcapDV = listValues[1];
                            baocao3.C1_DoanhThuBanHangVaCungcapDV = listValues[2];
                            baocao4.C1_DoanhThuBanHangVaCungcapDV = listValues[3];
                            break;
                        case "02":
                            listValues = ExtractXmlData(link);
                            baocao1.C2_CacKhoanGiamTruDanhThu = listValues[0];
                            baocao2.C2_CacKhoanGiamTruDanhThu = listValues[1];
                            baocao3.C2_CacKhoanGiamTruDanhThu = listValues[2];
                            baocao4.C2_CacKhoanGiamTruDanhThu = listValues[3];
                            break;
                        case "10":
                            listValues = ExtractXmlData(link);
                            baocao1.C3_DoanhThuThuanVeBanHangVaCungCapDV = listValues[0];
                            baocao2.C3_DoanhThuThuanVeBanHangVaCungCapDV = listValues[1];
                            baocao3.C3_DoanhThuThuanVeBanHangVaCungCapDV = listValues[2];
                            baocao4.C3_DoanhThuThuanVeBanHangVaCungCapDV = listValues[3];
                            break;
                        case "11":
                            listValues = ExtractXmlData(link);
                            baocao1.C4_GiaVonHangBan = listValues[0];
                            baocao2.C4_GiaVonHangBan = listValues[1];
                            baocao3.C4_GiaVonHangBan = listValues[2];
                            baocao4.C4_GiaVonHangBan = listValues[3];
                            break;
                        case "20":
                            listValues = ExtractXmlData(link);
                            baocao1.C5_LoiNhuanGopVeBanHangVaCungCapDV = listValues[0];
                            baocao2.C5_LoiNhuanGopVeBanHangVaCungCapDV = listValues[1];
                            baocao3.C5_LoiNhuanGopVeBanHangVaCungCapDV = listValues[2];
                            baocao4.C5_LoiNhuanGopVeBanHangVaCungCapDV = listValues[3];
                            break;
                        case "21":
                            listValues = ExtractXmlData(link);
                            baocao1.C6_DoanhThuHoatDongTaiChinh = listValues[0];
                            baocao2.C6_DoanhThuHoatDongTaiChinh = listValues[1];
                            baocao3.C6_DoanhThuHoatDongTaiChinh = listValues[2];
                            baocao4.C6_DoanhThuHoatDongTaiChinh = listValues[3];
                            break;
                        case "22":
                            listValues = ExtractXmlData(link);
                            baocao1.C7_ChiPhiTaiChinh = listValues[0];
                            baocao2.C7_ChiPhiTaiChinh = listValues[1];
                            baocao3.C7_ChiPhiTaiChinh = listValues[2];
                            baocao4.C7_ChiPhiTaiChinh = listValues[3];
                            break;
                        case "23":
                            listValues = ExtractXmlData(link);
                            baocao1.C7_1_ChiPhiLaiVay = listValues[0];
                            baocao2.C7_1_ChiPhiLaiVay = listValues[1];
                            baocao3.C7_1_ChiPhiLaiVay = listValues[2];
                            baocao4.C7_1_ChiPhiLaiVay = listValues[3];
                            break;
                        case "24":
                            listValues = ExtractXmlData(link);
                            baocao1.C8_PhanLaiLoTrongCtyLienDoanhLienKet = listValues[0];
                            baocao2.C8_PhanLaiLoTrongCtyLienDoanhLienKet = listValues[1];
                            baocao3.C8_PhanLaiLoTrongCtyLienDoanhLienKet = listValues[2];
                            baocao4.C8_PhanLaiLoTrongCtyLienDoanhLienKet = listValues[3];
                            break;
                        case "25":
                            listValues = ExtractXmlData(link);
                            baocao1.C9_ChiPhiBanHang = listValues[0];
                            baocao2.C9_ChiPhiBanHang = listValues[1];
                            baocao3.C9_ChiPhiBanHang = listValues[2];
                            baocao4.C9_ChiPhiBanHang = listValues[3];
                            break;
                        case "26":
                            listValues = ExtractXmlData(link);
                            baocao1.C10_ChiPhiQuanLyDoanhNghiep = listValues[0];
                            baocao2.C10_ChiPhiQuanLyDoanhNghiep = listValues[1];
                            baocao3.C10_ChiPhiQuanLyDoanhNghiep = listValues[2];
                            baocao4.C10_ChiPhiQuanLyDoanhNghiep = listValues[3];
                            break;
                        case "30":
                            listValues = ExtractXmlData(link);
                            baocao1.C11_LoiNhuanThuanTuHoatDongKinhDoanh = listValues[0];
                            baocao2.C11_LoiNhuanThuanTuHoatDongKinhDoanh = listValues[1];
                            baocao3.C11_LoiNhuanThuanTuHoatDongKinhDoanh = listValues[2];
                            baocao4.C11_LoiNhuanThuanTuHoatDongKinhDoanh = listValues[3];
                            break;
                        case "31":
                            listValues = ExtractXmlData(link);
                            baocao1.C12_ThuNhapKhac = listValues[0];
                            baocao2.C12_ThuNhapKhac = listValues[1];
                            baocao3.C12_ThuNhapKhac = listValues[2];
                            baocao4.C12_ThuNhapKhac = listValues[3];
                            break;
                        case "32":
                            listValues = ExtractXmlData(link);
                            baocao1.C13_ChiPhiKhac = listValues[0];
                            baocao2.C13_ChiPhiKhac = listValues[1];
                            baocao3.C13_ChiPhiKhac = listValues[2];
                            baocao4.C13_ChiPhiKhac = listValues[3];
                            break;
                        case "40":
                            listValues = ExtractXmlData(link);
                            baocao1.C14_LoiNhuanKhac = listValues[0];
                            baocao2.C14_LoiNhuanKhac = listValues[1];
                            baocao3.C14_LoiNhuanKhac = listValues[2];
                            baocao4.C14_LoiNhuanKhac = listValues[3];
                            break;
                        case "50":
                            listValues = ExtractXmlData(link);
                            baocao1.C15_TongLoiNhuanKeToanTruocThue = listValues[0];
                            baocao2.C15_TongLoiNhuanKeToanTruocThue = listValues[1];
                            baocao3.C15_TongLoiNhuanKeToanTruocThue = listValues[2];
                            baocao4.C15_TongLoiNhuanKeToanTruocThue = listValues[3];
                            break;
                        case "51":
                            listValues = ExtractXmlData(link);
                            baocao1.C16_ChiPhiThueTNDNHienHanh = listValues[0];
                            baocao2.C16_ChiPhiThueTNDNHienHanh = listValues[1];
                            baocao3.C16_ChiPhiThueTNDNHienHanh = listValues[2];
                            baocao4.C16_ChiPhiThueTNDNHienHanh = listValues[3];
                            break;
                        case "52":
                            listValues = ExtractXmlData(link);
                            baocao1.C17_ChiPhiThueTNDNHoanLai = listValues[0];
                            baocao2.C17_ChiPhiThueTNDNHoanLai = listValues[1];
                            baocao3.C17_ChiPhiThueTNDNHoanLai = listValues[2];
                            baocao4.C17_ChiPhiThueTNDNHoanLai = listValues[3];
                            break;
                        case "60":
                            listValues = ExtractXmlData(link);
                            baocao1.C18_LoiNhuanSauThueTNDN = listValues[0];
                            baocao2.C18_LoiNhuanSauThueTNDN = listValues[1];
                            baocao3.C18_LoiNhuanSauThueTNDN = listValues[2];
                            baocao4.C18_LoiNhuanSauThueTNDN = listValues[3];
                            break;
                        case "61":
                            listValues = ExtractXmlData(link);
                            baocao1.C18_1_LoiIchCuaCoDongThieuSo = listValues[0];
                            baocao2.C18_1_LoiIchCuaCoDongThieuSo = listValues[1];
                            baocao3.C18_1_LoiIchCuaCoDongThieuSo = listValues[2];
                            baocao4.C18_1_LoiIchCuaCoDongThieuSo = listValues[3];
                            break;
                        case "62":
                            listValues = ExtractXmlData(link);
                            baocao1.C18_2_LoiNhuanSauThueCuaCtyMe = listValues[0];
                            baocao2.C18_2_LoiNhuanSauThueCuaCtyMe = listValues[1];
                            baocao3.C18_2_LoiNhuanSauThueCuaCtyMe = listValues[2];
                            baocao4.C18_2_LoiNhuanSauThueCuaCtyMe = listValues[3];
                            break;
                        case "70":
                            listValues = ExtractXmlData(link);
                            baocao1.C19_LaiCoBanTrenCoPhieu = listValues[0];
                            baocao2.C19_LaiCoBanTrenCoPhieu = listValues[1];
                            baocao3.C19_LaiCoBanTrenCoPhieu = listValues[2];
                            baocao4.C19_LaiCoBanTrenCoPhieu = listValues[3];
                            break;
                            #endregion
                    }
                    #endregion
                }
            }
        }
        private List<long> ExtractXmlData(HtmlNode link)
        {
            List<long> listValue = new List<long>();
            HtmlNodeCollection tdNodes = link.ChildNodes;
            foreach (HtmlNode node in tdNodes)
            {
                if (node.Attributes["class"] != null)
                {
                    if (node.Attributes["class"].Value == "b_r_c")
                    {
                        string content = node.WriteContentTo();
                        content = content.Replace(" ", "");
                        try
                        {
                            if (content == "")
                            {
                                long value = 0;
                                listValue.Add(value);
                            }
                            else
                            {
                                long value = Convert.ToInt64(content.Replace(",", ""));
                                listValue.Add(value);
                            }

                        }
                        catch (Exception)
                        {
                            //Console.WriteLine("=============={0}", content.Replace(",", ""));
                        }

                    }
                }

            }
            return listValue;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string maCkString = File.ReadAllText("MaCK.txt");
            string[] maCks = maCkString.Split('_');

            for (int i = 0; i < maCks.Length; i++)
            {
                congty cty = new congty();

                cty.mack = maCks[i];
                cty.nhomnganh = "";
                cty.tencty = "";
                entities.congties.Add(cty);
            }
            entities.SaveChanges();
        }
    }
}
