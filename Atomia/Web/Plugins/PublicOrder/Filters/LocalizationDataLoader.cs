//-----------------------------------------------------------------------
// <copyright file="LocalizationDataLoader.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Atomia.Web.Base.Helpers.General;
using Elmah;

namespace Atomia.Web.Plugin.PublicOrder.Filters
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Attribute for loading localization data.
    /// </summary>
    public class LocalizationDataLoader : ActionFilterAttribute
    {
        /// <summary>
        /// Called when [action executing].
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool updated = false;

            // Here you will load only language data, other locale settings are set through InternationalizationProvider implementer, and Internationalization attribute treats them automaticaly
            try
            {
                // if language cookie exists
                if (filterContext.HttpContext != null && filterContext.HttpContext.Request != null && filterContext.HttpContext.Request.Cookies != null && filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"] != null && !String.IsNullOrEmpty(filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"].Value))
                {
                    AtomiaCultureInfo cookieLanguage = null;
                    try
                    {
                        JavaScriptSerializer js = new JavaScriptSerializer();

                        AtomiaCookieCollection cookieCollection = js.Deserialize<AtomiaCookieCollection>(System.Web.HttpContext.Current.Request.Cookies["AtomiaCookieCollection"].Value);

                        AtomiaCookie languageCookie = cookieCollection.GetAtomiaCookie(
                            "OrderLanguageCookie",
                            filterContext.HttpContext.Request.ApplicationPath,
                            filterContext.HttpContext.Request.Url.Host);

                        if (languageCookie != null && !String.IsNullOrEmpty(languageCookie.CookieValue))
                        {
                            cookieLanguage = js.Deserialize<AtomiaCultureInfo>(languageCookie.CookieValue);
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    if (cookieLanguage != null)
                    {
                        // get culture settings from session object, set in Internationalization attribute
                        AtomiaCultureInfo sessionCultureInfo = null;
                        if (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session["SessionAccountLanguages"] != null)
                        {
                            sessionCultureInfo = (AtomiaCultureInfo) filterContext.HttpContext.Session["SessionAccountLanguages"];
                        }

                        if (sessionCultureInfo == null)
                        {
                            // Dont reload page if Session language is null, its app start
                            filterContext.HttpContext.Session["SessionAccountLanguages"] = cookieLanguage;
                        }
                        else if (sessionCultureInfo.Language != cookieLanguage.Language || sessionCultureInfo.Culture != cookieLanguage.Culture)
                        {
                            filterContext.HttpContext.Session["SessionAccountLanguages"] = cookieLanguage;
                            updated = true;
                        }
                    }
                }

                if (updated)
                {
                    filterContext.HttpContext.Response.Redirect(filterContext.HttpContext.Request.RawUrl);
                }
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
                throw;
            }
        }
    }
}
