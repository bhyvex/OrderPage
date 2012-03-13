//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Frame.Models;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using Atomia.Web.Plugin.PublicOrder.Configurations;

using Elmah;

namespace Atomia.Web.Frame.Controllers
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Picker for the locale.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Partial view for this action.</returns>
        [AcceptVerbs(System.Net.WebRequestMethods.Http.Get)]
        public ActionResult LocalePicker(string returnUrl)
        {
            LocalePickerFormData localePickerFormData = new LocalePickerFormData
            {
                Countries = this.GetAvailableCountries().ToArray(),
                ReturnUrl = returnUrl
            };

            return this.PartialView("LocalePicker", localePickerFormData);
        }

        /// <summary>
        /// Picker for the locale.
        /// </summary>
        /// <param name="localePickerFormData">The locale picker form data.</param>
        /// <returns>Partial view for this action.</returns>
        [AcceptVerbs(System.Net.WebRequestMethods.Http.Post)]
        public ActionResult LocalePicker(LocalePickerFormData localePickerFormData)
        {
            if (!String.IsNullOrEmpty(localePickerFormData.SelectedCountry))
            {
                try
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();

                    AtomiaCookieCollection cookieCollection;
                    if (this.Request.Cookies["AtomiaCookieCollection"] != null && !String.IsNullOrEmpty(this.Request.Cookies["AtomiaCookieCollection"].Value))
                    {
                        cookieCollection = js.Deserialize<AtomiaCookieCollection>(this.Request.Cookies["AtomiaCookieCollection"].Value);
                    }
                    else
                    {
                        cookieCollection = new AtomiaCookieCollection();
                    }

                    AtomiaCookie aCookie = new AtomiaCookie(
                        this.Request.ApplicationPath,
                        this.Request.Url.Host,
                        localePickerFormData.SelectedCountry,
                        "OrderLocaleCookie");
                                               
                    cookieCollection.AddOrUpdateAtomiaCookie(aCookie);

                    string serializedCookieCollection = js.Serialize(cookieCollection);

                    HttpCookie cookie = new HttpCookie("AtomiaCookieCollection") { Value = serializedCookieCollection, Expires = DateTime.Now.AddYears(1) };

                    if (this.Request.Cookies["AtomiaCookieCollection"] != null && !String.IsNullOrEmpty(this.Request.Cookies["AtomiaCookieCollection"].Value))
                    {
                        this.Response.Cookies["AtomiaCookieCollection"].Value = cookie.Value;
                        this.Response.Cookies["AtomiaCookieCollection"].Expires = cookie.Expires;
                    }
                    else
                    {
                        this.Response.Cookies.Add(cookie);
                    }
                }
                catch (Exception ex)
                {
                    OrderPageLogger.LogOrderPageException(ex);
                    throw;
                }
            }

            this.Response.Redirect(localePickerFormData.ReturnUrl);

            // View does not exist, but redirect wont pass if some View is not called. :/
            return this.View("Index");
        }

        /// <summary>
        /// Picker for the language.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>Partial view for this action.</returns>
        [AcceptVerbs(System.Net.WebRequestMethods.Http.Get)]
        public ActionResult LanguagePicker(string returnUrl)
        {
            LanguagePickerFormData languagePickerFormData = new LanguagePickerFormData
            {
                Languages = this.GetAvailableLanguages().ToArray(),
                ReturnUrl = returnUrl
            };

            return this.PartialView("LanguagePicker", languagePickerFormData);
        }

        /// <summary>
        /// Picker for the language.
        /// </summary>
        /// <param name="languagePickerFormData">The language picker form data.</param>
        /// <returns>Partial view for this action.</returns>
        [AcceptVerbs(System.Net.WebRequestMethods.Http.Post)]
        public ActionResult LanguagePicker(LanguagePickerFormData languagePickerFormData)
        {
            if (!String.IsNullOrEmpty(languagePickerFormData.SelectedLanguage))
            {
                string language = languagePickerFormData.SelectedLanguage.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0];
                string culture = languagePickerFormData.SelectedLanguage.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1];
                AtomiaCultureInfo atomiaLanguages = new AtomiaCultureInfo { Language = language, Culture = culture };
                this.HttpContext.Session["SessionAccountLanguages"] = atomiaLanguages;

                try
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();

                    AtomiaCookieCollection cookieCollection;
                    if (this.Request.Cookies["AtomiaCookieCollection"] != null && !String.IsNullOrEmpty(this.Request.Cookies["AtomiaCookieCollection"].Value))
                    {
                        cookieCollection = js.Deserialize<AtomiaCookieCollection>(this.Request.Cookies["AtomiaCookieCollection"].Value);
                    }
                    else
                    {
                        cookieCollection = new AtomiaCookieCollection();
                    }

                    AtomiaCookie aCookie = new AtomiaCookie(
                        this.Request.ApplicationPath,
                        this.Request.Url.Host,
                        js.Serialize(atomiaLanguages),
                        "OrderLanguageCookie");

                    cookieCollection.AddOrUpdateAtomiaCookie(aCookie);

                    string serializedCookieCollection = js.Serialize(cookieCollection);

                    HttpCookie cookie = new HttpCookie("AtomiaCookieCollection") { Value = serializedCookieCollection, Expires = DateTime.Now.AddYears(1) };

                    if (this.Request.Cookies["AtomiaCookieCollection"] != null && !String.IsNullOrEmpty(this.Request.Cookies["AtomiaCookieCollection"].Value))
                    {
                        this.Response.Cookies["AtomiaCookieCollection"].Value = cookie.Value;
                        this.Response.Cookies["AtomiaCookieCollection"].Expires = cookie.Expires;
                    }
                    else
                    {
                        this.Response.Cookies.Add(cookie);    
                    }

                    try
                    {
                        using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                        {
                            service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                            string orderCurrencyResource = LocalizationHelpers.GlobalResource(String.Format("{0}Common, Currency" + languagePickerFormData.SelectedLanguage, this.HttpContext.Session["Theme"]))
                                                            ?? LocalizationHelpers.GlobalResource(String.Format("{0}Common, Currency", this.HttpContext.Session["Theme"]));
                            this.HttpContext.Session["OrderCurrencyResource"] = orderCurrencyResource;
                            string orderCurrencyCode = "SEK";

                            if (this.HttpContext.Application["CurrencyCode" + languagePickerFormData.SelectedLanguage] != null &&
                                !String.IsNullOrEmpty(
                                    (string)this.HttpContext.Application["CurrencyCode" + languagePickerFormData.SelectedLanguage]))
                            {
                                orderCurrencyCode =
                                    (string)this.HttpContext.Application["CurrencyCode" + languagePickerFormData.SelectedLanguage];
                            }

                            this.HttpContext.Session["OrderCurrencyCode"] = orderCurrencyCode;
                            Plugin.DomainSearch.Helpers.DomainSearchHelper.LoadProductsIntoSession(service, Guid.Empty, orderCurrencyCode, culture);
                        }
                    }
                    catch (Exception ex)
                    {
                        OrderPageLogger.LogOrderPageException(ex);
                    }

                }
                catch (Exception ex)
                {
                    OrderPageLogger.LogOrderPageException(ex);
                    throw;
                }
            }

            this.Response.Redirect(languagePickerFormData.ReturnUrl);

            // View does not exist, but redirect wont pass if some View is not called. :/
            return this.View("Index");
        }

        /// <summary>
        /// Gets the available countries.
        /// </summary>
        /// <returns>List of countries.</returns>
        private List<LocaleModel> GetAvailableCountries()
        {
            List<LocaleModel> result = new List<LocaleModel>();
            string cookieCountryCode = String.Empty;

            try
            {
                // Check if there is locale settings in cookie ad set if there is
                if (this.Request != null && this.Request.Cookies != null && this.Request.Cookies["AtomiaCookieCollection"] != null && !String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Cookies["AtomiaCookieCollection"].Value))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();

                    AtomiaCookieCollection cookieCollection = js.Deserialize<AtomiaCookieCollection>(System.Web.HttpContext.Current.Request.Cookies["AtomiaCookieCollection"].Value);

                    AtomiaCookie localeCookie = cookieCollection.GetAtomiaCookie(
                        "OrderLocaleCookie",
                        System.Web.HttpContext.Current.Request.ApplicationPath,
                        System.Web.HttpContext.Current.Request.Url.Host);

                    if (localeCookie != null && !String.IsNullOrEmpty(localeCookie.CookieValue))
                    {
                        cookieCountryCode = localeCookie.CookieValue;
                    }
                }

                PublicOrderConfigurationSection pocs = Atomia.Web.Plugin.PublicOrder.Helpers.LocalConfigurationHelper.GetLocalConfigurationSection();
                foreach (CountryItem country in pocs.CountriesList)
                {
                    string image = String.Empty;
                    if (!String.IsNullOrEmpty(country.Image))
                    {
                        image = country.Image;
                    }

                    result.Add(new LocaleModel { Code = country.Code, Image = image, IsDefault = String.IsNullOrEmpty(cookieCountryCode) ? country.Default : country.Code == cookieCountryCode });
                } 
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
                throw;
            }
            
            return result;
        }

        /// <summary>
        /// Gets the available languages.
        /// </summary>
        /// <returns>List of languages.</returns>
        private List<LanguageModel> GetAvailableLanguages()
        {
            string[] defaultLanguage = { "en", "US" };
            foreach (Atomia.Web.Base.Configs.Language configLanguage in Atomia.Web.Base.Configs.AppConfig.Instance.LanguagesList)
            {
                if (configLanguage.Default)
                {
                    string[] extractedLanguage = configLanguage.Name.Split('-');
                    if (extractedLanguage.Length > 2)
                    {
                        defaultLanguage[0] = extractedLanguage[0] + '-' + extractedLanguage[1];
                        defaultLanguage[1] = extractedLanguage[2];
                    }
                    else
                    {
                        defaultLanguage = extractedLanguage;
                    }

                    break;
                }
            }

            try
            {
                if (this.HttpContext.Session != null && this.HttpContext.Session["SessionAccountLanguages"] != null)
                {
                    AtomiaCultureInfo accountLanguages = this.HttpContext.Session["SessionAccountLanguages"] as AtomiaCultureInfo;
                    if (accountLanguages != null)
                    {
                        defaultLanguage[0] = accountLanguages.Language;
                        defaultLanguage[1] = accountLanguages.Culture;
                    }
                }
            }
            catch (Exception e)
            {
                OrderPageLogger.LogOrderPageException(e);
            }

            List<LanguageModel> languages = new List<LanguageModel>();

            foreach (Base.Configs.Language languageItem in Base.Configs.AppConfig.Instance.LanguagesList)
            {
                LanguageModel languageModel = new LanguageModel { Code = languageItem.Name, Name = languageItem.Name.Replace("-", String.Empty) };
                if (defaultLanguage.Length == 2)
                {
                    if (languageItem.Name == defaultLanguage[0] + '-' + defaultLanguage[1])
                    {
                        languageModel.IsDefault = true;
                    }
                }
                else if (defaultLanguage.Length == 1)
                {
                    if (languageItem.Name == defaultLanguage[0])
                    {
                        languageModel.IsDefault = true;
                    }
                }

                languages.Add(languageModel);
            }

            return languages;
        }
    }
}
