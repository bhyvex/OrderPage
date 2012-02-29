//-----------------------------------------------------------------------
// <copyright file="IndexForm.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using Atomia.Web.Base.Validation.ValidationAttributes;

namespace Atomia.Web.Plugin.PublicOrder.Models
{
    /// <summary>
    /// Index form class.
    /// </summary>
    public class IndexForm
    {
        /// <summary>
        /// Gets or sets the selected.
        /// </summary>
        /// <value>The selected.</value>
        [AtomiaRequired("ValidationErrors, ErrorEmptyField")]
        public string Selected { get; set; }

        /// <summary>
        /// Gets or sets the domains.
        /// </summary>
        /// <value>The domains.</value>
        public string Domains { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>The domain.</value>
        [AtomiaStringLength(55, "ValidationErrors, ErrorStringLength")]
        public string Domain { get; set; }
    }
}
