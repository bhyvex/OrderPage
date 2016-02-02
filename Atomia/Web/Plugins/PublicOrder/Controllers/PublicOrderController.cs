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
using System.Globalization;
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
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using Atomia.Web.Plugin.ProductsProvider;
using Atomia.Web.Plugin.PublicOrder.Configurations;
using Atomia.Web.Plugin.PublicOrder.Filters;
using Atomia.Web.Plugin.PublicOrder.Helpers;
using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;
using Atomia.Web.Plugin.PublicOrder.Models;
using Atomia.Web.Plugin.Validation.Helpers;

using DomainDataFromXML = Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml;

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
    [ServiceProvider]
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
        [ResellerDataProvider]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Index(string package, string lang, string sel)
        {
            bool skipIndexPage;
            if (this.HttpContext.Application.AllKeys.Contains("SkipIndexPage") && bool.TryParse(this.HttpContext.Application["SkipIndexPage"].ToString(), out skipIndexPage)
                    && skipIndexPage)
            {
                return RedirectToAction("Select", new { controller = "PublicOrder", area = "PublicOrder" });
            }

            if (this.RouteData.Values.ContainsKey("resellerHash"))
            {
                ResellerHelper.LoadResellerIntoSessionByHash((string)this.RouteData.Values["resellerHash"]);
            }
            else
            {
                ResellerHelper.LoadResellerIntoSessionByUrl(this.Request.Url.Host);
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

            // Throw away any previous submit form (used to re-fill form on canceled payment)
            Session["SavedSubmitForm"] = null;
            Session["SavedPaymentPlugin"] = null;

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
                tldBasedRegexes = DomainSearchHelper.GetTLDBasedRegexes(ResellerHelper.GetResellerId());
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
            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                Guid resellerId = ResellerHelper.GetResellerId();
                string currencyCode = ResellerHelper.GetResellerCurrencyCode();
                string countryCode = ResellerHelper.GetResellerCountryCode();
                
                DomainSearchHelper.LoadProductsIntoSession(service, Guid.Empty, resellerId, currencyCode, countryCode);

            return Json(null);
        }

        /// <summary>
        /// Index page called with Post
        /// </summary>
        /// <param name="IndexForm"></param>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [ResellerDataProvider]
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
                tldBasedRegexes = DomainSearchHelper.GetTLDBasedRegexes(ResellerHelper.GetResellerId());
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
        [ResellerDataProvider]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Select()
        {
            if (Session == null || Session["firstOption"] == null)
            {
                bool skipIndexPage;
                if (this.HttpContext.Application.AllKeys.Contains("SkipIndexPage") && bool.TryParse(this.HttpContext.Application["SkipIndexPage"].ToString(), out skipIndexPage)
                    && skipIndexPage)
                {
                    Session["firstOption"] = true;
                    Session["domains"] = new string[] { };
                    Session["singleDomain"] = null;
                    Session["subdomain"] = false;
                    Session["multiDomains"] = new List<DomainDataFromXml>();
                }
                else
                {
                    return RedirectToAction("Index");
                }
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
            var resellerId = ResellerHelper.GetResellerId();
            ViewData["ResellerId"] = resellerId;

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

            // Get currency decimal places from session
            int currencyDecimalPlaces = Session["CurrencyDecimalPlaces"] != null
                ? (int)Session["CurrencyDecimalPlaces"]
                : 2;

            ViewData["CurrencyDecimalPlaces"] = currencyDecimalPlaces;
            ViewData["CurrencyDecimalPlacesFormat"] = string.Format("#,###.{0}", new string('0', currencyDecimalPlaces));

            List<Country> countryList = new List<Country>();

            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);

            try
            {
                countryList = CountriesHelper.GetAllCountries(this.HttpContext, service);
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
            }

            List<string> europeanCountriesList = CountriesHelper.GetEUCountryCodes(countryList);
            string europeanCountries = europeanCountriesList.Aggregate(string.Empty, (current, country) => current + String.Format("{0} ", country));

            ViewData["EUCountries"] = europeanCountries.Trim();
            ViewData["CountryList"] = CountriesHelper.GetSupportedCountriesSelectList(countryList);

            PaymentMethod defaultPaymentMethod;
            IList<PaymentMethod> resellerPaymentMethods = ResellerHelper.GetResellerPaymentMethods(out defaultPaymentMethod);

            bool paymentEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "CCPayment");
            bool orderByPostEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "InvoiceByPost");
            bool orderByEmailEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "InvoiceByEmail");
            bool payPalEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "PayPal");
            bool payExRedirectEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "PayexRedirect");
            bool worldPayRedirectEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "WorldPayRedirect");
            bool dibsFlexwinEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "DibsFlexwin");
            bool worldPayXmlRedirectEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "WorldPayXmlRedirect");
            bool adyenHppEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "AdyenHpp");

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

            ViewData["OrderByPostId"] = orderByPostEnabled
                                            ? OrderModel.FetchPostOrderId(
                                                resellerId, null, Guid.Empty, currencyCode, countryCode)
                                            : string.Empty;

            // enabled payment method end
            ViewData["WasAnError"] = 0;

            string filterValue = Session["FilterByPackage"] != null ? (string)Session["FilterByPackage"] : null;

            ViewData["radioList"] = GeneralHelper.FilterPackages(
                this, service, Guid.Empty, resellerId, currencyCode, countryCode, filterValue);

            ViewData["ItemCategories"] = CustomerValidationHelper.GetItemCategories(resellerId);

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
                InvoiceCountryCode = countryCode,
                DomainRegCountryCode = countryCode
            };

            try
            {
                submitForm.WhoisContact = this.HttpContext.Application["ShowWhoIs"].ToString().ToLower() == "true";
            }
            catch
            {
                submitForm.WhoisContact = false;
            }

            if (this.Session["resellerAccountData"] != null)
            {
                submitForm.RadioBillingContact = "reseller";
                submitForm.RadioTechContact = "reseller";
            }

            string defaultPaymentPlugin = string.Empty;
            if (orderByEmailEnabled && defaultPaymentMethod.GuiPluginName == "InvoiceByEmail")
            {
                defaultPaymentPlugin = "PayWithInvoice";
            }
            else if (orderByPostEnabled && defaultPaymentMethod.GuiPluginName == "InvoiceByPost")
            {
                defaultPaymentPlugin = "PayWithInvoice";
            }
            else if ((paymentEnabled && defaultPaymentMethod.GuiPluginName == "CCPayment")
                || (payExRedirectEnabled && defaultPaymentMethod.GuiPluginName == "PayexRedirect")
                || (worldPayRedirectEnabled && defaultPaymentMethod.GuiPluginName == "WorldPay")
                || (dibsFlexwinEnabled && defaultPaymentMethod.GuiPluginName == "DibsFlexwin")
                || (worldPayXmlRedirectEnabled && defaultPaymentMethod.GuiPluginName == "WorldPayXmlRedirect")
                || (adyenHppEnabled && defaultPaymentMethod.GuiPluginName == "AdyenHpp"))
            {
                defaultPaymentPlugin = defaultPaymentMethod.GuiPluginName;
            }
            else if (payPalEnabled && defaultPaymentMethod.GuiPluginName == "PayPal")
            {
                defaultPaymentPlugin = "PayPal";
            }

            this.ControllerContext.HttpContext.Application["DefaultPaymentPlugin"] = ViewData["DefaultPaymentPlugin"] = defaultPaymentPlugin;
            if (Session["SavedPaymentPlugin"] != null && !string.IsNullOrEmpty((string)Session["SavedPaymentPlugin"]))
            {
                ViewData["DefaultPaymentPlugin"] = Session["SavedPaymentPlugin"];
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

            ViewData["AllowedDomainLength"] = allowedDomainLength;
            ViewData["NumberOfDomainsAllowed"] = numberOfDomainsAllowed;

            Session["dontShowTaxesForThisResellerHidden"] = (!GeneralHelper.TaxAreShownForReseller(this)).ToString().ToLower();

            if (Session["SavedSubmitForm"] != null)
            {
                // Used saved submit form from and earlier request to re-fill. (E.g. return to form from canceled payment).
                submitForm = (SubmitForm)Session["SavedSubmitForm"];
            }

            ViewData["SeparateUsernameAndEmail"] = Atomia.Common.Configuration.AtomiaCommon.Instance.SeparateUsernameAndEmail;

            return View(submitForm);
        }

        /// <summary>
        /// Renders select action recieving POST method call
        /// </summary>
        /// <param name="SubmitForm"></param>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [ResellerDataProvider]
        [AcceptVerbs(HttpVerbs.Post)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Select(SubmitForm SubmitForm)
        {
            bool useSeparateUsernameAndEmail = Atomia.Common.Configuration.AtomiaCommon.Instance.SeparateUsernameAndEmail;
            List<Country> countryList = new List<Country>();

            bool paymentMethodCc = false; // finish payment method CC
            ViewData["WasAnError"] = 0;

            PaymentMethod defaultPaymentMethod;
            IList<PaymentMethod> resellerPaymentMethods = ResellerHelper.GetResellerPaymentMethods(out defaultPaymentMethod);

            bool paymentEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "CCPayment");
            bool orderByPostEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "InvoiceByPost");
            bool orderByEmailEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "InvoiceByEmail");
            bool payPalEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "PayPal");
            bool payExRedirectEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "PayexRedirect");
            bool worldPayRedirectEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "WorldPayRedirect");
            bool dibsFlexwinEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "DibsFlexwin");
            bool worldPayXmlRedirectEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "WorldPayXmlRedirect");
            bool adyenHppEnabled = resellerPaymentMethods.Any(m => m.GuiPluginName == "AdyenHpp");

            ViewData["PayexRedirectEnabled"] = payExRedirectEnabled;
            ViewData["WorldPayRedirectEnabled"] = worldPayRedirectEnabled;
            ViewData["DibsFlexwinEnabled"] = dibsFlexwinEnabled;
            ViewData["WorldPayXmlRedirectEnabled"] = worldPayXmlRedirectEnabled;
            ViewData["AdyenHppEnabled"] = adyenHppEnabled;

            ViewData["AddingSubdomain"] = false;
            if (this.Session["subdomain"] != null)
            {
                ViewData["AddingSubdomain"] = (bool)this.Session["subdomain"];
            }

            ViewData["DefaultPaymentPlugin"] = ControllerContext.HttpContext.Application["DefaultPaymentPlugin"];

            string orderByPostId = string.Empty;
            IList<RadioRow> list;
            List<ProductDescription> currentCart;

            var resellerId = ResellerHelper.GetResellerId();
            ViewData["ResellerId"] = resellerId;
            var currencyCode = ResellerHelper.GetResellerCurrencyCode();
            var countryCode = ResellerHelper.GetResellerCountryCode();

            ViewData["PaymentEnabled"] = paymentEnabled;
            ViewData["PayPalEnabled"] = payPalEnabled;

            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);

            if (orderByPostEnabled)
            {
                orderByPostId = OrderModel.FetchPostOrderId(resellerId, null, Guid.Empty, currencyCode, countryCode);
                ViewData["OrderByPostId"] = orderByPostId;
                ViewData["OrderByPostEnabled"] = true;
            }
            else
            {
                ViewData["OrderByPostId"] = string.Empty;
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

            AtomiaCultureInfo atomiaCultureInfo = null;
            if (this.HttpContext.Session["SessionAccountLanguages"] != null)
            {
                atomiaCultureInfo = (AtomiaCultureInfo)this.HttpContext.Session["SessionAccountLanguages"];
            }

            string languageCode = atomiaCultureInfo != null ? atomiaCultureInfo.Language : null;
            list = OrderModel.FetchPackagesData(this, resellerId, null, Guid.Empty, currencyCode, countryCode, languageCode);

            currentCart = SubmitForm.CurrentCart;

            if (ModelState.IsValid)
            {
                // Save form to session to be able to refill, e.g. on canceled payment.
                Session["SavedSubmitForm"] = SubmitForm;

                // call Billing to Submit form
                try
                {
                    OrderServiceReferences.AtomiaBillingPublicService.PublicOrder newOrder;
                    OrderServiceReferences.AtomiaBillingPublicService.PublicOrder myOrder = new OrderServiceReferences.AtomiaBillingPublicService.PublicOrder();
                    countryList = CountriesHelper.GetAllCountries(this.HttpContext, service);

                    List<PublicOrderCustomData> orderCustomData = new List<PublicOrderCustomData>();

                    myOrder.Address = GeneralHelper.PrepareForSubmit(SubmitForm.Address);
                    myOrder.Address2 = GeneralHelper.PrepareForSubmit(SubmitForm.Address2);

                    myOrder.BillingFirstName = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceContactName);
                    myOrder.BillingLastName = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceContactLastName);
                    myOrder.BillingCompany = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceCompany);

                    myOrder.BillingAddress = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceAddress);
                    myOrder.BillingAddress2 = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceAddress2);
                    myOrder.BillingCity = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceCity);
                    myOrder.BillingCountry = GeneralHelper.PrepareForSubmit(string.IsNullOrEmpty(SubmitForm.InvoiceCountryCode) ? SubmitForm.CountryCode : SubmitForm.InvoiceCountryCode);
                    if (SubmitForm.InvoicePostNumber != null)
                    {
                        myOrder.BillingZip = GeneralHelper.PrepareForSubmit(SubmitForm.InvoicePostNumber.Trim());
                    }

                    myOrder.BillingEmail = GeneralHelper.PrepareForSubmit(SubmitForm.InvoiceEmail);

                    myOrder.BillingPhone = GeneralHelper.PrepareForSubmit(Atomia.Common.FormattingHelper.FormatPhoneNumber(SubmitForm.InvoiceTelephone, SubmitForm.InvoiceCountryCode));
                    myOrder.BillingFax = GeneralHelper.PrepareForSubmit(Atomia.Common.FormattingHelper.FormatPhoneNumber(SubmitForm.InvoiceFax, SubmitForm.InvoiceCountryCode));
                    myOrder.BillingMobile = GeneralHelper.PrepareForSubmit(Atomia.Common.FormattingHelper.FormatPhoneNumber(SubmitForm.InvoiceMobile, SubmitForm.InvoiceCountryCode));
                    
                    myOrder.City = GeneralHelper.PrepareForSubmit(SubmitForm.City);
                    myOrder.Company = GeneralHelper.PrepareForSubmit(SubmitForm.Company);
                    if (!string.IsNullOrEmpty(SubmitForm.OrgNumber))
                    {
                        string tmpString = GeneralHelper.PrepareForSubmit(SubmitForm.OrgNumber);
                        myOrder.CompanyNumber = tmpString;
                    }

                    myOrder.Country = GeneralHelper.PrepareForSubmit(SubmitForm.CountryCode);

                    myOrder.Currency = "SEK";
                    if (this.Session["OrderCurrencyCode"] != null && !string.IsNullOrEmpty((string)this.Session["OrderCurrencyCode"]))
                    {
                        myOrder.Currency = this.Session["OrderCurrencyCode"].ToString();
                    }

                    myOrder.Email = GeneralHelper.PrepareForSubmit(SubmitForm.Email);

                    myOrder.Fax = GeneralHelper.PrepareForSubmit(FormattingHelper.FormatPhoneNumber(SubmitForm.Fax, SubmitForm.CountryCode));
                    myOrder.Mobile = GeneralHelper.PrepareForSubmit(FormattingHelper.FormatPhoneNumber(SubmitForm.Mobile, SubmitForm.CountryCode));

                    myOrder.FirstName = GeneralHelper.PrepareForSubmit(SubmitForm.ContactName);
                    myOrder.LastName = GeneralHelper.PrepareForSubmit(SubmitForm.ContactLastName);
                    myOrder.LegalNumber = GeneralHelper.PrepareForSubmit(SubmitForm.VATNumber);

                    List<string> allPackagesIds = OrderModel.FetchAllPackagesIds(resellerId, null, Guid.Empty, currencyCode, countryCode);
                    IList<Product> products = GeneralHelper.GetProductProvider().GetShopProducts(resellerId, null, Guid.Empty, countryCode);
                    ProductDescription selectedPackage = currentCart.Find(p => allPackagesIds.Any(x => x == p.productID));
                    IList<Product> freePackages = OrderModel.FetchFreePackages(resellerId, null, Guid.Empty, currencyCode, countryCode);
                    IList<string> setupFeeIds = OrderModel.FetchSetupFeeIds(resellerId, null, Guid.Empty, countryCode, countryCode);

                    Session["PreselectedPackage"] = selectedPackage != null ? selectedPackage.productID : null;
                    Session["SavedPaymentPlugin"] = SubmitForm.RadioPaymentMethod;

                    List<PublicOrderItem> myOrderItems = new List<PublicOrderItem>();

                    string jsonDomainRegContact = string.Empty;
                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    if (SubmitForm.WhoisContact)
                    {
                        SubmitForm.DomainRegCity = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegCity);
                        SubmitForm.DomainRegCountryCode = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegCountryCode);
                        SubmitForm.DomainRegEmail = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegEmail);
                        SubmitForm.DomainRegFax = FormattingHelper.FormatPhoneNumber(SubmitForm.DomainRegFax, SubmitForm.DomainRegCountryCode);
                        SubmitForm.DomainRegContactName = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegContactName);
                        SubmitForm.DomainRegContactLastName = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegContactLastName);
                        SubmitForm.DomainRegCompany = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegCompany);
                        SubmitForm.DomainRegOrgNumber = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegOrgNumber);
                        SubmitForm.DomainRegAddress = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegAddress);
                        SubmitForm.DomainRegAddress2 = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegAddress2);
                        SubmitForm.DomainRegVATNumber = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegVATNumber);
                        SubmitForm.DomainRegTelephone = FormattingHelper.FormatPhoneNumber(SubmitForm.DomainRegTelephone, SubmitForm.DomainRegCountryCode);
                        SubmitForm.DomainRegPostNumber = GeneralHelper.PrepareForSubmit(SubmitForm.DomainRegPostNumber);
                        jsonDomainRegContact = javaScriptSerializer.Serialize(new DomainRegContact
                            {
                                City = SubmitForm.DomainRegCity,
                                Country = SubmitForm.DomainRegCountryCode,
                                Email = SubmitForm.DomainRegEmail,
                                Fax = SubmitForm.DomainRegFax,
                                Name = SubmitForm.DomainRegContactName + " " + SubmitForm.DomainRegContactLastName,
                                Org = SubmitForm.DomainRegCompany,
                                OrgNo = SubmitForm.DomainRegOrgNumber,
                                Street1 = SubmitForm.DomainRegAddress,
                                Street2 = SubmitForm.DomainRegAddress2,
                                VatNo = SubmitForm.DomainRegVATNumber,
                                Voice = SubmitForm.DomainRegTelephone,
                                Zip = SubmitForm.DomainRegPostNumber.Trim(),
                                CustomFields = javaScriptSerializer.Serialize(SubmitForm.CustomFields)
                            });
                    }

                    foreach (ProductDescription tmpProduct in currentCart)
                    {
                        // If post invoice is selected do not add it to orderItems since that product is added via orderCustomAttributes
                        if (SubmitForm.RadioPaymentMethod == "InvoiceByPost")
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
                            if (freePackages.Any(f => f.ArticleNumber == selectedPackage.productID))
                            {
                                // if the selected package in the cart is free
                                string domainValue = SubmitForm.FirstOption ? tmpProduct.productDesc : (SubmitForm.OwnDomain.StartsWith("www") ? SubmitForm.OwnDomain.Remove(0, 4) : SubmitForm.OwnDomain);

                                arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainName", Value = domainValue });

                                arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = "CsDomainParking" });
                                Product package = products.FirstOrDefault(p => p.ArticleNumber == selectedPackage.productID);
                                if (SubmitForm.FirstOption == false)
                                {
                                    arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "MainDomain", Value = "true" });
                                    if (package != null && package.Properties != null && package.Properties.ContainsKey("atomiaserviceextraproperties"))
                                    {
                                        arrayOfCustoms.Add(new PublicOrderItemProperty
                                        {
                                            Name = "AtomiaServiceExtraProperties",
                                            Value = package.Properties["atomiaserviceextraproperties"].
                                                ToString()
                                        });
                                    }
                                }

                                if (SubmitForm.WhoisContact)
                                {
                                    arrayOfCustoms.Add(new PublicOrderItemProperty
                                    {
                                        Name = "DomainRegContact",
                                        Value = jsonDomainRegContact
                                    });
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
                                Product package = products.FirstOrDefault(p => p.ArticleNumber == selectedPackage.productID);
                                if (package != null && package.Properties != null && package.Properties.ContainsKey("nowebsites") && package.Properties["nowebsites"].ToLowerInvariant() == "true")
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
                                        if (package != null && package.Properties != null && package.Properties.ContainsKey("atomiaserviceextraproperties"))
                                        {
                                            arrayOfCustoms.Add(new PublicOrderItemProperty
                                            {
                                                Name = "AtomiaServiceExtraProperties",
                                                Value = package.Properties["atomiaserviceextraproperties"].
                                                    ToString()
                                            });
                                        }
                                    }
                                    if (SubmitForm.WhoisContact)
                                    {
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainRegContact", Value = jsonDomainRegContact });
                                    }
                                }
                                else
                                {
                                    string websiteType = websitesAllowed ? "CsLinuxWebsite" : "CsDomainParking";
                                    arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "MainDomain", Value = "true" });
                                    if (package != null && package.Properties != null && package.Properties.ContainsKey("atomiaserviceextraproperties"))
                                    {
                                        arrayOfCustoms.Add(new PublicOrderItemProperty
                                        {
                                            Name = "AtomiaServiceExtraProperties",
                                            Value = package.Properties["atomiaserviceextraproperties"]
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
                            if (freePackages.Any(f => f.ArticleNumber == selectedPackage.productID))
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

                    PublicOrderConfigurationSection opcss = Helpers.LocalConfigurationHelper.GetLocalConfigurationSection();
                    Dictionary<string, string> emailProps = new Dictionary<string, string>();
                    JavaScriptSerializer jsemail = javaScriptSerializer;
                    bool addApplication = false;

                    foreach (PublicOrderItem myOrderItem in myOrderItems)
                    {
                        emailProps = new Dictionary<string, string>();
                        if (opcss.DomainRegistrySpecificProducts.GetItemByKey(myOrderItem.ItemNumber) != null)
                        {
                            List<PublicOrderItemProperty> arrayOfCustoms = myOrderItem.CustomData.ToList();
                            if (SubmitForm.DomainSpeciffic == null)
                            {
                                throw new Exception("Order could not be created. DomainRegistrySpecificAttributes missing for " + myOrderItem.ItemNumber + " domain");
                            }
                            string emailType =
                                opcss.DomainRegistrySpecificProducts.GetItemByKey(myOrderItem.ItemNumber).Email;
                            if (!string.IsNullOrEmpty(emailType))
                            {
                                string cccEmail =
                                opcss.DomainRegistrySpecificProducts.GetItemByKey(myOrderItem.ItemNumber).CccEmail;
                                emailProps.Add("Type", emailType);
                                if (myOrderItem.CustomData.Any(v => v.Name == "DomainName"))
                                {
                                    emailProps.Add("Domain", myOrderItem.CustomData.FirstOrDefault(v => v.Name == "DomainName").Value ?? "");
                                }
                                Dictionary<string, string> domainSpecific = new Dictionary<string, string>();
                                domainSpecific =
                                    jsemail.Deserialize<Dictionary<string, string>>(SubmitForm.DomainSpeciffic);
                                emailProps.Add("Name", domainSpecific.FirstOrDefault(v => v.Key == "AcceptName").Value ?? "");
                                emailProps.Add("Time", domainSpecific.FirstOrDefault(v => v.Key == "AcceptDate").Value ?? "");
                                emailProps.Add("Orgnum", myOrder.CompanyNumber);
                                emailProps.Add("Company", myOrder.Company);
                                emailProps.Add("Version", domainSpecific.FirstOrDefault(v => v.Key == "AcceptVersion").Value ?? "");
                                emailProps.Add("Ccc", cccEmail);
                                arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "MailOnOrder", Value = jsemail.Serialize(emailProps) });
                            }
                            arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainRegistrySpecificAttributes", Value = SubmitForm.DomainSpeciffic });
                            myOrderItem.CustomData = arrayOfCustoms.ToArray();
                        }

                        if (myOrderItem.ItemNumber == "HST-APPY")
                        {
                            addApplication = true;
                        }
                    }

                    if (addApplication)
                    {
                        PublicOrderItem tmpItem = myOrderItems.Where(i => i.CustomData != null && i.CustomData.Any(c => c.Name == "MainDomain")).FirstOrDefault();

                        if (tmpItem == null)
                        {
                            throw new Exception("Unable to find MainDomain order item");
                        }

                        PublicOrderItemProperty tmpProperty = tmpItem.CustomData.Where(c => c.Name == "DomainName").FirstOrDefault();

                        myOrderItems.Add(new PublicOrderItem
                        {
                            ItemId = Guid.Empty,
                            ItemNumber = "XSV-APP",
                            Quantity = 1,
                            CustomData = new[]
                                {
                                    new PublicOrderItemProperty
                                    {
                                        Name = "DomainName",
                                        Value = tmpProperty.Value
                                    },
                                    new PublicOrderItemProperty
                                    {
                                        Name = "ApplicationName",
                                        Value = "wordpress"
                                    }
                                }
                        });
                    }

                    if (SubmitForm.RadioPaymentMethod == "InvoiceByEmail")
                    {
                        orderCustomData.Add(new PublicOrderCustomData { Name = "PayByInvoice", Value = "true" });
                    }

                    if (SubmitForm.RadioPaymentMethod == "InvoiceByPost")
                    {
                        orderCustomData.Add(new PublicOrderCustomData { Name = "SendInvoiceByPost", Value = "true" });
                        orderCustomData.Add(new PublicOrderCustomData { Name = "PayByInvoice", Value = "true" });
                    }

                    string paymentMethod = SubmitForm.RadioPaymentMethod;
                    if (paymentMethod == "InvoiceByEmail" || paymentMethod == "InvoiceByPost") {
                        paymentMethod = "PayWithInvoice";
                    }
                    orderCustomData.Add(new PublicOrderCustomData { Name = "PaymentMethod", Value = paymentMethod });

                    if (!string.IsNullOrEmpty((string)Session["SpecialPID"]))
                    {
                        orderCustomData.Add(new PublicOrderCustomData { Name = "SpecialPID", Value = (string)Session["SpecialPID"] });
                    }

                    if (!string.IsNullOrEmpty(SubmitForm.VATValidationMessage))
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

                    if (SubmitForm.CustomFields != null && SubmitForm.CustomFields.Keys.Count > 0)
                    {
                        orderCustomData.Add(
                            new PublicOrderCustomData
                                {
                                    Name = "DomainContactCustomFields",
                                    Value = javaScriptSerializer.Serialize(SubmitForm.CustomFields)
                                });
                    }

                    // Add CustommData posted with submit, client added
                    if (!string.IsNullOrEmpty(SubmitForm.OrderCustomData))
                    {
                        try
                        {
                            JavaScriptSerializer js = javaScriptSerializer;
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

                    if (atomiaCultureInfo != null)
                    {
                        orderCustomData.Add(new PublicOrderCustomData { Name = "Language", Value = string.Format("{0}-{1}", atomiaCultureInfo.Language, atomiaCultureInfo.Culture) });
                    }

                    if (this.HttpContext.Application.AllKeys.Contains("ImmediateProvisioning") &&
                        this.HttpContext.Application["ImmediateProvisioning"].ToString().ToLowerInvariant() == "true")
                    {
                        orderCustomData.Add(new PublicOrderCustomData { Name = "ImmediateProvisioning", Value = "true" });
                    }

                    if (IndexHelper.IsImmediateLoginEnabled(this))
                    {
                        orderCustomData.Add(new PublicOrderCustomData { Name = "ShowHcpLandingPage", Value = "true" });
                    }

                    if (useSeparateUsernameAndEmail)
                    {
                        orderCustomData.Add(new PublicOrderCustomData { Name = "Username", Value = SubmitForm.Username });
                    }

                    myOrder.CustomData = orderCustomData.ToArray();
                    myOrder.OrderItems = myOrderItems.ToArray();
                    myOrder.ResellerId = ResellerHelper.GetResellerId();
                    myOrder.Phone = GeneralHelper.PrepareForSubmit(FormattingHelper.FormatPhoneNumber(SubmitForm.Telephone, SubmitForm.CountryCode));
                    myOrder.Zip = GeneralHelper.PrepareForSubmit(SubmitForm.PostNumber.Trim());
                    myOrder.PaymentMethod = SubmitForm.RadioPaymentMethod == "InvoiceByEmail" || SubmitForm.RadioPaymentMethod == "InvoiceByPost"
                                                ? OrderServiceReferences.AtomiaBillingPublicService.PaymentMethodEnum.PayByInvoice
                                                : OrderServiceReferences.AtomiaBillingPublicService.PaymentMethodEnum.PayByCard;

                    if (IndexHelper.IsImmediateLoginEnabled(this))
                    {
                        string resellerRootDomain = UriHelper.GetRootDomain(HttpContext.Request.Url.AbsoluteUri);
                        string token = string.Empty;
                        newOrder = service.CreateOrderWithLoginToken(myOrder, resellerRootDomain, out token);
                        this.Session["ImmediateLoginUrl"] = IndexHelper.GetImmediateLoginUrl(this, myOrder.Email, token);
                    }
                    else
                    {
                        newOrder = service.CreateOrder(myOrder);
                    }

                    if (newOrder == null)
                    {
                        throw new Exception("Order could not be created.");
                    }

                    this.Session["CreatedOrder"] = newOrder;
                    
                    if (SubmitForm.RadioPaymentMethod != "InvoiceByPost" && SubmitForm.RadioPaymentMethod != "InvoiceByEmail")
                    {
                        if (newOrder.Total > decimal.Zero)
                        {
                            paymentMethodCc = SubmitForm.RadioPaymentMethod != "PayPal";
                            string result = this.CreatePaymentTransaction(
                                this, newOrder, newOrder.Total, SubmitForm.RadioPaymentMethod);

                            if (!string.IsNullOrEmpty(result))
                            {
                                return Redirect(result);
                            }
                        }
                    }

                    if (IndexHelper.IsImmediateLoginEnabled(this))
                    {
                        Response.Redirect(this.Session["ImmediateLoginUrl"].ToString());
                    }
                    else
                    {
                        return this.RedirectToAction("Thankyou");
                    }
                }
                catch (Exception ex)
                {
                    OrderPageLogger.LogOrderPageException(ex);
                }
            }
            else
            {
                ViewData["WasAnError"] = 1;
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
                countryList = CountriesHelper.GetAllCountries(this.HttpContext, service);
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
            if (this.HttpContext.Application["DomainSearchAllowedDomainLength"] != null && (string)this.HttpContext.Application["DomainSearchAllowedDomainLength"] != string.Empty
                && this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != null && (string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"] != string.Empty)
            {
                allowedDomainLength = int.Parse((string)this.HttpContext.Application["DomainSearchAllowedDomainLength"]);
                numberOfDomainsAllowed = int.Parse((string)this.HttpContext.Application["DomainSearchNumberOfDomainsAllowed"]);
            }
            else
            {
                throw new ConfigurationErrorsException("Missing AllowedDomainLength or NumberOfDomainsAllowed in configuration");
            }

            ViewData["AllowedDomainLength"] = allowedDomainLength;
            ViewData["NumberOfDomainsAllowed"] = numberOfDomainsAllowed;

            // Check if there is locale setting for country in cookie ad set if there is
            if (System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"] != null && !string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"].Value))
            {
                countryCode = System.Web.HttpContext.Current.Request.Cookies["OrderLocaleCookie"].Value.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[1];
            }

            ViewData["defaultCountry"] = countryCode;

            // This is supose to be set in InternationalizationProvider
            CultureInfo locale = Thread.CurrentThread.CurrentCulture;

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

            ViewData["ItemCategories"] = CustomerValidationHelper.GetItemCategories(resellerId);
            ViewData["SeparateUsernameAndEmail"] = useSeparateUsernameAndEmail;

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
            AddressInfo addressInfo = new AddressInfo();
            try
            {
                var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);                
                    addressInfo = service.GetAddressInfoOrder(identifier, EntityType.Company, ResellerHelper.GetResellerId());
                }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
            }

            return Json(addressInfo);
        }

        /// <summary>
        /// Thankyou page
        /// </summary>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute(ClearSession = true)]
        [ResellerDataProvider]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Thankyou()
        {
            if (this.Session["CreatedOrder"] != null)
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                var createdOrder = this.Session["CreatedOrder"] as OrderServiceReferences.AtomiaBillingPublicService.PublicOrder;
                PublicOrderItemProperty domainRegContactAttr = null;

                if (createdOrder != null)
                {
                    foreach (var item in createdOrder.OrderItems)
                    {
                        // They are all the same so remove all to avoid JSON errors and use the last one as domainRegContactAttr.
                        domainRegContactAttr = item.CustomData.FirstOrDefault(cd => cd.Name.ToLower() == "domainregcontact");

                        if (domainRegContactAttr != null)
                        {
                            var customData = item.CustomData.ToList();
                            customData.Remove(domainRegContactAttr);
                            item.CustomData = customData.ToArray();
                        }
                    }
                }

                ViewData["CreatedOrderAsJson"] = js.Serialize(createdOrder);
                ViewData["DomainRegContactAsJson"] = domainRegContactAttr != null
                    ? domainRegContactAttr.Value
                    : "null";
                
                ViewData["CreatedOrderRaw"] = createdOrder;
                this.Session["CreatedOrder"] = null;

                // Throw away any previous submit form (used to re-fill form on canceled payment)
                this.Session["SavedSubmitForm"] = null;
                this.Session["SavedPaymentPlugin"] = null;
                this.Session["PreselectedPackage"] = null;
            }

            return View();
        }

        /// <summary>
        /// Action for payment failed info
        /// </summary>
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [ResellerDataProvider]
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
        [ResellerDataProvider]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Payment(string amount, string transactionReference, int transactionReferenceType, string status)
        {
            decimal decimalAmount;
            decimal.TryParse(amount, out decimalAmount);

            if (status.ToUpper() == "OK" || status.ToUpper() == "IN_PROGRESS")
            {
                if (IndexHelper.IsImmediateLoginEnabled(this))
                {
                    Response.Redirect(this.Session["ImmediateLoginUrl"].ToString());
                }
                else
                {
                    return this.RedirectToAction("Thankyou");
                }
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
            return ValidateDomain(domainName, false);
        }

        /// <summary>
        /// Validates the domain.
        /// </summary>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns>The Json with true/false if the domainName is succesfully validated.</returns>
        public ActionResult ValidateDomains(string domainName)
        {
            return ValidateDomain(domainName, true);
        }

        private ActionResult ValidateDomain(string domainName, bool allowWithoutTLD)
        {
            bool validated = true;

            string[] domainNames = domainName.Split('\n');

            if (domainNames != null && domainNames.Length > 0)
            {
                foreach (string domain in domainNames)
                {
                    if (domain.Contains("http://") || domain.Contains("https://") || domain.Contains("www."))
                    {
                        validated = false;
                    }
                    else
                    {
                        try
                        {
                            string finalDomainName = SimpleDnsPlus.IDNLib.Encode(allowWithoutTLD && !domainName.Contains(".") ? (domain + ".random") : domain);
                            validated = Regex.IsMatch(finalDomainName, RegularExpression.GetRegularExpression("EncodedDomain"));
                        }
                        catch (Exception ex)
                        {
                            OrderPageLogger.LogOrderPageException(new Exception("IDNLIB could not encode the following domain: " + domain));
                            OrderPageLogger.LogOrderPageException(ex);
                            validated = false;
                        }
                    }

                    if (!validated)
                    {
                        break;
                    }
                }
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
                var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                    AttributeData[] checkedDomains = service.CheckDomains(new[] { domainName });

                    for (int i = 0; i < checkedDomains.Length; i++)
                    {
                        if (checkedDomains[i].Value.ToLower() == "taken")
                        {
                            exists = true;
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

            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                Guid resellerId = ResellerHelper.GetResellerId();
                string currencyCode = ResellerHelper.GetResellerCurrencyCode();
                string countryCode = ResellerHelper.GetResellerCountryCode();

                result = DomainSearchHelper.MarkDomainsAsUnavailable(
                    domains,
                    service,
                    Guid.Empty,
                    resellerId,
                    currencyCode,
                    countryCode);

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

            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                string currencyCode = null;
                if (System.Web.HttpContext.Current.Session != null &&
                    System.Web.HttpContext.Current.Session["OrderCurrencyCode"] != null)
                {
                    currencyCode = System.Web.HttpContext.Current.Session["OrderCurrencyCode"] as string;
                }

                Guid resellerId = ResellerHelper.GetResellerId();
                string countryCode = ResellerHelper.GetResellerCountryCode();

                result = DomainSearchHelper.StartSearch(
                    domainsArray,
                    service,
                    Guid.Empty,
                    resellerId,
                    currencyCode,
                    countryCode);

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

            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                string currencyCode = null;
                if (System.Web.HttpContext.Current.Session != null &&
                    System.Web.HttpContext.Current.Session["OrderCurrencyCode"] != null)
                {
                    currencyCode = System.Web.HttpContext.Current.Session["OrderCurrencyCode"] as string;
                }
                
                Guid resellerId = ResellerHelper.GetResellerId();
                string countryCode = ResellerHelper.GetResellerCountryCode();

                status = DomainSearchHelper.GetAvailabilityStatus(
                    sTransactionId,
                    service,
                    Guid.Empty,
                    resellerId,
                    currencyCode,
                    countryCode);

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
            if (System.Web.HttpContext.Current.Session["OrderCurrencyCode"] != null && !string.IsNullOrEmpty((string)System.Web.HttpContext.Current.Session["OrderCurrencyCode"]))
            {
                currencyFromCookie = (string)System.Web.HttpContext.Current.Session["OrderCurrencyCode"];
            }

            var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
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

            return Json(result);
        }

        [ResellerDataProvider]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult PayPalConfirm()
        {
            string token = this.Request.Params["token"];
            string payerId = this.Request.Params["PayerID"];

            // get transaction bu id
            PublicPaymentTransaction transaction = GetTransationById(token);
            if (transaction == null)
            {
                throw new ArgumentException("Invalid token");
            }

            // Customer clicked cancel so we should mark transaction as FAILED and finish it.
            // There is no point in showing Confirm page.
            if (string.IsNullOrEmpty(payerId))
            {
                transaction.Status = "FAILED";
                transaction.StatusCode = "Cancelled";
                transaction.StatusCodeDescription = "Cancelled on PayPal page";
                return this.FinishPayment(transaction);
            }

            ViewData["PayAmount"] = transaction.Amount.ToString(".00");
            ViewData["ReferenceNumber"] = token;
            ViewData["PayerId"] = payerId;
            ViewData["currencyFormat"] = CultureHelper.CURRENCY_FORMAT;
            ViewData["numberFormat"] = CultureHelper.NUMBER_FORMAT;
            ViewData["Currency"] = transaction.CurrencyCode;

            string cancelUrl = !transaction.Attributes.Any(item => item.Name == "CancelUrl")
                                   ? this.Url.Action("Index", new { controller = "PublicOrder" })
                                   : transaction.Attributes.First(item => item.Name == "CancelUrl").Value;

            ViewData["CancelUrl"] = cancelUrl;

            return View();
        }

        [ResellerDataProvider]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayPalConfirm(string token, string PayerID, string action)
        {
            PublicPaymentTransaction transaction = GetTransationById(token);
            if (transaction == null)
            {
                // error: transaction does not exist
                throw new ArgumentException("Token is invalid");
            }

            // Update attributes with token and PayerID.
            List<AttributeData> attributeDatas = transaction.Attributes.ToList();
            if (!transaction.Attributes.Any(item => item.Name == "token"))
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

            transaction.Attributes = attributeDatas.ToArray();

            if (action == "cancel")
            {
                transaction.Status = "FAILED";
                transaction.StatusCode = "Cancelled";
                transaction.StatusCodeDescription = "Cancelled on confirmation page";
            }

            return this.FinishPayment(transaction);
        }

        /// <summary>
        /// Creates the payment transaction.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="order">The order.</param>
        /// <param name="paidAmount">The paid amount.</param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <returns>Creation of transaction success</returns>
        private string CreatePaymentTransaction(Controller controller, OrderServiceReferences.AtomiaBillingPublicService.PublicOrder order, decimal paidAmount, string paymentMethod)
        {
            PublicPaymentTransaction transaction = PaymentHelper.FillPaymentTransactionForOrder(order, Request, paidAmount, paymentMethod);

            string action;

            if (paymentMethod == "CCPayment")
            {
                action = controller.Url.Action("Payment", new { controller = "PublicOrder" });

            }
            else
            {
                if (controller.HttpContext != null && controller.HttpContext.Application != null &&
                    controller.HttpContext.Application.AllKeys.Contains(paymentMethod + "PaymentReturnAction") &&
                    !string.IsNullOrEmpty((string)controller.HttpContext.Application[paymentMethod + "PaymentReturnAction"]))
                {
                    action = controller.Url.Action((string)controller.HttpContext.Application[paymentMethod + "PaymentReturnAction"], new { controller = "PublicOrder" });
                }
                else
                {
                    action = controller.Url.Action("Payment", new { controller = "PublicOrder" });
                }


                List<AttributeData> attributeDatas = transaction.Attributes.ToList();
                if (!attributeDatas.Any(item => item.Name == "CancelUrl"))
                {
                    attributeDatas.Add(
                        new AttributeData
                        {
                            Name = "CancelUrl",
                            Value = controller.Url.Action("Select", new { controller = "PublicOrder" })
                        });
                }
                else
                {
                    attributeDatas.First(item => item.Name == "CancelUrl").Value =
                        controller.Url.Action("Select", new { controller = "PublicOrder" });
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
                var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                VatNumberValidationResultType valResult = service.ValidateVatNumber(countryCode, VATNumber);

                var dataToReturn = new { sEcho, validationResult = valResult.ToString().ToLower(), error = string.Empty, success = true };
                result = Json(dataToReturn, JsonRequestBehavior.AllowGet);
                
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
                string[] ordOptions = group.OrderPageOptions.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
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
        public ActionResult Norid(string domains, string company, string orgid, string name, string time)
        {
            ViewData["domains"] = domains ?? "";
            ViewData["company"] = company ?? "";
            ViewData["orgid"] = orgid ?? "";
            ViewData["name"] = name ?? "";
            ViewData["time"] = time ?? "";
            return View();
        }

        /// <summary>
        /// Gets the transation by identifier.
        /// </summary>
        /// <param name="transactionId">The transaction identifier.</param>
        /// <returns>Requested transaction.</returns>
        private PublicPaymentTransaction GetTransationById(string transactionId)
        {

            try
            {
                var service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                return service.GetPaymentTransactionById(transactionId);
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
                return null;
            }
        }

        /// <summary>
        /// Finishes the payment.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Action result.</returns>
        private ActionResult FinishPayment(PublicPaymentTransaction transaction)
        {
            PublicPaymentTransaction finishedTransaction = null;

            try
            {
                AtomiaBillingPublicService service = GeneralHelper.GetPublicOrderService(this.HttpContext.ApplicationInstance.Context);
                service.UpdatePaymentTransactionData(
                    transaction.TransactionId,
                    transaction.Status,
                    transaction.StatusCode,
                    transaction.StatusCodeDescription,
                    transaction.Attributes.Select(a => new NameValue { Name = a.Name, Value = a.Value }).ToArray());
                finishedTransaction = service.FinishPayment(transaction.TransactionId);
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
            return this.RedirectToAction(
                "Payment",
                "PublicOrder",
                new
                {
                    amount = transaction.Amount.ToString(locale),
                    transactionReference = finishedTransaction.TransactionReference,
                    transactionReferenceType = 0,
                    status = finishedTransaction.Status
                });
        }
    }
}
