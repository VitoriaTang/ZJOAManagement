using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZJOASystem.Models;

namespace ZJOASystem.Controllers
{
    public class MachineController : Controller
    {
        private MachineDBContext db = new MachineDBContext();

        [HttpPost]
        public ActionResult ImportMachines()
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

                        List<MachineRecord> items = CSVHelper.OpenMachineRecordCSV(allText);
                        
                        foreach (MachineRecord item in items)
                        {
                            string sqlQuery = string.Format(MachineDBContext.CHECK_MACHINEACTION_EXIST, item.Encode, item.AssignTime, Convert.ToInt32(item.AssignType));
                            List<int> existedResult = this.db.Database.SqlQuery<int>(sqlQuery).ToList<int>();

                            if (existedResult != null && existedResult.Count > 0 && existedResult[0]>0)
                            {
                                continue;
                            }
                            else
                            {
                                db.SaveMachineRecord(item);
                            }
                        }
                    }
                }
                return RedirectToAction("../Machine/Index");
            }
            else
            {
                return Redirect("../Account/Login");
            }
        }

        public FileResult ExportProductAction()
        {
            string sqlQuery = MachineDBContext.GET_MACHINEACTIONS;

            List<MachineRecord> result = this.db.Database.SqlQuery<MachineRecord>(sqlQuery).ToList<MachineRecord>();

            foreach (MachineRecord item in result)
            {
                string selectOpt = "SELECT UserEncode From machine_users WHERE AssignId=" + item.Id;
                List<string> operators = this.db.Database.SqlQuery<string>(selectOpt).ToList<string>();

                item.Users = new List<Operator>();
                foreach (string optEncode in operators)
                {
                    Operator optObj = new Operator();
                    optObj.Encode = optEncode;
                    item.Users.Add(optObj);
                }
            }

            String filePath = CSVHelper.SaveMachineRecordCSV(result);
            string contentType = "application/csv";
            return File(filePath, contentType, string.Format("MachineAssigns_{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        public JsonResult GetMachineRecords()
        {
            string sqlQuery = MachineDBContext.GET_MACHINEACTIONS + string.Format("  WHERE a.AssignType={0};", Convert.ToInt32(AssignType.Borrow)); 

            List<MachineRecord> result = this.db.Database.SqlQuery<MachineRecord>(sqlQuery).ToList<MachineRecord>();
            foreach (MachineRecord item in result)
            {
                sqlQuery = string.Format(MachineDBContext.GET_MACHINE_USERS, item.Id);
                List<Operator> operators = this.db.Database.SqlQuery<Operator>(sqlQuery).ToList<Operator>();

                item.Users = operators;
            }

            var machineList = (from item in result
                               select new
                               {
                                   item.Encode,
                                   item.Name,
                                   item.AssignTime,
                                   item.AssignComments,
                                   item.UsersNameText
                               });
            return Json(machineList, JsonRequestBehavior.AllowGet);
        }

        #region Index page
        
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

       #endregion

        public static int MachineCount
        {
            get
            {
                MachineDBContext db = new MachineDBContext();
                string sql = "SELECT COUNT(Encode) FROM machines";
                List<int> result = db.Database.SqlQuery<int>(sql).ToList<int>();
                return result[0];
            }
        }
    }
}