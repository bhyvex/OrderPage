//-----------------------------------------------------------------------
// <copyright file="SubmitForm.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Atomia.Common.Validation;
using Atomia.Web.Plugin.Cart.Models;
using Atomia.Web.Plugin.PublicOrder.Helpers;
using Atomia.Web.Plugin.Validation.ValidationAttributes;

namespace Atomia.Web.Plugin.PublicOrder.Models
{
    /// <summary>
    /// Submit Form class.
    /// </summary>
    public class SubmitForm
    {
        /// <summary>
        /// Gets or sets the radio you are.
        /// </summary>
        /// <value>The radio you are.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string RadioYouAre { get; set; }

        /// <summary>
        /// Gets or sets the invoice radio you are.
        /// </summary>
        /// <value>The invoice radio you are.</value>
        public string InvoiceRadioYouAre { get; set; }

        /// <summary>
        /// Gets or sets the radio payment method.
        /// </summary>
        /// <value>The radio payment method.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string RadioPaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the radio billing contact.
        /// </summary>
        /// <value>The radio billing contact.</value>
        public string RadioBillingContact { get; set; }

        /// <summary>
        /// Gets or sets the radio technical contact.
        /// </summary>
        /// <value>The radio technical contact.</value>
        public string RadioTechContact { get; set; }

        /// <summary>
        /// Gets or sets the org number.
        /// </summary>
        /// <value>The org number.</value>
        [CustomerValidation(CustomerValidationType.IdentityNumber, "CustomerValidation,IdentityNumber", CountryField = "CountryCode", ProductField = "CurrentCart.productID", ResellerIdField = "ResellerId")]
        public string OrgNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice org number.
        /// </summary>
        /// <value>The org number.</value>
        [CustomerValidation(CustomerValidationType.IdentityNumber, "CustomerValidation,IdentityNumber", CountryField = "InvoiceCountryCode", ProductField = "CurrentCart.productID", ResellerIdField = "ResellerId")]
        public string InvoiceOrgNumber { get; set; }

        /// <summary>
        /// Gets or sets the VAT number.
        /// </summary>
        /// <value>The VAT number.</value>
        [CustomerValidation(CustomerValidationType.VatNumber, "CustomerValidation,VatNumber", CountryField = "CountryCode", ProductField = "CurrentCart.productID", ResellerIdField = "ResellerId")]
        public string VATNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        /// <value>The name of the contact.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.FirstName, "CustomerValidation,FirstName")]
        public string ContactName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the contact.
        /// </summary>
        /// <value>The last name of the contact.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.LastName, "CustomerValidation,LastName")]
        public string ContactLastName { get; set; }

