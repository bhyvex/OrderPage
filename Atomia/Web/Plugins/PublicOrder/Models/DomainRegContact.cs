// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainRegContact.cs" company="Atomia AB">
//   Copyright (C) 2010 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   The DomainRegContact model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Atomia.Web.Plugin.PublicOrder.Models
{
    /// <summary>
    /// The DomainRegContact model.
    /// </summary>
    public class DomainRegContact
    {
        /// <summary>
        /// Gets or sets the logical ID.
        /// </summary>
        /// <value>The logical ID.</value>
        public string LogicalID { get; set; }

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
        /// Gets or sets the fax extension.
        /// </summary>
        /// <value>The fax extension.</value>
        public string FaxExtension { get; set; }

        /// <summary>
        /// Gets or sets the contact's id.
        /// </summary>
        /// <value>The contact's id.</value>
        public string Id { get; set; }

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
        /// Gets or sets the state.
        /// </summary>
        /// <value>The contact's state.</value>
        public string State { get; set; }

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
        /// Gets or sets the street3.
        /// </summary>
        /// <value>The street3.</value>
        public string Street3 { get; set; }

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
        /// Gets or sets the voice extension.
        /// </summary>
        /// <value>The voice extension.</value>
        public string VoiceExtension { get; set; }

        /// <summary>
        /// Gets or sets the zip.
        /// </summary>
        /// <value>The contact's zip.</value>
        public string Zip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is reseller contact.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is reseller contact; otherwise, <c>false</c>.
        /// </value>
        public bool IsResellerContact { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary contact.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is primary contact; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrimaryContact { get; set; }

        /// <summary>
        /// Gets or sets the read only properties.
        /// </summary>
        /// <value>The read only properties.</value>
        public List<string> ReadOnlyProperties { get; set; }
    }
}