using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ZJOASystem.Models;

namespace ZJOASystem.Controllers
{
    public class ProductController : Controller
    {
        private ProductDBContext db = new ProductDBContext();

        #region ProductList page
        /// <summary>
        /// Get : Product/ProductList
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductList()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult ProductList(string attribute)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    string attributeValue = "";
                    bool isDelete = false;
                    string operation = "";
                    try
                    {
                        attributeValue = Request.Form.GetValues("jsondata")[0];
                    }
                    catch
                    {
                        attributeValue = "";
                    }
                    try
                    {
                        operation = Request.Form.GetValues("operation")[0];
                        isDelete = (string.Equals(operation, "Delete", StringComparison.CurrentCultureIgnoreCase));
                    }
                    catch
                    {
                        isDelete = false;
                    }

                    try
                    {
                        JavaScriptSerializer Serializer = new JavaScriptSerializer();
                        InnerProductBase innerObj = Serializer.Deserialize<InnerProductBase>(attributeValue);

                        ProductBase newItem = new ProductBase(innerObj.Number, innerObj.Name, innerObj.ParentNumber);

                        this.db.SaveProductBase(newItem, isDelete);
                    }
                    catch
                    {

                    }
                }
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }
        /// <summary>
        /// Get: Product/GetProductBases
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProductBases()
        {
            string sqlQuery = string.Format(ProductDBContext.GETALLPRODUCTBASES_SQL);
            List<ProductBase> result = this.db.Database.SqlQuery<ProductBase>(sqlQuery).ToList<ProductBase>();
            foreach (ProductBase item in result)
            {
                if (item.Number.Equals(item.ParentNumber, StringComparison.CurrentCultureIgnoreCase))
                {
                    item.ParentNumber = "0";
                }
            }

            var productList = (from item in result
                               select new
                               {
                                   item.Number,
                                   item.Name,
                                   item.ParentNumber
                               });
            return Json(productList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Import()
        {
            if (Request.IsAuthenticated)
            {
                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    using (var inputStream = file.InputStream)
                    {
                        StreamReader reader = new StreamReader(inputStream);
                        string allText = reader.ReadToEnd();
                        reader.Close();

                        List<ProductBase> items = CSVHelper.OpenCSV(allText);
                        foreach (ProductBase item in items)
                        {
                            db.SaveProductBase(item, false);
                        }
                    }
                }
                return RedirectToAction("../Product/ProductList");
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }
        #endregion

       
        #region Create page
        public JsonResult GetEmployeeList()
        {
            EmployeeDBContext employeeDb = new EmployeeDBContext();
            List<InnerEmployee> innerList = employeeDb.GetInnerEmployees(null);

            var list = (from item in innerList
                        select new
                        {
                            item.Encode,
                            item.Name,
                        });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Setup
        public JsonResult GetTopProducts()
        {
            List<string> productStatus = null;
            try
            {
                string productStatusText = Request.QueryString["status"];
                if (!string.IsNullOrEmpty(productStatusText))
                {
                    productStatus = new List<string>(productStatusText.Split(','));
                }
            }
            catch
            {
                productStatus = null;
            }


            string sqlQuery = string.Format(ProductDBContext.GET_TOP_PRODUCTLIST_SQL, Convert.ToInt32(ProductStatus.Disabled));
            if (productStatus != null && productStatus.Count > 0)
            {
                StringBuilder statusSqlBuilder = new StringBuilder();
                foreach (string status in productStatus)
                {
                    if (statusSqlBuilder.Length > 0)
                    {
                        statusSqlBuilder.Append(" OR ");
                    }
                    statusSqlBuilder.Append("a.Status=" + status);
                }
                if (statusSqlBuilder.Length > 0)
                {
                    sqlQuery += string.Format("AND ({0}) ", statusSqlBuilder.ToString());
                }
            }

            List<Product> result = this.db.Database.SqlQuery<Product>(sqlQuery).ToList<Product>();

            List<InnerProduct> resultList = new List<InnerProduct>();

            foreach (Product item in result)
            {
                InnerProduct innerObj = new InnerProduct(
                    string.Format("{0}{1}{2}{3}", item.ProductBaseNumber, item.YearNumber, item.BatchNumber, item.SerialNumber),
                    item.Name, item.Description, item.Status, item.ProductGuid, Guid.Empty);
                resultList.Add(innerObj);
            }

            var productList = (from item in resultList
                               select new
                               {
                                   item.Number,
                                   item.Name,
                                   item.Description,
                                   item.StatusText,
                                   item.ProductGuid
                               });
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductsByBaseNumber()
        {
            string productBaseNumber = Request.QueryString["baseid"];
            if (!string.IsNullOrEmpty(productBaseNumber))
            {
                productBaseNumber = productBaseNumber.Substring(0, 2);
            }
            string productGuid = Request.QueryString["baseguid"];

            if (string.IsNullOrEmpty(productGuid))
            {
                return GetTopProducts();
            }
            List<InnerProduct> resultList = new List<InnerProduct>();

            GetChildProductsByNumber(productBaseNumber, Guid.Parse(productGuid), ref resultList);

            var productList = (from item in resultList
                               select new
                               {
                                   item.Number,
                                   item.Name,
                                   item.Description,
                                   item.StatusText,
                                   item.ProductGuid
                               });
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        private void GetChildProductsByNumber(string baseId, Guid productGuid, ref List<InnerProduct> resultList)
        {
            string sqlQuery = string.Format(ProductDBContext.GET_PRODUCTBASE_TOP_SQL, baseId);
            List<string> result = this.db.Database.SqlQuery<string>(sqlQuery).ToList<string>();

            foreach (string item in result)
            {
                sqlQuery = string.Format(ProductDBContext.GET_PRODUCTLIST_BYNUMBER_SQL,
                    Convert.ToInt32(ProductStatus.Disabled), item);
                List<Product> productresult = this.db.Database.SqlQuery<Product>(sqlQuery).ToList<Product>();

                foreach (Product pItem in productresult)
                {
                    InnerProduct innerObj = new InnerProduct(
                    string.Format("{0}{1}{2}{3}", pItem.ProductBaseNumber,
                    pItem.YearNumber, pItem.BatchNumber, pItem.SerialNumber),
                    pItem.Name, pItem.Description, pItem.Status, pItem.ProductGuid, productGuid);
                    resultList.Add(innerObj);

                    GetChildProductsByNumber(pItem.ProductBaseNumber, pItem.ProductGuid, ref resultList);
                }

            }
        }
        #endregion

        
        #region Setup
        [HttpPost]
        public ActionResult ImportProductAction()
        {
            if (Request.IsAuthenticated)
            {
                int actionType = Convert.ToInt32(Request.QueryString["actiontype"]);

                if (Request.Files != null && Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    using (var inputStream = file.InputStream)
                    {
                        StreamReader reader = new StreamReader(inputStream);
                        string allText = reader.ReadToEnd();
                        reader.Close();

                        List<ActionRecord> items = CSVHelper.OpenActionRecordCSV(allText);
                        List<ActionRecord> newItems = new List<ActionRecord>();
                        newItems.AddRange(items);
                        foreach (ActionRecord item in items)
                        {
                            GetChildren(ref newItems, item);
                        }

                        foreach (ActionRecord item in newItems)
                        {
                            db.SaveActionRecord(item);
                        }
                    }
                }
                if (actionType == Convert.ToInt32(ActionType.Setup))
                {
                    return RedirectToAction("../Product/Setup");
                }
                else if (actionType == Convert.ToInt32(ActionType.Test))
                {
                    return RedirectToAction("../Product/Test");
                }
                else
                {
                    return RedirectToAction("../Product/Setup");
                }

            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        private void GetChildren(ref List<ActionRecord> items, ActionRecord item)
        {
            string sqlQuery = string.Format("SELECT * FROM ( {0} ) tmp WHERE tmp.ParentNumber='{1}'",
                ProductDBContext.GET_PRODUCTS_WITHPARENT, item.ProductNumber);

            List<InnerProductBase> children = this.db.Database.SqlQuery<InnerProductBase>(sqlQuery).ToList<InnerProductBase>();
            if (children != null && children.Count > 0)
            {
                foreach (InnerProductBase child in children)
                {
                    ActionRecord childRecord = new ActionRecord();
                    childRecord.ProductNumber = child.Number;
                    childRecord.ProductName = child.Name;
                    childRecord.ParentNumber = child.ParentNumber;
                    childRecord.Operators = item.Operators;
                    childRecord.ActionComments = item.ActionComments;
                    childRecord.ActionTime = item.ActionTime;
                    childRecord.ActionType = item.ActionType;
                    childRecord.AdditionalInfo = item.AdditionalInfo;
                    childRecord.BoxNumber = item.BoxNumber;
                    childRecord.BoxName = item.BoxName;

                    if (!items.Contains(childRecord))
                    {
                        items.Add(childRecord);

                        GetChildren(ref items, childRecord);
                    }
                }
            }
        }

        public FileResult ExportProductAction()
        {
            int actionType = Convert.ToInt32(Request.QueryString["actiontype"]); 
            
            string sqlQuery = ProductDBContext.GET_PRODUCTACTIONS + string.Format("  WHERE a.ActionType={0};", actionType);

            List<ActionRecord> result = this.db.Database.SqlQuery<ActionRecord>(sqlQuery).ToList<ActionRecord>();

            foreach (ActionRecord item in result)
            {
                string selectOpt = "SELECT Operator From action_operators WHERE ActionId=" + item.Id;
                List<string> operators = this.db.Database.SqlQuery<string>(selectOpt).ToList<string>();

                item.Operators = new List<Operator>();
                foreach (string optEncode in operators)
                {
                    Operator optObj = new Operator();
                    optObj.Encode = optEncode;
                    item.Operators.Add(optObj);
                }
            }

            bool needAdditonalInfo = (actionType == Convert.ToInt32(ActionType.Deliever));
            bool needBox = (actionType == Convert.ToInt32(ActionType.Package));
            String filePath = CSVHelper.SaveActionRecordCSV(result, needAdditonalInfo,needBox);
            string contentType = "application/csv";
            return File(filePath, contentType, string.Format("ProductActions_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        [Authorize(Roles = "Admin,组装组")]
        public ActionResult Setup()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        public JsonResult GetActionRecords()
        { 
            int actionType = Convert.ToInt32( Request.QueryString["actiontype"] ); 
            string sqlQuery = ProductDBContext.GET_PRODUCTACTIONS + string.Format("  WHERE a.ActionType={0};", actionType);

            List<ActionRecord> result = this.db.Database.SqlQuery<ActionRecord>(sqlQuery).ToList<ActionRecord>();
            foreach (ActionRecord item in result)
            {
                sqlQuery = string.Format(ProductDBContext.GET_ACTION_OPERATORS, item.Id);
                List<Operator> operators = this.db.Database.SqlQuery<Operator>(sqlQuery).ToList<Operator>();

                item.Operators = operators;
            }

            var productList = (from item in result
                               select new
                               {
                                   item.ProductNumber,
                                   item.ProductName,
                                   item.ParentNumber,
                                   item.ActionTime,
                                   item.ActionComments,
                                   item.OperatorsText,
                                   item.AdditionalInfo,
                                   item.BoxNumber
                               });
            return Json(productList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Test
        [Authorize(Roles = "Admin,检测组")]
        public ActionResult Test()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        #endregion

        #region Package
        [Authorize(Roles = "Admin,装箱组")]
        public ActionResult Package()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        #endregion

        #region Deliever
        [Authorize(Roles = "Admin,发货组")]
        public ActionResult Deliever()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        #endregion

        #region Print
        public JsonResult GetPrintTemplate()
        {
            List<PrintTemplate> result = new List<PrintTemplate>(); 

            string filepath = ConfigurationManager.AppSettings["printTemplateFolder"];

            string[] files = Directory.GetFiles(filepath, "*.xsl");

            if (files != null && files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    string fullPath = files[i];
                    FileInfo fileinfo = new FileInfo(fullPath);
                    if (fileinfo.Exists)
                    {
                        PrintTemplate item = new PrintTemplate();
                        item.Name = fileinfo.Name.Substring(0, fileinfo.Name.Length-4);
                        item.Path = fileinfo.FullName;

                        string content = System.IO.File.ReadAllText(fileinfo.FullName, Encoding.UTF8);

                        item.Content = content;

                        result.Add(item);
                    }
                }

            }

            var templates = (from item in result
                             select new
                             {
                                 item.Name,
                                 item.Content
                             });
            return Json(templates, JsonRequestBehavior.AllowGet); 
        }
        #endregion
    }
}
