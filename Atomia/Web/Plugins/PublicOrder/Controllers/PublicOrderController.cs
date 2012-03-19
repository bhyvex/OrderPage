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
using System.Text;
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
using Atomia.Web.Plugin.PublicOrder.Models;
using Elmah;
using DomainDataFromXML = Atomia.Web.Plugin.DomainSearch.Models.DomainDataFromXml;

#endregion Using namespaces
namespace Atomia.Web.Plugin.PublicOrder.Controllers
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

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
        /// <returns>The View for this action.</returns>
        [UrlManagerAttribute]
        [AcceptVerbs(HttpVerbs.Get)]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Index()
        {
            if (this.RouteData.Values.ContainsKey("resellerHash"))
            {
                ResellerHelper.LoadResellerIntoSession((string)this.RouteData.Values["resellerHash"]);
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

                if (IndexForm.Selected == "first")
                {
                    if (string.IsNullOrEmpty(IndexForm.Domains))
                    {
                        return View();
                    }

                    string[] viewData_domains;
                    List<DomainDataFromXML> domainData;

                    // Get ViewData
                    IndexHelper.FirstOptionSelected(this, out viewData_domains, out domainData, IndexForm.Domains.Trim());

                    Session["firstOption"] = true;
                    Session["domains"] = viewData_domains;
                    Session["multiDomains"] = domainData;
                }
                else if (IndexForm.Selected == "second" && !string.IsNullOrEmpty(IndexForm.Domain))
                {
                    if (IndexForm.Domain == string.Empty)
                    {
                        return View();
                    }

                    string[] viewData_domains;
                    DomainDataFromXML domainData;

                    // Get ViewData
                    IndexHelper.SecondOptionSelected(out viewData_domains, out domainData, IndexForm.Domain.Trim(new[] { '.', ' ' }).Trim());

                    Session["firstOption"] = false;
                    Session["domains"] = viewData_domains;
                    Session["singleDomain"] = domainData;
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
                tldBasedRegexes = DomainSearch.Helpers.DomainSearchHelper.GetTLDBasedRegexes();
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
            string[] domainsSubmitted = domain.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);

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

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                ViewData["PaymentEnabled"] = paymentEnabled;

                ViewData["OrderByPostId"] = orderByPostEnabled ? OrderModel.FetchPostOrderIdFromXml(service, Guid.Empty, null, null) : string.Empty;
                ViewData["OrderByPostEnabled"] = orderByPostEnabled;

                ViewData["OrderByEmailEnabled"] = orderByEmailEnabled;

                // enabled payment method end
                ViewData["WasAnError"] = 0;
                ViewData["radioList"] = OrderModel.FetchPackagesDataFromXml(this, service, Guid.Empty, currencyCode, countryCode);
            }

            this.ViewData["OwnDomain"] = string.Empty;

            if (!((bool)this.Session["firstOption"]))
            {
                DomainDataFromXML singleDomain = (DomainDataFromXML)this.Session["singleDomain"];
                this.ViewData["Domain"] = singleDomain;
                this.ViewData["OwnDomain"] = singleDomain.ProductName;
            }

            ViewData["firstOption"] = (bool)Session["firstOption"];

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

            if (orderByEmailEnabled && opcs.InvoiceByEmail.Default)
            {
                submitForm.RadioPaymentMethod = "email";
            }
            else if (orderByPostEnabled && opcs.InvoiceByPost.Default)
            {
                submitForm.RadioPaymentMethod = "post";
            }
            else if (paymentEnabled && opcs.OnlinePayment.Default)
            {
                submitForm.RadioPaymentMethod = "card";
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
            string orderByPostId = string.Empty;
            List<string> currentArrayOfProducts;
            List<RadioRow> list;
            List<ProductDescription> currentCart;

            using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
            {
                service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();
                if (paymentEnabled)
                {
                    ViewData["PaymentEnabled"] = true;
                }
                else
                {
                    ViewData["PaymentEnabled"] = false;
                }

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

                for (int i = 0; i < currentArrayOfProducts.Count; i += 2)
                {
                    currentCart.Add(new ProductDescription { productID = currentArrayOfProducts[i], productDesc = currentArrayOfProducts[i + 1] });
                }

                if (!SubmitForm.FirstOption)
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

                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();

                    List<string> allPackagesIds = OrderModel.FetchAllPackagesIdsDataFromXml(service, Guid.Empty, null, null);
                    ProductDescription selectedPackage = currentCart.Find(p => allPackagesIds.Any(x => x == p.productID));
                    string setupFeeId = OrderModel.FetchSetupFeeIdFromXml(service, Guid.Empty, null, null);

                    if (!currentCart.Any(item => item.productID != setupFeeId && item.productID != selectedPackage.productID))
                    {
                        // There is only package (and setup fee) in the cart, raise ex that domain is needed as well
                        throw new AtomiaServerSideValidationException("ArrayOfProducts", this.GlobalResource("ValidationErrors, ErrorNoDomain"), SubmitForm);
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
                            tmpString = tmpString.Replace("-", string.Empty);
                            tmpString = tmpString.Insert(6, "-");
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
                        ProductDescription selectedPackage = currentCart.Find(p => allPackagesIds.Any(x => x == p.productID));
                        List<ProductItem> freePackageId = OrderModel.FetchFreePackageIdFromXml(service, Guid.Empty, null, null);
                        string setupFeeId = OrderModel.FetchSetupFeeIdFromXml(service, Guid.Empty, null, null);

                        List<PublicOrderItem> myOrderItems = new List<PublicOrderItem>();

                        foreach (ProductDescription tmpProduct in currentCart)
                        {
                            // if it's setup fee, continue (it will be added with package product if needed)
                            if (tmpProduct.productID == setupFeeId)
                            {
                                continue;
                            }

                            // If post invoice is selected do not add it to orderItems since that product is added via orderCustomAttributes
                            if (SubmitForm.RadioPaymentMethod == "post")
                            {
                                if (tmpProduct.productID == orderByPostId)
                                {
                                    continue;
                                }
                            }

                            if (tmpProduct.productID != selectedPackage.productID)
                            {
                                // it's domain
                                List<PublicOrderItemProperty> arrayOfCustoms = new List<PublicOrderItemProperty>();
                                if (freePackageId.Exists(f => f.ArticalNumber == selectedPackage.productID))
                                {
                                    // if the selected package in the cart is free
                                    string domainValue = SubmitForm.FirstOption ? tmpProduct.productDesc : (SubmitForm.OwnDomain.StartsWith("www") ? SubmitForm.OwnDomain.Remove(0, 4) : SubmitForm.OwnDomain);

                                    arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainName", Value = domainValue });

                                    arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = "CsDomainParking" });

                                    PublicOrderItem tmpItem = new PublicOrderItem
                                    {
                                        ItemId = Guid.Empty,
                                        ItemNumber = tmpProduct.productID,
                                        Quantity = 1,
                                        CustomData = arrayOfCustoms.ToArray()
                                    };

                                    myOrderItems.Add(tmpItem);
                                }
                                else
                                {
                                    // if the selected package in the cart is not free
                                    if (SubmitForm.FirstOption)
                                    {
                                        // check if the current product is main domain
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainName", Value = tmpProduct.productDesc });

                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = tmpProduct.productDesc == SubmitForm.MainDomainSelect ? "CsLinuxWebsite" : "CsDomainParking" });
                                    }
                                    else
                                    {
                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "DomainName", Value = SubmitForm.OwnDomain.StartsWith("www") ? SubmitForm.OwnDomain.Remove(0, 4) : SubmitForm.OwnDomain });

                                        arrayOfCustoms.Add(new PublicOrderItemProperty { Name = "AtomiaService", Value = "CsLinuxWebsite" });
                                    }

                                    PublicOrderItem tmpItem = new PublicOrderItem
                                    {
                                        ItemId = Guid.Empty,
                                        ItemNumber = tmpProduct.productID,
                                        Quantity = 1,
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
                                        Quantity = 1
                                    };

                                    myOrderItems.Add(tmpItem);


                                    
                                    // check if there's setup fee to be added
                                    if (!String.IsNullOrEmpty(setupFeeId))
                                    {
                                        tmpItem = new PublicOrderItem
                                                      {
                                                        ItemId = Guid.Empty, 
                                                        ItemNumber = setupFeeId, 
                                                        Quantity = 1
                                                      };

                                        myOrderItems.Add(tmpItem);
                                    }
                                }
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

                        myOrder.PaymentMethod = SubmitForm.RadioPaymentMethod == "card" ? Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService.PaymentMethodEnum.PayByCard : Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService.PaymentMethodEnum.PayByInvoice;

                        newOrder = service.CreateOrder(myOrder);

                        if (newOrder == null)
                        {
                            throw new Exception("Order could not be created.");
                        }

                        this.Session["CreatedOrder"] = newOrder;
                    }

                    if (SubmitForm.RadioPaymentMethod == "card")
                    {
                        paymentMethodCc = true;

                        string result = this.CreatePaymentTransaction(this, newOrder, newOrder.Total);

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
                ViewData["Domain"] = singleDomain;
                ViewData["firstOption"] = (bool)Session["firstOption"];
                ViewData["OwnDomain"] = singleDomain.ProductName;
            }

            string toSend = string.Empty;
            for (int i = 0; i < currentCart.Count; i++)
            {
                if (i < currentCart.Count - 1)
                {
                    toSend += currentCart[i].productID + "|" + currentCart[i].productDesc + "|";
                }
                else
                {
                    toSend += currentCart[i].productID + "|" + currentCart[i].productDesc;
                }
            }

            // Needed for Cart partial
            ViewData["CartProducts"] = toSend;

            ViewData["radioList"] = list;

            // Needed for Cart partial
            ViewData["switchedId"] = list[0].productId + "|" + list[0].productNameDesc;

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
               string emailDomain = (email.Split('@'))[1];
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

            if (status.ToUpper() == "OK")
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
                validated = Regex.IsMatch(finalDomainName, Atomia.Common.RegularExpression.GetRegularExpression("EncodedDomain"));
            }
            catch (Exception ex)
            {
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
        public ActionResult RecalculateCart(string arrayOfProducts, string arrayOfProductNames, string arrayOfProductQuantities, bool displayProductName, bool displayProductPeriod, bool displayProductNumberOfItems, bool displayProductPrice, bool displayProductDiscount, bool displayProductTotalPrice, bool displayOrderSubAmount, bool displayOrderTaxes, bool displayOrderTotal, string chosenCountry, int globalCounter, string campaignCode, bool pricesIncludingVAT, string orderCustomAttributes, string orderAddress)
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

        /// <summary>
        /// Creates the payment transaction.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="order">The order.</param>
        /// <param name="paidAmount">The paid amount.</param>
        /// <returns>Creation of transaction success</returns>
        private string CreatePaymentTransaction(Controller controller, Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService.PublicOrder order, decimal paidAmount)
        {
            PublicPaymentTransaction transaction = PaymentHelper.FillPaymentTransactionForOrder(order, Request, paidAmount);

            string action = controller.Url.Action("Payment", new { controller = "PublicOrder" });

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

                using (Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService.AtomiaBillingPublicService publicOrderService = new AtomiaBillingPublicService())
                {

                    publicOrderService.Url = this.HttpContext.Application["OrderApplicationPublicServiceURL"].ToString();                                    

                    VatNumberValidationResultType valResult = publicOrderService.ValidateVatNumber(
                        countryCode, VATNumber);

                    var dataToReturn =
                        new { sEcho, validationResult = valResult.ToString().ToLower(), error = "", success = true };
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
    }
}