//-----------------------------------------------------------------------
// <copyright file="PublicOrderPartialsController.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Atomia.Common;
using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.Cart.Models;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
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

            string[] domains = (string[])Session["domains"];
            string tmp = string.Empty;
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

            ViewData["domainsForCheck"] = tmp;

            ViewData["OwnDomain"] = string.Empty;

            ViewData["RegDomainFront"] = RegularExpression.GetRegularExpression("DomainFront");
            ViewData["RegDomain"] = RegularExpression.GetRegularExpression("Domain");

            int allowedDomainLength;
            int numberOfDomainsAllowed;
            int domainSearchTimeout;
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != String.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != String.Empty
                && this.HttpContext.Application["DomainSearchTimeout"] != null && (string)this.HttpContext.Application["DomainSearchTimeout"] != String.Empty)
            {
                allowedDomainLength = Int32.Parse((string) this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = Int32.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
                domainSearchTimeout = Int32.Parse((string)this.HttpContext.Application["DomainSearchTimeout"]);
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
            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                List<RadioRow> list = OrderModel.FetchPackagesDataFromXml(this, service, Guid.Empty, null, null);

                string switchedId = list[0].productId + "|" + list[0].productNameDesc + "|" + list[0].RenewalPeriodId;
                if (list[0].SetupFee != null)
                {
                    switchedId += "|" + list[0].SetupFee.productID + "|" + list[0].SetupFee.productDesc + "|" +
                                  list[0].SetupFee.RenewalPeriodId;
                }

                ViewData["switchedId"] = switchedId;

                string setupFeeId = OrderModel.FetchSetupFeeIdFromXml(service, Guid.Empty, null, null);
                if (setupFeeId != String.Empty)
                {
                    ProductDescription setupFee = OrderModel.FetchSetupFeePackageFromXml(service, Guid.Empty, null, null);

                    ViewData["CartProducts"] = list[0].productId + '|' + list[0].productNameDesc + '|' + list[0].RenewalPeriodId + '|' + true + '|' +
                        setupFee.productID + '|' + setupFee.productDesc + '|' + setupFee.RenewalPeriodId + '|' + false;
                }
                else
                {
                    ViewData["CartProducts"] = list[0].productId + '|' + list[0].productNameDesc + '|' + list[0].RenewalPeriodId + '|' + true;
                }

                PublicOrderConfigurationSection opcs = LocalConfigurationHelper.GetLocalConfigurationSection();
                bool orderByPostEnabled = Boolean.Parse(opcs.InvoiceByPost.Enabled);

                if (orderByPostEnabled)
                {
                    ViewData["OrderByPostId"] = OrderModel.FetchPostOrderIdFromXml(service, Guid.Empty, null, null);
                    ViewData["OrderByPostEnabled"] = true;
                }
                else
                {
                    ViewData["OrderByPostId"] = String.Empty;
                    ViewData["OrderByPostEnabled"] = false;
                }
            }

            return PartialView();
        }
    }
}
