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
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using Atomia.Web.Plugin.PublicOrder.Configurations;

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
            if (HttpContext.Current.Session["resellerAccountData"] != null)
            {
                return ((AccountData)HttpContext.Current.Session["resellerAccountData"]).Id;
            }
            
            throw new ApplicationException("Reseller not found.");
        }

        /// <summary>
        /// Gets the reseller country code.
        /// </summary>
        /// <returns>Reseller's country code.</returns>
        public static string GetResellerCountryCode()
        {
            if (HttpContext.Current.Session["resellerAccountData"] != null)
            {
                return ((AccountData)HttpContext.Current.Session["resellerAccountData"]).DefaultCountry.Code;
            }

            throw new ApplicationException("Reseller not found.");
        }

        /// <summary>
        /// Gets the reseller currency code.
        /// </summary>
        /// <returns>Reseller's currency code.</returns>
        public static string GetResellerCurrencyCode()
        {
            if (HttpContext.Current.Session["resellerAccountData"] != null)
            {
                return ((AccountData)HttpContext.Current.Session["resellerAccountData"]).DefaultCurrencyCode;
            }

            throw new ApplicationException("Reseller not found.");
        }

        /// <summary>
        /// Gets the reseller languages.
        /// </summary>
        /// <returns>List of reseller's languages.</returns>
        public static IList<Language> GetResellerLanguages()
        {
            AccountData resellerAccountData = HttpContext.Current.Session["resellerAccountData"] != null
                                                  ? HttpContext.Current.Session["resellerAccountData"] as AccountData
                                                  : null;

            // If there are defined languages for reseller, use them, if not, fallback to config.
            return resellerAccountData != null && resellerAccountData.Languages != null && resellerAccountData.Languages.Length > 0
                       ? resellerAccountData.Languages.Select(
                           language => new Language { Code = language, IsDefault = language == resellerAccountData.DefaultLanguage }).ToList()
                       : (from Base.Configs.Language languageItem in Base.Configs.AppConfig.Instance.LanguagesList
                          select new Language { Code = languageItem.Name, IsDefault = languageItem.Default }).ToList();
        }

        /// <summary>
        /// Gets the reseller payment methods.
        /// </summary>
        /// <param name="defaultPaymentMethod">The default payment method.</param>
        /// <returns>List of reseller's payment methods.</returns>
        public static IList<PaymentMethod> GetResellerPaymentMethods(out PaymentMethod defaultPaymentMethod)
        {
            AccountData resellerAccountData = HttpContext.Current.Session["resellerAccountData"] != null
                                          ? HttpContext.Current.Session["resellerAccountData"] as AccountData : null;
            if (resellerAccountData != null && resellerAccountData.PaymentMethods != null && resellerAccountData.PaymentMethods.Length > 0)
            {
                IList<PaymentMethod> paymentMethods = resellerAccountData.PaymentMethods.ToList();
                PaymentMethod defaultMethod = resellerAccountData.DefaultPaymentMethod;

                // Add InvoiceByPost and InvoiceByEmail if they are enabled in config but not in reseller's list of payment methods
                PublicOrderConfigurationSection config = LocalConfigurationHelper.GetLocalConfigurationSection();

                if ((bool.Parse(config.InvoiceByPost.Enabled) && !resellerAccountData.PaymentMethods.Any(p => p.GuiPluginName == "InvoiceByPost")) ||
                    (bool.Parse(config.InvoiceByEmail.Enabled) && !resellerAccountData.PaymentMethods.Any(p => p.GuiPluginName == "InvoiceByEmail")))
                {
                    if (bool.Parse(config.InvoiceByPost.Enabled) && !paymentMethods.Any(p => p.GuiPluginName == "InvoiceByPost"))
                    {
                        PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "InvoiceByPost" };
                        paymentMethods.Add(paymentMethod);
                        if (defaultMethod == null && config.InvoiceByPost.Default)
                        {
                            defaultMethod = paymentMethod;
                        }
                    }

                    if (bool.Parse(config.InvoiceByEmail.Enabled) && !paymentMethods.Any(p => p.GuiPluginName == "InvoiceByEmail"))
                    {
                        PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "InvoiceByEmail" };
                        paymentMethods.Add(paymentMethod);
                        if (defaultMethod == null && config.InvoiceByEmail.Default)
                        {
                            defaultMethod = paymentMethod;
                        }
                    }

                    resellerAccountData.DefaultPaymentMethod = defaultMethod;
                    resellerAccountData.PaymentMethods = paymentMethods.ToArray();
                }

                defaultPaymentMethod = defaultMethod;
                return paymentMethods;
            }
            else
            {
                // Load payment methods from config
                IList<PaymentMethod> paymentMethods = new List<PaymentMethod>();
                PaymentMethod defaultMethod = null;

                PublicOrderConfigurationSection config = LocalConfigurationHelper.GetLocalConfigurationSection();
                if (bool.Parse(config.InvoiceByPost.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "InvoiceByPost" };
                    paymentMethods.Add(paymentMethod);
                    if (config.InvoiceByPost.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.InvoiceByEmail.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "InvoiceByEmail" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.InvoiceByEmail.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.OnlinePayment.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "CCPayment" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.OnlinePayment.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.PayPal.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "PayPal" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.PayPal.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.PayexRedirect.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "PayexRedirect" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.PayexRedirect.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.WorldPay.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "WorldPay" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.WorldPay.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.DibsFlexwin.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "DibsFlexwin" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.DibsFlexwin.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.WorldPayXml.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "WorldPayXml" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.WorldPayXml.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                if (bool.Parse(config.AdyenHpp.Enabled))
                {
                    PaymentMethod paymentMethod = new PaymentMethod { GuiPluginName = "AdyenHpp" };
                    paymentMethods.Add(paymentMethod);
                    if (defaultMethod == null && config.AdyenHpp.Default)
                    {
                        defaultMethod = paymentMethod;
                    }
                }

                defaultPaymentMethod = defaultMethod;
                return paymentMethods;
            }
        }

        /// <summary>
        /// Loads the reseller into session by hash.
        /// </summary>
        /// <param name="resellerHash">The reseller hash.</param>
        public static void LoadResellerIntoSessionByHash(string resellerHash)
        {
            // Load reseller's data only if it's not present in session.
            if (HttpContext.Current.Session["resellerAccountData"] == null 
                || ((AccountData)HttpContext.Current.Session["resellerAccountData"]).Hash != resellerHash)
            {
                var service = GeneralHelper.GetPublicOrderService(HttpContext.Current.ApplicationInstance.Context);
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

                    if (HttpContext.Current.Session["showContactOptions"] == null)
                    {
                        HttpContext.Current.Session.Add("showContactOptions", true);
                    }
                    else
                    {
                        HttpContext.Current.Session["showContactOptions"] = true;
                    }

                    if (HttpContext.Current.Session["CurrencyDecimalPlaces"] == null)
                    {
                        HttpContext.Current.Session.Add("CurrencyDecimalPlaces", resellerAccountData.CurrencyDecimalPlaces);
                    }
                    else
                    {
                        HttpContext.Current.Session["CurrencyDecimalPlaces"] = resellerAccountData.CurrencyDecimalPlaces;
                    }
                }
                else
                {
                    throw new ApplicationException(string.Format("Reseller with hash {0} not found.", resellerHash));
                }
            }
        }

        /// <summary>
        /// Loads the reseller into session by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static void LoadResellerIntoSessionByUrl(string url)
        {
            // Load reseller's data only if it's not present in session.
            if (HttpContext.Current.Session["resellerAccountData"] == null)
            {
                AtomiaBillingPublicService service = GeneralHelper.GetPublicOrderService(HttpContext.Current.ApplicationInstance.Context);
                AccountData resellerAccountData = service.GetResellerDataByUrl(url) ?? service.GetDefaultResellerData();
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

                    if (HttpContext.Current.Session["showContactOptions"] == null)
                    {
                        HttpContext.Current.Session.Add("showContactOptions", false);
                    }
                    else
                    {
                        HttpContext.Current.Session["showContactOptions"] = false;
                    }

                    if (HttpContext.Current.Session["CurrencyDecimalPlaces"] == null)
                    {
                        HttpContext.Current.Session.Add("CurrencyDecimalPlaces", resellerAccountData.CurrencyDecimalPlaces);
                    }
                    else
                    {
                        HttpContext.Current.Session["CurrencyDecimalPlaces"] = resellerAccountData.CurrencyDecimalPlaces;
                    }
                }
                else
                {
                    throw new ApplicationException(
                        string.Format("Reseller for url {0} nor default reseller not found.", url));
                }
            }
        }
    }
}