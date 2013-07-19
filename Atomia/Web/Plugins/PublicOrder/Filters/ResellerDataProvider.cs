// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResellerDataLoader.cs" company="Atomia AB">
//   Copyright (C) 2013 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Represents serializable custom data entry.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using namespaces
using System.Linq;
using System.Web.Mvc;

using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using Atomia.Web.Plugin.PublicOrder.Helpers;

#endregion Using namespaces

namespace Atomia.Web.Plugin.PublicOrder.Filters
{
    /// <summary>
    /// Attribute used for getting reseller data needed for views.
    /// </summary>
    public class ResellerDataProvider : ActionFilterAttribute
    {
        /// <summary>
        /// Called when action is executing.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session != null)
            {
                if (filterContext.HttpContext.Session["ResellerHomeUrl"] == null || filterContext.HttpContext.Session["ResellerName"] == null)
                {
                    if ((filterContext.HttpContext.Session["ResellerLoaded"] == null ||
                         filterContext.HttpContext.Session["ResellerLoaded"].ToString().ToLowerInvariant() != "true"))
                    {
                        if (filterContext.RouteData.Values.ContainsKey("resellerHash"))
                        {
                            ResellerHelper.LoadResellerIntoSessionByHash(
                                (string)filterContext.RouteData.Values["resellerHash"]);
                        }
                        else
                        {
                            ResellerHelper.LoadResellerIntoSessionByUrl(filterContext.HttpContext.Request.Url.AbsoluteUri);
                        }

                        // Flag which prevents another loading of reseller is set to true even if there's no reseller.
                        // If we didn't get it now, we won't get it in future attempts too.
                        filterContext.HttpContext.Session["ResellerLoaded"] = "true";
                    }

                    if (filterContext.HttpContext.Session["resellerAccountData"] != null)
                    {
                        AccountData accountData = filterContext.HttpContext.Session["resellerAccountData"] as AccountData;
                        if (accountData != null)
                        {
                            filterContext.HttpContext.Session["ResellerName"] = accountData.Name;

                            if (accountData.AdditionalData != null && accountData.AdditionalData.Any(a => a.Name == "HomeUrl"))
                            {
                                filterContext.HttpContext.Session["ResellerHomeUrl"] =
                                    accountData.AdditionalData.First(a => a.Name == "HomeUrl").Value;
                            }
                        }
                    }
                }

                filterContext.Controller.ViewData["ResellerHomeUrl"] = filterContext.HttpContext.Session["ResellerHomeUrl"];
                filterContext.Controller.ViewData["ResellerName"] = filterContext.HttpContext.Session["ResellerName"];
            }
        }
    }
}