using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZJOASystem.Models
{
    public class ProductDBContext : DbContext
    {
        
        public static string GETPRODUCTBASES_SQL = "SELECT Number,Name,ParentNumber FROM productbases WHERE ParentNumber='{0}'";
        public static string GETALLPRODUCTBASES_SQL = "SELECT Number,Name,ParentNumber FROM productbases";
        public static string DELETEPRODUCTBASES_SQL = "DELETE FROM productbases WHERE Number='{0}';";
        public static string INSERTPRODUCTBASES_SQL = " INSERT INTO productbases(Number,Name,ParentNumber) VALUES ('{0}','{1}','{2}');";

        public static string GETNUMBER_BYPARENT_SQL = "SELECT Number FROM productbases WHERE ParentNumber='{0}'";

        public static string GETALLPRODUCTLIST_SQL = @"SELECT Id, Name, Description, ProductBaseNumber,YearNumber, BatchNumber, SerialNumber, Status,ProductGuid FROM Products
            WHERE Status<{0} ";

        public static string GET_TOP_PRODUCTLIST_SQL = @"SELECT a.Id, a.Name, a.Description, a.ProductBaseNumber,
            a.YearNumber, a.BatchNumber, a.SerialNumber, a.Status, a.ProductGuid FROM products a 
            LEFT JOIN productbases b ON a.ProductBaseNumber=b.Number 
            WHERE a.Status<{0} AND ( b.ParentNumber is null OR b.ParentNumber='0')";

        public static string GET_PRODUCTLIST_BYPARENTBASENUMBER_SQL = @"SELECT a.Id, a.Name, a.Description, a.ProductBaseNumber,
            a.YearNumber, a.BatchNumber, a.SerialNumber, a.Status, a.ProductGuid FROM products a 
            LEFT JOIN productbases b ON a.ProductBaseNumber=b.Number 
            WHERE a.Status<{0} AND b.ParentNumber ='{1}'";

        public static string GET_PRODUCTLIST_BYNUMBER_SQL = @"SELECT Id, a.Name, a.Description, a.ProductBaseNumber,
            a.YearNumber, a.BatchNumber, a.SerialNumber, a.Status, a.ProductGuid FROM products a 
            WHERE a.Status<{0} AND a.ProductBaseNumber ='{1}'";

        public static string GET_PRODUCTBASE_TOP_SQL = @"SELECT Number FROM productbases WHERE ParentNumber ='{0}'";

        public static string INSERT_PRODUCT_SQL = @"INSERT INTO Products 
            (Name, Description, ProductBaseNumber,YearNumber, BatchNumber, SerialNumber,ProductGuid) 
            VALUES ('{0}','{1}', '{2}','{3}','{4}','{5}','{6}')";

       
       
        public static string DELETE_PRODUCT_SQL = "DELETE FREOM productlists WHERE ProductGuid='{0}'";
        public static string UPDATE_PRODUCT_STATUS_SQL = "UPDATE Products SET Status={0} WHERE ProductGuid='{1}'";

        public static string UPDATE_PRODUCT_PROPERTIES = @"UPDATE Products SET Name = '{0}', Description='{1}', Encode='{2}' 
            WHERE Id={3}";

        public static string DELETE_PRODUCT_BYGUID = "DELETE FROM Products WHERE ProductGuid='{0}'";
        
        public static string GETADDITION_SQL = @"SELECT AdditionGuid, TrackNumber, Sender, Receiver, 
            SenderTelephone, ReceiverTelephone, Departure, Destination, Comments, ProductGuid FROM ProductAdditions 
            WHERE ProductGuid='{0}';";

        public static string DELETEADDITION_SQL = @"DELETE FROM ProductAdditions WHERE ProductGuid='{0}'";
        public static string INSERTADDITION_SQL = @"INSERT INTO ProductAdditions(AdditionGuid, TrackNumber, Sender, Receiver, 
            SenderTelephone, ReceiverTelephone, Departure, Destination, ProductGuid) 
            VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}', '{7}','{8}')";



        public static string GET_PRODUCTACTIONS = @"SELECT  a.Id as Id, Concat(b.ProductBaseNumber,b.YearNumber, b.BatchNumber, b.SerialNumber) as ProductNumber, 
                b.Name as ProductName, Concat(c.ProductBaseNumber,c.YearNumber, c.BatchNumber, c.SerialNumber) as ParentNumber, 
                a.ActionTime, a.ActionComments, a.ActionType, d.Content as AdditionalInfo,
                Concat(bo.ProductBaseNumber,bo.YearNumber, bo.BatchNumber, bo.SerialNumber) as BoxNumber, bo.Name as BoxName
                FROM product_actions a 
	                Inner Join products b on a.ProductId=b.Id 
                    Left Join products c on b.ParentId=c.Id
                    Left Join products bo on b.BoxId=bo.Id
                    Left Join action_additionals d on a.AdditionalId=d.Id";

        public static string GET_PRODUCTS_WITHPARENT = @"SELECT  a.Id as Id, Concat(a.ProductBaseNumber,a.YearNumber, a.BatchNumber, a.SerialNumber) as Number, 
                a.Name as Name, Concat(b.ProductBaseNumber,b.YearNumber, b.BatchNumber, b.SerialNumber) as ParentNumber
                FROM products a 
                    Left Join products b on a.ParentId=b.Id";

        public static string GET_ACTION_OPERATORS = @"SELECT b.Encode, b.Name 
                FROM action_operators a INNER JOIN employees b ON a.Operator=b.Encode
                WHERE a.ActionId={0}";

        public ProductDBContext()
            : base("name=DefaultConnection")
        {
            System.Data.Entity.Database.SetInitializer(new MySqlProductInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Action>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        
        public List<ProductAddition> GetProductAddition(Guid productGuid)
        {
            string sqlcmd = string.Format(GETADDITION_SQL, productGuid);
            List<ProductAddition> result = this.Database.SqlQuery<ProductAddition>(sqlcmd).ToList<ProductAddition>();

            return result;
        }

        public void SaveProductAddition(ProductAddition additionInfo)
        {
             string sqlcmd = string.Format(INSERTADDITION_SQL, 
                additionInfo.AdditionGuid, additionInfo.TrackNumber, additionInfo.Sender, additionInfo.Receiver,
                additionInfo.SenderTelephone, additionInfo.ReceiverTelephone, additionInfo.Departure, additionInfo.Destination, 
                 additionInfo.ProductGuid);
            this.Database.ExecuteSqlCommand(sqlcmd);
        }

        internal void SaveProductBase(ProductBase item, bool isDelete)
        {
            try
            {
                string sqlcmd = string.Format(DELETEPRODUCTBASES_SQL, item.Number);
                this.Database.ExecuteSqlCommand(sqlcmd);

                if (!isDelete)
                {
                    sqlcmd = string.Format(INSERTPRODUCTBASES_SQL, item.Number, item.Name, item.ParentNumber);
                    this.Database.ExecuteSqlCommand(sqlcmd);
                }
            }
            catch (Exception err)
            {

            }
            
        }



        
        private void ParseChildren(string parentNumber, Guid parentGuid, ref  List<KeyValuePair<Guid, Guid>> result,
            ref List<KeyValuePair<string, Guid>> baseIds)
        {
            string sql = string.Format(GETNUMBER_BYPARENT_SQL, parentNumber);
            List<string> childBaseIds = this.Database.SqlQuery<string>(sql).ToList<string>();

            if (childBaseIds == null || childBaseIds.Count == 0)
            {
                return;
            }

            for (int i = baseIds.Count - 1; i >= 0; i--)
            {
                KeyValuePair<string, Guid> item = baseIds[i];
                bool exist = false;


                foreach (string childId in childBaseIds)
                {
                    if (item.Key.Equals(childId, StringComparison.CurrentCultureIgnoreCase))
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    result.Add(new KeyValuePair<Guid, Guid>(item.Value, parentGuid));
                    baseIds.Remove(item);
                }

                ParseChildren(item.Key, item.Value, ref result, ref baseIds);
            }

        }

       

        internal void SaveActionRecord(ActionRecord item)
        {
            string sql = string.Format("CALL proc_insert_productaction ('{0}', '{1}' ,'{2}', {3}, '{4}','{5}','{6}','{7}', '{8}','{9}')",
                  item.ProductNumber, 
                  item.ParentNumber, 
                  item.ProductName, 
                  Convert.ToInt32(item.ActionType), 
                  item.ActionTime,
                  string.IsNullOrEmpty(item.ActionComments)? "":item.ActionComments,
                  string.IsNullOrEmpty(item.BoxNumber) ? "" : item.BoxNumber,
                  string.IsNullOrEmpty(item.BoxName) ? "" : item.BoxName,
                  string.IsNullOrEmpty(item.AdditionalInfo) ? "" : item.AdditionalInfo, 
                  item.OperatorsEncodeText);
            
            this.Database.ExecuteSqlCommand(sql);
        }
    }
}