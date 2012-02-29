//-----------------------------------------------------------------------
// <copyright file="PaymentFormData.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

namespace Atomia.Web.Plugin.PublicOrder.Models
{
    /// <summary>
    /// Form data class for payments
    /// </summary>
    public class PaymentFormData
    {
        /// <summary>
        /// Gets or sets the payment date.
        /// </summary>
        /// <value>The payment date.</value>
        public string PaymentDate { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        /// <value>The currency.</value>
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the amount payed.
        /// </summary>
        /// <value>The amount payed.</value>
        public string AmountPayed { get; set; }
    }
}
