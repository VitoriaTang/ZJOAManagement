using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZJOASystem.Models;

namespace ZJOASystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                ViewBag.Role = CurrentRole();
                return View();
            }
            else
            {
                return Redirect("~/Account/Login");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string CurrentRole()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;

                ApplicationDbContext context = new ApplicationDbContext();

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                var userRoles = userManager.GetRoles(user.GetUserId());

                string roleText = "";

                for (int i = 0; i < userRoles.Count; i++)
                {
                    roleText += userRoles[i].ToString() + "_";
                }

                return roleText;
            }
            return "";
        }
    }
}