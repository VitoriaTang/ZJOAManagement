using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZJOASystem.Controllers
{
    public class ResourceReader
    {
        public static string GetString(string name)
        {
            return Properties.Resources.ResourceManager.GetString(name);
        }
    }
}