//-----------------------------------------------------------------------
// <copyright file="LanguagePickerFormData.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

namespace Atomia.Web.Frame.Models
{
    /// <summary>
    /// Form data.
    /// </summary>
    public class LanguagePickerFormData
    {
        /// <summary>
        /// Gets or sets the return URL.
        /// </summary>
        /// <value>The return URL.</value>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>The languages.</value>
        public LanguageModel[] Languages { get; set; }

        /// <summary>
        /// Gets or sets the selected language.
        /// </summary>
        /// <value>The selected language.</value>
        public string SelectedLanguage { get; set; }
    }
}
