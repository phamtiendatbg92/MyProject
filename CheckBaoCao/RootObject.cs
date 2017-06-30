using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.CheckBaoCao
{
    public class RootObject
    {
        public Model model { get; set; }
        public Error error { get; set; }
    }

    public class FinanceInfoFirst
    {
        public string QUARTER { get; set; }
        public int companyId { get; set; }
        public string currencyCode { get; set; }
        public int displayLevel { get; set; }
        public int displayOrder { get; set; }
        public bool firstPagingIndex { get; set; }
        public bool hasChild { get; set; }
        public string itemName { get; set; }
        public string locale { get; set; }
        public string month101112 { get; set; }
        public string month123 { get; set; }
        public string month456 { get; set; }
        public string month789 { get; set; }
        public int numberItem { get; set; }
        public int pagingIndex { get; set; }
        public string q1 { get; set; }
        public string q2 { get; set; }
        public string q3 { get; set; }
        public string q4 { get; set; }
        public string reportType { get; set; }
        public object strCreatedBy { get; set; }
        public object strCreatedDate { get; set; }
        public object strCreatedDateFrom { get; set; }
        public object strCreatedDateTo { get; set; }
        public string strFiscalDate1 { get; set; }
        public string strFiscalDate2 { get; set; }
        public string strFiscalDate3 { get; set; }
        public string strFiscalDate4 { get; set; }
        public string strFiscalDate5 { get; set; }
        public object strModifiedBy { get; set; }
        public object strModifiedDateFrom { get; set; }
        public object strModifiedDateTo { get; set; }
        public string strNumericValue1 { get; set; }
        public string strNumericValue2 { get; set; }
        public string strNumericValue3 { get; set; }
        public string strNumericValue4 { get; set; }
        public string strNumericValue5 { get; set; }
        public bool usingPaging { get; set; }
    }

    public class FinanceInfoList
    {
        public string QUARTER { get; set; }
        public int companyId { get; set; }
        public string currencyCode { get; set; }
        public int displayLevel { get; set; }
        public int displayOrder { get; set; }
        public bool firstPagingIndex { get; set; }
        public bool hasChild { get; set; }
        public string itemName { get; set; }
        public string locale { get; set; }
        public string month101112 { get; set; }
        public string month123 { get; set; }
        public string month456 { get; set; }
        public string month789 { get; set; }
        public int numberItem { get; set; }
        public int pagingIndex { get; set; }
        public string q1 { get; set; }
        public string q2 { get; set; }
        public string q3 { get; set; }
        public string q4 { get; set; }
        public string reportType { get; set; }
        public object strCreatedBy { get; set; }
        public object strCreatedDate { get; set; }
        public object strCreatedDateFrom { get; set; }
        public object strCreatedDateTo { get; set; }
        public string strFiscalDate1 { get; set; }
        public string strFiscalDate2 { get; set; }
        public string strFiscalDate3 { get; set; }
        public string strFiscalDate4 { get; set; }
        public string strFiscalDate5 { get; set; }
        public object strModifiedBy { get; set; }
        public object strModifiedDateFrom { get; set; }
        public object strModifiedDateTo { get; set; }
        public string strNumericValue1 { get; set; }
        public string strNumericValue2 { get; set; }
        public string strNumericValue3 { get; set; }
        public string strNumericValue4 { get; set; }
        public string strNumericValue5 { get; set; }
        public bool usingPaging { get; set; }
    }

    public class PagingInfo
    {
        public int index { get; set; }
        public int indexPage { get; set; }
        public int offset { get; set; }
        public List<object> pagingItems { get; set; }
        public int total { get; set; }
        public int totalPage { get; set; }
    }

    public class SearchObject
    {
        public bool annual { get; set; }
        public int companyId { get; set; }
        public bool firstPagingIndex { get; set; }
        public string fiscalQuarter { get; set; }
        public string fiscalYear { get; set; }
        public string locale { get; set; }
        public string moneyRate { get; set; }
        public int numberItem { get; set; }
        public int numberTerm { get; set; }
        public int pagingIndex { get; set; }
        public string reportType { get; set; }
        public object strCreatedBy { get; set; }
        public object strCreatedDate { get; set; }
        public object strCreatedDateFrom { get; set; }
        public object strCreatedDateTo { get; set; }
        public object strModifiedBy { get; set; }
        public object strModifiedDateFrom { get; set; }
        public object strModifiedDateTo { get; set; }
        public bool usingPaging { get; set; }
    }

    public class Model
    {
        public object breadcrumbs { get; set; }
        public object callbackFunc { get; set; }
        public object callerKey { get; set; }
        public object callerValue { get; set; }
        public FinanceInfoFirst financeInfoFirst { get; set; }
        public List<FinanceInfoList> financeInfoList { get; set; }
        public string locale { get; set; }
        public object moneyRateCol { get; set; }
        public object pageDescription { get; set; }
        public object pageKeywords { get; set; }
        public object pageTitle { get; set; }
        public PagingInfo pagingInfo { get; set; }
        public List<object> quarterList { get; set; }
        public SearchObject searchObject { get; set; }
        public object symbol { get; set; }
        public List<int> termList { get; set; }
        public List<object> yearList { get; set; }
    }

    public class FieldErrors
    {
    }

    public class Error
    {
        public List<object> actionErrors { get; set; }
        public List<object> actionMessages { get; set; }
        public FieldErrors fieldErrors { get; set; }
        public bool hasActionErrors { get; set; }
        public bool hasActionMessages { get; set; }
        public bool hasErrors { get; set; }
        public bool hasFieldErrors { get; set; }
    }

    
}
