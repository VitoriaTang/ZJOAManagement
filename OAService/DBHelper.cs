using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MySql.Data.MySqlClient;

namespace OAService
{
    public class DBHelper
    {
        public static string DBConnection = "server=localhost;User Id=root;password=Pass1234;Persist Security Info=True;database=zjoadb;charset=utf8";

        public static string GetRecords(string sqlQuery)
        {
            StringBuilder builder = new StringBuilder();

            using (MySqlConnection connection = new MySqlConnection(DBConnection))
            {
                MySqlCommand command = new MySqlCommand(sqlQuery);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    StringBuilder recordBuilder = new StringBuilder();
                    recordBuilder.Append("{");
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string fieldName = reader.GetName(i);
                        string fieldValue =Convert.ToString( reader.GetValue(i));

                        recordBuilder.AppendFormat("\"{0}\":\"{1}\"", reader.GetName(i), fieldValue);
                        if (i > 0)
                        {
                            recordBuilder.Append(",");
                        }
                    }
                    recordBuilder.Append("}");

                    if (builder.Length > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append(recordBuilder.ToString());
                }
                
                reader.Close();
            }
            return "{"+builder.ToString()+"}";
        }
    }
}