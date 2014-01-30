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
using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

#endregion Using namespaces

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Helper class.
    /// </summary>
    public class GeneralHelper
    {
        /// <summary>
        /// Collection of Phone Codes
        /// </summary>
        public static List<PhoneCodePair> CountryPhoneCode = new List<PhoneCodePair> 
        {
            new PhoneCodePair { PhoneCode = "93",   CountryCode = "AF" },
            new PhoneCodePair { PhoneCode = "355",  CountryCode = "AL" },
            new PhoneCodePair { PhoneCode = "213",  CountryCode = "DZ" },
            new PhoneCodePair { PhoneCode = "1684", CountryCode = "AS" },
            new PhoneCodePair { PhoneCode = "376",  CountryCode = "AD" },
            new PhoneCodePair { PhoneCode = "244",  CountryCode = "AO" },
            new PhoneCodePair { PhoneCode = "1672", CountryCode = "AQ" },
            new PhoneCodePair { PhoneCode = "1672", CountryCode = "AQ" },
            new PhoneCodePair { PhoneCode = "1268", CountryCode = "AG" },
            new PhoneCodePair { PhoneCode = "54",   CountryCode = "AR" },
            new PhoneCodePair { PhoneCode = "374",  CountryCode = "AM" },
            new PhoneCodePair { PhoneCode = "297",  CountryCode = "AW" },
            new PhoneCodePair { PhoneCode = "61",   CountryCode = "AU" },
            new PhoneCodePair { PhoneCode = "43",   CountryCode = "AT" },
            new PhoneCodePair { PhoneCode = "994",  CountryCode = "AZ" },
            new PhoneCodePair { PhoneCode = "1242", CountryCode = "BS" },
            new PhoneCodePair { PhoneCode = "973",  CountryCode = "BH" },
            new PhoneCodePair { PhoneCode = "880",  CountryCode = "BD" },
            new PhoneCodePair { PhoneCode = "1246", CountryCode = "BB" },
            new PhoneCodePair { PhoneCode = "375",  CountryCode = "BY" },
            new PhoneCodePair { PhoneCode = "32",   CountryCode = "BE" },
            new PhoneCodePair { PhoneCode = "501",  CountryCode = "BZ" },
            new PhoneCodePair { PhoneCode = "229",  CountryCode = "BJ" },
            new PhoneCodePair { PhoneCode = "1441", CountryCode = "BM" },
            new PhoneCodePair { PhoneCode = "975",  CountryCode = "BT" },
            new PhoneCodePair { PhoneCode = "591",  CountryCode = "BO" },
            new PhoneCodePair { PhoneCode = "387",  CountryCode = "BA" },
            new PhoneCodePair { PhoneCode = "267",  CountryCode = "BW" },
            new PhoneCodePair { PhoneCode = "55",   CountryCode = "BR" },
            new PhoneCodePair { PhoneCode = "1284", CountryCode = "VG" },
            new PhoneCodePair { PhoneCode = "673",  CountryCode = "BN" },
            new PhoneCodePair { PhoneCode = "359",  CountryCode = "BG" },
            new PhoneCodePair { PhoneCode = "226",  CountryCode = "BF" },
            new PhoneCodePair { PhoneCode = "95",   CountryCode = "MM" },
            new PhoneCodePair { PhoneCode = "257",  CountryCode = "BI" },
            new PhoneCodePair { PhoneCode = "855",  CountryCode = "KH" },
            new PhoneCodePair { PhoneCode = "237",  CountryCode = "CM" },
            new PhoneCodePair { PhoneCode = "1",    CountryCode = "CA" },
            new PhoneCodePair { PhoneCode = "1345", CountryCode = "KY" },
            new PhoneCodePair { PhoneCode = "235",  CountryCode = "TD" },
            new PhoneCodePair { PhoneCode = "56",   CountryCode = "CL" },
            new PhoneCodePair { PhoneCode = "86",   CountryCode = "CN" },
            new PhoneCodePair { PhoneCode = "57",   CountryCode = "CO" },
            new PhoneCodePair { PhoneCode = "242",  CountryCode = "CG" },
            new PhoneCodePair { PhoneCode = "682",  CountryCode = "CK" },
            new PhoneCodePair { PhoneCode = "506",  CountryCode = "CR" },
            new PhoneCodePair { PhoneCode = "385",  CountryCode = "HR" },
            new PhoneCodePair { PhoneCode = "53",   CountryCode = "CU" },
            new PhoneCodePair { PhoneCode = "357",  CountryCode = "CY" },
            new PhoneCodePair { PhoneCode = "420",  CountryCode = "CZ" },
            new PhoneCodePair { PhoneCode = "45",   CountryCode = "DK" },
            new PhoneCodePair { PhoneCode = "1767", CountryCode = "DM" },
            new PhoneCodePair { PhoneCode = "1809", CountryCode = "DO" },
            new PhoneCodePair { PhoneCode = "593",  CountryCode = "EC" },
            new PhoneCodePair { PhoneCode = "20",   CountryCode = "EG" },
            new PhoneCodePair { PhoneCode = "503",  CountryCode = "SV" },
            new PhoneCodePair { PhoneCode = "240",  CountryCode = "GQ" },
            new PhoneCodePair { PhoneCode = "372",  CountryCode = "EE" },
            new PhoneCodePair { PhoneCode = "251",  CountryCode = "ET" },
            new PhoneCodePair { PhoneCode = "298",  CountryCode = "FO" },
            new PhoneCodePair { PhoneCode = "679",  CountryCode = "FJ" },
            new PhoneCodePair { PhoneCode = "358",  CountryCode = "FI" },
            new PhoneCodePair { PhoneCode = "33",   CountryCode = "FR" },
            new PhoneCodePair { PhoneCode = "596",  CountryCode = "MQ" },
            new PhoneCodePair { PhoneCode = "594",  CountryCode = "GF" },
            new PhoneCodePair { PhoneCode = "689",  CountryCode = "PF" },
            new PhoneCodePair { PhoneCode = "241",  CountryCode = "GA" },
            new PhoneCodePair { PhoneCode = "220",  CountryCode = "GM" },
            new PhoneCodePair { PhoneCode = "995",  CountryCode = "GE" },
            new PhoneCodePair { PhoneCode = "49",   CountryCode = "DE" },
            new PhoneCodePair { PhoneCode = "233",  CountryCode = "GH" },
            new PhoneCodePair { PhoneCode = "350",  CountryCode = "GI" },
            new PhoneCodePair { PhoneCode = "30",   CountryCode = "GR" },
            new PhoneCodePair { PhoneCode = "299",  CountryCode = "GL" },
            new PhoneCodePair { PhoneCode = "1473", CountryCode = "GD" },
            new PhoneCodePair { PhoneCode = "590",  CountryCode = "GP" },
            new PhoneCodePair { PhoneCode = "1671", CountryCode = "GU" },
            new PhoneCodePair { PhoneCode = "502",  CountryCode = "GT" },
            new PhoneCodePair { PhoneCode = "224",  CountryCode = "GN" },
            new PhoneCodePair { PhoneCode = "245",  CountryCode = "GW" },
            new PhoneCodePair { PhoneCode = "592",  CountryCode = "GY" },
            new PhoneCodePair { PhoneCode = "509",  CountryCode = "HT" },
            new PhoneCodePair { PhoneCode = "504",  CountryCode = "HN" },
            new PhoneCodePair { PhoneCode = "852",  CountryCode = "HK" },
            new PhoneCodePair { PhoneCode = "36",   CountryCode = "HU" },
            new PhoneCodePair { PhoneCode = "354",  CountryCode = "IS" },
            new PhoneCodePair { PhoneCode = "91",   CountryCode = "IN" },
            new PhoneCodePair { PhoneCode = "62",   CountryCode = "ID" },
            new PhoneCodePair { PhoneCode = "964",  CountryCode = "IQ" },
            new PhoneCodePair { PhoneCode = "353",  CountryCode = "IE" },
            new PhoneCodePair { PhoneCode = "972",  CountryCode = "IL" },
            new PhoneCodePair { PhoneCode = "39",   CountryCode = "IT" },
            new PhoneCodePair { PhoneCode = "1876", CountryCode = "JM" },
            new PhoneCodePair { PhoneCode = "81",   CountryCode = "JP" },
            new PhoneCodePair { PhoneCode = "962",  CountryCode = "JO" },
            new PhoneCodePair { PhoneCode = "7",    CountryCode = "KZ" },
            new PhoneCodePair { PhoneCode = "254",  CountryCode = "KE" },
            new PhoneCodePair { PhoneCode = "965",  CountryCode = "KW" },
            new PhoneCodePair { PhoneCode = "996",  CountryCode = "KG" },
            new PhoneCodePair { PhoneCode = "371",  CountryCode = "LV" },
            new PhoneCodePair { PhoneCode = "961",  CountryCode = "LB" },
            new PhoneCodePair { PhoneCode = "266",  CountryCode = "LS" },
            new PhoneCodePair { PhoneCode = "423",  CountryCode = "LI" },
            new PhoneCodePair { PhoneCode = "370",  CountryCode = "LT" },
            new PhoneCodePair { PhoneCode = "352",  CountryCode = "LU" },
            new PhoneCodePair { PhoneCode = "853",  CountryCode = "MO" },
            new PhoneCodePair { PhoneCode = "389",  CountryCode = "MK" },
            new PhoneCodePair { PhoneCode = "261",  CountryCode = "MG" },
            new PhoneCodePair { PhoneCode = "265",  CountryCode = "MW" },
            new PhoneCodePair { PhoneCode = "60",   CountryCode = "MY" },
            new PhoneCodePair { PhoneCode = "960",  CountryCode = "MV" },
            new PhoneCodePair { PhoneCode = "223",  CountryCode = "ML" },
            new PhoneCodePair { PhoneCode = "356",  CountryCode = "MT" },
            new PhoneCodePair { PhoneCode = "222",  CountryCode = "MR" },
            new PhoneCodePair { PhoneCode = "230",  CountryCode = "MU" },
            new PhoneCodePair { PhoneCode = "52",   CountryCode = "MX" },
            new PhoneCodePair { PhoneCode = "52",   CountryCode = "MX" },
            new PhoneCodePair { PhoneCode = "373",  CountryCode = "MD" },
            new PhoneCodePair { PhoneCode = "377",  CountryCode = "MC" },
            new PhoneCodePair { PhoneCode = "976",  CountryCode = "MN" },
            new PhoneCodePair { PhoneCode = "212",  CountryCode = "MA" },
            new PhoneCodePair { PhoneCode = "258",  CountryCode = "MZ" },
            new PhoneCodePair { PhoneCode = "95",   CountryCode = "MM" },
            new PhoneCodePair { PhoneCode = "264",  CountryCode = "NA" },
            new PhoneCodePair { PhoneCode = "674",  CountryCode = "NR" },
            new PhoneCodePair { PhoneCode = "977",  CountryCode = "NP" },
            new PhoneCodePair { PhoneCode = "31",   CountryCode = "NL" },
            new PhoneCodePair { PhoneCode = "599",  CountryCode = "AN" },
            new PhoneCodePair { PhoneCode = "687",  CountryCode = "NC" },
            new PhoneCodePair { PhoneCode = "64",   CountryCode = "NZ" },
            new PhoneCodePair { PhoneCode = "505",  CountryCode = "NI" },
            new PhoneCodePair { PhoneCode = "227",  CountryCode = "NE" },
            new PhoneCodePair { PhoneCode = "234",  CountryCode = "NG" },
            new PhoneCodePair { PhoneCode = "1670", CountryCode = "MP" },
            new PhoneCodePair { PhoneCode = "47",   CountryCode = "NO" },
            new PhoneCodePair { PhoneCode = "968",  CountryCode = "OM" },
            new PhoneCodePair { PhoneCode = "92",   CountryCode = "PK" },
            new PhoneCodePair { PhoneCode = "680",  CountryCode = "PW" },
            new PhoneCodePair { PhoneCode = "970",  CountryCode = "PS" },
            new PhoneCodePair { PhoneCode = "507",  CountryCode = "PA" },
            new PhoneCodePair { PhoneCode = "675",  CountryCode = "PG" },
            new PhoneCodePair { PhoneCode = "595",  CountryCode = "PY" },
            new PhoneCodePair { PhoneCode = "51",   CountryCode = "PE" },
            new PhoneCodePair { PhoneCode = "63",   CountryCode = "PH" },
            new PhoneCodePair { PhoneCode = "48",   CountryCode = "PL" },
            new PhoneCodePair { PhoneCode = "351",  CountryCode = "PT" },
            new PhoneCodePair { PhoneCode = "1787", CountryCode = "PR" },
            new PhoneCodePair { PhoneCode = "1939", CountryCode = "PR" },
            new PhoneCodePair { PhoneCode = "974",  CountryCode = "QA" },
            new PhoneCodePair { PhoneCode = "40",   CountryCode = "RO" },
            new PhoneCodePair { PhoneCode = "7",    CountryCode = "RU" },
            new PhoneCodePair { PhoneCode = "250",  CountryCode = "RW" },
            new PhoneCodePair { PhoneCode = "378",  CountryCode = "SM" },
            new PhoneCodePair { PhoneCode = "966",  CountryCode = "SA" },
            new PhoneCodePair { PhoneCode = "221",  CountryCode = "SN" },
            new PhoneCodePair { PhoneCode = "381",  CountryCode = "RS" },
            new PhoneCodePair { PhoneCode = "381",  CountryCode = "CS" },
            new PhoneCodePair { PhoneCode = "248",  CountryCode = "SC" },
            new PhoneCodePair { PhoneCode = "232",  CountryCode = "SL" },
            new PhoneCodePair { PhoneCode = "65",   CountryCode = "SG" },
            new PhoneCodePair { PhoneCode = "421",  CountryCode = "SK" },
            new PhoneCodePair { PhoneCode = "386",  CountryCode = "SI" },
            new PhoneCodePair { PhoneCode = "252",  CountryCode = "SO" },
            new PhoneCodePair { PhoneCode = "27",   CountryCode = "ZA" },
            new PhoneCodePair { PhoneCode = "34",   CountryCode = "ES" },
            new PhoneCodePair { PhoneCode = "34",   CountryCode = "IC" },
            new PhoneCodePair { PhoneCode = "94",   CountryCode = "LK" },
            new PhoneCodePair { PhoneCode = "249",  CountryCode = "SD" },
            new PhoneCodePair { PhoneCode = "597",  CountryCode = "SR" },
            new PhoneCodePair { PhoneCode = "268",  CountryCode = "SZ" },
            new PhoneCodePair { PhoneCode = "46",   CountryCode = "SE" },
            new PhoneCodePair { PhoneCode = "41",   CountryCode = "CH" },
            new PhoneCodePair { PhoneCode = "886",  CountryCode = "TW" },
            new PhoneCodePair { PhoneCode = "992",  CountryCode = "TJ" },
            new PhoneCodePair { PhoneCode = "66",   CountryCode = "TH" },
            new PhoneCodePair { PhoneCode = "228",  CountryCode = "TG" },
            new PhoneCodePair { PhoneCode = "1868", CountryCode = "TT" },
            new PhoneCodePair { PhoneCode = "216",  CountryCode = "TN" },
            new PhoneCodePair { PhoneCode = "90",   CountryCode = "TR" },
            new PhoneCodePair { PhoneCode = "993",  CountryCode = "TM" },
            new PhoneCodePair { PhoneCode = "1649", CountryCode = "TC" },
            new PhoneCodePair { PhoneCode = "255",  CountryCode = "TZ" },
            new PhoneCodePair { PhoneCode = "256",  CountryCode = "UG" },
            new PhoneCodePair { PhoneCode = "380",  CountryCode = "UA" },
            new PhoneCodePair { PhoneCode = "971",  CountryCode = "AE" },
            new PhoneCodePair { PhoneCode = "44",   CountryCode = "GB" },
            new PhoneCodePair { PhoneCode = "1",    CountryCode = "US" },
            new PhoneCodePair { PhoneCode = "598",  CountryCode = "UY" },
            new PhoneCodePair { PhoneCode = "1340", CountryCode = "VI" },
            new PhoneCodePair { PhoneCode = "998",  CountryCode = "UZ" },
            new PhoneCodePair { PhoneCode = "678",  CountryCode = "VU" },
            new PhoneCodePair { PhoneCode = "58",   CountryCode = "VE" },
            new PhoneCodePair { PhoneCode = "84",   CountryCode = "VN" },
            new PhoneCodePair { PhoneCode = "681",  CountryCode = "WF" },
            new PhoneCodePair { PhoneCode = "685",  CountryCode = "WS" },
            new PhoneCodePair { PhoneCode = "967",  CountryCode = "YE" },
            new PhoneCodePair { PhoneCode = "260",  CountryCode = "ZM" },
            new PhoneCodePair { PhoneCode = "263",  CountryCode = "ZW" },
            new PhoneCodePair { PhoneCode = "1829", CountryCode = "DO" },
            new PhoneCodePair { PhoneCode = "382",  CountryCode = "ME" }
        };

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
        /// Formats the phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="countryCode">The country code.</param>
        /// <returns>Formated Phone Number.</returns>
        public static string FormatPhoneNumber(string phoneNumber, string countryCode)
        {
            if (String.IsNullOrEmpty(phoneNumber))
            {
                return String.Empty;
            }

            string phoneCode = string.Empty;
            string phoneNum = string.Empty;

            if (!CountryPhoneCode.Any(item => item.CountryCode == countryCode))
            {
                throw new Exception("Could not find the country " + countryCode + " in the list of CountryPhoneCode list.");
            }

            string invdefaultPhoneCode = CountryPhoneCode.FirstOrDefault(c => c.CountryCode == countryCode).PhoneCode;
            phoneNumber = phoneNumber.Replace("-", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Replace("/", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty).Replace(" ", string.Empty);
            if (phoneNumber.Contains('|'))
            {
                string[] invtmpStr = phoneNumber.Split('|');
                phoneNumber = invtmpStr[0];

                phoneNum = phoneNumber.TrimStart('0');
                phoneCode = invdefaultPhoneCode;
            }
            else
            {
                phoneNumber = phoneNumber.TrimStart('0');
                if (!phoneNumber.StartsWith("+"))
                {
                    if (phoneNumber.StartsWith(invdefaultPhoneCode))
                    {
                        phoneCode = invdefaultPhoneCode;
                        phoneNum = phoneNumber.Remove(0, invdefaultPhoneCode.Length);
                    }
                    else
                    {
                        phoneCode = invdefaultPhoneCode;
                        phoneNum = phoneNumber;
                    }
                }else
                {
                    phoneCode = String.Empty;
                    phoneNum = phoneNumber.Remove(0,1);
                }

            }

            return "+" + phoneCode + "." + phoneNum.TrimStart('0');
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
                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = controller.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                    AttributeData[] checkedDomains = service.CheckDomains(domains);

                    for (int i = 0; i < checkedDomains.Length; i++)
                    {
                        if (checkedDomains[i].Value.ToLower() == "taken")
                        {
                            result += checkedDomains[i].Name + "|TAKEN ";
                        }
                        else
                        {
                            bool passed = false;
                            List<string> tldBasedRegexesStrings = DomainSearch.Helpers.DomainSearchHelper.GetTLDBasedRegexes();

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
                
                Guid resellerId = new Guid(OrderModel.FetchResellerGuidFromXml());

                using (Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService.AtomiaBillingPublicService orderService = new AtomiaBillingPublicService())
                {
                    orderService.Url = controller.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                    bool resellerShowTaxBool = orderService.ShowTaxForReseller(resellerId);
                    result = resellerShowTaxBool;
                }


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
            List<RadioRow> packages = OrderModel.FetchPackagesDataFromXml(controller, atomiaBillingPublicService, accountId, resellerId, currencyCode, countryCode);
            if (!string.IsNullOrEmpty(filterValue))
            {
                // get all packages from the config
                    List<ProductItem> packageProducts = HostingProducts.Helpers.ProductsManager.ListProductsFromConfiguration();
                    packages = packages.Where(rr =>
                                                  {
                                                      ProductItem item = packageProducts.FirstOrDefault(p => p.ArticalNumber == rr.productId);
                                                      if (item == null)
                                                      {
                                                          return false;
                                                      }

                                                      object value;
                                                      if (item.AllProperties.TryGetValue("groups", out value))
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
    }
}
