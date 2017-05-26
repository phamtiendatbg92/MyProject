using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis.Utility
{
    public static class DatabaseUtility
    {
        private static stocksqlEntities entities = new stocksqlEntities();
        public static List<congty> GetAllCty()
        {
            return entities.congties.ToList();
        }
        public static Dictionary<string, List<bctc>> GetAllBctc()
        {
            Dictionary<string, List<bctc>> bctcDic = new Dictionary<string, List<bctc>>();
            List<bctc> bctcList = entities.bctcs.ToList();
            int count = bctcList.Count;
            for (int i = 0; i < count; i++)
            {
                if (bctcDic.ContainsKey(bctcList[i].mack))
                {
                    bctcDic[bctcList[i].mack].Add(bctcList[i]);
                }
                else
                {
                    List<bctc> listTemp = new List<bctc>();
                    listTemp.Add(bctcList[i]);
                    bctcDic.Add(bctcList[i].mack, listTemp);
                }
            }
            return bctcDic;
        }
        public static Dictionary<string, List<bctc>> GetBctcNeedUpdate()
        {
            Dictionary<string, List<bctc>> bctcDic = new Dictionary<string, List<bctc>>();
            List<bctc> bctcList = entities.bctcs.ToList();
            int count = bctcList.Count;
            for (int i = 0; i < count; i++)
            {
                if (!bctcList[i].IsChecked)
                {
                    if (bctcDic.ContainsKey(bctcList[i].mack))
                    {
                        bctcDic[bctcList[i].mack].Add(bctcList[i]);
                    }
                    else
                    {
                        List<bctc> listTemp = new List<bctc>();
                        listTemp.Add(bctcList[i]);
                        bctcDic.Add(bctcList[i].mack, listTemp);
                    }
                }
            }
            return bctcDic;
        }
        public static congty FindCtyByMack(string mack)
        {
            List<congty> abc = GetAllCty();
            int count = abc.Count;
            for (int i = 0; i < count; i++)
            {
                if (abc[i].mack == mack)
                {
                    return abc[i];
                }
            }
            return null;
        }
        public static void SaveChange()
        {
            entities.SaveChanges();
        }
    }
}
