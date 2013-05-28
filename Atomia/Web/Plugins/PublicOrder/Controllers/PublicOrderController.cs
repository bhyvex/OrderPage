    //-----------------------------------------------------------------------
// <copyright file="PublicOrderController.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

#region Using namespaces
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Atomia.Common;
using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Base.Validation;
using Atomia.Web.Plugin.Cart.Helpers;
using Atomia.Web.Plugin.Cart.Models;
using Atomia.Web.Plugin.DomainSearch.Helpers;
using Atomia.Web.Plugin.DomainSearch.Models;
using Atomia.Web.Plugin.HostingProducts.Models;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using Atomia.Web.Plugin.PublicOrder.Configurations;
using Atomia.Web.Plugin.PublicOrder.Filters;
using Atomia.Web.Plugin.PublicOrder.Helpers;
using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;
using Atomia.Web.Plugin.PublicOrder.Models;

using DomainDataFromXML = Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml;
using System.Globalization;

#endregion Using namespaces

namespace Atomia.Web.Plugin.PublicOrder.Controllers
{
    /// <summary>
    /// Order Controller class
    /// </summary>
    [LocalizationDataLoader]
    [Internationalization(InterfaceImplementer = true)]
    [CompressResponse]
    [TranslationHelper]
    public class PublicOrderController : Controller
    {
        /// <summary>
        /// Renders action for default.
        /// </summary>
        /// <param name="package">The product group.</param>
        /// <param name="lang">The language.</param>
        /// <param name="sel">Represents pre-selected hosting package.</param>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Index(string package, string lang, string sel)
        {
            if (this.RouteData.Values.ContainsKey("resellerHash"))
            {
                ResellerHelper.LoadResellerIntoSessionByHash((string)this.RouteData.Values["resellerHash"]);
            }
            else
            {
                ResellerHelper.LoadResellerIntoSessionByUrl(this.Request.Url.AbsoluteUri);
            }

            ViewData["WasAnError"] = 0;
            if (Session["WasAnErrorFromQuery"] != null)
            {
                ViewData["WasAnError"] = (int)Session["WasAnErrorFromQuery"];
            }

            ViewData["RegDomainFront"] = RegularExpression.GetRegularExpression("DomainFront");
            ViewData["RegDomain"] = RegularExpression.GetRegularExpression("Domain");

            int allowedDomainLength;
            int numberOfDomainsAllowed;
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != String.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != String.Empty)
            {
                allowedDomainLength = Int32.Parse((string)this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = Int32.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
            }
            else
            {
                throw new ConfigurationErrorsException("Missing AllowedDomainLength or NumberOfDomainsAllowed in configuration");
            }

            // if productGroup set, check which order options are allowed for it
            bool groupExists;
            ViewData["OrderOptions"] = this.GetOrderOptionsForProductGroup(package, out groupExists);

            // add package to session so that packages can be filtered
            Session["FilterByPackage"] = null;
            Session["PreselectedPackage"] = null;
            if (!string.IsNullOrEmpty(sel))
            {
                Session["PreselectedPackage"] = sel;
            }
            
            if (!string.IsNullOrEmpty(package) && groupExists)
            {
                Session["FilterByPackage"] = package;
            }

            if (((List<OrderOptions>)ViewData["OrderOptions"]).Count == 0)
            {
                Session["firstOption"] = false;
                Session["domains"] = null;
                Session["singleDomain"] = null;
                Session["subdomain"] = false;

                // if no order options redirect to second page
                return RedirectToAction("Select", new { controller = "PublicOrder", area = "PublicOrder" });
            }

            ViewData["AllowedDomainLength"] = allowedDomainLength;
            ViewData["NumberOfDomainsAllowed"] = numberOfDomainsAllowed;

            string tldBasedRegexesSplited = string.Empty;
            List<string> tldBasedRegexes;

            try
            {
                tldBasedRegexes = DomainSearchHelper.GetTLDBasedRegexes();
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
                throw;
            }

            for (int i = 0; i < tldBasedRegexes.Count; i++)
            {
                if (i < tldBasedRegexes.Count - 1)
                {
                    tldBasedRegexesSplited += tldBasedRegexes[i] + " ";
                }
                else
                {
                    tldBasedRegexesSplited += tldBasedRegexes[i];
                }
            }

            ViewData["RegDomainTLDBased"] = tldBasedRegexesSplited;
            ViewData["AddingSubdomain"] = false;
            Session["subdomain"] = null;

            Session["SpecialPID"] = Request.QueryString["PID"];

            // if url includes domain go straight to select page
            string queryStringDomain = Request.QueryString["domain"];
            if (!String.IsNullOrEmpty(queryStringDomain))
            {
                return RedirectToAction("Domain", new { domain = queryStringDomain });
            }
            
            return View();
        }

        /// <summary>
        /// Loads the products into session.
        /// </summary>
        /// <returns>Json object.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoadProductsIntoSession()
        {
            // call any function using HostingProducts to initialize loading
            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                string countryCode = ResellerHelper.GetResellerCountryCode();
                string currencyCode = ResellerHelper.GetResellerCurrencyCode();

                DomainSearchHelper.LoadProductsIntoSession(service, Guid.Empty, currencyCode, countryCode);
            }

            return Json(null);
        }

        /// <summary>
        /// Index page called with Post
        /// </summary>
        /// <param name="IndexForm"></param>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Post)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Index(IndexForm IndexForm)
        {
            ViewData["WasAnError"] = 0;
            try
            {
                var errors = DataAnnotationsValidationRunner.GetErrors(IndexForm);
                if (errors.Any())
                {
                    ViewData["WasAnError"] = 1;
                    throw new AtomiaServerSideValidationException(errors);
                }

                string[] viewDataDomains;
                if (IndexForm.Selected == "first" && !string.IsNullOrEmpty(IndexForm.Domains))
                {
                    List<DomainDataFromXML> domainData;

                    // Get ViewData
                    IndexHelper.FirstOptionSelected(this, out viewDataDomains, out domainData, IndexForm.Domains.Trim());

                    Session["firstOption"] = true;
                    Session["subdomain"] = false;
                    Session["domains"] = viewDataDomains;
                    Session["multiDomains"] = domainData;
                }
                else if (IndexForm.Selected == "second" && !string.IsNullOrEmpty(IndexForm.Domain))
                {
                    DomainDataFromXML domainData;

                    // Get ViewData
                    IndexHelper.SecondOptionSelected(out viewDataDomains, out domainData, IndexForm.Domain.Trim(new[] { '.', ' ' }).Trim());

                    Session["firstOption"] = false;
                    Session["subdomain"] = false;
                    Session["domains"] = viewDataDomains;
                    Session["singleDomain"] = domainData;
                }
                else if (IndexForm.Selected == "subdomain" && !string.IsNullOrEmpty(IndexForm.SubDomain))
                {
                    //// should this be the same as 'second' option ?
                    string mainDomain = (string)this.HttpContext.Application["AllowAddSubdomain"];
                    if (string.IsNullOrEmpty(mainDomain))
                    {
                        new AtomiaServerSideValidationException("MainDomain", "Adding subdomains is not allowed. Reason: main domain not set.");
                    }

                    DomainDataFromXML domainData;

                    // Get ViewData
                    IndexHelper.SecondOptionSelected(out viewDataDomains, out domainData, IndexForm.SubDomain.Trim(new[] { '.', ' ' }).Trim() + "." + mainDomain);

                    Session["firstOption"] = false;
                    Session["domains"] = viewDataDomains;
                    Session["singleDomain"] = domainData;
                    Session["subdomain"] = true;
                }
                else
                {
                    return this.View();
                }
            }
            catch (AtomiaServerSideValidationException ex)
            {
                ViewData["WasAnError"] = 1;

                ex.AddModelStateErrors(ModelState, string.Empty);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Select", new { controller = "PublicOrder", area = "PublicOrder" });
            }

            // error
            this.ViewData["WasAnError"] = 1;
            this.ViewData["RegDomainFront"] = RegularExpression.GetRegularExpression("DomainFront");
            this.ViewData["RegDomain"] = RegularExpression.GetRegularExpression("Domain");

            int allowedDomainLength;
            int numberOfDomainsAllowed;
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != String.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != String.Empty)
            {
                allowedDomainLength = Int32.Parse((string)this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = Int32.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
            }
            else
            {
                throw new ConfigurationErrorsException("Missing AllowedDomainLength or NumberOfDomainsAllowed in configuration");
            }

            ViewData["AllowedDomainLength"] = allowedDomainLength;
            ViewData["NumberOfDomainsAllowed"] = numberOfDomainsAllowed;

            string tldBasedRegexesSplited = string.Empty;
            List<string> tldBasedRegexes;
            try
            {
                tldBasedRegexes = DomainSearchHelper.GetTLDBasedRegexes();
            }
            catch (ConfigurationErrorsException ex)
            {
               OrderPageLogger.LogOrderPageException(ex);
                throw;
            }

            for (int i = 0; i < tldBasedRegexes.Count; i++)
            {
                if (i < tldBasedRegexes.Count - 1)
                {
                    tldBasedRegexesSplited += tldBasedRegexes[i] + " ";
                }
                else
                {
                    tldBasedRegexesSplited += tldBasedRegexes[i];
                }
            }

            this.ViewData["RegDomainTLDBased"] = tldBasedRegexesSplited;

            return this.View();
        }

