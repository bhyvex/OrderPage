using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;

namespace Atomia.Web.Plugin.PublicOrder.Filters
{
    public class ServiceProvider : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Session == null)
            {
                return;
            }

            if (filterContext.HttpContext.Session["PublicOrderService"] == null)
            {
                return;
            }

            ((AtomiaBillingPublicService) filterContext.HttpContext.Session["PublicOrderService"]).Dispose();
            filterContext.HttpContext.Session.Remove("PublicOrderService");
        }
    }
}
