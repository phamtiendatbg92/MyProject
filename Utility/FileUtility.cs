using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.Utility
{
    public static class FileUtility
    {
        public static string[] ReadAllMack()
        {
            string maCkString = File.ReadAllText("MaCK.txt");
            string[] maCks = maCkString.Split('_');
            return maCks;
        }

        public static void WriteResultToTextFile(List<StringBuilder> listContent, string url)
        {
            string[] content = new string[listContent.Count];
            for (int i = 0; i < listContent.Count; i++)
            {
                content[i] = listContent[i].ToString();
            }
            File.WriteAllLines(url, content);
        }
        public static StringBuilder InitResultData(string mack, string nam, string quy)
        {
            StringBuilder s = new StringBuilder();
            AppendValue(s, Fields.MaCk, mack);
            AppendValue(s, Fields.Quy, quy);
            AppendValue(s, Fields.Nam, nam);
            return s;
        }
        public static void AppendValue(StringBuilder s, string field, string value)
        {
            s.Append(field + "_" + value + ";");
        }
    }
}
