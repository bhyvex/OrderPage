// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainRegContact.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   The DomainRegContact model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atomia.Web.Plugin.PublicOrder.Models
{
    /// <summary>
    /// The DomainRegContact model used for serializing DomainRegContact fields on SubmitForm to JSON.
    /// </summary>
    public class DomainRegContact
    {
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The contact's city.</value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The contact's email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the fax.
        /// </summary>
        /// <value>The contact's fax.</value>
        public string Fax { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The contact's name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the org.
        /// </summary>
        /// <value>The contact's organization.</value>
        public string Org { get; set; }

        /// <summary>
        /// Gets or sets the org no.
        /// </summary>
        /// <value>The org no.</value>
        public string OrgNo { get; set; }

        /// <summary>
        /// Gets or sets the street1.
        /// </summary>
        /// <value>The street1.</value>
        public string Street1 { get; set; }

        /// <summary>
        /// Gets or sets the street2.
        /// </summary>
        /// <value>The street2.</value>
        public string Street2 { get; set; }

        /// <summary>
        /// Gets or sets the vat no.
        /// </summary>
        /// <value>The vat no.</value>
        public string VatNo { get; set; }

        /// <summary>
        /// Gets or sets the voice.
        /// </summary>
        /// <value>The voice.</value>
        public string Voice { get; set; }

        /// <summary>
        /// Gets or sets the zip.
        /// </summary>
        /// <value>The contact's zip.</value>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets serialized dictionary of custom fields.
        /// </summary>
        /// <value>The custom fields.</value>
        public string CustomFields { get; set; }
    }
}