using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ZJOASystem.Models;

namespace ZJOASystem.Controllers
{
    public static class HtmlHelperExtensions
    {
        

        private static String GetActionTypeName(ActionType actionType)
        {
            switch (actionType)
            {
                case ActionType.Create:
                    return ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_CREATE");
                    
                case ActionType.Setup:
                    return ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_SETUP");
                   
                case ActionType.Test:
                    return ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_TEST");
                    
                case ActionType.Package:
                    return ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_PACKAGE");
                    
                case ActionType.Fix:
                    return ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_FIX");
                    
                case ActionType.Deliever:
                    return ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_DELIEVER");
                   
                default:
                    return ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_CREATE");   
            }
        }

        public static string GetUrl(HttpRequestBase request, string extend)
        {
            string schema = request.Url.Scheme;
            string host = request.Url.Host;
            int port = request.Url.Port;

            return string.Format("{0}://{1}:{2}/{3}", schema, host, port.ToString(), extend);
        }

        public static MvcHtmlString GetStatusLabel(UrlHelper urlHelper, ProductStatus productStatus)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<div class='form-group'>");
            string text = "";
            switch (productStatus)
            {
                case ProductStatus.Disabled:
                    text = ResourceReader.GetString("STATUS_DISABLED");
                    break;
                case ProductStatus.Qualified:
                    text = ResourceReader.GetString("STATUS_QUALIFIED");
                    break;
                case ProductStatus.Unqualified:
                    text = ResourceReader.GetString("STATUS_UNQUALIFIED");
                    break;
                case ProductStatus.Fixed:
                    text = ResourceReader.GetString("STATUS_FIXED");
                    break;
                case ProductStatus.Packaged:
                    text = ResourceReader.GetString("STATUS_PACKAGE");
                    break;
                case ProductStatus.Delievered:
                    text = ResourceReader.GetString("STATUS_DELIEVERED");
                    break;
                default:
                    text = ResourceReader.GetString("STATUS_UNKNOWN");
                    break;
            }

            builder.Append(string.Format("<div class='col-md-10'><span>{0}</span></div>", text));
            builder.AppendLine("</div");

            return new MvcHtmlString(builder.ToString());
        }
    }
}