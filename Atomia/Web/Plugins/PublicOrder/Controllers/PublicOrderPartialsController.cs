//-----------------------------------------------------------------------
// <copyright file="PublicOrderPartialsController.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Atomia.Common;
using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.Cart.Models;
using Atomia.Web.Plugin.PublicOrder.Configurations;
using Atomia.Web.Plugin.PublicOrder.Filters;
using Atomia.Web.Plugin.PublicOrder.Helpers;

namespace Atomia.Web.Plugin.PublicOrder.Controllers
{
    /// <summary>
    /// Controller for partials.
    /// </summary>
    [Internationalization]
    [TranslationHelper]
    public class PublicOrderPartialsController : Controller
    {
        /// <summary>
        /// Searches the domains.
        /// </summary>
        /// <returns>Partial view for this action.</returns>
        [UrlManager]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult DomainSearch()
        {
            ViewData["multiDomains"] = this.Session["multiDomains"];

            ViewData["firstOption"] = (bool)Session["firstOption"];
            ViewData["ResellerId"] = ResellerHelper.GetResellerId();

            string[] domains = (string[])Session["domains"];
            string tmp = string.Empty;
            if (domains != null)
            {
                for (int i = 0; i < domains.Length; i++)
                {
                    if (i < domains.Length - 1)
                    {
                        tmp += domains[i] + " ";
                    }
                    else
                    {
                        tmp += domains[i];
                    }
                }
            }

            ViewData["domainsForCheck"] = tmp;

            ViewData["OwnDomain"] = string.Empty;

            ViewData["RegDomainFront"] = RegularExpression.GetRegularExpression("DomainFront");
            ViewData["RegDomain"] = RegularExpression.GetRegularExpression("Domain");

            int allowedDomainLength;
            int numberOfDomainsAllowed;
            int domainSearchTimeout;
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != string.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != string.Empty
                && this.HttpContext.Application["DomainSearchTimeout"] != null && (string)this.HttpContext.Application["DomainSearchTimeout"] != string.Empty)
            {
                allowedDomainLength = int.Parse((string)this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = int.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
                domainSearchTimeout = int.Parse((string)this.HttpContext.Application["DomainSearchTimeout"]);
            }
            else
            {
                throw new ConfigurationErrorsException("Missing AllowedDomainLength or NumberOfDomainsAllowed or DomainSearchTimeout in configuration");
            }

            ViewData["AllowedDomainLength"] = allowedDomainLength;
            ViewData["NumberOfDomainsAllowed"] = numberOfDomainsAllowed;
            ViewData["domainSearchTimeout"] = domainSearchTimeout;

            return PartialView();
        }

        /// <summary>
        /// Carts this instance.
        /// </summary>
        /// <returns>Partial view for this action.</returns>
        [UrlManager]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Cart()
        {
            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
            string countryCode = ResellerHelper.GetResellerCountryCode();
            string currencyCode = ResellerHelper.GetResellerCurrencyCode();
            Guid resellerId = ResellerHelper.GetResellerId();
            string filterValue = Session["FilterByPackage"] != null ? (string)Session["FilterByPackage"] : null;
            List<RadioRow> list = GeneralHelper.FilterPackages(this, service, Guid.Empty, resellerId, currencyCode, countryCode, filterValue);
            RadioRow preselectedId = list[0];

            // choose package to select
            if (Session["PreselectedPackage"] != null)
            {
                if (list.Exists(rr => rr.productId == (string)Session["PreselectedPackage"]))
                {
                    preselectedId = list.First(rr => rr.productId == (string)Session["PreselectedPackage"]);
                }
            }

            string switchedId = preselectedId.productId + "|" + preselectedId.productNameDesc + "|" + preselectedId.RenewalPeriodId;

            if (preselectedId.SetupFee != null)
            {
                ViewData["CartProducts"] = preselectedId.productId + '|' + preselectedId.productNameDesc + '|' +
                                            preselectedId.RenewalPeriodId + '|' + true + '|' +
                                            preselectedId.SetupFee.productID + '|' +
                                            preselectedId.SetupFee.productDesc + '|' +
                                            preselectedId.SetupFee.RenewalPeriodId + '|' + false;

                switchedId += "|" + preselectedId.SetupFee.productID + "|" + preselectedId.SetupFee.productDesc +
                                "|" + preselectedId.SetupFee.RenewalPeriodId;
            }
            else
            {
                ViewData["CartProducts"] = preselectedId.productId + '|' + preselectedId.productNameDesc + '|' +
                                            preselectedId.RenewalPeriodId + '|' + true;
            }

            ViewData["switchedId"] = switchedId;

            PublicOrderConfigurationSection opcs = LocalConfigurationHelper.GetLocalConfigurationSection();
            bool orderByPostEnabled = bool.Parse(opcs.InvoiceByPost.Enabled);

            if (orderByPostEnabled)
            {
                ViewData["OrderByPostId"] = OrderModel.FetchPostOrderId(resellerId, null, Guid.Empty, currencyCode, countryCode);
                ViewData["OrderByPostEnabled"] = true;
            }
            else
            {
                ViewData["OrderByPostId"] = string.Empty;
                ViewData["OrderByPostEnabled"] = false;
            }
            
            return PartialView();
        }
    }
}
