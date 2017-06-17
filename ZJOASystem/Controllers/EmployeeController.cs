using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ZJOASystem.Models;

namespace ZJOASystem.Controllers
{
    public class EmployeeController : Controller
    {
        private EmployeeDBContext db = new EmployeeDBContext();


        public ActionResult DepartmentIndex()
        {
            if (Request.IsAuthenticated)
            {
                return View(db.Departments.ToList());
            }
            else
            {
                return Redirect("../Account/Login");
            }
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

        public JsonResult GetEmployees()
        {
           List<Employee> result =  this.db.Employees.ToList<Employee>();
           
           var employeeList = (from item in result
                              select new
                              {
                                  item.Id,
                                  item.Name,
                                  item.Encode,
                                  item.Telephone,
                                  item.Department
                              });
           return Json(employeeList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DepartmentDetail(int? id)
        {
            if (Request.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Department department = db.Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                return View(department);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }
        // GET: /Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (Request.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee employee = db.Employees.Find(id);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                return View(employee);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        public ActionResult CreateDepartment()
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

        // POST: /Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDepartment([Bind(Include = "Name,Telephone")] Department department)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    foreach (var key in Request.Form.Keys)
                    {
                        if (key is String)
                        {
                            string keyText = Convert.ToString(key);

                            if (keyText.Equals("managerlist", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string value = Request.Form.GetValues(keyText)[0];
                                department.ManagerId = Convert.ToInt32(value);
                            }

                            if (keyText.Equals("departmentlist", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string depvalue = Request.Form.GetValues(keyText)[0];
                                department.ParentId = Convert.ToInt32(depvalue);
                            }
                        }

                    }
                    db.Departments.Add(department);
                    db.SaveChanges();
                    return RedirectToAction("DepartmentIndex");
                }

                return View(department);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }
        // GET: /Employee/Create
        public ActionResult Create()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return Redirect("Account/Login");
            }
        }

        // POST: /Employee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Encode, Telephone,Email, Address")] Employee employee)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    foreach (var key in Request.Form.Keys)
                    {
                        if (key is String)
                        {
                            string keyText = Convert.ToString(key);

                            if (keyText.StartsWith("department_"))
                            {
                                string value = Request.Form.GetValues(keyText)[0];
                                if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("on", StringComparison.CurrentCultureIgnoreCase) || value == "1")
                                {
                                    int departmentId = Convert.ToInt32(keyText.Replace("department_", ""));

                                    if (departmentId > 0)
                                    {
                                        Department departmentObj = this.db.GetDepartment(departmentId);
                                        if (departmentObj != null)
                                        {
                                            employee.Departments.Add(departmentObj);
                                        }
                                    }

                                }
                            }
                        }

                    }
                    db.Employees.Add(employee);
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }

