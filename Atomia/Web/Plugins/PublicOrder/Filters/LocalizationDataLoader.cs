//-----------------------------------------------------------------------
// <copyright file="LocalizationDataLoader.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Plugin.PublicOrder.Helpers;

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
                if (filterContext.HttpContext != null 
                    && filterContext.HttpContext.Request != null)
                {
                    AtomiaCultureInfo languageObj = null;
                    bool shouldUpdateCookie = false;
                    if (filterContext.HttpContext.Request.Params["lang"] != null)
                    {
                        string selLanguage = filterContext.HttpContext.Request.Params["lang"];
                        string language = selLanguage.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        string culture = selLanguage.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        languageObj = new AtomiaCultureInfo { Language = language, Culture = culture };

                        shouldUpdateCookie = true;
                    }
                    else if (filterContext.HttpContext.Request.Cookies != null && filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"] != null 
                        && !String.IsNullOrEmpty(filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"].Value))
                    {
                        try
                        {
                            JavaScriptSerializer js = new JavaScriptSerializer();

                            AtomiaCookieCollection cookieCollection =
                                js.Deserialize<AtomiaCookieCollection>(System.Web.HttpContext.Current.Request.Cookies["AtomiaCookieCollection"].Value);

                            AtomiaCookie languageCookie = cookieCollection.GetAtomiaCookie(
                                "OrderLanguageCookie",
                                filterContext.HttpContext.Request.ApplicationPath,
                                filterContext.HttpContext.Request.Url.Host);

                            if (languageCookie != null && !String.IsNullOrEmpty(languageCookie.CookieValue))
                            {
                                languageObj = js.Deserialize<AtomiaCultureInfo>(languageCookie.CookieValue);
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        if (filterContext.RouteData.Values.ContainsKey("resellerHash"))
                        {
                            ResellerHelper.LoadResellerIntoSessionByHash((string)filterContext.RouteData.Values["resellerHash"]);
                        }
                        else
                        {
                            ResellerHelper.LoadResellerIntoSessionByUrl(filterContext.HttpContext.Request.Url.Host);
                        }

                        IList<Language> languages = ResellerHelper.GetResellerLanguages();
                        Language defaultResellerLanguage = languages.FirstOrDefault(l => l.IsDefault);
                        if (defaultResellerLanguage != null)
                        {
                            string language = defaultResellerLanguage.Code.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0];
                            string culture = defaultResellerLanguage.Code.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1];
                            languageObj = new AtomiaCultureInfo { Language = language, Culture = culture };
                        }
                    }

                    if (languageObj != null)
                    {
                        // get culture settings from session object, set in Internationalization attribute
                        AtomiaCultureInfo sessionCultureInfo = null;
                        if (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session["SessionAccountLanguages"] != null)
                        {
                            sessionCultureInfo = (AtomiaCultureInfo)filterContext.HttpContext.Session["SessionAccountLanguages"];
                        }

                        if (sessionCultureInfo == null)
                        {
                            // Dont reload page if Session language is null, its app start
                            filterContext.HttpContext.Session["SessionAccountLanguages"] = languageObj;
                        }
                        else if (sessionCultureInfo.Language != languageObj.Language || sessionCultureInfo.Culture != languageObj.Culture)
                        {
                            filterContext.HttpContext.Session["SessionAccountLanguages"] = languageObj;
                            updated = true;
                            if (shouldUpdateCookie)
                            {
                                this.UpdateCookie(filterContext, languageObj);
                            }
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

        /// <summary>
        /// Updates the cookie.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <param name="languageObj">The language obj.</param>
        private void UpdateCookie(ActionExecutingContext filterContext, AtomiaCultureInfo languageObj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            AtomiaCookieCollection cookieCollection;
            if (filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"] != null 
                && !String.IsNullOrEmpty(filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"].Value))
            {
                cookieCollection = js.Deserialize<AtomiaCookieCollection>(filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"].Value);
            }
            else
            {
                cookieCollection = new AtomiaCookieCollection();
            }

            AtomiaCookie aCookie = new AtomiaCookie(
                filterContext.HttpContext.Request.ApplicationPath,
                filterContext.HttpContext.Request.Url.Host,
                js.Serialize(languageObj),
                "OrderLanguageCookie");

            cookieCollection.AddOrUpdateAtomiaCookie(aCookie);

            string serializedCookieCollection = js.Serialize(cookieCollection);

            HttpCookie cookie = new HttpCookie("AtomiaCookieCollection") { Value = serializedCookieCollection, Expires = DateTime.Now.AddYears(1) };

            if (filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"] != null && !String.IsNullOrEmpty(filterContext.HttpContext.Request.Cookies["AtomiaCookieCollection"].Value))
            {
                filterContext.HttpContext.Response.Cookies["AtomiaCookieCollection"].Value = cookie.Value;
                filterContext.HttpContext.Response.Cookies["AtomiaCookieCollection"].Expires = cookie.Expires;
            }
            else
            {
                filterContext.HttpContext.Response.Cookies.Add(cookie);
            }
        }
    }
}
