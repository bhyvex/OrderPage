//-----------------------------------------------------------------------
// <copyright file="LocalConfigurationHelper.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Plugin.PublicOrder.Configurations;
using Elmah;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Helper class.
    /// </summary>
    public class LocalConfigurationHelper
    {
        /// <summary>
        /// Gets the local configuration section.
        /// </summary>
        /// <returns>Configuration section.</returns>
        public static PublicOrderConfigurationSection GetLocalConfigurationSection()
        {
            PublicOrderConfigurationSection result = null;

            string currentDir = Directory.GetCurrentDirectory();
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;

            System.Configuration.Configuration config = PluginLoaderHelper.GetConfigFile(
                PluginConfigurationHelper.GetAssemblyFilePath(codeBase), 
                PluginConfigurationHelper.GetConfigFilePath(codeBase));

            try
            {
                result = PublicOrderConfigurationSection.GetConfig(config);

                Directory.SetCurrentDirectory(currentDir);
            }
            catch (Exception e)
            {
                OrderPageLogger.LogOrderPageException(e);
            }

            return result;
        }
    }
}