                return View(employee);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        public ActionResult EditDepartment(int? id)
        {
            if (Request.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Department department = db.Departments.Find(id);
                if (department == null)
                {
                    return HttpNotFound();
                }
                return View(department);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        // POST: /Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment([Bind(Include = "Id,Name,Telephone")] Department department)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {

                    foreach (var key in Request.Form.Keys)
                    {
                        if (key is String)
                        {
                            string keyText = Convert.ToString(key);

                            if (keyText.Equals("managerlist", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string value = Request.Form.GetValues(keyText)[0];
                                department.ManagerId = Convert.ToInt32(value);
                            }

                            if (keyText.Equals("departmentlist", StringComparison.CurrentCultureIgnoreCase))
                            {
                                string depvalue = Request.Form.GetValues(keyText)[0];
                                department.ParentId = Convert.ToInt32(depvalue);
                            }
                        }

                    }
                    db.Entry(department).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("DepartmentIndex");
                }
                return View(department);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        // GET: /Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Request.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee employee = db.Employees.Find(id);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                return View(employee);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        // POST: /Employee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Encode,Telephone,Email,Address")] Employee employee)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    foreach (var key in Request.Form.Keys)
                    {
                        if (key is String)
                        {
                            string keyText = Convert.ToString(key);

                            if (keyText.StartsWith("department_"))
                            {
                                string value = Request.Form.GetValues(keyText)[0];
                                if (value.Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.Equals("on", StringComparison.CurrentCultureIgnoreCase) || value == "1")
                                {
                                    int departmentId = Convert.ToInt32(keyText.Replace("department_", ""));

                                    if (departmentId > 0)
                                    {
                                        Department departmentObj = this.db.GetDepartment(departmentId);
                                        if (departmentObj != null)
                                        {
                                            employee.Departments.Add(departmentObj);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(employee);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        // GET: /Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Request.IsAuthenticated)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee employee = db.Employees.Find(id);
                if (employee == null)
                {
                    return HttpNotFound();
                }
                return View(employee);
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        // POST: /Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Request.IsAuthenticated)
            {
                Employee employee = db.Employees.Find(id);
                db.Employees.Remove(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        public ActionResult DeleteDepartment(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
            db.SaveChanges();
            return RedirectToAction("DepartmentIndex");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public static MvcHtmlString RenderDepartmentText(Employee employee)
        {
            StringBuilder builder = new StringBuilder();

            if (employee.Departments == null || employee.Departments.Count == 0)
            {
                EmployeeDBContext db = new EmployeeDBContext();

                db.SetEmployeeDepartments(employee);
            }
            builder.Append(employee.GetDepartmentTexts());

            return MvcHtmlString.Create(builder.ToString());
        }
        public static MvcHtmlString RenderDepartmentList(Employee employee)
        {
            StringBuilder builder = new StringBuilder();

            EmployeeDBContext db = new EmployeeDBContext();

            string checkedstring = (employee == null || employee.Belongs(0)) ? "checked" : "";
            builder.AppendLine(String.Format("<div><input name='department_0' type='checkbox' {0}>{1}</input></div>",
                checkedstring, ZJOASystem.Controllers.ResourceReader.GetString("DEPARTMENT_NONE")));

            List<Department> alldepartments = db.Departments.ToList<Department>();
            if (alldepartments != null && alldepartments.Count > 0)
            {
                foreach (Department department in alldepartments)
                {
                    checkedstring = (employee != null && employee.Belongs(department.Id)) ? "checked" : "";
                    builder.AppendLine(String.Format("<div><input name='department_{0}' type='checkbox' {1}>{2}</input></div>",
                        department.Id, checkedstring, department.Name));
                }
            }

            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString RenderDepartmentTree()
        {
            EmployeeDBContext db = new EmployeeDBContext();

            StringBuilder builder = new StringBuilder();
            RenderChildDepartments(ref db, ref builder, 0);

            return MvcHtmlString.Create(builder.ToString());
        }

        private static void RenderChildDepartments(ref EmployeeDBContext db, ref StringBuilder builder, int parentId)
        {
            List<Department> departments = db.Database.SqlQuery<Department>("SELECT * FROM Departments WHERE ParentId={0}", parentId).ToList<Department>();
            if (departments != null && departments.Count > 0)
            {
                builder.AppendLine("<ul class='treeview-menu'>");

                foreach (Department item in departments)
                {
                    builder.AppendLine("<li>");
                    builder.AppendLine(string.Format("<a href='../Employee/DepartmentDetail/{0}'>{1}</a>", item.Id, item.Name));
                    RenderChildDepartments(ref db, ref builder, item.Id);
                    builder.AppendLine("</li>");
                }
                builder.AppendLine("</ul>");
            }

        }

        public static MvcHtmlString RenderDepartmentsCombo(Department departmentItem)
        {
            StringBuilder builder = new StringBuilder();

            EmployeeDBContext db = new EmployeeDBContext();

            builder.AppendLine("<select name='departmentList'>");

            string selectedstring = (departmentItem == null || departmentItem.ParentId == 0) ? "selected" : "";

            builder.AppendLine(String.Format("<option name='department_0' value=0 {0}>{1}</option>",
                     selectedstring, ZJOASystem.Controllers.ResourceReader.GetString("DEPARTMENT_NONE")));

            List<Department> alldepartments = db.Departments.ToList<Department>();
            if (alldepartments != null && alldepartments.Count > 0)
            {
                foreach (Department department in alldepartments)
                {
                    if (departmentItem == null || department.Id != departmentItem.Id)
                    {
                        selectedstring = (departmentItem == null || departmentItem.ParentId == department.Id) ? "selected" : "";

                        builder.AppendLine(String.Format("<option name='department_{0}' value='{0}' {1}>{2}</option>",
                            department.Id, selectedstring, department.Name));
                    }
                }
            }
            builder.AppendLine("</select>");
            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString RenderEmployeesCombo(Department departmentItem, string namePrefix, bool includeNone, bool includeEncode)
        {
            StringBuilder builder = new StringBuilder();

            EmployeeDBContext db = new EmployeeDBContext();

            builder.AppendLine(string.Format("<select name='{0}List'>", namePrefix));

            string selectedstring = (departmentItem == null || departmentItem.ManagerId == 0) ? "selected" : "";

            if (includeNone)
            {
                builder.AppendLine(String.Format("<option name='{0}_0' value='0' {1}>{2}</option>",
                    namePrefix, selectedstring, ZJOASystem.Controllers.ResourceReader.GetString("DEPARTMENT_NONE")));
            }
            List<Employee> allemployees = (departmentItem == null) ? db.Employees.ToList<Employee>() : db.GetDepartmentEmployees(departmentItem.Id);
            if (allemployees != null && allemployees.Count > 0)
            {
                foreach (Employee employee in allemployees)
                {
                    selectedstring = (departmentItem != null && departmentItem.ManagerId == employee.Id) ? "selected" : "";

                    if (includeEncode)
                    {
                        builder.AppendLine(String.Format("<option name='{0}_{1}' value='{1}' {2}>{3}_{4}</option>",
                        namePrefix, employee.Id, selectedstring, employee.Name, Convert.ToString(employee.Encode)));
                    }
                    else
                    {
                        builder.AppendLine(String.Format("<option name='{0}_{1}' value='{1}' {2}>{3}</option>",
                        namePrefix, employee.Id, selectedstring, employee.Name));
                    }
                }
            }
            builder.AppendLine("</select>");
            return MvcHtmlString.Create(builder.ToString());
        }

        public static object RenderEmployeeName(int employeeId)
        {
            StringBuilder builder = new StringBuilder();

            EmployeeDBContext db = new EmployeeDBContext();

            if (employeeId == 0)
            {
                builder.AppendLine(ZJOASystem.Controllers.ResourceReader.GetString("DEPARTMENT_NONE"));
            }
            else
            {
                Employee employee = db.Employees.Find(employeeId);
                if (employee == null)
                {
                    builder.AppendLine(ZJOASystem.Controllers.ResourceReader.GetString("DEPARTMENT_NONE"));
                }
                else
                {
                    builder.AppendLine(employee.Name);
                }
            }


            return MvcHtmlString.Create(builder.ToString());
        }

        public static object RenderDepartmentName(int departmentId)
        {
            StringBuilder builder = new StringBuilder();

            EmployeeDBContext db = new EmployeeDBContext();

            if (departmentId == 0)
            {
                builder.AppendLine(ZJOASystem.Controllers.ResourceReader.GetString("DEPARTMENT_NONE"));
            }
            else
            {
                Department department = db.Departments.Find(departmentId);
                if (department == null)
                {
                    builder.AppendLine(ZJOASystem.Controllers.ResourceReader.GetString("DEPARTMENT_NONE"));
                }
                else
                {
                    builder.AppendLine(department.Name);
                }
            }


            return MvcHtmlString.Create(builder.ToString());
        }

    }
}