        /// <summary>
        /// Action that redirects to select page with domain for search avoiding Index page 
        /// </summary>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Domain()
        {
            string domain = this.RouteData.Values["domain"].ToString();

            string[] viewData_domains;
            List<DomainDataFromXML> domainData;

            // Validate domains because they come from URL
            string[] domainsSubmitted = domain.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            Regex regexFront = new Regex(RegularExpression.GetRegularExpression("DomainFront"));
            Regex regexFull = new Regex(RegularExpression.GetRegularExpression("Domain"));

            int allowedDomainLength;
            int numberOfDomainsAllowed;
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != String.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != String.Empty)
            {
                allowedDomainLength = Int32.Parse((string)this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = Int32.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
            }
            else
            {
                throw new ConfigurationErrorsException("Missing AllowedDomainLength or NumberOfDomainsAllowed in configuration");
            }

            bool wasAnError = false;
            for (int i = 0; i < domainsSubmitted.Length; i++)
            {
                if (domainsSubmitted.Length > numberOfDomainsAllowed)
                {
                    // allowed number of domains searched exceeded
                    Session["WasAnErrorFromQuery"] = 2;
                    wasAnError = true;
                }

                if (!regexFront.IsMatch(domainsSubmitted[i]) && !regexFull.IsMatch(domainsSubmitted[i]))
                {
                    // domains contain characters that are not allowed
                    Session["WasAnErrorFromQuery"] = 3;
                    wasAnError = true;
                }

                if (domainsSubmitted[i].Length > allowedDomainLength)
                {
                    // allowed length of domain name (number of characters) exeeded
                    Session["WasAnErrorFromQuery"] = 3;
                    wasAnError = true;
                }

                if (wasAnError)
                {
                    return RedirectToAction("Index");
                }
            }

            // Get ViewData
            IndexHelper.FirstOptionSelected(this, out viewData_domains, out domainData, domain.Trim());

            Session["domains"] = viewData_domains.ToArray();
            Session["firstOption"] = true;
            Session["multiDomains"] = domainData;

            return RedirectToAction("Select");
        }

