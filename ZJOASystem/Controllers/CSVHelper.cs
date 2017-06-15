using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using ZJOASystem.Models;

namespace ZJOASystem.Controllers
{
    public class CSVHelper
    {
        public static String SaveCSV(List<ProductBase> datalist, bool allowExportColumn)
        {
            StringBuilder builder = new StringBuilder();

            if (allowExportColumn)
            {
                builder.AppendLine("Number,Name,ParentNumber" );
            }
            //Column
            for (int i = 0; i < datalist.Count; i++)
            {
                ProductBase item = datalist[i];

                string number = item.Number;
                
                string name = (item.Name.Contains(" ") || item.Name.Contains('"')
                        || item.Name.Contains('\r') || item.Name.Contains('\n'))? string.Format("\"{0}\"", item.Name): item.Name;

                builder.AppendLine(string.Format("{0},{1},{2}", item.Number, name, item.ParentNumber));
            }
            
            return builder.ToString();
        }

        public static List<ProductBase> OpenCSV(string csvContent, bool firstLineIsColumn)
        {
            if (string.IsNullOrEmpty(csvContent))
            {
                return null;
            }

            List<ProductBase> result = new List<ProductBase>();
            string[] lines = csvContent.Split('\r');
            int numberIndex = 0;
            int nameIndex = 1;
            int parentnumberIndex = 2;
            if (firstLineIsColumn)
            {
                string columnList = lines[0];
                string[] columns = columnList.Split(',');
                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].Equals("Number", StringComparison.CurrentCultureIgnoreCase))
                    {
                        numberIndex = i;
                    }
                    else if (columns[i].Equals("Name", StringComparison.CurrentCultureIgnoreCase))
                    {
                        nameIndex = i;
                    }
                    else if (columns[i].Equals("ParentNumber", StringComparison.CurrentCultureIgnoreCase))
                    {
                        parentnumberIndex = i;
                    }
                }
            }

            int startIndex = firstLineIsColumn ? 1 : 0;
            int columnCount = 3;

            Dictionary< ProductBase, int> tempResultList = new Dictionary< ProductBase, int>();
            for (int i = startIndex; i < lines.Length; )
            {
                string line = lines[i];
                int nextIndex = i;
                List<string> tempList = null;
                List<string> columns = CombineNewLine(line, i, columnCount, ref lines, out nextIndex, ref tempList);

                string number= columns[numberIndex];
                string parentNumber = Convert.ToString(columns[parentnumberIndex]);
                if (string.IsNullOrEmpty(parentNumber))
                {
                    parentNumber = "0";
                }
                ProductBase productItem = new ProductBase(number, columns[nameIndex], parentNumber);

                result.Add(productItem);
                i = nextIndex;
            }

            return result;
        }

        private static List<string> CombineNewLine(string line, int index, int columnCount, ref string[] lines, out int nextIndex, ref List<String> innerList)
        {
            nextIndex = index;
            if (!line.Contains('"'))
            {
                nextIndex = index + 1;
                return new List<string>(line.Split(','));
            }

            List<string> result = new List<string>();
            if (innerList != null)
            {
                result = innerList;
            }
            
            StringBuilder itemBuilder = new StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ',')
                {
                    if (itemBuilder.Length == 0)
                    {
                        result.Add("");
                    }
                    else
                    {
                        string itemText = itemBuilder.ToString();
                        if (itemText.Contains('"'))
                        {
                            string tempText = itemText.Replace("\"", "");
                            if ((itemText.Length - tempText.Length) % 2 != 0)
                            {
                                itemBuilder.Append(",");
                                break;
                            }
                        }
                        
                    }
                    
                    result.Add(itemBuilder.ToString());
                }
            }

            if (result.Count < columnCount)
            {
                return CombineNewLine(itemBuilder.ToString() + lines[index + 1], index + 1, columnCount, ref lines, out nextIndex, ref result);
            }
            else
            {
                nextIndex = index + 1;
                return result;
            }
        }

    }
}