        /// <summary>
        /// Gets or sets the name of the invoice contact.
        /// </summary>
        /// <value>The name of the invoice contact.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.FirstName, "CustomerValidation,FirstName")]
        public string InvoiceContactName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the invoice contact.
        /// </summary>
        /// <value>The last name of the invoice contact.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.LastName, "CustomerValidation,LastName")]
        public string InvoiceContactLastName { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        [CustomerValidation(CustomerValidationType.CompanyName, "CustomerValidation,CompanyName")]
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the invoice company.
        /// </summary>
        /// <value>The invoice company.</value>
        [CustomerValidation(CustomerValidationType.CompanyName, "CustomerValidation,CompanyName")]
        public string InvoiceCompany { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Address, "CustomerValidation,address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the invoice address.
        /// </summary>
        /// <value>The invoice address.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Address, "CustomerValidation,address")]
        public string InvoiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the address2.
        /// </summary>
        /// <value>The address2.</value>
        [CustomerValidation(CustomerValidationType.Address, "CustomerValidation,address")]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the invoice address2.
        /// </summary>
        /// <value>The invoice address2.</value>
        [CustomerValidation(CustomerValidationType.Address, "CustomerValidation,address")]
        public string InvoiceAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the post number.
        /// </summary>
        /// <value>The post number.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Zip, "CustomerValidation,Zip", CountryField = "CountryCode", ApplyReplace = true)]
        public string PostNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice post number.
        /// </summary>
        /// <value>The invoice post number.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Zip, "CustomerValidation,Zip", CountryField = "InvoiceCountryCode", ApplyReplace = true)]
        public string InvoicePostNumber { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city .</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.City, "CustomerValidation,City")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the invoice city.
        /// </summary>
        /// <value>The invoice city.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.City, "CustomerValidation,City")]
        public string InvoiceCity { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>The country code.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Country, "CustomerValidation,Country")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the invoice country code.
        /// </summary>
        /// <value>The invoice country code.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Country, "CustomerValidation,Country")]
        public string InvoiceCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the telephone.
        /// </summary>
        /// <value>The telephone.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Phone, "CustomerValidation,Phone", CountryField = "CountryCode")]
        public string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        /// <value>The mobile.</value>
        [CustomerValidation(CustomerValidationType.Mobile, "CustomerValidation,Mobile", CountryField = "CountryCode")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the invoice telephone.
        /// </summary>
        /// <value>The invoice telephone.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Phone, "CustomerValidation,Phone", CountryField = "InvoiceCountryCode")]
        public string InvoiceTelephone { get; set; }

        /// <summary>
        /// Gets or sets the invoice mobile.
        /// </summary>
        /// <value>The invoice mobile.</value>
        [CustomerValidation(CustomerValidationType.Mobile, "CustomerValidation,Mobile", CountryField = "InvoiceCountryCode")]
        public string InvoiceMobile { get; set; }

        /// <summary>
        /// Gets or sets the fax.
        /// </summary>
        /// <value>The fax  .</value>
        [CustomerValidation(CustomerValidationType.Fax, "CustomerValidation,Fax", CountryField = "CountryCode")]
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets the invoice fax.
        /// </summary>
        /// <value>The invoice fax.</value>
        [CustomerValidation(CustomerValidationType.Fax, "CustomerValidation,Fax", CountryField = "InvoiceCountryCode")]
        public string InvoiceFax { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Email, "CustomerValidation,Email")]
        [AtomiaUsername("BillingResources,ErrorUsernameNotAvailable")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the invoice email.
        /// </summary>
        /// <value>The invoice email.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Email, "CustomerValidation,Email")]
        public string InvoiceEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [second address].
        /// </summary>
        /// <value><c>true</c> if [second address]; otherwise, <c>false</c>.</value>
        public bool SecondAddress { get; set; }

        /// <summary>
        /// Gets or sets the array of products.
        /// </summary>
        /// <value>The array of products.</value>
        [AtomiaRequired("ValidationErrors, ErrorNoDomain")]
        public string ArrayOfProducts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [first option].
        /// </summary>
        /// <value><c>true</c> if [first option]; otherwise, <c>false</c>.</value>
        public bool FirstOption { get; set; }

        /// <summary>
        /// Gets or sets the search domains.
        /// </summary>
        /// <value>The search domains.</value>
        public string SearchDomains { get; set; }

        /// <summary>
        /// Gets or sets the own domain.
        /// </summary>
        /// <value>The own domain.</value>
        public string OwnDomain { get; set; }

        /// <summary>
        /// Gets or sets the main domain select.
        /// </summary>
        /// <value>The main domain select.</value>
        public string MainDomainSelect { get; set; }

        /// <summary>
        /// Gets or sets the error term.
        /// </summary>
        /// <value>The error term.</value>
        public string errorTerm { get; set; }

        /// <summary>
        /// Gets or sets the order custom data.
        /// </summary>
        /// <value>The order custom data.</value>
        public string OrderCustomData { get; set; }

        /// <summary>
        /// Gets or sets the VATValidationMessage.
        /// </summary>
        public string VATValidationMessage { get; set; }

        /// <summary>
        /// Gets or sets the VATValidationMessage.
        /// </summary>
        public string DomainSpeciffic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [whois contact].
        /// </summary>
        /// <value><c>true</c> if [whois contact]; otherwise, <c>false</c>.</value>
        public bool WhoisContact { get; set; }

        [CustomFieldsValidation("CustomFields", "CustomerValidation,CustomFields", CountryField = "CountryCode", ProductField = "CurrentCart.productID", ResellerIdField = "ResellerId")]
        public IDictionary<string, string> CustomFields { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The contact's city.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.City, "CustomerValidation,City")]
        public string DomainRegCity { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Country, "CustomerValidation,Country")]
        public string DomainRegCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The contact's email.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Email, "CustomerValidation,Email")]
        public string DomainRegEmail { get; set; }

        /// <summary>
        /// Gets or sets the fax.
        /// </summary>
        /// <value>The contact's fax.</value>
        [CustomerValidation(CustomerValidationType.Fax, "CustomerValidation,Fax", CountryField = "DomainRegCountryCode")]
        public string DomainRegFax { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The contact's first name.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.FirstName, "CustomerValidation,FirstName")]
        public string DomainRegContactName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The contact's last name.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.LastName, "CustomerValidation,LastName")]
        public string DomainRegContactLastName { get; set; }

        /// <summary>
        /// Gets or sets the org.
        /// </summary>
        /// <value>The contact's organization.</value>
        [CustomerValidation(CustomerValidationType.CompanyName, "CustomerValidation,CompanyName")]
        public string DomainRegCompany { get; set; }

        /// <summary>
        /// Gets or sets the org no.
        /// </summary>
        /// <value>The org no.</value>
        [CustomerValidation(CustomerValidationType.IdentityNumber, "CustomerValidation,IdentityNumber", CountryField = "DomainRegCountryCode", ProductField = "CurrentCart.productID", ResellerIdField = "ResellerId")]
        public string DomainRegOrgNumber { get; set; }

        /// <summary>
        /// Gets or sets the VAT no.
        /// </summary>
        /// <value>The org no.</value>
        [CustomerValidation(CustomerValidationType.VatNumber, "CustomerValidation,VatNumber", CountryField = "DomainRegCountryCode", ProductField = "CurrentCart.productID", ResellerIdField = "ResellerId")]
        public string DomainRegVATNumber { get; set; }

        /// <summary>
        /// Gets or sets the street1.
        /// </summary>
        /// <value>The street1.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Address, "CustomerValidation,address")]
        public string DomainRegAddress { get; set; }

        /// <summary>
        /// Gets or sets the street2.
        /// </summary>
        /// <value>The street2.</value>
        [CustomerValidation(CustomerValidationType.Address, "CustomerValidation,address")]
        public string DomainRegAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the voice.
        /// </summary>
        /// <value>The voice.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Phone, "CustomerValidation,Phone", CountryField = "DomainRegCountryCode")]
        public string DomainRegTelephone { get; set; }

        /// <summary>
        /// Gets or sets the zip.
        /// </summary>
        /// <value>The contact's zip.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [CustomerValidation(CustomerValidationType.Zip, "CustomerValidation,Zip", CountryField = "DomainRegCountryCode", ApplyReplace = true)]
        public string DomainRegPostNumber { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [AtomiaUsernameRequired("ValidationErrors, ErrorEmptyField")]
        [AtomiaUsername("BillingResources,ErrorUsernameNotAvailable", AtomiaUsernameAttributeType.Username)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the reseller identifier.
        /// </summary>
        /// <value>The reseller identifier.</value>
        public string ResellerId
        {
            get
            {
                return ResellerHelper.GetResellerId().ToString();
            }
            set
            {
            }
        }

        public List<ProductDescription> CurrentCart
        {
            get
            {
                var currentCart = new List<ProductDescription>();
                var currentArrayOfProducts = this.ArrayOfProducts.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();

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
                if (!string.IsNullOrEmpty(this.OwnDomain))
                {
                    currentCart.Add(
                        new ProductDescription
                            {
                                productID =
                                    OrderModel.FetchOwnDomainId(
                                        ResellerHelper.GetResellerId(),
                                        null,
                                        Guid.Empty,
                                        ResellerHelper.GetResellerCurrencyCode(),
                                        ResellerHelper.GetResellerCountryCode()),
                                productDesc = this.OwnDomain
                            });
                }

                return currentCart;
            }
        }
    }
}
