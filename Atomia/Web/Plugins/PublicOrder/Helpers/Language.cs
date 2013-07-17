// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Language.cs" company="Atomia AB">
//   Copyright (C) 2013 Atomia AB. All rights reserved
// </copyright>
// <summary>
//   Language representation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Language representation.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code string .</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }
    }
}