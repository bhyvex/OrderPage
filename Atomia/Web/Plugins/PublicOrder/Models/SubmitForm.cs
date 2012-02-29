//-----------------------------------------------------------------------
// <copyright file="SubmitForm.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using Atomia.Web.Base.Validation.ValidationAttributes;

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
        public string RadioPaymentMethod { get; set; }

        /// <summary>
        /// Gets or sets the org number.
        /// </summary>
        /// <value>The org number.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        public string OrgNumber { get; set; }

        /// <summary>
        /// Gets or sets the VAT number.
        /// </summary>
        /// <value>The VAT number.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        public string VATNumber { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact.
        /// </summary>
        /// <value>The name of the contact.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string ContactName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the contact.
        /// </summary>
        /// <value>The last name of the contact.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string ContactLastName { get; set; }

        /// <summary>
        /// Gets or sets the name of the invoice contact.
        /// </summary>
        /// <value>The name of the invoice contact.</value>
        public string InvoiceContactName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the invoice contact.
        /// </summary>
        /// <value>The last name of the invoice contact.</value>
        public string InvoiceContactLastName { get; set; }

        /// <summary>
        /// Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        public string Company { get; set; }

        /// <summary>
        /// Gets or sets the invoice company.
        /// </summary>
        /// <value>The invoice company.</value>
        public string InvoiceCompany { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the invoice address.
        /// </summary>
        /// <value>The invoice address.</value>
        public string InvoiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the address2.
        /// </summary>
        /// <value>The address2.</value>
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the invoice address2.
        /// </summary>
        /// <value>The invoice address2.</value>
        public string InvoiceAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the post number.
        /// </summary>
        /// <value>The post number.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string PostNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice post number.
        /// </summary>
        /// <value>The invoice post number.</value>
        public string InvoicePostNumber { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city .</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the invoice city.
        /// </summary>
        /// <value>The invoice city.</value>
        public string InvoiceCity { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>The country code.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the invoice country code.
        /// </summary>
        /// <value>The invoice country code.</value>
        public string InvoiceCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the telephone.
        /// </summary>
        /// <value>The telephone.</value>
        [AtomiaStringLength(20, "ValidationErrors, ErrorStringLength")]
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the mobile.
        /// </summary>
        /// <value>The mobile.</value>
        [AtomiaStringLength(20, "ValidationErrors, ErrorStringLength")]
        public string Mobile { get; set; }

        /// <summary>
        /// Gets or sets the invoice telephone.
        /// </summary>
        /// <value>The invoice telephone.</value>
        [AtomiaStringLength(20, "ValidationErrors, ErrorStringLength")]
        public string InvoiceTelephone { get; set; }

        /// <summary>
        /// Gets or sets the invoice mobile.
        /// </summary>
        /// <value>The invoice mobile.</value>
        [AtomiaStringLength(20, "ValidationErrors, ErrorStringLength")]
        public string InvoiceMobile { get; set; }

        /// <summary>
        /// Gets or sets the telephone processed.
        /// </summary>
        /// <value>The telephone processed.</value>
        public string TelephoneProcessed { get; set; }

        /// <summary>
        /// Gets or sets the mobile processed.
        /// </summary>
        /// <value>The mobile processed.</value>
        public string MobileProcessed { get; set; }

        /// <summary>
        /// Gets or sets the invoice mobile processed.
        /// </summary>
        /// <value>The invoice mobile processed.</value>
        public string InvoiceMobileProcessed { get; set; }

        /// <summary>
        /// Gets or sets the invoice telephone processed.
        /// </summary>
        /// <value>The invoice telephone processed.</value>
        public string InvoiceTelephoneProcessed { get; set; }

        /// <summary>
        /// Gets or sets the fax processed.
        /// </summary>
        /// <value>The fax processed.</value>
        public string FaxProcessed { get; set; }

        /// <summary>
        /// Gets or sets the fax.
        /// </summary>
        /// <value>The fax  .</value>
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets the invoice fax processed.
        /// </summary>
        /// <value>The invoice fax processed.</value>
        public string InvoiceFaxProcessed { get; set; }

        /// <summary>
        /// Gets or sets the invoice fax.
        /// </summary>
        /// <value>The invoice fax.</value>
        public string InvoiceFax { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        [AtomiaRegularExpression("Email", "ValidationErrors, ErrorInvalidEmail", true)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the invoice email.
        /// </summary>
        /// <value>The invoice email.</value>
        [AtomiaStringLength(255, "ValidationErrors, ErrorStringLength")]
        [AtomiaRegularExpression("Email", "ValidationErrors, ErrorInvalidEmail", true)]
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
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
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

    }
}
