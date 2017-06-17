using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZJOASystem.Models
{
    public class ProductDBContext : DbContext
    {
        public static string GETUNASSIGNEDPRODUCTS_SQL = @"SELECT a.Id, a.Name, a.Description, a.Encode, a.ProductGuid, b.ParentGuid,a.Status FROM Products a 
            LEFT JOIN  productlists b on a.ProductGuid=b.ProductGuid 
            WHERE b.ParentGuid ='{0}' and a.Status<{1};";

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

        public static string GETPRODUCT_PARENT_SQL = "SELECT ParentGuid FROM productlists WHERE ProductGuid='{0}'";
        public static string INSERT_PRODUCT_SQL = @"INSERT INTO Products 
            (Name, Description, ProductBaseNumber,YearNumber, BatchNumber, SerialNumber,ProductGuid) 
            VALUES ('{0}','{1}', '{2}','{3}','{4}','{5}','{6}')";

        public static string INSERT_ACTION_SQL = @"INSERT INTO Actions (ProductNumber,ActionGuid, ActionType, ActionTime, Comments) 
            VALUES('{0}','{1}', {2},'{3}', '{4}')";

        public static string INSERT_ACTION_OPERATOR_SQL = @"INSERT INTO ActionOperators (ActionGuid, EmployeeEncode) 
            VALUES('{0}','{1}')";

        public static string GETPRODUCTLIST_SQL = @"SELECT a.Id, a.Name,a.Description, a.Encode,a.ProductGuid, b.ParentGuid, a.Status FROM Products a 
            LEFT JOIN  productlists b on a.ProductGuid=b.ProductGuid
            WHERE a.Status<{0} ";

        public static string GETPRODUCTLIST_BYID_SQL = @"SELECT a.Id, a.Name, a.Description, a.Encode, a.ProductGuid, b.ParentGuid, a.Status FROM Products a 
            LEFT JOIN  productlists b on a.ProductGuid=b.ProductGuid WHERE a.Id={0};";

        public static string WHERETEXT = "AND b.ParentGuid is not Null AND b.ParentGuid <>'00000000-0000-0000-0000-000000000000'";
        public static string GETACTIONS_STATUS_SQL = @"SELECT Id, ActionType, ActionEmployeeId, ActionTime, Comments, ProductGuid 
            FROM actions WHERE ProductGuid='{0}' AND ActionType={1} ORDER BY ActionTime DESC;";

        
        public static string GETACTIONID_SQL = "SELECT Id FROM Actions WHERE Name='{0}'";

        public static string GETACTIONS_SQL = @"SELECT Id, ActionType, ActionEmployeeEncode, ActionTime, Comments,ProductEncode FROM Actions 
            WHERE ProductEncode='{0}' ORDER BY ActionTime DESC";

        public static string GETACTIONSBYPID_SQL = @"SELECT Id, ActionType, ActionEmployeeId, ActionTime, Comments, ProductGuid FROM Actions 
            WHERE ProductGuid='{0}' ORDER BY ActionTime DESC";

        public static string GETPRODUCTCHILDREN_SQL = "SELECT ProductGuid FROM productlists WHERE ParentGuid='{0}';";

        
        

        public static string DELETE_PRODUCTLIST_SQL = @"DELETE FROM productlists WHERE ProductGuid='{0}'";
        public static string INSERT_PRODUCTLIST_SQL = @"INSERT INTO productlists (ProductGuid, ParentGuid) 
            VALUES('{0}','{1}')";
        public static string DELETE_PRODUCT_SQL = "DELETE FREOM productlists WHERE ProductGuid='{0}'";
        public static string UPDATE_PRODUCT_STATUS_SQL = "UPDATE Products SET Status={0} WHERE ProductGuid='{1}'";

        public static string UPDATE_PRODUCT_PROPERTIES = @"UPDATE Products SET Name = '{0}', Description='{1}', Encode='{2}' 
            WHERE Id={3}";

        public static string DELETE_PRODUCT_BYGUID = "DELETE FROM Products WHERE ProductGuid='{0}'";
        public static string DELETE_PRODUCTLIST_BYGUID = "DELETE FROM ProductLists WHERE ProductGuid='{0}'";
        public static string DELETE_PRODUCTACTIONS = "DELETE FROM Actions WHERE ProductGuid='{0}'";

        public static string GETADDITION_SQL = @"SELECT AdditionGuid, TrackNumber, Sender, Receiver, 
            SenderTelephone, ReceiverTelephone, Departure, Destination, Comments, ProductGuid FROM ProductAdditions 
            WHERE ProductGuid='{0}';";

        public static string DELETEADDITION_SQL = @"DELETE FROM ProductAdditions WHERE ProductGuid='{0}'";
        public static string INSERTADDITION_SQL = @"INSERT INTO ProductAdditions(AdditionGuid, TrackNumber, Sender, Receiver, 
            SenderTelephone, ReceiverTelephone, Departure, Destination, Comments, ProductGuid) 
            VALUES ('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}', '{7}','{8}','{9}')";
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

        private List<Product> _innerList = null;
        public List<ZJOASystem.Models.Product> Products
        {
            get
            {
                if (_innerList == null)
                {
                    string sqlQuery = string.Format( GETALLPRODUCTLIST_SQL, Convert.ToInt32(ProductStatus.Disabled));
                    _innerList = this.Database.SqlQuery<Product>(sqlQuery).ToList<Product>();
                }
                return _innerList;
            }
        }

        public int GetActionId(string actionname)
        {
            string selectcmd = string.Format(GETACTIONID_SQL,actionname);
            List<int> ids = this.Database.SqlQuery<int>(selectcmd).ToList<int>();

            if (ids != null && ids.Count > 0)
            {
                return ids[0];
            }
            else
            {
                return 0;
            }
        }

       /*
        public void SaveProduct(Product product, Models.Action action)
        {

            if (action.ActionType == ActionType.Create)
            {
                string insertcmd = String.Format(INSERT_PRODUCT_SQL,
                           product.Name, product.Encode, product.Description,product.ProductGuid);
                this.Database.ExecuteSqlCommand(insertcmd);

                insertcmd = String.Format(INSERT_PRODUCTLIST_SQL,
                           product.ProductGuid, Guid.Empty);
                this.Database.ExecuteSqlCommand(insertcmd);

                string insertCommand = String.Format(INSERT_ACTION_SQL,
                    product.ProductGuid, Convert.ToInt32(action.ActionType), action.ActionEmployeeId,
                    action.ActionTime, action.Comments);
                this.Database.ExecuteSqlCommand(insertCommand);
            }
            else if (action.ActionType == ActionType.Setup)
            {
                Guid parentGuid = product.ParentGuid;
                Guid productGuid = product.ProductGuid;

                string sqlcomd = String.Format(DELETE_PRODUCTLIST_SQL, productGuid);
                this.Database.ExecuteSqlCommand(sqlcomd);
                sqlcomd = String.Format(INSERT_PRODUCTLIST_SQL, productGuid, parentGuid);
                this.Database.ExecuteSqlCommand(sqlcomd);

                sqlcomd = String.Format(INSERT_ACTION_SQL,
                    productGuid, Convert.ToInt32(action.ActionType), action.ActionEmployeeId, action.ActionTime, action.Comments);
                this.Database.ExecuteSqlCommand(sqlcomd);
            }
            else if (action.ActionType == ActionType.Test || action.ActionType == ActionType.Fix 
                || action.ActionType == ActionType.Package || action.ActionType == ActionType.Deliever)
            {
                Guid guid = product.ProductGuid;

                string updateCommand = string.Format(UPDATE_PRODUCT_STATUS_SQL, Convert.ToInt32(product.Status), guid);
                this.Database.ExecuteSqlCommand(updateCommand);

                string insertCommand = String.Format(INSERT_ACTION_SQL,
                    guid, Convert.ToInt32(action.ActionType), action.ActionEmployeeId,
                    action.ActionTime, action.Comments);
                this.Database.ExecuteSqlCommand(insertCommand);
            }
            else if (action.ActionType == ActionType.UnSetup)
            {
                Guid productGuid = product.ProductGuid;

                string sqlCommand = String.Format(DELETE_PRODUCT_SQL, productGuid);
                this.Database.ExecuteSqlCommand(sqlCommand);
            }
            
        }
        private int SaveProducts(IEnumerable<System.Data.Entity.Infrastructure.DbEntityEntry<Product>> entities)
        {
            int result = 0;

            foreach (var entry in entities)
            {
                if (entry.State == EntityState.Added)
                {

                    string insertcmd = String.Format(
                       "INSERT INTO Products (Name, Encode, Description) VALUES ('{0}','{1}', '{2}')",
                       entry.Entity.Name, entry.Entity.Encode, entry.Entity.Description);
                    this.Database.ExecuteSqlCommand(insertcmd);

                    List<Action> actions = entry.Entity.Actions;
                    foreach (Action action in actions)
                    {
                        string insertCommand = String.Format(
                       "INSERT INTO Actions (ProductEncode, ActionType, ActionEmployeeEncode, ActionTime,Comments) VALUES('{0}',{1}, '{2}','{3}', '{4}')",
                         entry.Entity.Encode, Convert.ToInt32(action.ActionType), action.ActionEmployeeEncode,
                         action.ActionTime, action.Comments);
                        this.Database.ExecuteSqlCommand(insertCommand);
                    }

                    result += 1;
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Modified)
                {
                    using (var tran = this.Database.BeginTransaction())
                    {
                        try
                        {
                            string updatecmd = String.Format(
                               "UPDATE Products SET Name='{0}', Encode='{1}', Description='{2}' WHERE Id={3}",
                               entry.Entity.Name, entry.Entity.Encode, entry.Entity.Description, entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(updatecmd);

                            var product = this.FindProduct(entry.Entity.Id);

                            if (product != null && product.Encode != null && !product.Encode.Equals(entry.Entity.Encode, StringComparison.CurrentCultureIgnoreCase))
                            {
                                string updateActioncmd = String.Format(
                               "UPDATE Actions SET ProductEncode='{0}' WHERE ProductEncode='{1}'",
                                entry.Entity.Encode, product.Encode);
                                this.Database.ExecuteSqlCommand(updatecmd);
                            }
                            List<Action> actions = entry.Entity.Actions;
                            foreach (Action action in actions)
                            {
                                string insertCommand =
                                    String.Format("INSERT INTO Actions (ProductEncode, ActionType, ActionEmployeeEncode, ActionTime,Comments) VALUES('{0}',{1}, '{2}','{3}', '{4}')",
                                    entry.Entity.Encode, Convert.ToInt32(action.ActionType),
                                    action.ActionEmployeeEncode, action.ActionTime, action.Comments);
                                this.Database.ExecuteSqlCommand(insertCommand);
                            }


                            tran.Commit();
                            result += 1;
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }

                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    using (var tran = this.Database.BeginTransaction())
                    {
                        try
                        {
                            string deleteCommand = String.Format("DELETE FROM Actions WHERE ProductEncode={0}", entry.Entity.Encode);
                            this.Database.ExecuteSqlCommand(deleteCommand);

                            string deleteCmd = String.Format(
                               "DELETE FROM Products WHERE Id={0}", entry.Entity.Id);
                            this.Database.ExecuteSqlCommand(deleteCmd);

                            tran.Commit();
                            result += 1;
                        }
                        catch
                        {
                            tran.Rollback();
                        }
                    }
                }
            }

            return result;
        }
        */
        public Product FindProduct(int? id, bool needGetActions)
        {
            /*
            if (id.HasValue)
            {
                string sqlQuery = string.Format(GETPRODUCTLIST_BYID_SQL, id.Value);
                List<Product> result = this.Database.SqlQuery<Product>(sqlQuery).ToList<Product>();
                if (result != null && result.Count > 0)
                {
                    if (needGetActions)
                    {
                        result[0].GetActionsFromDB();
                    }
                    return result[0];
                }
                return null;
            }*/
            return null;
        }


        public Product GetProducts(Guid productGuid)
        {
            string sqlQuery = string.Format(GETPRODUCTLIST_SQL,Convert.ToInt32(ProductStatus.Disabled))+
                string.Format(" AND a.ProductGuid='{0}'",  productGuid);
            List<Product> result = this.Database.SqlQuery<Product>(sqlQuery).ToList<Product>();
            if (result != null && result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        public List<Guid> GetChildrenProducts(Guid parentGuid)
        {
            string sqlQuery = string.Format(GETPRODUCTCHILDREN_SQL, parentGuid);
            List<Guid> result = this.Database.SqlQuery<Guid>(sqlQuery).ToList<Guid>();

            return result;
        }

        internal int GetActionEmployeeId(int p, ActionType actionType)
        {
            throw new NotImplementedException();
        }

        public List<Action> GetActionList(Guid productGuid)
        {
            string sqlCmd = string.Format(GETACTIONSBYPID_SQL, productGuid);
            List<Action> actions = this.Database.SqlQuery<Action>(sqlCmd).ToList<Action>();
            return actions;
        }

        public void UpdateProduct(Product product)
        {
            /*
            string sqlcmd = string.Format(UPDATE_PRODUCT_PROPERTIES, product.Name, product.Description, product.Encode, product.Id);
            this.Database.ExecuteSqlCommand(sqlcmd);*/
        }

        public void DeleteProduct(Guid productGuid,ProductStatus status, bool realDelete)
        {
            if (realDelete)
            {
                string sqlcmd = string.Format(DELETE_PRODUCT_BYGUID, productGuid);
                this.Database.ExecuteSqlCommand(sqlcmd);

                sqlcmd = string.Format(DELETE_PRODUCTLIST_BYGUID, productGuid);
                this.Database.ExecuteSqlCommand(sqlcmd);

                sqlcmd = string.Format(DELETE_PRODUCTACTIONS, productGuid);
                this.Database.ExecuteSqlCommand(sqlcmd);
            }
            else
            {
                ProductStatus newStatus = ProductStatus.Disabled;
                string updateCmd = string.Format(UPDATE_PRODUCT_STATUS_SQL,Convert.ToInt32( newStatus), productGuid);
                this.Database.ExecuteSqlCommand(updateCmd);
            }
        }

        public List<ProductAddition> GetProductAddition(Guid productGuid)
        {
            string sqlcmd = string.Format(GETADDITION_SQL, productGuid);
            List<ProductAddition> result = this.Database.SqlQuery<ProductAddition>(sqlcmd).ToList<ProductAddition>();

            return result;
        }

        public void SaveProductAddition(ProductAddition additionInfo)
        {
            string sqlcmd = string.Format(DELETE_PRODUCTACTIONS, additionInfo.ProductGuid);
            this.Database.ExecuteSqlCommand(sqlcmd);

            sqlcmd = string.Format(INSERTADDITION_SQL, 
                Guid.NewGuid(), additionInfo.TrackNumber, additionInfo.Sender, additionInfo.Receiver,
                additionInfo.SenderTelephone, additionInfo.ReceiverTelephone, additionInfo.Departure, additionInfo.Destination, 
                additionInfo.Comments, additionInfo.ProductGuid);
            this.Database.ExecuteSqlCommand(sqlcmd);
        }

        internal void SaveProduct(Product item)
        {
            try
            {
                string sqlcmd = string.Format(INSERT_PRODUCT_SQL,
                    item.Name, item.Description, item.ProductBaseNumber,
                    item.YearNumber, item.BatchNumber, item.SerialNumber, item.ProductGuid);
                this.Database.ExecuteSqlCommand(sqlcmd);

                string productNumber = string.Format("{0}{1}{2}{3}",item.ProductBaseNumber,item.YearNumber,item.BatchNumber,item.SerialNumber);

                foreach (Action action in item.Actions)
                {
                    Guid actionGuid = Guid.NewGuid();
                    sqlcmd = string.Format(INSERT_ACTION_SQL,
                        productNumber, actionGuid, Convert.ToInt32(action.ActionType), action.ActionTime, action.Comments);
                    this.Database.ExecuteSqlCommand(sqlcmd);

                    foreach (string opt in action.Operators)
                    {
                        string optValue = opt.Trim();

                        if (!string.IsNullOrEmpty(optValue))
                        {
                            sqlcmd = string.Format(INSERT_ACTION_OPERATOR_SQL,
                                actionGuid, optValue);
                            this.Database.ExecuteSqlCommand(sqlcmd);
                        }
                    }
                }
            }
            catch (Exception err)
            {

            }
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



        public void SaveProductList(Guid topGuid, List<Guid> childList, List<string> operators, ProductStatus status)
        {
            List<KeyValuePair<Guid, Guid>> allResult = new List<KeyValuePair<Guid, Guid>>();
            string sql = string.Format("SELECT ProductBaseNumber FROM products WHERE ProductGuid='{0}'", topGuid);
            List<string> result = this.Database.SqlQuery<string>(sql).ToList<string>();
            string topbaseId = null;
            if (result != null && result.Count > 0)
            {
                topbaseId = result[0];
            }

            List<KeyValuePair<string, Guid>> baseIds = new List<KeyValuePair<string,Guid>> ();
            foreach (Guid item in childList)
            {
                sql = string.Format("SELECT ProductBaseNumber FROM products WHERE ProductGuid='{0}'", item);
                result = this.Database.SqlQuery<string>(sql).ToList<string>();

                if (result != null && result.Count > 0)
                {
                    baseIds.Add( new KeyValuePair<string,Guid>(result[0], item));
                }
            }


            ParseChildren(topbaseId, topGuid,ref allResult, ref baseIds);

            if (allResult != null && allResult.Count > 0)
            {
                foreach (KeyValuePair<Guid, Guid> item in allResult)
                {

                    sql = string.Format(INSERT_PRODUCTLIST_SQL, item.Key, item.Value);
                    this.Database.ExecuteSqlCommand(sql);

                    sql = string.Format("UPDATE products SET Status={0} WHERE ProductGuid='{1}'", Convert.ToInt32(status), item.Value);
                    this.Database.ExecuteSqlCommand(sql);

                    sql = string.Format("UPDATE products SET Status={0} WHERE ProductGuid='{1}'", Convert.ToInt32(status), item.Key);
                    this.Database.ExecuteSqlCommand(sql);
                }

                sql = string.Format("SELECT Concat(ProductBaseNumber,YearNumber,BatchNumber,SerialNumber) as Number FROM products WHERE ProductGuid='{0}'", topGuid);
                List<string> numbers = this.Database.SqlQuery<string>(sql).ToList<string>();

                Guid newActGuid = Guid.NewGuid();
                sql = string.Format(INSERT_ACTION_SQL,
                       numbers[0], newActGuid, Convert.ToInt32(ActionType.Setup), 
                       DateTime.Now, "");
                this.Database.ExecuteSqlCommand(sql);

                foreach (string opt in operators)
                {
                    string optValue = opt.Trim();

                    if (!string.IsNullOrEmpty(optValue))
                    {
                        sql = string.Format(INSERT_ACTION_OPERATOR_SQL,
                            newActGuid, optValue);
                        this.Database.ExecuteSqlCommand(sql);
                    }
                }
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
            foreach (KeyValuePair<string, Guid> item in baseIds)
            {
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
                }

                ParseChildren(item.Key, item.Value, ref result, ref baseIds);
            }


        }

        public void SaveProductStatus(string number, Guid guid, ProductStatus productStatus, ActionType actionType, List<string> operators, string actionComments)
        {
            UpdateProductStatus(guid, productStatus);

            Guid actionGuid = Guid.NewGuid();
            string sql = string.Format(INSERT_ACTION_SQL, number, actionGuid,
                Convert.ToInt32(actionType), DateTime.Now, actionComments);
            this.Database.ExecuteSqlCommand(sql);

            if(operators != null && operators.Count > 0){
                foreach(string oper in operators){
                   string operValue = oper.Trim();

                   if (!string.IsNullOrEmpty(operValue))
                   {
                       sql = string.Format(INSERT_ACTION_OPERATOR_SQL, actionGuid, operValue);
                       this.Database.ExecuteSqlCommand(sql);
                   }
                }
            }
        }

        private void UpdateProductStatus(Guid productGuid, ProductStatus status)
        {
            string sql = string.Format("UPDATE products SET Status={0} WHERE ProductGuid='{1}'", Convert.ToInt32(status), productGuid);
            this.Database.ExecuteSqlCommand(sql);

            sql =string.Format( "SELECT ProductGuid FROM productlists WHERE ParentGuid='{0}'", productGuid);
            List<string> children = this.Database.SqlQuery<string>(sql).ToList<string>();
            if (children != null && children.Count > 0)
            {
                foreach (string childGuid in children)
                {
                    UpdateProductStatus(Guid.Parse(childGuid), status);
                }
            }
        }
    }
}