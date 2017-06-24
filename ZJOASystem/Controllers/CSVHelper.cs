using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

        public static List<ProductBase> OpenCSV(string csvContent)
        {
            if (string.IsNullOrEmpty(csvContent))
            {
                return null;
            }

            List<ProductBase> result = new List<ProductBase>();
            string[] lines = csvContent.Split('\r');
            int numberIndex = 0;
            int nameIndex = -1;
            int parentnumberIndex = -1;

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

            int startIndex = 1;
            int columnCount = 3;

            Dictionary<ProductBase, int> tempResultList = new Dictionary<ProductBase, int>();
            for (int i = startIndex; i < lines.Length;)
            {
                string line = lines[i];
                int nextIndex = i;
                List<string> tempList = null;
                List<string> fields = CombineNewLine(line, i, columnCount, ref lines, out nextIndex, ref tempList);

                string number = fields[numberIndex];
                string parentNumber = Convert.ToString(fields[parentnumberIndex]);
                if (string.IsNullOrEmpty(parentNumber))
                {
                    parentNumber = "0";
                }
                ProductBase productItem = new ProductBase(number, fields[nameIndex], parentNumber);

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


        internal static List<ActionRecord> OpenActionRecordCSV(string csvContent)
        {
            if (string.IsNullOrEmpty(csvContent))
            {
                return null;
            }

            List<ActionRecord> result = new List<ActionRecord>();
            string[] lines = csvContent.Split('\r');
            int numberIndex = -1;
            int nameIndex = -1;
            int parentnumberIndex = -1;
            int actionTypeIndex = -1;
            int actionTimeIndex = -1;
            int operatorIndex = -1;
            int additonalInfoIndex = -1;
            string columnList = lines[0];
            string[] columns = columnList.Split(',');
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Equals("ProductNumber", StringComparison.CurrentCultureIgnoreCase))
                {
                    numberIndex = i;
                }
                else if (columns[i].Equals("ProductName", StringComparison.CurrentCultureIgnoreCase))
                {
                    nameIndex = i;
                }
                else if (columns[i].Equals("ParentNumber", StringComparison.CurrentCultureIgnoreCase))
                {
                    parentnumberIndex = i;
                }
                else if (columns[i].Equals("ActionType", StringComparison.CurrentCultureIgnoreCase))
                {
                    actionTypeIndex = i;
                }
                else if (columns[i].Equals("ActionTime", StringComparison.CurrentCultureIgnoreCase))
                {
                    actionTimeIndex = i;
                }
                else if (columns[i].Equals("Operators", StringComparison.CurrentCultureIgnoreCase))
                {
                    operatorIndex = i;
                }
                else if (columns[i].Equals("AdditionalInfo", StringComparison.CurrentCultureIgnoreCase))
                {
                    additonalInfoIndex = i;
                }
            }

            int startIndex = 1;

            // We will not consider the \r\n cases
            for (int i = startIndex; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.TrimStart('\n');
                if(string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string[] fields = line.Split(',');

                string name = fields[nameIndex];
                string number = fields[numberIndex];
                string parentNumber = (parentnumberIndex == -1) ? null : fields[parentnumberIndex];
                string actionType = fields[actionTypeIndex];
                string actionTime = fields[actionTimeIndex];
                string actionComments = "Import from CSV file";
                string operators = fields[operatorIndex];

                ActionRecord recordItem = new ActionRecord();
                recordItem.ProductName = name;
                recordItem.ProductNumber = number;
                recordItem.ParentNumber = parentNumber;
                if (additonalInfoIndex != -1)
                {
                    recordItem.AdditionalInfo = fields[additonalInfoIndex];
                }
                int actionTypeValue = 0;
                int.TryParse(actionType, out actionTypeValue);
                recordItem.ActionType = (ActionType)actionTypeValue;
                DateTime timeValue = DateTime.Now;
                if (!DateTime.TryParse(actionTime, out timeValue))
                {
                    timeValue = DateTime.Now;
                }
                recordItem.ActionTime = timeValue;
                recordItem.ActionComments = actionComments;

                if (!string.IsNullOrEmpty(operators))
                {
                    recordItem.Operators = new List<Operator>();
                    string[] operatorsArray = operators.Split(';');
                    foreach (string optValue in operatorsArray)
                    {
                        string optEncode = optValue.Trim();
                        if (!string.IsNullOrEmpty(optEncode))
                        {
                            Operator optObj = new Operator();
                            optObj.Encode = optEncode;

                            recordItem.Operators.Add(optObj);
                        }
                    }
                }

                result.Add(recordItem);
            }

            return result;
        }

        internal static string SaveActionRecordCSV(List<ActionRecord> data, bool needExportAdditonal)
        {
            string filepath = ConfigurationManager.AppSettings["outputFolder"];
            filepath = filepath + Guid.NewGuid().ToString() + ".csv";
            using (FileStream stream = File.OpenWrite(filepath))
            {

                StringBuilder sb = new StringBuilder();

                String header = "ProductNumber,ProductName,ParentNumber,ActionType,ActionTime,Operators";
                if (needExportAdditonal)
                {
                    header += ",AdditionalInfo";
                }
                sb.AppendLine(header);

                if (data != null && data.Count > 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        ActionRecord item = data[i];
                        string itemText = string.Join(",", item.ProductNumber, item.ProductName, item.ParentNumber, Convert.ToInt32(item.ActionType), item.ActionTime.ToString("yyyyMMdd HH:mm:ss"), item.OperatorsEncodeText);

                        if (needExportAdditonal)
                        {
                            itemText += "," + item.AdditionalInfo;
                        }
                        sb.AppendLine(itemText);
                    }

                    byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(sb.ToString());

                    stream.Write(bytes, 0, bytes.Length);

                    stream.Close();
                }
            }

            return filepath;
        }
    }
}