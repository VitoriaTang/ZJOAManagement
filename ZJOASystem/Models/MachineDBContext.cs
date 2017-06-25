using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ZJOASystem.Models
{
    public class MachineDBContext : DbContext
    {
        public MachineDBContext()
            : base("name=DefaultConnection")
        {
            System.Data.Entity.Database.SetInitializer(new MySqlMachineInitializer());
        }

        internal void SaveMachineRecord(MachineRecord item)
        {
            string sql  = string.Format("CALL proc_assign_machine ('{0}', '{1}' ,{2}, '{3}', '{4}','{5}')",
                  item.Encode, item.Name, Convert.ToInt32(item.AssignType), item.AssignTime, item.AssignComments, item.UsersEncodeText);
            this.Database.ExecuteSqlCommand(sql);
        }


        public static string GET_MACHINEACTIONS = @"SELECT a.Id, b.Encode, b.Name, a.AssignType, a.AssignTime, a.AssignComments 
                FROM machine_assigns a LEFT JOIN machines b on a.MachineId=b.Id ";

        public static string CHECK_MACHINEACTION_EXIST = @"SELECT Count(a.Id)
                FROM machine_assigns a LEFT JOIN machines b on a.MachineId=b.Id WHERE b.Encode='{0}' AND a.AssignTime='{1}' AND a.AssignType={2}";
        public static string GET_MACHINE_USERS = "SELECT b.Encode, b.Name From machine_users a INNER JOIN Employees b on a.UserEncode=b.Encode WHERE a.AssignId={0}";
    }
}