// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResellerHelper.cs" company="Atomia AB">
//   Copyright (C) 2012 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Hepler class for reseller related logic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using namespaces
using System;
using System.Web;

using Atomia.Web.Plugin.Cart.Models;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;

#endregion Using namespaces

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Hepler class for reseller related logic.
    /// </summary>
    public class ResellerHelper
    {
        /// <summary>
        /// Gets the reseller id.
        /// </summary>
        /// <returns>Reseller's id.</returns>
        public static Guid GetResellerId()
        {
            return HttpContext.Current.Session["resellerAccountData"] != null
                       ? ((AccountData)HttpContext.Current.Session["resellerAccountData"]).Id
                       : new Guid(OrderModel.FetchResellerGuidFromXml());
        }

        /// <summary>
        /// Gets the reseller country code.
        /// </summary>
        /// <returns>Reseller's country code.</returns>
        public static string GetResellerCountryCode()
        {
            return HttpContext.Current.Session["resellerAccountData"] != null
                       ? ((AccountData)HttpContext.Current.Session["resellerAccountData"]).DefaultCountry.Code
                       : CountriesHelper.GetDefaultCountryCodeFromConfig();
        }

        /// <summary>
        /// Gets the reseller currency code.
        /// </summary>
        /// <returns>Reseller's currency code.</returns>
        public static string GetResellerCurrencyCode()
        {
            return HttpContext.Current.Session["resellerAccountData"] != null
                       ? ((AccountData)HttpContext.Current.Session["resellerAccountData"]).DefaultCurrencyCode
                       : CountriesHelper.GetDefaultCurrencyCodeFromConfig(CountriesHelper.GetDefaultCountryCodeFromConfig());
        }

        /// <summary>
        /// Loads the reseller into session.
        /// </summary>
        /// <param name="resellerHash">The reseller hash.</param>
        public static void LoadResellerIntoSession(string resellerHash)
        {
            // Load reseller's data only if it's not present in session.
            if (HttpContext.Current.Session["resellerAccountData"] == null 
                || ((AccountData)HttpContext.Current.Session["resellerAccountData"]).Hash != resellerHash)
            {
                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = HttpContext.Current.Application["OrderApplicationPublicServiceURL"].ToString();

                    AccountData resellerAccountData = service.GetAccountDataByHash(resellerHash);
                    if (resellerAccountData != null)
                    {
                        if (HttpContext.Current.Session["resellerAccountData"] == null)
                        {
                            HttpContext.Current.Session.Add("resellerAccountData", resellerAccountData);
                        }
                        else
                        {
                            HttpContext.Current.Session["resellerAccountData"] = resellerAccountData;
                        }
                    }
                }
            }
        }
    }
}