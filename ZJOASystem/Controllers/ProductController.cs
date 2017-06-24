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

        #region Index page
        public JsonResult GetProducts()
        {
            string sqlQuery = string.Format(ProductDBContext.GETALLPRODUCTLIST_SQL, Convert.ToInt32(ProductStatus.Disabled));
            List<Product> result = this.db.Database.SqlQuery<Product>(sqlQuery).ToList<Product>();

            List<InnerProduct> resultList = new List<InnerProduct>();

            foreach (Product item in result)
            {
                sqlQuery = string.Format(ProductDBContext.GETPRODUCT_PARENT_SQL, item.ProductGuid);
                List<Guid> guidresult = this.db.Database.SqlQuery<Guid>(sqlQuery).ToList<Guid>();

                Guid parentGuid = (guidresult != null && guidresult.Count > 0) ? guidresult[0] : Guid.Empty;

                InnerProduct innerObj = new InnerProduct(
                    string.Format("{0}{1}{2}{3}", item.ProductBaseNumber, item.YearNumber, item.BatchNumber, item.SerialNumber),
                    item.Name, item.Description, item.Status, item.ProductGuid, parentGuid);
                resultList.Add(innerObj);
            }

            var productList = (from item in resultList
                               select new
                               {
                                   item.Number,
                                   item.Name,
                                   item.Description,
                                   item.StatusText,
                                   item.ProductGuid,
                                   item.ParentGuid
                               });
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
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

        [HttpPost]
        public ActionResult Index(string searchkey)
        {
            if (Request.IsAuthenticated)
            {
                string createValue = string.Empty;
                try
                {
                    createValue = Request.Form.GetValues("product_create")[0];
                }
                catch
                {
                    createValue = string.Empty;
                }

                if (!string.IsNullOrEmpty(createValue))
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    InnerProductActions innerObj = Serializer.Deserialize<InnerProductActions>(createValue);

                    Product newItem = new Product();
                    newItem.ProductGuid = Guid.NewGuid();
                    newItem.Name = innerObj.Name;
                    newItem.Description = innerObj.Description;
                    newItem.ProductBaseNumber = innerObj.Number.Substring(0, 2);
                    newItem.YearNumber = innerObj.Number.Substring(2, 8);
                    newItem.BatchNumber = innerObj.Number.Substring(10, 3);
                    newItem.SerialNumber = innerObj.Number.Substring(13, 3);

                    Models.Action action = new Models.Action();
                    action.ActionType = ActionType.Create;
                    action.ActionTime = DateTime.Now;
                    action.Operators = innerObj.GetEmployeeEncodes();
                    newItem.Actions.Add(action);

                    this.db.SaveProduct(newItem);
                }

                string setupValue = "";
                try
                {
                    setupValue = Request.Form.GetValues("product_setup")[0];
                }
                catch
                {
                    setupValue = string.Empty;
                }

                if (!string.IsNullOrEmpty(setupValue))
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    InnerComplexProduct innerObj = Serializer.Deserialize<InnerComplexProduct>(setupValue);


                    this.db.SaveProductList(innerObj.ProductGuid, innerObj.GetChildrenGuidList(), innerObj.GetOperators(), ProductStatus.Setup);
                }

                SaveProjectStatus("product_test");

                SaveProjectStatus("product_fix");

                SaveProjectStatus("product_package");

                string delieverValue = "";
                try
                {
                    delieverValue = Request.Form.GetValues("product_deliever")[0];
                }
                catch
                {
                    delieverValue = string.Empty;
                }

                if (!string.IsNullOrEmpty(delieverValue))
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    InnerProductDelieverInfo innerObj = Serializer.Deserialize<InnerProductDelieverInfo>(delieverValue);

                    this.db.SaveProductStatus(innerObj.Number, innerObj.ProductGuid, innerObj.Status,
                        ActionType.Deliever, innerObj.GetOperators(), innerObj.ActionComments);

                    ProductAddition addition = new ProductAddition();
                    addition.AdditionGuid = Guid.NewGuid();
                    addition.Departure = innerObj.Departure;
                    addition.Destination = innerObj.Destination;
                    addition.ProductGuid = innerObj.ProductGuid;
                    addition.Receiver = innerObj.Receiver;
                    addition.ReceiverTelephone = innerObj.ReceiverTelephone;
                    addition.Sender = innerObj.Sender;
                    addition.SenderTelephone = innerObj.SenderTelephone;
                    addition.TrackNumber = innerObj.TrackNumber;
                    this.db.SaveProductAddition(addition);
                }
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        private void SaveProjectStatus(string requestKey)
        {
            string value = "";
            try
            {
                value = Request.Form.GetValues(requestKey)[0];
            }
            catch
            {
                value = string.Empty;
            }

            if (!string.IsNullOrEmpty(value))
            {
                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                InnerProductAction innerObj = Serializer.Deserialize<InnerProductAction>(value);

                this.db.SaveProductStatus(innerObj.Number, innerObj.ProductGuid,
                    innerObj.Status, innerObj.ActionType, innerObj.GetOperators(), innerObj.ActionComments);
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

        #region Print
        public JsonResult GetProductDetailByGuid()
        {
            string productGuid = Request.QueryString["baseguid"];
            string sql = string.Format("SELECT Id, Name, Description, ProductBaseNumber,YearNumber, BatchNumber, SerialNumber, Status,ProductGuid FROM Products WHERE ProductGuid='{0}'",
                productGuid);
            List<Product> result = this.db.Database.SqlQuery<Product>(sql).ToList<Product>();

            List<InnerProductDetails> innerResults = new List<InnerProductDetails>();
            foreach (Product product in result)
            {
                InnerProductDetails item = new InnerProductDetails();
                item.Number = product.Number;
                item.ProductGuid = product.ProductGuid;
                item.Name = product.Name;
                item.Description = product.Description;
                
                string statustext = "";
                switch(product.Status){
                    case ProductStatus.Setup:
                        statustext = ResourceReader.GetString("STATUS_SETUP");
                        break;
                    case ProductStatus.Qualified:
                        statustext = ResourceReader.GetString("STATUS_QUALIFIED");
                        break;
                    case ProductStatus.Unqualified:
                        statustext = ResourceReader.GetString("STATUS_UNQUALIFIED");
                        break;
                    case ProductStatus.Fixed:
                        statustext = ResourceReader.GetString("STATUS_FIXED");
                        break;
                    case ProductStatus.Packaged:
                        statustext = ResourceReader.GetString("STATUS_PACKAGED");
                        break;
                    case ProductStatus.Delievered:
                        statustext = ResourceReader.GetString("STATUS_DELIEVERED");
                        break;
                    case ProductStatus.Disabled:
                        statustext = ResourceReader.GetString("STATUS_DISABLED");
                        break;
                }
                item.StatusText = statustext;

                string sqlActions = string.Format(@"SELECT Id, ActionType, ActionGuid, ActionTime, Comments,ProductNumber FROM Actions 
                         WHERE ProductNumber='{0}' ORDER BY ActionTime DESC", product.Number);

                List<Models.Action> productActions = this.db.Database.SqlQuery<Models.Action>(sqlActions).ToList<Models.Action>();

                StringBuilder actionBuilder = new StringBuilder();
                if (productActions != null && productActions.Count >0)
                {
                    foreach (Models.Action actItem in productActions)
                    {
                        string operatorText = "";
                        string operatorsql = string.Format(
                            "SELECT b.Name FROM actionoperators a INNER JOIN employees b on a.EmployeeEncode=b.Encode WHERE a.ActionGuid='{0}'",actItem.ActionGuId) ;
                        List<string> operatorResult = this.db.Database.SqlQuery<string>(operatorsql).ToList<string>();
                        if (operatorResult != null && operatorResult.Count > 0)
                        {
                            operatorText = string.Join(",", operatorResult.ToArray());
                        }

                        string actionText = "";
                        switch (actItem.ActionType)
                        {
                            case ActionType.Create:
                                actionText = ResourceReader.GetString("PRODUCT_CREATE");
                                break;
                            case ActionType.Setup:
                                actionText = ResourceReader.GetString("PRODUCT_SETUP");
                                break;
                            case ActionType.Fix:
                                actionText = ResourceReader.GetString("PRODUCT_FIX");
                                break;
                            case ActionType.Test:
                                actionText = ResourceReader.GetString("PRODUCT_TEST");
                                break;
                            case ActionType.Package:
                                actionText = ResourceReader.GetString("PRODUCT_PACKAGE");
                                break;
                            case ActionType.Deliever:
                                actionText = ResourceReader.GetString("PRODUCT_DELIEVER");
                                break;
                        }
                        actionBuilder.AppendLine(string.Format("<span>{0} {1} {2} {3}</span><br/>",
                            actItem.ActionTime.ToString("yyyy-MM-dd HH:mm:ss"), actionText, operatorText, actItem.Comments)); 
                    }
                }

                item.ActionList = actionBuilder.ToString();

                innerResults.Add(item);
            }
            var jsonResults = (from item in innerResults
                             select new
                             {
                                 item.Number,
                                 item.Name,
                                 item.Description,
                                 item.StatusText,
                                 item.ProductGuid,
                                 item.ActionList
                             });
            return Json(jsonResults, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductPrintTemplate() 
        {
            List<PrintTemplate> result = new List<PrintTemplate>();

            string filepath = ConfigurationManager.AppSettings["printTemplateFolder"];

            string[] files = Directory.GetFiles(filepath, "*.xml");

            if (files != null && files.Length > 0)
            {
                for(int i=0;i<files.Length;i++){
                    string fullPath = files[i];
                    FileInfo fileinfo = new FileInfo(fullPath);
                    if (fileinfo.Exists)
                    {
                        PrintTemplate item = new PrintTemplate();
                        item.Name = fileinfo.Name;
                        item.Path = fileinfo.FullName;
                        result.Add(item);
                    }
                }
                
            }

            var templates = (from item in result
                               select new
                               {
                                   item.Name,
                                   item.Path
                               });
            return Json(templates, JsonRequestBehavior.AllowGet);
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
            String filePath = CSVHelper.SaveActionRecordCSV(result, needAdditonalInfo);
            string contentType = "application/csv";
            return File(filePath, contentType, string.Format("ProductActions_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
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
                                   item.AdditionalInfo
                               });
            return Json(productList, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Test
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
    }
}
