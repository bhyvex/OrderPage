//-----------------------------------------------------------------------
// <copyright file="GeneralHelper.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

#region Using namespaces
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

using Atomia.Web.Base.Configs;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Plugin.Cart.Models;
using Atomia.Web.Plugin.HostingProducts.Models;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using Atomia.Web.Plugin.ProductsProvider;
using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

using Microsoft.Practices.Unity;

#endregion Using namespaces

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Helper class.
    /// </summary>
    public class GeneralHelper
    {
        /// <summary>
        /// Prepares for submit.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>Trimmed string</returns>
        public static string PrepareForSubmit(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return String.Empty;
            }

            if (String.Compare(str, "null") == 0 || String.Compare(str, "NULL") == 0 || String.Compare(str, "Null") == 0)
            {
                return String.Empty;
            }

            return str.Trim();
        }

        /// <summary>
        /// Gets the unavailable domains.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="domains">The domains.</param>
        /// <returns>Array of unavailable domains.</returns>
        public static string GetUnavailableDomainsHelper(Controller controller, string[] domains)
        {
            string result = string.Empty;
            try
            {
                var service = GetPublicOrderService(controller.HttpContext.ApplicationInstance.Context);
                AttributeData[] checkedDomains = service.CheckDomains(domains);

                List<string> tldBasedRegexesStrings = DomainSearch.Helpers.DomainSearchHelper.GetTLDBasedRegexes(ResellerHelper.GetResellerId());

                for (int i = 0; i < checkedDomains.Length; i++)
                {
                    if (checkedDomains[i].Value.ToLower() == "taken")
                    {
                        result += checkedDomains[i].Name + "|TAKEN ";
                    }
                    else
                    {
                        bool passed = false;
                        // if there are no tld-special regexes disregard and continue
                        if (tldBasedRegexesStrings.Count > 0)
                        {
                            List<Regex> tldBasedRegexes = new List<Regex>();
                            for (int j = 0; j < tldBasedRegexesStrings.Count; j++)
                            {
                                tldBasedRegexes.Add(new Regex(tldBasedRegexesStrings[j]));
                            }

                            string tmpStr = SimpleDnsPlus.IDNLib.Decode(checkedDomains[i].Name.Trim());
                            for (var g = 0; g < tldBasedRegexes.Count; g++)
                            {
                                if (tldBasedRegexes[g].IsMatch(tmpStr))
                                {
                                    passed = true;
                                }
                            }

                            if (!passed)
                            {
                                result += checkedDomains[i].Name + "|SPECIAL ";
                            }
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
                throw;
            }

            return result.TrimEnd(' ');
        }

        public static object FetchSetting(string settingName, string codeBase, bool returnValueOnly)
        {
            PluginSetting pluginSettingsInstance = null;

            string currentDir = Directory.GetCurrentDirectory();

            try
            {
                Configuration config = PluginLoaderHelper.GetConfigFile(codeBase, PluginConfigurationHelper.GetConfigFilePath(codeBase));

                AppConfig appConfig = AppConfig.GetAppConfigSection(config);

                bool found = false;

                foreach (PluginSettingsPlugin plugin in appConfig.PluginSettingsList)
                {
                    foreach (PluginSetting pluginSetting in plugin)
                    {
                        if (pluginSetting.Name.ToUpper() == settingName.ToUpper())
                        {
                            found = true;

                            if (!String.IsNullOrEmpty(pluginSetting.Theme))
                            {
                                if (pluginSetting.Theme.ToLower() !=
                                    HttpContext.Current.Session["Theme"].ToString().ToLower())
                                {
                                    found = false;
                                }
                            }

                            if (found)
                            {
                                pluginSettingsInstance = pluginSetting;
                                break;
                            }
                        }
                    }

                    if (found)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                OrderPageLogger.LogOrderPageException(e);
            }

            Directory.SetCurrentDirectory(currentDir);

            if (pluginSettingsInstance != null && returnValueOnly)
            {
                return pluginSettingsInstance.Value.Trim();
            }

            return pluginSettingsInstance;
        }

        /// <summary>
        /// </summary>
        /// <param name="pluginParameterName">
        /// The plugin parameter name.
        /// </param>
        /// <returns>
        /// </returns>
        public static string FetchPluginParameterFromConfig(string pluginParameterName)
        {
            return FetchSetting(pluginParameterName, Assembly.GetExecutingAssembly().CodeBase, true).ToString();
        }

        /// <summary>
        /// For currently logged account, get parent - that is resseller and returns property ShowTax
        /// </summary>
        /// <returns></returns>
        public static bool TaxAreShownForReseller(Controller controller)
        {
            bool result = true;
            try
            {
                AtomiaBillingPublicService service = GetPublicOrderService(HttpContext.Current.ApplicationInstance.Context);
                result = service.ShowTaxForReseller(ResellerHelper.GetResellerId());
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
            }

            return result;
        }

        /// <summary>
        /// Filters the packages.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="atomiaBillingPublicService">The atomia billing public service.</param>
        /// <param name="accountId">The account id.</param>
        /// <param name="resellerId">The reseller id.</param>
        /// <param name="currencyCode">The currency code.</param>
        /// <param name="countryCode">The country code.</param>
        /// <param name="filterValue">The filter value.</param>
        /// <returns>Filtered list of packages.</returns>
        public static List<RadioRow> FilterPackages(Controller controller, AtomiaBillingPublicService atomiaBillingPublicService, Guid accountId, Guid resellerId, string currencyCode, string countryCode, string filterValue)
        {
            List<RadioRow> packages = OrderModel.FetchPackagesData(controller, resellerId, null, accountId, currencyCode, countryCode).ToList();
            if (!string.IsNullOrEmpty(filterValue))
            {
                // get all packages from the config
                IList<Product> packageProducts = GetProductProvider().GetShopProducts(resellerId, null, accountId, countryCode);
                packages = packages.Where(rr =>
                                              {
                                                  Product item = packageProducts.FirstOrDefault(p => p.ArticleNumber == rr.productId);
                                                  if (item == null)
                                                  {
                                                      return false;
                                                  }

                                                  string value;
                                                  if (item.Properties.TryGetValue("groups", out value))
                                                  {
                                                      return value
                                                          .ToString()
                                                          .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                                          .ToList()
                                                          .Exists(v => v == filterValue);
                                                  }

                                                  return false;
                                              }).ToList();
            }

            return packages;
        }

        public static AtomiaBillingPublicService GetPublicOrderService(HttpContext context)
        {
            if (context.Session != null)
            {
                if (context.Session["PublicOrderService"] != null)
                {
                    return (AtomiaBillingPublicService)context.Session["PublicOrderService"];
                }
            }

            context.Session["PublicOrderService"] = new AtomiaBillingPublicService
                       {
                           Url =
                               context.Application["OrderApplicationPublicServiceURL"].ToString(),
                           Timeout =
                               Int32.Parse(
                                   context.Application["OrderApplicationPublicServiceTimeout"].
                                       ToString())
                       };

            return (AtomiaBillingPublicService) context.Session["PublicOrderService"];

        }

        /// <summary>
        /// Gets the product provider.
        /// </summary>
        /// <returns>Instance of product provider.</returns>
        public static IProductsProvider GetProductProvider()
        {
            return ClassContainer.Instance.UnityContainer.Resolve<IProductsProvider>();
        }
    }
}
