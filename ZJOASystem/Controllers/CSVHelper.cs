using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
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

            for (int i = 1; i < lines.Length;i++)
            {
                string line = lines[i];
                line = line.TrimStart('\n');
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string[] fields = line.Split(',');

                string number = fields[numberIndex];
                string parentNumber = Convert.ToString(fields[parentnumberIndex]);
                if (string.IsNullOrEmpty(parentNumber))
                {
                    parentNumber = "0";
                }
                ProductBase productItem = new ProductBase(number, fields[nameIndex], parentNumber);

                result.Add(productItem);
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
            int boxIndex = -1;
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
                else if (columns[i].Equals("Box", StringComparison.CurrentCultureIgnoreCase))
                {
                    boxIndex = i;
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
                DateTime timeValue = GetDatetime(actionTime, "yyyyMMdd HH:mm:ss");
                
                recordItem.ActionTime = timeValue;
                recordItem.ActionComments = actionComments;

                if (boxIndex > -1)
                {
                    recordItem.BoxNumber = fields[boxIndex];
                }
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

        internal static string SaveActionRecordCSV(List<ActionRecord> data, bool needExportAdditonal, bool needBoxInfo)
        {
            string filepath = ConfigurationManager.AppSettings["outputFolder"];
            filepath = filepath + Guid.NewGuid().ToString() + ".csv";
            using (FileStream stream = File.OpenWrite(filepath))
            {

                StringBuilder sb = new StringBuilder();

                String header = "ProductNumber,ProductName,ParentNumber,ActionType,ActionTime,Operators";
                if (needBoxInfo)
                {
                    header += ",Box";
                }
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
                        if (needBoxInfo)
                        {
                            itemText += "," + item.BoxNumber;
                        }
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

        internal static List<MachineRecord> OpenMachineRecordCSV(string csvContent)
        {
            if (string.IsNullOrEmpty(csvContent))
            {
                return null;
            }

            List<MachineRecord> result = new List<MachineRecord>();
            string[] lines = csvContent.Split('\r');
            int numberIndex = -1;
            int nameIndex = -1;
            int assignTypeIndex = -1;
            int assignTimeIndex = -1;
            int userIndex = -1;
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
                else if (columns[i].Equals("AssignType", StringComparison.CurrentCultureIgnoreCase))
                {
                    assignTypeIndex = i;
                }
                else if (columns[i].Equals("AssignTime", StringComparison.CurrentCultureIgnoreCase))
                {
                    assignTimeIndex = i;
                }
                else if (columns[i].Equals("Users", StringComparison.CurrentCultureIgnoreCase))
                {
                    userIndex = i;
                }
            }

            int startIndex = 1;

            // We will not consider the \r\n cases
            for (int i = startIndex; i < lines.Length; i++)
            {
                string line = lines[i];
                line = line.TrimStart('\n');
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string[] fields = line.Split(',');

                string name = fields[nameIndex];
                string number = fields[numberIndex];
                string assignType = fields[assignTypeIndex];
                string assignTime = fields[assignTimeIndex];
                string assignComments = "Import from CSV file";
                string users = fields[userIndex];

                MachineRecord recordItem = new MachineRecord();
                recordItem.Name = name;
                recordItem.Encode = number;
                int assignTypeValue = 0;
                int.TryParse(assignType, out assignTypeValue);
                recordItem.AssignType = (AssignType)assignTypeValue;
                DateTime timeValue = GetDatetime(assignTime, "yyyyMMdd HH:mm:ss");
                recordItem.AssignTime = timeValue;
                recordItem.AssignComments = assignComments;

                if (!string.IsNullOrEmpty(users))
                {
                    recordItem.Users = new List<Operator>();
                    string[] operatorsArray = users.Split(';');
                    foreach (string optValue in operatorsArray)
                    {
                        string optEncode = optValue.Trim();
                        if (!string.IsNullOrEmpty(optEncode))
                        {
                            Operator optObj = new Operator();
                            optObj.Encode = optEncode;

                            recordItem.Users.Add(optObj);
                        }
                    }
                }

                result.Add(recordItem);
            }

            return result;
        }

        private static DateTime GetDatetime(string timeString, string timeFormat)
        {
            DateTime timeValue = DateTime.Now;
            IFormatProvider formatProvider = CultureInfo.CurrentCulture;
            if (!DateTime.TryParseExact(timeString,timeFormat , formatProvider, DateTimeStyles.AssumeLocal, out timeValue))
            {
                timeValue = DateTime.Now;
            }
            return timeValue;
        }

        internal static string SaveMachineRecordCSV(List<MachineRecord> data)
        {
            string filepath = ConfigurationManager.AppSettings["outputFolder"];
            filepath = filepath + Guid.NewGuid().ToString() + ".csv";
            using (FileStream stream = File.OpenWrite(filepath))
            {

                StringBuilder sb = new StringBuilder();

                String header = "Number,Name,AssignType,AssignTime,Users";
              
                sb.AppendLine(header);

                if (data != null && data.Count > 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        MachineRecord item = data[i];
                        string itemText = string.Join(",", item.Encode, item.Name,  Convert.ToInt32(item.AssignType), item.AssignTime.ToString("yyyyMMdd HH:mm:ss"), item.UsersEncodeText);

                      
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