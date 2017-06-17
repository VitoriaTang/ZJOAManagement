using System;
using System.Collections.Generic;
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
                    string operation="";
                    try
                    {
                        attributeValue = Request.Form.GetValues("jsondata")[0];
                    }
                    catch{
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

                    try{
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
                catch{
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

                string testValue = "";
                try
                {
                    testValue = Request.Form.GetValues("product_test")[0];
                }
                catch
                {
                    testValue = string.Empty;
                }

                if (!string.IsNullOrEmpty(testValue))
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    InnerProductAction innerObj = Serializer.Deserialize<InnerProductAction>(testValue);


                    this.db.SaveProductStatus(innerObj.Number,innerObj.ProductGuid, 
                        innerObj.Status, innerObj.ActionType, innerObj.GetOperators(), innerObj.ActionComments);
                }

                string fixValue = "";
                try
                {
                    fixValue = Request.Form.GetValues("product_fix")[0];
                }
                catch
                {
                    fixValue = string.Empty;
                }

                if (!string.IsNullOrEmpty(fixValue))
                {
                    JavaScriptSerializer Serializer = new JavaScriptSerializer();
                    InnerProductAction innerObj = Serializer.Deserialize<InnerProductAction>(fixValue);

                    this.db.SaveProductStatus(innerObj.Number, innerObj.ProductGuid,
                        innerObj.Status, innerObj.ActionType, innerObj.GetOperators(), innerObj.ActionComments);
                }
                return View();
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
               string  productStatusText = Request.QueryString["status"];
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
            string sqlQuery =string.Format( ProductDBContext.GET_PRODUCTBASE_TOP_SQL, baseId);
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

                    GetChildProductsByNumber(pItem.ProductBaseNumber,pItem.ProductGuid, ref resultList);
                }
                
            }
        }
        #endregion


        #region Others, TODO, Need modify

        // GET: /Product/
        /*
        
        */
        // GET: /Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.FindProduct(id, true);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

       
        // GET: /Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.FindProduct(id, false);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: /Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Name,Description,Encode")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: /Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.FindProduct(id, true);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Product product = db.FindProduct(id, false);
            //db.Products.Remove(product);
            //db.DeleteProduct(product.ProductGuid, product.Status, false);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: /Product/Create
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

        public JsonResult GetTestProducts()
        {
            //string sqlQuery = string.Format(ProductDBContext.GETPRODUCTLIST_SQL, Convert.ToInt32(ProductStatus.Disabled)) + " " + ProductDBContext.WHERETEXT;
            //List<ProductAction> result = this.db.Database.SqlQuery<ProductAction>(sqlQuery).ToList<ProductAction>();
            //List<ProductAction> products = new List<ProductAction>();
            //foreach (ProductAction item in result)
            //{
            //    Guid guid = item.ProductGuid;
                
            //    if (item.ParentGuid.Equals(guid))
            //    {
            //        item.ParentGuid = Guid.Empty;
            //    }
            //    string actionSqlQuery = string.Format(ProductDBContext.GETACTIONS_STATUS_SQL, item.ProductGuid, Convert.ToInt32(ActionType.Test));
            //    List<Models.Action> actionDS = this.db.Database.SqlQuery<Models.Action>(actionSqlQuery).ToList<Models.Action>();

            //    if (actionDS != null && actionDS.Count > 0)
            //    {
            //        item.ActionTime = actionDS[0].ActionTime;
            //        item.ActionType = ActionType.Test;
            //        EmployeeDBContext employeeDb = new EmployeeDBContext();
            //        Employee employee = employeeDb.Employees.Find(actionDS[0].ActionEmployeeId);
            //        if (employee != null )
            //        {
            //            item.ActionEmployee = string.Join("_",employee.Name,employee.Encode);

            //        }
            //    }

            //    products.Add(item);
            //}
            //var productList = (from item in products
            //                   select new
            //                   {
            //                       item.ProductGuid,
            //                       item.NameEncode,
            //                       item.Encode,
            //                       item.ParentGuid,
            //                       item.ActionEmployee,
            //                       item.ActionTime,
            //                       item.Status
            //                   });
            //return Json(productList, JsonRequestBehavior.AllowGet);
            return null; 
        }

      

        
        /*
        public JsonResult GetUnassignProducts()
        {
            string sqlQuery = string.Format(ProductDBContext.GETUNASSIGNEDPRODUCTS_SQL, Guid.Empty, Convert.ToInt32(ProductStatus.Disabled));
            List<Product> result = this.db.Database.SqlQuery<Product>(sqlQuery).ToList<Product>();
            var products = (from product in result
                            select new
                            {
                                product.Name,
                                product.Encode,
                                product.ProductGuid
                            });
            
            return Json(products, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActionEmployees()
        {
            EmployeeDBContext employeeDb = new EmployeeDBContext();
            List<InnerEmployee> employees = employeeDb.GetInnerEmployees(null);

            var result = (from item in employees
                            select new
                            {
                                item.Name,
                                item.Encode,
                                item.NameEncode
                            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private string GetProductJson(Guid productGuid, bool isTopLevel)
        {
            StringBuilder builder = new StringBuilder();
            Product product = this.db.GetProducts(productGuid);
            builder.Append("{");

            List<Guid> children = this.db.GetChildrenProducts(productGuid);

            if (!isTopLevel || (children!= null &&children.Count >0))
            {
                if (product != null)
                {
                    builder.Append(product.ToJSONString());
                }
            }
            if (children != null && children.Count > 0)
            {
                builder.Append("children:[");

                for (int i = 0; i < children.Count; i++)
                {
                    string childJson = GetProductJson(children[i], false);

                    if (!string.IsNullOrEmpty(childJson))
                    {
                        builder.Append(childJson);
                    }
                    if (i < children.Count - 1)
                    {
                        builder.Append(",");
                    }
                }
               
                builder.Append("]");
            }
            builder.Append("}");

            return builder.ToString();
        }
        */
        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Setup(string attribute)
        {
            if (Request.IsAuthenticated)
            {
                /*
              if (ModelState.IsValid)
              {
                  
                  Product newProduct = null;
                  ZJOASystem.Models.Action action = new Models.Action();
                  int employeeId = 0;
                  foreach (var key in Request.Form.Keys)
                  {
                      if (key is String)
                      {
                          string keyText = Convert.ToString(key);
                          if (keyText.Equals("jsondata", StringComparison.CurrentCultureIgnoreCase))
                          {
                              string value = Request.Form.GetValues(keyText)[0];
                              JavaScriptSerializer Serializer = new JavaScriptSerializer();
                              InnerProduct innerObj = Serializer.Deserialize<InnerProduct>(value);

                              newProduct = db.GetProducts(innerObj.ProductGuid);
                              if (newProduct != null)
                              {
                                  newProduct.ParentGuid = innerObj.ParentGuid;
                              }
                          }
                          else if (keyText.Equals("operation"))
                          {
                              string operation = Request.Form.GetValues(keyText)[0];
                              if (operation.Equals("Setup", StringComparison.CurrentCultureIgnoreCase))
                              {
                                  action.ActionType = ActionType.Setup;
                              }
                              else if (operation.Equals("UnSetup", StringComparison.CurrentCultureIgnoreCase))
                              {
                                  action.ActionType = ActionType.UnSetup;
                              }
                                
                          }
                          else if (keyText.StartsWith("operator"))
                          {
                              string employeeEncode = Convert.ToString(Request.Form.GetValues(keyText)[0]).Split('_')[1];

                              EmployeeDBContext employeeDb = new EmployeeDBContext();
                              employeeId = employeeDb.GetEmployeeId(employeeEncode);
                          }
                      }

                  }

                  if (newProduct != null)
                  {

                      action.ActionEmployeeId = employeeId;
                      action.ActionTime = DateTime.Now;
                        
                      db.SaveProduct(newProduct, action);
                  }
                   
              }
*/
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }


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

                        List<ProductBase> items = CSVHelper.OpenCSV(allText, false);
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

        // GET: /Product/Create
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

        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Test(string attribute)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    UpdateProjectStatus( ActionType.Test);
                }
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }


        }


        private void UpdateProjectStatus( ActionType actionType)
        {
            /*
            Product newProduct = null;
            ZJOASystem.Models.Action action = new Models.Action();
            string employeeEncode = "";
            foreach (var key in Request.Form.Keys)
            {
                if (key is String)
                {
                    string keyText = Convert.ToString(key);
                    if (keyText.Equals("jsondata", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string value = Request.Form.GetValues(keyText)[0];
                        JavaScriptSerializer Serializer = new JavaScriptSerializer();
                        InnerTestProduct innerObj = Serializer.Deserialize<InnerTestProduct>(value);

                        newProduct = db.GetProducts(innerObj.ProductGuid);


                        if (newProduct != null)
                        {
                            employeeEncode = innerObj.ActionEmployee.Split('_')[1];
                            EmployeeDBContext employeeDb = new EmployeeDBContext();

                            action.ActionEmployeeId = employeeDb.GetEmployeeId(employeeEncode);
                            action.ActionTime = DateTime.Now;
                            action.ActionType = actionType;
                            action.Comments = innerObj.ActionComments;
                            newProduct.Status = innerObj.Status;
                            db.SaveProduct(newProduct, action);
                        }


                        break;
                    }

                }

            }*/
        }

        // GET: /Product/Create
        public ActionResult Fix()
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
        public ActionResult Fix(string attribute)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    UpdateProjectStatus(ActionType.Fix);
                }
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }


        public JsonResult GetNeedFixProducts()
        {
            //string sqlQuery = string.Format(ProductDBContext.GETPRODUCTLIST_SQL, Convert.ToInt32(ProductStatus.Disabled)) 
            //    + " " + ProductDBContext.WHERETEXT + string.Format(" and (Status={0} or Status={1})", Convert.ToInt32(ProductStatus.Unqualified),Convert.ToInt32(ProductStatus.Fixed));
            //List<ProductAction> result = this.db.Database.SqlQuery<ProductAction>(sqlQuery).ToList<ProductAction>();
            //List<ProductAction> products = new List<ProductAction>();
            //foreach (ProductAction item in result)
            //{
            //    Guid guid = item.ProductGuid;

            //    if (item.ParentGuid.Equals(guid))
            //    {
            //        item.ParentGuid = Guid.Empty;
            //    }
            //    string actionSqlQuery = string.Format(ProductDBContext.GETACTIONS_STATUS_SQL, item.ProductGuid, Convert.ToInt32(ActionType.Fix));
            //    List<Models.Action> actionDS = this.db.Database.SqlQuery<Models.Action>(actionSqlQuery).ToList<Models.Action>();

            //    if (actionDS != null && actionDS.Count > 0)
            //    {
            //        item.ActionTime = actionDS[0].ActionTime;
            //        item.ActionType = ActionType.Fix;
            //        EmployeeDBContext employeeDb = new EmployeeDBContext();
            //        Employee employee = employeeDb.Employees.Find(actionDS[0].ActionEmployeeId);
            //        if (employee != null)
            //        {
            //            item.ActionEmployee = string.Join("_", employee.Name, employee.Encode);

            //        }
            //    }

            //    products.Add(item);
            //}
            //var productList = (from item in products
            //                   select new
            //                   {
            //                       item.ProductGuid,
            //                       item.NameEncode,
            //                       item.Encode,
            //                       item.ParentGuid,
            //                       item.ActionEmployee,
            //                       item.ActionTime,
            //                       item.Status
            //                   });
            //return Json(productList, JsonRequestBehavior.AllowGet);
            return null;
        }

        // GET: /Product/Create
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

        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Package(string attribute)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    UpdateProjectStatus(ActionType.Package);
                }
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        public JsonResult GetNeedPackageProducts()
        {
            //string sqlQuery = string.Format(ProductDBContext.GETPRODUCTLIST_SQL, Convert.ToInt32(ProductStatus.Disabled))
            //    + " " + ProductDBContext.WHERETEXT + string.Format(" and (Status={0} or Status={1})", Convert.ToInt32(ProductStatus.Qualified), Convert.ToInt32(ProductStatus.Packaged));
            //List<ProductAction> result = this.db.Database.SqlQuery<ProductAction>(sqlQuery).ToList<ProductAction>();
            //List<ProductAction> products = new List<ProductAction>();
            //foreach (ProductAction item in result)
            //{
            //    Guid guid = item.ProductGuid;

            //    if (item.ParentGuid.Equals(guid))
            //    {
            //        item.ParentGuid = Guid.Empty;
            //    }
            //    string actionSqlQuery = string.Format(ProductDBContext.GETACTIONS_STATUS_SQL, item.ProductGuid, Convert.ToInt32(ActionType.Package));
            //    List<Models.Action> actionDS = this.db.Database.SqlQuery<Models.Action>(actionSqlQuery).ToList<Models.Action>();

            //    if (actionDS != null && actionDS.Count > 0)
            //    {
            //        item.ActionTime = actionDS[0].ActionTime;
            //        item.ActionType = ActionType.Package;
            //        EmployeeDBContext employeeDb = new EmployeeDBContext();
            //        Employee employee = employeeDb.Employees.Find(actionDS[0].ActionEmployeeId);
            //        if (employee != null)
            //        {
            //            item.ActionEmployee = string.Join("_", employee.Name, employee.Encode);

            //        }
            //        item.Comments = actionDS[0].Comments;
            //    }

            //    products.Add(item);
            //}
            //var productList = (from item in products
            //                   select new
            //                   {
            //                       item.ProductGuid,
            //                       item.NameEncode,
            //                       item.Encode,
            //                       item.ParentGuid,
            //                       item.ActionEmployee,
            //                       item.ActionTime,
            //                       item.Status,
            //                       item.Comments
            //                   });
            //return Json(productList, JsonRequestBehavior.AllowGet);
            return null;
        }


        // GET: /Product/Create
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

        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Deliever(string attribute)
        {
            if (Request.IsAuthenticated)
            {
                /*
                if (ModelState.IsValid)
                {
                    Product newProduct = null;
                    ZJOASystem.Models.Action action = new Models.Action();
                    string employeeEncode = "";
                    foreach (var key in Request.Form.Keys)
                    {
                        if (key is String)
                        {
                            string keyText = Convert.ToString(key);
                            if (keyText.Equals("jsondata", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string value = Request.Form.GetValues(keyText)[0];
                                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                                InnerProductDelieverInfo innerObj = Serializer.Deserialize<InnerProductDelieverInfo>(value);

                                newProduct = db.GetProducts(innerObj.ProductGuid);


                                if (newProduct != null)
                                {
                                    employeeEncode = innerObj.ActionEmployee.Split('_')[1];
                                    EmployeeDBContext employeeDb = new EmployeeDBContext();

                                    action.ActionEmployeeId = employeeDb.GetEmployeeId(employeeEncode);
                                    action.ActionTime = DateTime.Now;
                                    action.ActionType = ActionType.Deliever;
                                    action.Comments = innerObj.ActionComments;
                                    newProduct.Status = innerObj.Status;
                                    db.SaveProduct(newProduct, action);

                                    ProductAddition additionInfo = new ProductAddition();
                                    additionInfo.ProductGuid = innerObj.ProductGuid;
                                    additionInfo.Receiver = innerObj.Receiver;
                                    additionInfo.ReceiverTelephone = innerObj.ReceiverTelephone;
                                    additionInfo.Sender = innerObj.Sender;
                                    additionInfo.SenderTelephone = innerObj.SenderTelephone;
                                    additionInfo.TrackNumber = innerObj.TrackNumber;
                                    additionInfo.Departure = innerObj.Departure;
                                    additionInfo.Destination = innerObj.Destination;
                                    db.SaveProductAddition(additionInfo);
                                }


                                break;
                            }

                        }

                    } 
                }*/
                return View();
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        public JsonResult GetNeedDelieverProducts()
        {
            //string sqlQuery = string.Format(ProductDBContext.GETPRODUCTLIST_SQL, Convert.ToInt32(ProductStatus.Disabled))
            //    + " " + ProductDBContext.WHERETEXT + string.Format(" and (Status={0} or Status={1})", Convert.ToInt32(ProductStatus.Packaged), Convert.ToInt32(ProductStatus.Delievered));
            //List<ProductAction> result = this.db.Database.SqlQuery<ProductAction>(sqlQuery).ToList<ProductAction>();
            //List<InnerProductDelieverInfo> products = new List<InnerProductDelieverInfo>();

            //EmployeeDBContext employeeDb = new EmployeeDBContext();
                    
            //foreach (ProductAction item in result)
            //{
            //    Guid guid = item.ProductGuid;

            //    InnerProductDelieverInfo pItem= new InnerProductDelieverInfo();
            //    pItem.ProductGuid = guid;
            //    if (item.ParentGuid.Equals(guid))
            //    {
            //        pItem.ParentGuid = Guid.Empty;
            //    }
            //    else{
            //        pItem.ParentGuid = item.ParentGuid;
            //    }

            //    pItem.Encode = item.Encode;
            //    pItem.Name = item.Name;
            //    pItem.Status = item.Status;

            //    List<ProductAddition> additions = this.db.GetProductAddition(guid);
            //    if (additions != null && additions.Count >0)
            //    {
            //        ProductAddition additionItem = additions[0];
            //        pItem.AdditionGuid = additionItem.AdditionGuid;
            //        pItem.Comments = additionItem.Comments;
            //        pItem.Departure = additionItem.Departure;
            //        pItem.Destination = additionItem.Destination;
            //        pItem.Receiver = additionItem.Receiver;
            //        pItem.ReceiverTelephone = additionItem.ReceiverTelephone;
            //        pItem.Sender = additionItem.Sender;
            //        pItem.SenderTelephone = additionItem.SenderTelephone;
            //        pItem.TrackNumber = additionItem.TrackNumber;
            //    }
            //    string actionSqlQuery = string.Format(ProductDBContext.GETACTIONS_STATUS_SQL, item.ProductGuid, Convert.ToInt32(ActionType.Deliever));
            //    List<Models.Action> actionDS = this.db.Database.SqlQuery<Models.Action>(actionSqlQuery).ToList<Models.Action>();

            //    if (actionDS != null && actionDS.Count > 0)
            //    {
            //        Employee employee = employeeDb.Employees.Find(actionDS[0].ActionEmployeeId);
            //        if (employee != null)
            //        {
            //            pItem.ActionEmployee = string.Join("_", employee.Name, employee.Encode);
            //        }
            //        pItem.ActionComments = actionDS[0].Comments;
            //        pItem.ActionTime = actionDS[0].ActionTime;
            //    }

            //    products.Add(pItem);
            //}
            //var productList = (from item in products
            //                   select new
            //                   {
            //                       item.Name,
            //                       item.Encode,
            //                       item.NameEncode,
            //                       item.ProductGuid,
            //                       item.ParentGuid,
            //                       item.ActionEmployee,
            //                       item.ActionTime,
            //                       item.Status,
            //                       item.TrackNumber,
            //                       item.Sender,
            //                       item.SenderTelephone,
            //                       item.Receiver,
            //                       item.ReceiverTelephone,
            //                       item.Departure,
            //                       item.Destination,
            //                       item.ActionComments,
            //                       item.AdditionGuid
            //                   });
            //return Json(productList, JsonRequestBehavior.AllowGet);
            return null;
        }
        #endregion
    }
}
