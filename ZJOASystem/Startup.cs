using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System.Web.Security;
using ZJOASystem.Models;

[assembly: OwinStartupAttribute(typeof(ZJOASystem.Startup))]
namespace ZJOASystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }

        public string[] RolesList = {"Admin", "组装组", "装箱组","检测组","发货组" };

        private void CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!roleManager.RoleExists(roleName))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();

                role.Name = roleName;
                roleManager.Create(role);
            }
        }
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            for (int i = 0; i < RolesList.Length; i++)
            {
                CreateRole(roleManager, RolesList[i]);
            }

            ApplicationUser adminUser = UserManager.FindByName<ApplicationUser>("admin");
            if (adminUser == null)
            {
                var user = new ApplicationUser();
                user.UserName = "admin";
                user.Email = "";

                string password = "admin";
                string encypwd = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "md5");
                user.EncPassword = encypwd;

                var chkUser = UserManager.Create(user, encypwd);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }
            }
        }
    }
}
