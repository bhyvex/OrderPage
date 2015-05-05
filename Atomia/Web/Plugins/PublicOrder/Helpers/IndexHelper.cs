//-----------------------------------------------------------------------
// <copyright file="IndexHelper.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Atomia.Web.Plugin.DomainSearch.Models;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Class containing helper functions for Index View Action  
    /// </summary>
    public class IndexHelper
    {
        /// <summary>
        /// Firsts the option selected.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewDataDomains">The view data_domains.</param>
        /// <param name="domainData">The domain data.</param>
        /// <param name="indexFormDomains">The index form_ domains.</param>
        public static void FirstOptionSelected(Controller controller, out string[] viewDataDomains, out List<DomainDataFromXml> domainData, string indexFormDomains)
        {
            string[] domainsSubmitted = indexFormDomains.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            string domainsArray = string.Empty; // format for StartSearch
            for (int i = 0; i < domainsSubmitted.Length; i++)
            {
                if (i < domainsSubmitted.Length - 1)
                {
                    domainsArray += domainsSubmitted[i].Trim() + ' ';
                }
                else
                {
                    domainsArray += domainsSubmitted[i].Trim();
                }
            }

            List<string> domains = DomainSearch.Helpers.DomainSearchHelper.GetDomainsForCheck(domainsArray, ResellerHelper.GetResellerId());

            DomainDataFromXml[] unavailableDomains, domainDataFromDs;

            var service = GeneralHelper.GetPublicOrderService(controller.HttpContext.ApplicationInstance.Context);
                string countryCode = ResellerHelper.GetResellerCountryCode();
                string currencyCode = ResellerHelper.GetResellerCurrencyCode();
                Guid resellerId = ResellerHelper.GetResellerId();

                unavailableDomains = DomainSearch.Helpers.DomainSearchHelper.MarkDomainsAsUnavailable(
                    GeneralHelper.GetUnavailableDomainsHelper(controller, domains.ToArray()),
                    service,
                    Guid.Empty,
                    resellerId,
                    currencyCode,
                    countryCode);

                bool hideUnavailableDomains = true;
                if (controller.HttpContext.Application.AllKeys.Contains("HideUnavailableDomains"))
                {
                    hideUnavailableDomains = controller.HttpContext.Application["HideUnavailableDomains"].ToString().ToLower() == "true";
                }

                foreach (DomainDataFromXml unavailableDomain in unavailableDomains)
                {
                    // markDomains returns decoded domains
                    string domain = SimpleDnsPlus.IDNLib.Encode(unavailableDomain.ProductName);
                    if (domains.Any(d => d == domain) && hideUnavailableDomains)
                    {
                        domains.Remove(SimpleDnsPlus.IDNLib.Encode(domain));
                    }
                }

                domainDataFromDs = DomainSearch.Helpers.DomainSearchHelper.GetDomainData(
                    domains.ToArray(), 
                    service, 
                    Guid.Empty, 
                    resellerId, 
                    currencyCode, 
                    countryCode);

            domainData = unavailableDomains.Select(data => new DomainDataFromXml
                {
                    IsDomain = data.IsDomain, 
                    ProductDuration = data.ProductDuration, 
                    ProductID = data.ProductID, 
                    ProductName = data.ProductName, 
                    ProductPrice = data.ProductPrice, 
                    ProductStatus = data.ProductStatus,
                }).ToList();

            domainData.AddRange(domainDataFromDs.Select(data => new DomainDataFromXml
                {
                    IsDomain = data.IsDomain, 
                    NumberOfDomains = data.NumberOfDomains, 
                    ProductDuration = data.ProductDuration, 
                    ProductID = data.ProductID, 
                    ProductName = data.ProductName, 
                    ProductPrice = data.ProductPrice, 
                    ProductStatus = data.ProductStatus, 
                    TransactionId = data.TransactionId
                }));

            viewDataDomains = domains.ToArray();
        }

        /// <summary>
        /// Seconds the option selected.
        /// </summary>
        /// <param name="viewDataDomains">The view data_domains.</param>
        /// <param name="domainData">The domain data.</param>
        /// <param name="indexFormDomain">The index form_ domain.</param>
        public static void SecondOptionSelected(out string[] viewDataDomains, out DomainDataFromXml domainData, string indexFormDomain)
        {
            viewDataDomains = new string[] { };

            domainData = new DomainDataFromXml { IsDomain = true, ProductName = indexFormDomain, ProductStatus = "success" };
        }

        /// <summary>
        /// Checks if immediate login is enabled or not.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>True if ImmediateLogin is set to true, otherwise false.</returns>
        public static bool IsImmediateLoginEnabled(Controller controller)
        {
            bool ret = false;

            if (controller.HttpContext.Application.AllKeys.Contains("ImmediateLogin") &&
                controller.HttpContext.Application["ImmediateLogin"].ToString().ToLowerInvariant() == "true")
            {
                ret = true;
            }

            return ret;
        }

        public static string GetImmediateLoginUrl(Controller controller, string username, string token)
        {
            string url = string.Empty;

            if (!controller.HttpContext.Application.AllKeys.Contains("ImmediageLoginIdentityUrl") ||
                string.IsNullOrEmpty(controller.HttpContext.Application["ImmediageLoginIdentityUrl"].ToString()))
            {
                throw new Exception("You have to set ImmediageLoginIdentityUrl to use the ImmediateLogin feature.");
            }

            url = controller.HttpContext.Application["ImmediageLoginIdentityUrl"].ToString();
            url = string.Format("{0}?username={1}&token={2}", url, HttpUtility.UrlEncode(username), token);

            return url;
        }
    }
}
