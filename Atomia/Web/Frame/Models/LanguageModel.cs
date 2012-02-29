//-----------------------------------------------------------------------
// <copyright file="LanguageModel.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

namespace Atomia.Web.Frame.Models
{
    /// <summary>
    /// Language model class.
    /// </summary>
    public class LanguageModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name string .</value>
        public string Name { get; set; }

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