        /// <summary>
        /// Renders Select page 
        /// </summary>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Select()
        {
            if (Session == null || Session["firstOption"] == null)
            {
                return RedirectToAction("Index");
            }

            // supported countries
            string countryCode = ResellerHelper.GetResellerCountryCode();

            // Check if there is locale setting for country in cookie ad set if there is)
            if (System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"] != null && !String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"].Value))
            {
                countryCode = System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"].Value.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[1];
            }

            ViewData["defaultCountry"] = countryCode;
            string currencyCode = ResellerHelper.GetResellerCurrencyCode();

            // Show or hide Personal number field
            bool showPersonalNumber = true;
            string showPersonalNumberSetting = GeneralHelper.FetchPluginParameterFromConfig("ShowPersonalNumber");

            if (!string.IsNullOrEmpty(showPersonalNumberSetting))
            {
                bool parsedValue;
                if (bool.TryParse(showPersonalNumberSetting, out parsedValue))
                {
                    showPersonalNumber = parsedValue;
                }
            }

            ViewData["ShowPersonalNumber"] = showPersonalNumber;

            // This is supose to be set in InternationalizationProvider
            System.Globalization.CultureInfo locale = Thread.CurrentThread.CurrentCulture;

            ViewData["decimalSeparator"] = locale.NumberFormat.NumberDecimalSeparator;
            ViewData["groupSeparator"] = locale.NumberFormat.NumberGroupSeparator;

            List<Country> countryList = new List<Country>();

            try
            {
                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                    countryList = service.GetCountries().ToList();
                }
            }
            catch (Exception ex)
            {
               OrderPageLogger.LogOrderPageException(ex);
            }

            List<string> europeanCountriesList = CountriesHelper.GetEUCountryCodes(countryList);
            string europeanCountries = europeanCountriesList.Aggregate(string.Empty, (current, country) => current + String.Format("{0} ", country));
            
            ViewData["EUCountries"] = europeanCountries.Trim();
            ViewData["CountryList"] = CountriesHelper.GetSupportedCountriesSelectList(countryList);

            // enabled payment method?
            PublicOrderConfigurationSection opcs = Helpers.LocalConfigurationHelper.GetLocalConfigurationSection();

            bool paymentEnabled = Boolean.Parse(opcs.OnlinePayment.Enabled);
            bool orderByPostEnabled = Boolean.Parse(opcs.InvoiceByPost.Enabled);
            bool orderByEmailEnabled = Boolean.Parse(opcs.InvoiceByEmail.Enabled);
            bool payPalEnabled = Boolean.Parse(opcs.PayPal.Enabled);
            bool payExRedirectEnabled = Boolean.Parse(opcs.PayexRedirect.Enabled);
            bool worldPayRedirectEnabled = Boolean.Parse(opcs.WorldPay.Enabled);
            bool dibsFlexwinEnabled = Boolean.Parse(opcs.DibsFlexwin.Enabled);
            bool worldPayXmlRedirectEnabled = Boolean.Parse(opcs.WorldPayXml.Enabled);
            bool adyenHppEnabled = Boolean.Parse(opcs.AdyenHpp.Enabled);

            ViewData["PaymentEnabled"] = paymentEnabled;
            ViewData["PayPalEnabled"] = payPalEnabled;
            ViewData["OrderByPostEnabled"] = orderByPostEnabled;
            ViewData["OrderByEmailEnabled"] = orderByEmailEnabled;
            ViewData["PayexRedirectEnabled"] = payExRedirectEnabled;
            ViewData["WorldPayRedirectEnabled"] = worldPayRedirectEnabled;
            ViewData["DibsFlexwinEnabled"] = dibsFlexwinEnabled;
            ViewData["WorldPayXmlRedirectEnabled"] = worldPayXmlRedirectEnabled;
            ViewData["AdyenHppEnabled"] = adyenHppEnabled;

            ViewData["firstOption"] = (bool)Session["firstOption"];

            ViewData["AddingSubdomain"] = false;
            if (this.Session["subdomain"] != null)
            {
                ViewData["AddingSubdomain"] = (bool)this.Session["subdomain"];
            }

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                ViewData["OrderByPostId"] = orderByPostEnabled ? OrderModel.FetchPostOrderIdFromXml(service, Guid.Empty, null, null) : string.Empty;

                // enabled payment method end
                ViewData["WasAnError"] = 0;

                string filterValue = Session["FilterByPackage"] != null ? (string)Session["FilterByPackage"] : null;
                ViewData["radioList"] = GeneralHelper.FilterPackages(this, service, Guid.Empty, currencyCode, countryCode, filterValue);
            }

            this.ViewData["OwnDomain"] = string.Empty;

            if (!((bool)this.Session["firstOption"]))
            {
                DomainDataFromXML singleDomain = (DomainDataFromXML)this.Session["singleDomain"];
                if (singleDomain != null)
                {
                    this.ViewData["Domain"] = singleDomain;
                    this.ViewData["OwnDomain"] = singleDomain.ProductName;
                }
            }

            // default values
            SubmitForm submitForm = new SubmitForm
            {
                RadioYouAre = "private",
                InvoiceRadioYouAre = "private",
                CountryCode = countryCode,
                InvoiceCountryCode = countryCode
            };

            if (this.Session["resellerAccountData"] != null)
            {
                submitForm.RadioBillingContact = "reseller";
                submitForm.RadioTechContact = "reseller";
            }

            string defaultPaymentPlugin = string.Empty;
            if (orderByEmailEnabled && opcs.InvoiceByEmail.Default)
            {
                submitForm.RadioPaymentMethod = "email";
                defaultPaymentPlugin = "PayWithInvoice";
            }
            else if (orderByPostEnabled && opcs.InvoiceByPost.Default)
            {
                submitForm.RadioPaymentMethod = "post";
                defaultPaymentPlugin = "PayWithInvoice";
            }
            else if ((paymentEnabled && opcs.OnlinePayment.Default) || (payExRedirectEnabled && opcs.PayexRedirect.Default)
                || (worldPayRedirectEnabled && opcs.WorldPay.Default) || (dibsFlexwinEnabled && opcs.DibsFlexwin.Default)
                || (worldPayXmlRedirectEnabled && opcs.WorldPayXml.Default) || (adyenHppEnabled && opcs.AdyenHpp.Default))
            {
                submitForm.RadioPaymentMethod = "card";
                if (payExRedirectEnabled && opcs.PayexRedirect.Default)
                {
                    defaultPaymentPlugin = "PayexRedirect";
                }
                else if (worldPayRedirectEnabled && opcs.WorldPay.Default)
                {
                    defaultPaymentPlugin = "WorldPayRedirect";
                }
                else if (worldPayXmlRedirectEnabled && opcs.WorldPayXml.Default)
                {
                    defaultPaymentPlugin = "WorldPayXmlRedirect";
                }
                else
                {
                    defaultPaymentPlugin = "CCPayment";
                }
            }
            else if (payPalEnabled && opcs.PayPal.Default)
            {
                submitForm.RadioPaymentMethod = "paypal";
                defaultPaymentPlugin = "PayPal";
            }

            this.ControllerContext.HttpContext.Application["DefaultPaymentPlugin"] = defaultPaymentPlugin;
            ViewData["RegDomainFront"] = RegularExpression.GetRegularExpression("DomainFront");
            ViewData["RegDomain"] = RegularExpression.GetRegularExpression("Domain");

            int allowedDomainLength;
            int numberOfDomainsAllowed;
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != String.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != String.Empty)
            {
                allowedDomainLength = Int32.Parse((string)this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = Int32.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
            }
            else
            {
                throw new ConfigurationErrorsException("Missing AllowedDomainLength or NumberOfDomainsAllowed in configuration");
            }

            ViewData["AllowedDomainLength"] = allowedDomainLength;
            ViewData["NumberOfDomainsAllowed"] = numberOfDomainsAllowed;

            Session["dontShowTaxesForThisResellerHidden"] = (!GeneralHelper.TaxAreShownForReseller(this)).ToString().ToLower();

            return View(submitForm);
        }

        /// <summary>
        /// Renders select action recieving POST method call
        /// </summary>
        /// <param name="SubmitForm"></param>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Post)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Select(SubmitForm SubmitForm)
        {
            List<Country> countryList = new List<Country>();

            bool paymentMethodCc = false; // finish payment method CC
            ViewData["WasAnError"] = 0;

            // enabled payment method?
            PublicOrderConfigurationSection opcs = Helpers.LocalConfigurationHelper.GetLocalConfigurationSection();

            bool paymentEnabled = Boolean.Parse(opcs.OnlinePayment.Enabled);
            bool orderByPostEnabled = Boolean.Parse(opcs.InvoiceByPost.Enabled);
            bool orderByEmailEnabled = Boolean.Parse(opcs.InvoiceByEmail.Enabled);
            bool payPalEnabled = Boolean.Parse(opcs.PayPal.Enabled);
            bool payExRedirectEnabled = Boolean.Parse(opcs.PayexRedirect.Enabled);
            bool worldPayRedirectEnabled = Boolean.Parse(opcs.WorldPay.Enabled);
            bool dibsFlexwinEnabled = Boolean.Parse(opcs.DibsFlexwin.Enabled);
            bool worldPayXmlRedirectEnabled = Boolean.Parse(opcs.WorldPayXml.Enabled);
            bool adyenHppEnabled = Boolean.Parse(opcs.WorldPayXml.Enabled);
            ViewData["PayexRedirectEnabled"] = payExRedirectEnabled;
            ViewData["WorldPayRedirectEnabled"] = worldPayRedirectEnabled;
            ViewData["DibsFlexwinEnabled"] = dibsFlexwinEnabled;
            ViewData["WorldPayXmlRedirectEnabled"] = worldPayXmlRedirectEnabled;
            ViewData["AdyenHppEnabled"] = adyenHppEnabled;

            string orderByPostId = string.Empty;
            List<string> currentArrayOfProducts;
            List<RadioRow> list;
            List<ProductDescription> currentCart;

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                
                ViewData["PaymentEnabled"] = paymentEnabled;

                ViewData["PayPalEnabled"] = payPalEnabled;

                if (orderByPostEnabled)
                {
                    orderByPostId = OrderModel.FetchPostOrderIdFromXml(service, Guid.Empty, null, null);
                    ViewData["OrderByPostId"] = orderByPostId;
                    ViewData["OrderByPostEnabled"] = true;
                }
                else
                {
                    ViewData["OrderByPostId"] = String.Empty;
                    ViewData["OrderByPostEnabled"] = false;
                }

                if (orderByEmailEnabled)
                {
                    ViewData["OrderByEmailEnabled"] = true;
                }
                else
                {
                    ViewData["OrderByEmailEnabled"] = false;
                }

                currentArrayOfProducts = SubmitForm.ArrayOfProducts.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                list = OrderModel.FetchPackagesDataFromXml(this, service, Guid.Empty, null, null);

                currentCart = new List<ProductDescription>();

                for (int i = 0; i < currentArrayOfProducts.Count; i += 3)
                {
                    currentCart.Add(
                        new ProductDescription
                        {
                            productID = currentArrayOfProducts[i], 
                            productDesc = currentArrayOfProducts[i + 1],
                            RenewalPeriodId = currentArrayOfProducts[i + 2]
                        });
                }

                // this includes own and sub domain
                if (!string.IsNullOrEmpty(SubmitForm.OwnDomain))
                {
                    currentCart.Add(new ProductDescription { productID = OrderModel.FetchOwnDomainIdFromXml(service, Guid.Empty, null, null), productDesc = SubmitForm.OwnDomain });
                }
            }

            try
            {
                var errors = DataAnnotationsValidationRunner.GetErrors(SubmitForm);
                if (errors.Any())
                {
                    ViewData["WasAnError"] = 1;

                    throw new AtomiaServerSideValidationException(errors);
                }

                if (SubmitForm.SecondAddress)
                {
                    if (string.IsNullOrEmpty(SubmitForm.InvoiceContactName))
                    {
                        throw new AtomiaServerSideValidationException("errorName", this.GlobalResource("ValidationErrors, ErrorEmptyField"), SubmitForm);
                    }

                    if (string.IsNullOrEmpty(SubmitForm.InvoiceAddress))
                    {
                        throw new AtomiaServerSideValidationException("errorAddress", this.GlobalResource("ValidationErrors, ErrorEmptyField"), SubmitForm);
                    }

                    if (string.IsNullOrEmpty(SubmitForm.InvoicePostNumber))
                    {
                        throw new AtomiaServerSideValidationException("errorPostNumber", this.GlobalResource("ValidationErrors, ErrorEmptyField"), SubmitForm);
                    }

                    if (string.IsNullOrEmpty(SubmitForm.InvoiceCity))
                    {
                        throw new AtomiaServerSideValidationException("errorCity", this.GlobalResource("ValidationErrors, ErrorEmptyField"), SubmitForm);
                    }

                    if (string.IsNullOrEmpty(SubmitForm.InvoiceTelephone))
                    {
                        throw new AtomiaServerSideValidationException("errorTelephone", this.GlobalResource("ValidationErrors, ErrorEmptyField"), SubmitForm);
                    }

                    if (string.IsNullOrEmpty(SubmitForm.InvoiceEmail))
                    {
                        throw new AtomiaServerSideValidationException("errorEmail", this.GlobalResource("ValidationErrors, ErrorEmptyField"), SubmitForm);
                    }
                }
            }
            catch (AtomiaServerSideValidationException ex)
            {
                ViewData["WasAnError"] = 1;

                ex.AddModelStateErrors(ModelState, String.Empty);
            }

            if (ModelState.IsValid)
            {
                // call Billing to Submit form
                try
                {
                    OrderServiceReferences.AtomiaBillingPublicService.PublicOrder newOrder;

                    using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                    {
                        service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                        OrderServiceReferences.AtomiaBillingPublicService.PublicOrder myOrder = new OrderServiceReferences.AtomiaBillingPublicService.PublicOrder();

                        countryList = service.GetCountries().ToList();

                        List<PublicOrderCustomData> orderCustomData = new List<PublicOrderCustomData>();

                        myOrder.Address = GeneralHelper.PrepareForSubmit(SubmitForm.Address);
                        myOrder.Address2 = GeneralHelper.PrepareForSubmit(SubmitForm.Address2);

                        if (SubmitForm.SecondAddress)
                        {
                            myOrder.BillingFirstName = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceContactName);
                            myOrder.BillingLastName = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceContactLastName);
                            myOrder.BillingCompany = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceCompany);

                            myOrder.BillingAddress = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceAddress);
                            myOrder.BillingAddress2 = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceAddress2);
                            myOrder.BillingCity = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceCity);
                            myOrder.BillingCountry = GeneralHelper.PrepareForSubmit(String.IsNullOrEmpty(SubmitForm.InvoiceCountryCode) ? SubmitForm.CountryCode : SubmitForm.InvoiceCountryCode);
                            if (SubmitForm.InvoicePostNumber != null)
                            {
                                myOrder.BillingZip = GeneralHelper.PrepareForSubmit(SubmitForm.InvoicePostNumber.Replace(" ", String.Empty));
                            }

                            myOrder.BillingEmail = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceEmail);

                            myOrder.BillingPhone = GeneralHelper.PrepareForSubmit(GeneralHelper.FormatPhoneNumber(SubmitForm.InvoiceTelephoneProcessed, SubmitForm.InvoiceCountryCode));
                            myOrder.BillingFax = GeneralHelper.PrepareForSubmit(GeneralHelper.FormatPhoneNumber(SubmitForm.InvoiceFaxProcessed, SubmitForm.InvoiceCountryCode));
                            myOrder.BillingMobile = GeneralHelper.PrepareForSubmit(GeneralHelper.FormatPhoneNumber(SubmitForm.InvoiceMobileProcessed, SubmitForm.InvoiceCountryCode));
                        }

                        myOrder.City = GeneralHelper.PrepareForSubmit(SubmitForm.City);
                        myOrder.Company = GeneralHelper.PrepareForSubmit(SubmitForm.Company);
                        if (!String.IsNullOrEmpty(SubmitForm.OrgNumber))
                        {
                            string tmpString = GeneralHelper.PrepareForSubmit(SubmitForm.OrgNumber);
                            myOrder.CompanyNumber = tmpString;
                        }

                        myOrder.Country = GeneralHelper.PrepareForSubmit(SubmitForm.CountryCode);

                        myOrder.Currency = "SEK";
                        if (this.Session["OrderCurrencyCode"] != null && !String.IsNullOrEmpty((string)this.Session["OrderCurrencyCode"]))
                        {
                            myOrder.Currency = this.Session["OrderCurrencyCode"].ToString();
                        }

                        myOrder.Email = GeneralHelper.PrepareForSubmit(SubmitForm.Email);

                        myOrder.Fax = GeneralHelper.PrepareForSubmit(GeneralHelper.FormatPhoneNumber(SubmitForm.FaxProcessed, SubmitForm.CountryCode));
                        myOrder.Mobile = GeneralHelper.PrepareForSubmit(GeneralHelper.FormatPhoneNumber(SubmitForm.MobileProcessed, SubmitForm.CountryCode));

                        myOrder.FirstName = GeneralHelper.PrepareForSubmit(SubmitForm.ContactName);
                        myOrder.LastName = GeneralHelper.PrepareForSubmit(SubmitForm.ContactLastName);
                        myOrder.LegalNumber = GeneralHelper.PrepareForSubmit(SubmitForm.VATNumber);

                        List<string> allPackagesIds = OrderModel.FetchAllPackagesIdsDataFromXml(service, Guid.Empty, null, null);
                        List<ProductItem> products = HostingProducts.Helpers.ProductsManager.ListProductsFromConfiguration();
                        ProductDescription selectedPackage = currentCart.Find(p => allPackagesIds.Any(x => x == p.productID));
                        List<ProductItem> freePackageId = OrderModel.FetchFreePackageIdFromXml(service, Guid.Empty, null, null);
                        IList<string> setupFeeIds = OrderModel.FetchSetupFeeIdsFromXml(service, Guid.Empty, null, null);

                        List<PublicOrderItem> myOrderItems = new List<PublicOrderItem>();

                        foreach (ProductDescription tmpProduct in currentCart)
                        {
                            // If post invoice is selected do not add it to orderItems since that product is added via orderCustomAttributes
                            if (SubmitForm.RadioPaymentMethod == "post")
                            {
                                if (tmpProduct.productID == orderByPostId)
                                {
                                    continue;
                                }
                            }

                            // Get renewal period id for current cart product
                            Guid renewalPeriodId = Guid.Empty;
                            if (!string.IsNullOrEmpty(tmpProduct.RenewalPeriodId))
                            {
                                try
                                {
                                    renewalPeriodId = new Guid(tmpProduct.RenewalPeriodId);
                                }
                                catch (Exception)
                                {
                                }
                            }

                            if (setupFeeIds.Any(id => id == tmpProduct.productID))
                            {
                                PublicOrderItem tmpItem = new PublicOrderItem
                                {
                                    ItemId = Guid.Empty,
                                    ItemNumber = tmpProduct.productID,
                                    RenewalPeriodId = renewalPeriodId,
                                    Quantity = 1
                                };

                                myOrderItems.Add(tmpItem);
                            }
                            else if (tmpProduct.productID != selectedPackage.productID)
                            {
                                // it's domain
                                List<PublicOrderItemProperty> arrayOfCustoms = new List<PublicOrderItemProperty>();
                                if (freePackageId.Exists(f => f.ArticalNumber == selectedPackage.productID))
                                {
                                    // if the selected package in the cart is free
                                    string domainValue = SubmitForm.FirstOption ? tmpProduct.productDesc : (SubmitForm.OwnDomain.StartsWith("www") ? SubmitForm.OwnDomain.Remove(0, 4) : SubmitForm.OwnDomain);

                                    arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainName", Value = domainValue });

                                    arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = "CsDomainParking" });
                                    ProductItem package = products.FirstOrDefault(p => p.ArticalNumber == selectedPackage.productID);
                                    if (SubmitForm.FirstOption == false)
                                    {
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "MainDomain", Value = "true" });
                                        if (package != null && package.AllProperties != null && package.AllProperties.ContainsKey("atomiaserviceextraproperties"))
                                        {
                                            arrayOfCustoms.Add(new PublicOrderItemProperty
                                            {
                                                Name = "AtomiaServiceExtraProperties",
                                                Value = package.AllProperties["atomiaserviceextraproperties"].
                                                    ToString()
                                            });
                                        }
                                    }

                                    PublicOrderItem tmpItem = new PublicOrderItem
                                    {
                                        ItemId = Guid.Empty,
                                        ItemNumber = tmpProduct.productID,
                                        Quantity = 1,
                                        RenewalPeriodId = renewalPeriodId,
                                        CustomData = arrayOfCustoms.ToArray()
                                    };

                                    myOrderItems.Add(tmpItem);
                                }
                                else
                                {
                                    bool websitesAllowed = true;
                                    ProductItem package = products.FirstOrDefault(p => p.ArticalNumber == selectedPackage.productID);
                                    if (package != null && package.AllProperties != null && package.AllProperties.ContainsKey("nowebsites") && package.AllProperties["nowebsites"].ToString().ToLowerInvariant() == "true")
                                    {
                                        websitesAllowed = false;
                                    }

                                    

                                    // if the selected package in the cart is not free)
                                    if (SubmitForm.FirstOption)
                                    {
                                        string websiteType = websitesAllowed
                                                                 ? (tmpProduct.productDesc == SubmitForm.MainDomainSelect
                                                                        ? "CsLinuxWebsite"
                                                                        : "CsDomainParking")
                                                                 : "CsDomainParking";
                                        
                                        // check if the current product is main domain
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainName", Value = tmpProduct.productDesc });
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = websiteType });
                                        if (tmpProduct.productDesc == SubmitForm.MainDomainSelect)
                                        {
                                            arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "MainDomain", Value = "true" });
                                            if (package != null && package.AllProperties != null && package.AllProperties.ContainsKey("atomiaserviceextraproperties"))
                                            {
                                                arrayOfCustoms.Add(new PublicOrderItemProperty
                                                {
                                                    Name = "AtomiaServiceExtraProperties",
                                                    Value = package.AllProperties["atomiaserviceextraproperties"].
                                                        ToString()
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string websiteType = websitesAllowed ? "CsLinuxWebsite" : "CsDomainParking";
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "MainDomain", Value = "true" });
                                        if (package != null && package.AllProperties != null && package.AllProperties.ContainsKey("atomiaserviceextraproperties"))
                                        {
                                            arrayOfCustoms.Add(new PublicOrderItemProperty
                                            {
                                                Name = "AtomiaServiceExtraProperties",
                                                Value = package.AllProperties["atomiaserviceextraproperties"].
                                                    ToString()
                                            });
                                        }
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainName", Value = SubmitForm.OwnDomain.StartsWith("www") ? SubmitForm.OwnDomain.Remove(0, 4) : SubmitForm.OwnDomain });
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = websiteType });
                                    }

                                    PublicOrderItem tmpItem = new PublicOrderItem
                                    {
                                        ItemId = Guid.Empty,
                                        ItemNumber = tmpProduct.productID,
                                        Quantity = 1,
                                        RenewalPeriodId = renewalPeriodId,
                                        CustomData = arrayOfCustoms.ToArray()
                                    };

                                    myOrderItems.Add(tmpItem);
                                }
                            }
                            else
                            {
                                // it's package
                                if (freePackageId.Exists(f => f.ArticalNumber == selectedPackage.productID))
                                {
                                    // if it's free package, just add it
                                    PublicOrderItem tmpItem = new PublicOrderItem
                                                                  {
                                                                      ItemId = Guid.Empty, 
                                                                      ItemNumber = tmpProduct.productID, 
                                                                      RenewalPeriodId = renewalPeriodId,
                                                                      Quantity = 1
                                                                  };

                                    myOrderItems.Add(tmpItem);
                                }
                                else
                                {
                                    // if it's not a free package 
                                    PublicOrderItem tmpItem = new PublicOrderItem
                                    {
                                        ItemId = Guid.Empty,
                                        ItemNumber = tmpProduct.productID,
                                        RenewalPeriodId = renewalPeriodId,
                                        Quantity = 1
                                    };

                                    myOrderItems.Add(tmpItem);
                                }
                            }
                        }
                        
                        foreach (PublicOrderItem myOrderItem in myOrderItems)
                        {
                            if(myOrderItem.ItemNumber == "DOM_NO")
                            {
                                if(SubmitForm.DomainSpeciffic == null)
                                {
                                    throw new Exception("Order could not be created. DomainRegistrySpecificAttributes missing for .NO domain");
                                }
                                List<PublicOrderItemProperty> arrayOfCustoms  = myOrderItem.CustomData.ToList();
                                arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainRegistrySpecificAttributes", Value = SubmitForm.DomainSpeciffic });
                                myOrderItem.CustomData = arrayOfCustoms.ToArray();
                            }
                        }

                        if (SubmitForm.RadioPaymentMethod == "email")
                        {
                            orderCustomData.Add(new PublicOrderCustomData { Name = "PayByInvoice", Value = "true" });
                        }

                        if (SubmitForm.RadioPaymentMethod == "post")
                        {
                            orderCustomData.Add(new PublicOrderCustomData { Name = "SendInvoiceByPost", Value = "true" });
                            orderCustomData.Add(new PublicOrderCustomData { Name = "PayByInvoice", Value = "true" });
                        }

                        if (!String.IsNullOrEmpty((string)Session["SpecialPID"]))
                        {
                            orderCustomData.Add(new PublicOrderCustomData { Name = "SpecialPID", Value = (string)Session["SpecialPID"] });
                        }

                        if (!String.IsNullOrEmpty(SubmitForm.VATValidationMessage))
                        {
                            orderCustomData.Add(new PublicOrderCustomData { Name = "VATValidationMessage", Value = SubmitForm.VATValidationMessage });
                        }

                        if (!string.IsNullOrEmpty(SubmitForm.RadioBillingContact))
                        {
                            orderCustomData.Add(new PublicOrderCustomData { Name = "BillingContact", Value = SubmitForm.RadioBillingContact });
                        }

                        if (!string.IsNullOrEmpty(SubmitForm.RadioTechContact))
                        {
                            orderCustomData.Add(new PublicOrderCustomData { Name = "TechContact", Value = SubmitForm.RadioTechContact });
                        }

                        // Add CustommData posted with submit, client added
                        if (!String.IsNullOrEmpty(SubmitForm.OrderCustomData))
                        {
                            try
                            {
                                JavaScriptSerializer js = new JavaScriptSerializer();
                                orderCustomData.AddRange(js.Deserialize<PublicOrderCustomData[]>(SubmitForm.OrderCustomData));
                            }
                            catch (Exception ex)
                            {
                               OrderPageLogger.LogOrderPageException(ex);
                                throw;
                            }
                        }

                        // Add IP address of the customer as the custom order attribute
                        string ip = this.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                        if (!string.IsNullOrEmpty(ip))
                        {
                            string[] ipRange = ip.Split(',');
                            string trueIp = ipRange[0];

                            orderCustomData.Add(new PublicOrderCustomData { Name = "IpAddress", Value = trueIp });
                        }
                        else
                        {
                            ip = this.Request.ServerVariables["REMOTE_ADDR"];
                            orderCustomData.Add(new PublicOrderCustomData { Name = "IpAddress", Value = ip });
                        }

                        if (this.HttpContext.Session["SessionAccountLanguages"] != null)
                        {
                            AtomiaCultureInfo atomiaCultureInfo = (AtomiaCultureInfo)this.HttpContext.Session["SessionAccountLanguages"];
                            orderCustomData.Add(new PublicOrderCustomData { Name = "Language", Value = String.Format("{0}-{1}", atomiaCultureInfo.Language, atomiaCultureInfo.Culture) });
                        }

                        myOrder.CustomData = orderCustomData.ToArray();
                        myOrder.OrderItems = myOrderItems.ToArray();

                        myOrder.ResellerId = ResellerHelper.GetResellerId();

                        myOrder.Phone = GeneralHelper.PrepareForSubmit(GeneralHelper.FormatPhoneNumber(SubmitForm.TelephoneProcessed, SubmitForm.CountryCode));

                        myOrder.Zip = GeneralHelper.PrepareForSubmit(SubmitForm.PostNumber.Replace(" ", String.Empty));

                        if (!SubmitForm.SecondAddress)
                        {
                            myOrder.BillingFirstName = GeneralHelper.PrepareForSubmit(SubmitForm.ContactName);
                            myOrder.BillingLastName = GeneralHelper.PrepareForSubmit(SubmitForm.ContactLastName);
                            myOrder.BillingCompany = GeneralHelper.PrepareForSubmit(SubmitForm.Company);
                            myOrder.BillingAddress = GeneralHelper.PrepareForSubmit(SubmitForm.Address);
                            myOrder.BillingAddress2 = GeneralHelper.PrepareForSubmit(SubmitForm.Address2);
                            myOrder.BillingCity = GeneralHelper.PrepareForSubmit(SubmitForm.City);
                            myOrder.BillingCountry = GeneralHelper.PrepareForSubmit(SubmitForm.CountryCode);
                            myOrder.BillingZip = GeneralHelper.PrepareForSubmit(SubmitForm.PostNumber.Replace(" ", String.Empty));
                            myOrder.BillingEmail = GeneralHelper.PrepareForSubmit(SubmitForm.Email);
                            myOrder.BillingPhone = myOrder.Phone;
                            myOrder.BillingFax = GeneralHelper.PrepareForSubmit(SubmitForm.Fax);
                            myOrder.BillingMobile = GeneralHelper.PrepareForSubmit(SubmitForm.Mobile);
                        }

                        myOrder.PaymentMethod = SubmitForm.RadioPaymentMethod == "card"
                                                    ? OrderServiceReferences.AtomiaBillingPublicService.PaymentMethodEnum.PayByCard
                                                    : OrderServiceReferences.AtomiaBillingPublicService.PaymentMethodEnum.PayByInvoice;

                        newOrder = service.CreateOrder(myOrder);

                        if (newOrder == null)
                        {
                            throw new Exception("Order could not be created.");
                        }

                        this.Session["CreatedOrder"] = newOrder;
                    }

                    if (SubmitForm.RadioPaymentMethod == "card" || SubmitForm.RadioPaymentMethod == "paypal")
                    {
                        paymentMethodCc = (SubmitForm.RadioPaymentMethod == "card");

                        string result = this.CreatePaymentTransaction(this, newOrder, newOrder.Total, SubmitForm.RadioPaymentMethod);

                        if (!String.IsNullOrEmpty(result))
                        {
                            return Redirect(result);
                        }
                    }

                    return this.RedirectToAction("Thankyou");
                }
                catch (Exception ex)
                {
                   OrderPageLogger.LogOrderPageException(ex);
                }
            }

            if ((bool)Session["firstOption"])
            {
                ViewData["multiDomains"] = this.Session["multiDomains"];
                ViewData["firstOption"] = (bool)Session["firstOption"];
                ViewData["domainsForCheck"] = SubmitForm.SearchDomains;
                ViewData["OwnDomain"] = string.Empty;
            }
            else
            {
                DomainDataFromXML singleDomain = (DomainDataFromXML)Session["singleDomain"];
                ViewData["OwnDomain"] = string.Empty;
                ViewData["firstOption"] = (bool)Session["firstOption"];
                if (singleDomain != null)
                {
                    ViewData["Domain"] = singleDomain;
                    ViewData["OwnDomain"] = singleDomain.ProductName;
                }
            }

            string cartProducts = string.Empty;
            for (int i = 0; i < currentCart.Count; i++)
            {
                bool package = list.Where(listItem => listItem.productId == currentCart[i].productID).Count() > 0;

                if (i < currentCart.Count - 1)
                {
                    cartProducts += currentCart[i].productID + "|" + currentCart[i].productDesc + "|" + currentCart[i].RenewalPeriodId + "|" + package + "|";
                }
                else
                {
                    cartProducts += currentCart[i].productID + "|" + currentCart[i].productDesc + "|" + currentCart[i].RenewalPeriodId + "|" + package;
                }
            }

            // Needed for Cart partial
            ViewData["CartProducts"] = cartProducts;

            ViewData["radioList"] = list;

            // Needed for Cart partial
            string switchedId = list[0].productId + "|" + list[0].productNameDesc + "|" + list[0].RenewalPeriodId;
            if (list[0].SetupFee != null)
            {
                switchedId += "|" + list[0].SetupFee.productID + "|" + list[0].SetupFee.productDesc + "|" +
                              list[0].SetupFee.RenewalPeriodId;
            }

            ViewData["switchedId"] = switchedId;

            if (paymentMethodCc)
            {
                // only if CC payment failed
                ViewData["WasAnError"] = 2;
            }
            else
            {
                ViewData["WasAnError"] = 1;
            }

            // supported countries
            try
            {
                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                    countryList = service.GetCountries().ToList();
                }
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
            }

            ViewData["CountryList"] = CountriesHelper.GetSupportedCountriesSelectList(countryList);
            List<string> euCountries = CountriesHelper.GetEUCountryCodes(countryList);
            string euString = string.Empty;
            for (int i = 0; i < euCountries.Count; i++)
            {
                if (i < euCountries.Count - 1)
                {
                    euString += euCountries[i] + ' ';
                }
                else
                {
                    euString += euCountries[i];
                }
            }

            ViewData["EUCountries"] = euString;

            ViewData["RegDomainFront"] = RegularExpression.GetRegularExpression("DomainFront");
            ViewData["RegDomain"] = RegularExpression.GetRegularExpression("Domain");

            int allowedDomainLength;
            int numberOfDomainsAllowed;
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != String.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != String.Empty)
            {
                allowedDomainLength = Int32.Parse((string)this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = Int32.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
            }
            else
            {
                throw new ConfigurationErrorsException("Missing AllowedDomainLength or NumberOfDomainsAllowed in configuration");
            }

            ViewData["AllowedDomainLength"] = allowedDomainLength;
            ViewData["NumberOfDomainsAllowed"] = numberOfDomainsAllowed;

            string countryCode = ResellerHelper.GetResellerCountryCode();

            // Check if there is locale setting for country in cookie ad set if there is
            if (System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"] != null && !String.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"].Value))
            {
                countryCode = System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"].Value.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[1];
            }

            ViewData["defaultCountry"] = countryCode;

            // This is supose to be set in InternationalizationProvider
            System.Globalization.CultureInfo locale = Thread.CurrentThread.CurrentCulture;

            ViewData["decimalSeparator"] = locale.NumberFormat.NumberDecimalSeparator;
            ViewData["groupSeparator"] = locale.NumberFormat.NumberGroupSeparator;

            // Show or hide Personal number field
            bool showPersonalNumber = true;
            string showPersonalNumberSetting = GeneralHelper.FetchPluginParameterFromConfig("ShowPersonalNumber");

            if (!string.IsNullOrEmpty(showPersonalNumberSetting))
            {
                bool parsedValue;
                if (bool.TryParse(showPersonalNumberSetting, out parsedValue))
                {
                    showPersonalNumber = parsedValue;
                }
            }

            ViewData["ShowPersonalNumber"] = showPersonalNumber;

            return View(SubmitForm);
        }

        /// <summary>
        /// Action for getting personal data from personal number
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <param name="companyType">Type of the company.</param>
        /// <returns>The json for this action.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetAddressInfo(string identifier, string companyType)
        {
            return Json(null);
        }

        /// <summary>
        /// Thankyou page
        /// </summary>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute(ClearSession = true)]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Thankyou()
        {
            if (this.Session["CreatedOrder"] != null)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                
                ViewData["CreatedOrderRaw"] = this.Session["CreatedOrder"];
                ViewData["CreatedOrderAsJson"] = js.Serialize(this.Session["CreatedOrder"]);

                this.Session["CreatedOrder"] = null;
            }
            
            return View();
        }

        /// <summary>
        /// Action for payment failed info
        /// </summary>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult PaymentFailed()
        {
            return View();
        }

        /// <summary>
        /// Action called for generating tos pages, if internal
        /// </summary>
        /// <param name="rName">Name of the r.</param>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Tos(string rName)
        {
            ViewData["TOSResource"] = rName;

            return View();
        }

        /// <summary>
        /// Checks if customer allready exists
        /// </summary>
        /// <returns>The json for this action.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckEmail()
        {
            var email = Request["email"];
            string id = string.Empty;
            bool emailExists;

            try
            {
                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                    emailExists = service.CheckEmail(id, email);
                }
            }
            catch (Exception ex)
            {
               OrderPageLogger.LogOrderPageException(ex);
                throw;
            }

            return Json(emailExists);
        }
        
        /// <summary>
        /// Checks if email domain is valid (against IDN validation).
        /// </summary>
        /// <returns>The json for this action.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckEmailDomain()
        {
            var email = Request["email"];
            string id = string.Empty;
            bool emailDomainValid = false;

            try
            {
               string emailDomain = email.Split('@')[1];
               SimpleDnsPlus.IDNLib.Encode(emailDomain);
               emailDomainValid = true;
            }
            catch (Exception)
            {
                emailDomainValid = false;
            }

            return Json(emailDomainValid);
        }

        /// <summary>
        /// Gets string containing concatenated unavailable domains separated by space
        /// </summary>
        /// <param name="domains">The domains.</param>
        /// <returns>The json for this action.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetUnavailableDomains(string[] domains)
        {
            return Json(GeneralHelper.GetUnavailableDomainsHelper(this, domains));
        }

        /// <summary>
        /// Action to which payExAsync handler returns payment result
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <param name="transactionReference">The transaction reference.</param>
        /// <param name="transactionReferenceType">Type of the transaction reference.</param>
        /// <param name="status">The status.</param>
        /// <returns>Redirects to thankyou page.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Payment(string amount, string transactionReference, int transactionReferenceType, string status)
        {
            decimal decimalAmount;
            Decimal.TryParse(amount, out decimalAmount);

            if (status.ToUpper() == "OK" || status.ToUpper() == "IN_PROGRESS")
            {
                return RedirectToAction("Thankyou");
            }

            if (status.ToUpper() == "FAILED")
            {
                return RedirectToAction("PaymentFailed");
            }

            return View("Index");
        }

        /// <summary>
        /// Validates the domain.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns>The Json with true/false if the domainName is succesfully validated.</returns>
        public ActionResult ValidateDomain(string domainName)
        {
            if (domainName.Contains("http://") || domainName.Contains("https://") || domainName.Contains("www."))
            {
                return Json(false);
            }

            bool validated;

            try
            {
                string finalDomainName = SimpleDnsPlus.IDNLib.Encode(domainName);
                validated = Regex.IsMatch(finalDomainName, RegularExpression.GetRegularExpression("EncodedDomain"));
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(new Exception("IDNLIB could not encode the following domain: " + domainName));
                OrderPageLogger.LogOrderPageException(ex);
                validated = false;
            }

            return Json(validated);
        }

        /// <summary>
        /// Checks the domain for existance in the system. 
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns>The Json with true if the domainName is found in the system, else false.</returns>
        public ActionResult DomainExistsInTheSystem(string domainName)
        {
            bool exists = false;

            try
            {
                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                    AttributeData[] checkedDomains = service.CheckDomains(new[] { domainName });

                    for (int i = 0; i < checkedDomains.Length; i++)
                    {
                        if (checkedDomains[i].Value.ToLower() == "taken")
                        {
                            exists = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               OrderPageLogger.LogOrderPageException(ex);
                throw;
            }

            return Json(exists);
        }

        /// <summary>
        /// Marks the domains as unavailable.
        /// </summary>
        /// <param name="domains">The domains.</param>
        /// <returns>json object.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MarkDomainsAsUnavailable(string domains)
        {
            DomainDataFromXML[] result;

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                result = DomainSearchHelper.MarkDomainsAsUnavailable(
                    domains,
                    service,
                    Guid.Empty,
                    null,
                    null);
            }

            return Json(result);
        }

        /// <summary>
        /// Start domain search
        /// </summary>
        /// <param name="domainsArray">The domains array.</param>
        /// <returns>json object.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StartSearch(string[] domainsArray)
        {
            DomainDataFromXML[] result;

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                string currencyCode = null;
                if (System.Web.HttpContext.Current.Session != null &&
                    System.Web.HttpContext.Current.Session["OrderCurrencyCode"] != null)
                {
                    currencyCode = System.Web.HttpContext.Current.Session["OrderCurrencyCode"] as string;
                }

                result = DomainSearchHelper.StartSearch(
                    domainsArray,
                    service,
                    Guid.Empty,
                    currencyCode,
                    null);
            }

            return Json(result);
        }

        /// <summary>
        /// Check domain search results
        /// </summary>
        /// <param name="sTransactionId">The s transaction id.</param>
        /// <returns>status result.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetStatus(string sTransactionId)
        {
            AvailabilityStatus status;

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                string currencyCode = null;
                if (System.Web.HttpContext.Current.Session != null && 
                    System.Web.HttpContext.Current.Session["OrderCurrencyCode"] != null)
                {
                    currencyCode = System.Web.HttpContext.Current.Session["OrderCurrencyCode"] as string;
                }

                status = DomainSearchHelper.GetAvailabilityStatus(
                    sTransactionId,
                    service,
                    Guid.Empty,
                    currencyCode,
                    null);
            }

            return Json(new
            {
                TransactionId = status.TransactionId,
                DomainStatuses = status.DomainStatuses,
                FinishSearch = status.FinishSearch
            });
        }

        /// <summary>
        /// Recalculates the cart.
        /// </summary>
        /// <param name="arrayOfProducts">The array of products.</param>
        /// <param name="arrayOfProductNames">The array of product names.</param>
        /// <param name="arrayOfProductQuantities">The array of product quantities.</param>
        /// <param name="arrayOfRenewalPeriods">The array of product renewal periods.</param>
        /// <param name="displayProductName">if set to <c>true</c> [display product name].</param>
        /// <param name="displayProductPeriod">if set to <c>true</c> [display product period].</param>
        /// <param name="displayProductNumberOfItems">if set to <c>true</c> [display product number of items].</param>
        /// <param name="displayProductPrice">if set to <c>true</c> [display product price].</param>
        /// <param name="displayProductDiscount">if set to <c>true</c> [display product discount].</param>
        /// <param name="displayProductTotalPrice">if set to <c>true</c> [display product total price].</param>
        /// <param name="displayOrderSubAmount">if set to <c>true</c> [display order sub amount].</param>
        /// <param name="displayOrderTaxes">if set to <c>true</c> [display order taxes].</param>
        /// <param name="displayOrderTotal">if set to <c>true</c> [display order total].</param>
        /// <param name="chosenCountry">The chosen country.</param>
        /// <param name="globalCounter">The global counter.</param>
        /// <param name="campaignCode">The campaign code.</param>
        /// <param name="pricesIncludingVAT">if set to <c>true</c> [prices including VAT].</param>
        /// <param name="orderCustomAttributes">The order custom attributes.</param>
        /// <param name="orderAddress">The order address.</param>
        /// <returns>Json Object.</returns>
        public ActionResult RecalculateCart(string arrayOfProducts, string arrayOfProductNames, string arrayOfProductQuantities, string arrayOfRenewalPeriods, bool displayProductName, bool displayProductPeriod, bool displayProductNumberOfItems, bool displayProductPrice, bool displayProductDiscount, bool displayProductTotalPrice, bool displayOrderSubAmount, bool displayOrderTaxes, bool displayOrderTotal, string chosenCountry, int globalCounter, string campaignCode, bool pricesIncludingVAT, string orderCustomAttributes, string orderAddress)
        {
            ShoppingCart result;

            string currencyFromCookie = null;
            if (System.Web.HttpContext.Current.Session["OrderCurrencyCode"] != null && !String.IsNullOrEmpty((string)System.Web.HttpContext.Current.Session["OrderCurrencyCode"]))
            {
                currencyFromCookie = (string)System.Web.HttpContext.Current.Session["OrderCurrencyCode"];
            }

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                bool resellerTos = Session != null && Session["resellerAccountData"] != null;
                result = CartHelper.RecalculateCart(
                    this,
                    arrayOfProducts,
                    arrayOfProductNames,
                    arrayOfProductQuantities,
                    arrayOfRenewalPeriods,
                    displayProductName,
                    displayProductPeriod,
                    displayProductNumberOfItems,
                    displayProductPrice,
                    displayProductDiscount,
                    displayProductTotalPrice,
                    displayOrderSubAmount,
                    displayOrderTaxes,
                    displayOrderTotal,
                    chosenCountry,
                    globalCounter,
                    service,
                    Guid.Empty,
                    currencyFromCookie,
                    ResellerHelper.GetResellerId(),
                    null,
                    campaignCode,
                    pricesIncludingVAT,
                    orderCustomAttributes,
                    orderAddress,
                    null,
                    resellerTos);
            }

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PayPalConfirm()
        {
            string token = this.Request.Params["token"];
            string PayerID = this.Request.Params["PayerID"];

            // get transaction bu id

            PublicPaymentTransaction transaction = GetTransationById(token);
            if (transaction == null)
            {
                throw new ArgumentException("Invalid token");
            }

            var amount = transaction.Amount;
            ViewData["PayAmount"] = amount.ToString(".00");

            ViewData["ReferenceNumber"] = token;
            ViewData["PayerId"] = PayerID;

            ViewData["currencyFormat"] = CultureHelper.CURRENCY_FORMAT;
            ViewData["numberFormat"] = CultureHelper.NUMBER_FORMAT;

            ViewData["Currency"] = transaction.CurrencyCode;

            string cancelUrl;
            if (!transaction.Attributes.Any(item => item.Name == "CancelUrl"))
            {
                cancelUrl = Url.Action("Index", new { controller = "PublicOrder" });
            }
            else
            {
                cancelUrl = transaction.Attributes.First(item => item.Name == "CancelUrl").Value;
            }

            ViewData["CancelUrl"] = cancelUrl;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayPalConfirm(string token, string PayerID)
        {
            PublicPaymentTransaction transaction = GetTransationById(token);
            if (transaction == null)
            {
                // error: transaction does not exist
                throw new ArgumentException("Token is invalid");
            }

            string errorMessage = string.Empty;
            var amount = transaction.Amount;

            List<AttributeData> attributeDatas = transaction.Attributes.ToList();
            if (!attributeDatas.Any(item => item.Name == "token"))
            {
                attributeDatas.Add(new AttributeData { Name = "token", Value = token });
            }
            else
            {
                attributeDatas.First(item => item.Name == "token").Value = token;
            }

            if (!attributeDatas.Any(item => item.Name == "payerid"))
            {
                attributeDatas.Add(new AttributeData { Name = "payerid", Value = PayerID });
            }
            else
            {
                attributeDatas.First(item => item.Name == "payerid").Value = PayerID;
            }

            List<NameValue> nameValues = new List<NameValue>();

            foreach (var item in attributeDatas)
            {
                nameValues.Add(new NameValue { Name = item.Name, Value = item.Value });
            }

            PublicPaymentTransaction finishedTransaction = null;

            try
            {
                using (AtomiaBillingPublicService publicOrderService = new AtomiaBillingPublicService())
                {
                    publicOrderService.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                    publicOrderService.UpdatePaymentTransactionData(token, transaction.Status, transaction.StatusCode, transaction.StatusCodeDescription, nameValues.ToArray());
                    finishedTransaction = publicOrderService.FinishPayment(transaction.TransactionId);
                }
                
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
            }

            if (finishedTransaction == null)
            {
                // error: transaction does not exist
                throw new ArgumentException("Transcation could not be finished.");
            }

            CultureInfo locale = CultureInfo.CreateSpecificCulture("en-US");

            // we send it as a string to avoid culture issues
            string amountStr = transaction.Amount.ToString(locale);

            return RedirectToAction(
                "Payment",
                "PublicOrder",
                new
                {
                    amount = amountStr,
                    transactionReference = finishedTransaction.TransactionReference,
                    transactionReferenceType = 0,
                    status = finishedTransaction.Status
                });
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PayExConfirmRedirect()
        {
            string orderRef = this.Request.Params["orderRef"];

            PublicPaymentTransaction transaction = GetTransationById(orderRef);
            if (transaction == null)
            {
                // error: transaction does not exist
                throw new ArgumentException("Token is invalid");
            }

            List<AttributeData> attributeDatas = transaction.Attributes.ToList();
            if (!attributeDatas.Any(item => item.Name == "orderRef"))
            {
                attributeDatas.Add(new AttributeData { Name = "orderRef", Value = orderRef });
            }
            else
            {
                attributeDatas.First(item => item.Name == "orderRef").Value = orderRef;
            }

            PublicPaymentTransaction finishedTransaction = null;

            try
            {
                using (AtomiaBillingPublicService publicOrderService = new AtomiaBillingPublicService())
                {
                    publicOrderService.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                    publicOrderService.UpdatePaymentTransactionData(orderRef, transaction.Status, transaction.StatusCode, transaction.StatusCodeDescription, attributeDatas.Select(item => new NameValue { Name = item.Name, Value = item.Value }).ToArray());
                    finishedTransaction = publicOrderService.FinishPayment(transaction.TransactionId);
                }
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
            }

            if (finishedTransaction == null)
            {
                // error: transaction does not exist
                throw new ArgumentException("Transcation could not be finished.");
            }


            CultureInfo locale = CultureInfo.CreateSpecificCulture("en-US");

            // we send it as a string to avoid culture issues
            string amountStr = transaction.Amount.ToString(locale);

            return RedirectToAction(
               "Payment",
               "PublicOrder",
               new
               {
                   amount = amountStr,
                   transactionReference = finishedTransaction.TransactionReference,
                   transactionReferenceType = 0,
                   status = finishedTransaction.Status
               });
        }

        /// <summary>
        /// Creates the payment transaction.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="order">The order.</param>
        /// <param name="paidAmount">The paid amount.</param>
        /// <returns>Creation of transaction success</returns>
        private string CreatePaymentTransaction(Controller controller, OrderServiceReferences.AtomiaBillingPublicService.PublicOrder order, decimal paidAmount, string paymentMethod)
        {
            PublicPaymentTransaction transaction = PaymentHelper.FillPaymentTransactionForOrder(order, Request, paidAmount);

            string action = null;

            if (paymentMethod == "card")
            {
                PublicOrderConfigurationSection opcs = Helpers.LocalConfigurationHelper.GetLocalConfigurationSection();
                if (Boolean.Parse(opcs.OnlinePayment.Enabled))
                {
                    action = controller.Url.Action("Payment", new { controller = "PublicOrder" });
                }
                else if (Boolean.Parse(opcs.PayexRedirect.Enabled))
                {
                    action = controller.Url.Action("PayExConfirmRedirect", new { controller = "PublicOrder" });

                    List<AttributeData> attributeDatas = transaction.Attributes.ToList();
                    if (!attributeDatas.Any(item => item.Name == "CancelUrl"))
                    {
                        attributeDatas.Add(new AttributeData { Name = "CancelUrl", Value = controller.Url.Action("Select", new { controller = "PublicOrder" }) });
                    }
                    else
                    {
                        attributeDatas.First(item => item.Name == "CancelUrl").Value = controller.Url.Action("Select", new { controller = "PublicOrder" });
                    }
                }
                else if (Boolean.Parse(opcs.WorldPay.Enabled))
                {
                    action = controller.Url.Action("Payment", new { controller = "PublicOrder" });

                    List<AttributeData> attributeDatas = transaction.Attributes.ToList();
                    if (!attributeDatas.Any(item => item.Name == "CancelUrl"))
                    {
                        attributeDatas.Add(new AttributeData { Name = "CancelUrl", Value = controller.Url.Action("Select", new { controller = "PublicOrder" }) });
                    }
                    else
                    {
                        attributeDatas.First(item => item.Name == "CancelUrl").Value = controller.Url.Action("Select", new { controller = "PublicOrder" });
                    }
                }
                else if (Boolean.Parse(opcs.DibsFlexwin.Enabled))
                {
                    action = controller.Url.Action("Payment", new { controller = "PublicOrder" });

                    List<AttributeData> attributeDatas = transaction.Attributes.ToList();
                    if (!attributeDatas.Any(item => item.Name == "CancelUrl"))
                    {
                        attributeDatas.Add(new AttributeData { Name = "CancelUrl", Value = controller.Url.Action("Select", new { controller = "PublicOrder" }) });
                    }
                    else
                    {
                        attributeDatas.First(item => item.Name == "CancelUrl").Value = controller.Url.Action("Select", new { controller = "PublicOrder" });
                    }
                }
                else if (Boolean.Parse(opcs.WorldPayXml.Enabled))
                {
                    action = controller.Url.Action("Payment", new { controller = "PublicOrder" });

                    List<AttributeData> attributeDatas = transaction.Attributes.ToList();
                    if (!attributeDatas.Any(item => item.Name == "CancelUrl"))
                    {
                        attributeDatas.Add(new AttributeData { Name = "CancelUrl", Value = controller.Url.Action("Select", new { controller = "PublicOrder" }) });
                    }
                    else
                    {
                        attributeDatas.First(item => item.Name == "CancelUrl").Value = controller.Url.Action("Select", new { controller = "PublicOrder" });
                    }
                }
                else if (Boolean.Parse(opcs.AdyenHpp.Enabled))
                {
                    action = controller.Url.Action("Payment", new { controller = "PublicOrder" });

                    List<AttributeData> attributeDatas = transaction.Attributes.ToList();
                    if (!attributeDatas.Any(item => item.Name == "CancelUrl"))
                    {
                        attributeDatas.Add(new AttributeData { Name = "CancelUrl", Value = controller.Url.Action("Select", new { controller = "PublicOrder" }) });
                    }
                    else
                    {
                        attributeDatas.First(item => item.Name == "CancelUrl").Value = controller.Url.Action("Select", new { controller = "PublicOrder" });
                    }
                }
            }
            else if (paymentMethod == "paypal")
            {
                action = controller.Url.Action("PayPalConfirm", new { controller = "PublicOrder" });

                List<AttributeData> attributeDatas = transaction.Attributes.ToList();
                if (!attributeDatas.Any(item => item.Name == "CancelUrl"))
                {
                    attributeDatas.Add(new AttributeData { Name = "CancelUrl", Value = controller.Url.Action("Select", new { controller = "PublicOrder" }) });
                }
                else
                {
                    attributeDatas.First(item => item.Name == "CancelUrl").Value = controller.Url.Action("Select", new { controller = "PublicOrder" });
                }
            }

            return PaymentHelper.CreatePaymentTransaction(controller, order, paidAmount, action, transaction);
        }
        
        /// <summary>
        /// Validate VAT number
        /// </summary>
        /// <param name="sEcho"></param>
        /// <param name="countryCode"></param>
        /// <param name="VATNumber"></param>
        /// <returns>One of the values from next set: Invalid, Valid, ValidationError</returns>
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ValidateVatNumber(string sEcho, string countryCode, string VATNumber)
        {
            JsonResult result;

            try
            {
                using (AtomiaBillingPublicService publicOrderService = new AtomiaBillingPublicService())
                {
                    publicOrderService.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();                                    

                    VatNumberValidationResultType valResult = publicOrderService.ValidateVatNumber(
                        countryCode, VATNumber);

                    var dataToReturn = new { sEcho, validationResult = valResult.ToString().ToLower(), error = string.Empty, success = true };
                    result = Json(dataToReturn, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
               OrderPageLogger.LogOrderPageException(ex);
                var dataToReturn = new
                {
                    sEcho,
                    validationResult = string.Empty,
                    error = this.LocalResource("Select", "VATValidationResultNotValidated"), // ex.Message,
                    success = false
                };
                result = Json(dataToReturn, JsonRequestBehavior.AllowGet);
            }

            return result;
        }

        private PublicPaymentTransaction GetTransationById(string transactionId)
        {
            
            try
            {
                using (AtomiaBillingPublicService publicOrderService = new AtomiaBillingPublicService())
                {
                    publicOrderService.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                    return publicOrderService.GetPaymentTransactionById(transactionId);
                }
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets the payment options for product group.
        /// </summary>
        /// <param name="productGroup">The product group.</param>
        /// <param name="groupExists">if set to <c>true</c> [group exists].</param>
        /// <returns>List of allowed order options.</returns>
        private List<OrderOptions> GetOrderOptionsForProductGroup(string productGroup, out bool groupExists)
        {
            groupExists = true;
            List<OrderOptions> result = new List<OrderOptions>();
            PublicOrderConfigurationSection opcs = Helpers.LocalConfigurationHelper.GetLocalConfigurationSection();
            ProductGroup group = (from ProductGroup g in opcs.ProductGroups where g.GroupName == productGroup select g).FirstOrDefault();
            bool addSubdomainOption = false;
            if (!string.IsNullOrEmpty((string)this.HttpContext.Application["AllowAddSubdomain"]))
            {
                this.ViewData["SubdomainValue"] = ((string)this.HttpContext.Application["AllowAddSubdomain"]).ToLowerInvariant();
                addSubdomainOption = true;
            }

            bool allowOwnDomain = !string.IsNullOrEmpty((string)this.HttpContext.Application["AllowOwnDomain"]) &&
                                  ((string)this.HttpContext.Application["AllowOwnDomain"]).ToLowerInvariant() == "true";
            if (string.IsNullOrEmpty(productGroup) || group == null)
            {
                groupExists = false;
                result.Add(OrderOptions.New);

                if (allowOwnDomain)
                {
                    result.Add(OrderOptions.Own);
                }

                // if set to true, add option to add subdomains to some predefined domain);
                if (addSubdomainOption)
                {
                    result.Add(OrderOptions.Sub);
                }
            }
            else
            {
                string[] ordOptions = group.OrderPageOptions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ordOptions.ToList().ForEach(oo =>
                                                {
                                                    if (OrderOptions.GetOrderOption(oo) != null)
                                                    {
                                                        result.Add(OrderOptions.GetOrderOption(oo));
                                                    }
                                                });

                if (!addSubdomainOption && result.Exists(r => r.Value == OrderOptions.Sub.Value))
                {
                    result.Remove(OrderOptions.Sub);
                }

                if (!allowOwnDomain && result.Exists(r => r.Value == OrderOptions.Own.Value))
                {
                    result.Remove(OrderOptions.Own);
                }
            }

            return result;
        }

        /// <summary>
        /// Renders Norid Declaration recieving POST method call
        /// </summary>
        /// <param name="NoridData"></param>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        public ActionResult Norid()
        {

            return View();
        }
    }
}