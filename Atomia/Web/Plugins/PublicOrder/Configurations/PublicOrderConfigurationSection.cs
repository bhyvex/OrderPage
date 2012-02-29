//-----------------------------------------------------------------------
// <copyright file="PublicOrderConfigurationSection.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

namespace Atomia.Web.Plugin.PublicOrder.Configurations
{
    /// <summary>
    /// Partial class for configuration.
    /// </summary>
    public partial class PublicOrderConfigurationSection : global::System.Configuration.ConfigurationSection
    {
        /// <summary>
        /// Gets the config section.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The OrderPage configuration section.</returns>
        public static PublicOrderConfigurationSection GetConfig(System.Configuration.Configuration configuration)
        {
            return (PublicOrderConfigurationSection)configuration.GetSection("publicOrderConfigurationSection");
        }
    }
}
