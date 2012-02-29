//-----------------------------------------------------------------------
// <copyright file="InternationalizationProvider.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using System.Web.Script.Serialization;
using Atomia.Common;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Base.Interfaces;
using System.Web.Mvc;

namespace Atomia.Web.Plugin.PublicOrder.GeneralItems
{
    /// <summary>
    /// Internationalization implementer class.
    /// </summary>
    public class InternationalizationProvider : IInternationaliazationProvider
    {
        #region IInternationaliazationProvider Members

        /// <summary>
        /// Fetches the culture info.
        /// </summary>
        /// <returns>Culture info.</returns>
        public System.Globalization.CultureInfo FetchCultureInfo()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo currentLocale = CultureInfo.GetCultureInfo(currentCulture.Name);
            if (currentLocale.IsNeutralCulture)
            {
                currentLocale = CultureInfo.CreateSpecificCulture(currentLocale.Name);
            }

            // Set OrderCurrencyLocaleName and then in LocalizationDataLoader load resource into session
            string orderCurrencyResource = LocalizationHelpers.GlobalResource(String.Format("{0}Common, Currency" + currentLocale.Name, System.Web.HttpContext.Current.Session["Theme"]))
                                            ?? LocalizationHelpers.GlobalResource(String.Format("{0}Common, Currency", System.Web.HttpContext.Current.Session["Theme"]));
            System.Web.HttpContext.Current.Session["OrderCurrencyResource"] = orderCurrencyResource;
            string orderCurrencyCode = "SEK";

            if (System.Web.HttpContext.Current.Application["CurrencyCode" + currentLocale.Name] != null &&
                !String.IsNullOrEmpty(
                    (string)System.Web.HttpContext.Current.Application["CurrencyCode" + currentLocale.Name]))
            {
                orderCurrencyCode =
                    (string)System.Web.HttpContext.Current.Application["CurrencyCode" + currentLocale.Name];
            }

            System.Web.HttpContext.Current.Session["OrderCurrencyCode"] = orderCurrencyCode;

            // Check if there is locale settings in cookie and set if there is
            if (System.Web.HttpContext.Current.Request != null && 
                System.Web.HttpContext.Current.Request.Cookies != null && 
                System.Web.HttpContext.Current.Request.Cookies["AtomiaCookieCollection"] != null && 
                !String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Cookies["AtomiaCookieCollection"].Value))
            {
                JavaScriptSerializer js = new JavaScriptSerializer();

                AtomiaCookieCollection cookieCollection = js.Deserialize<AtomiaCookieCollection>(System.Web.HttpContext.Current.Request.Cookies["AtomiaCookieCollection"].Value);

                AtomiaCookie localeCookie = cookieCollection.GetAtomiaCookie(
                    "OrderLocaleCookie",
                    System.Web.HttpContext.Current.Request.ApplicationPath,
                    System.Web.HttpContext.Current.Request.Url.Host);

                if (localeCookie != null && !String.IsNullOrEmpty(localeCookie.CookieValue))
                {
                    string cookieCountryCode = localeCookie.CookieValue;

                    if (currentCulture.TwoLetterISOLanguageName != cookieCountryCode)
                    {
                        CultureInfo cookieCulture = CultureInfo.GetCultureInfo(cookieCountryCode);
                        if (cookieCulture.IsNeutralCulture)
                        {
                            cookieCulture = CultureInfo.CreateSpecificCulture(cookieCulture.Name);
                        }

                        // CurrencyFormat
                        System.Web.HttpContext.Current.Session["CurrencyFormat"] = CultureHelper.CURRENCY_FORMAT;

                        // ShortDateFormat
                        System.Web.HttpContext.Current.Session["ShortDateFormat"] = cookieCulture.DateTimeFormat.ShortDatePattern;
                        currentLocale.DateTimeFormat.ShortDatePattern = cookieCulture.DateTimeFormat.ShortDatePattern;

                        currentLocale.NumberFormat.CurrencySymbol = string.Empty;
                        currentLocale.NumberFormat.NumberDecimalDigits = 2;

                        System.Web.HttpContext.Current.Session["OrderCurrencyResource"] = LocalizationHelpers.GlobalResource(String.Format("{0}Common, Currency" + cookieCountryCode, System.Web.HttpContext.Current.Session["Theme"]));
                        System.Web.HttpContext.Current.Session["OrderCurrencyCode"] = System.Web.HttpContext.Current.Application["CurrencyCode" + cookieCountryCode];
                    }
                }
            }

            return currentLocale;
        }

        #endregion
    }
}
