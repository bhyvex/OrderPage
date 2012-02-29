//-----------------------------------------------------------------------
// <copyright file="UrlManagerAttribute.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Atomia.Web.Plugin.PublicOrder.Configurations;
using Atomia.Web.Plugin.PublicOrder.Helpers;

namespace Atomia.Web.Plugin.PublicOrder.Filters
{
    /// <summary>
    /// Filter that manages query strings.
    /// </summary>
    public class UrlManagerAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether to clear session.
        /// </summary>
        /// <value><c>true</c> if [clear session]; otherwise, <c>false</c>.</value>
        public bool ClearSession { get; set; }

        /// <summary>
        /// Called before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, bool> queryStringNames = new Dictionary<string, bool>();

            PublicOrderConfigurationSection opcs = LocalConfigurationHelper.GetLocalConfigurationSection();

            foreach (QueryString queryString in opcs.QueryStringList)
            {
                string qsValue = filterContext.HttpContext.Request.QueryString[queryString.Name];
                if (String.IsNullOrEmpty(qsValue))
                {
                    if (filterContext.HttpContext.Session["qs_" + queryString.Name] != null && filterContext.HttpContext.Session["qs_" + queryString.Name].ToString() != String.Empty)
                    {
                        qsValue = filterContext.HttpContext.Session["qs_" + queryString.Name].ToString();
                    }
                }

                if (!String.IsNullOrEmpty(qsValue))
                {
                    if (queryString.PassToView)
                    {
                        filterContext.Controller.ViewData["qs_" + queryString.Name] = qsValue;
                    }

                    // If action demands that after its completion all session variables for qs`s be removed (eg. thankyou page)
                    if (this.ClearSession)
                    {
                        filterContext.HttpContext.Session["qs_" + queryString.Name] = null;
                    }
                    else
                    {
                        filterContext.HttpContext.Session["qs_" + queryString.Name] = qsValue;

                        // Create list for session so other plugins can use querystrings if needed
                        queryStringNames.Add(queryString.Name, queryString.PassToView);
                    }
                }
            }

            if (queryStringNames.Count > 0)
            {
                filterContext.HttpContext.Session["order_QueryStringsDictionary"] = queryStringNames;
            }
            else
            {
                filterContext.HttpContext.Session["order_QueryStringsDictionary"] = null;
            }
        }
    }
}
