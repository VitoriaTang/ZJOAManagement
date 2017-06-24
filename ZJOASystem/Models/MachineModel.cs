using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ZJOASystem.Models
{
    public class MachineModel
    {
        public int Id { get; set; }
        public string Encode { get; set; }
        public string Name { get; set; }
        public List<Operator> Users { get; set; }

        public string UsersEncodeText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                if (Users != null && Users.Count > 0)
                {
                    foreach (Operator item in Users)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(",");
                        }
                        builder.Append(item.Encode);
                    }
                }
                return builder.ToString();
            }
        }

        public string UsersNameText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                if (Users != null && Users.Count > 0)
                {
                    foreach (Operator item in Users)
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(",");
                        }
                        builder.Append(item.Name);
                    }
                }
                return builder.ToString();
            }
        }
    }
}