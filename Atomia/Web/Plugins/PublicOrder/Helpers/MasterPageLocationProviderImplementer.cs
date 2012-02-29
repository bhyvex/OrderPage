//-----------------------------------------------------------------------
// <copyright file="MasterPageLocationProviderImplementer.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Reflection;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Base.Interfaces;
using Elmah;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Implements location finding of the Site.Master for this plugin
    /// </summary>
    public class MasterPageLocationProviderImplementer : IMasterPageLocationProvider
    {
        #region IMasterPageLocationProvider Members

        /// <summary>
        /// Fetches the master page location for a given plugin page(view).
        /// </summary>
        /// <param name="controller">The plugin controller.</param>
        /// <param name="action">The plugin action(view).</param>
        /// <param name="area">The plugin area.</param>
        /// <param name="location">The location of the master page (relative to the Theme\{theme_name} folder).</param>
        /// <returns>
        /// Boolean - whether the master page location was defined in the plugin
        /// </returns>
        public bool FetchMasterPageLocation(string controller, string action, string area, out string location)
        {
            if (String.IsNullOrEmpty(controller) || String.IsNullOrEmpty(action))
            {
                location = String.Empty;
                return false;
            }

            if (!PluginLoaderHelper.PopulateMasterPageLocation(Base.Configs.AppConfig.Instance.MasterPageLocationsList, controller, action, area, false, out location))
            {
                // we didn't find the master page location in global config file, trying to find in the plugin config file
                string currentDir = Directory.GetCurrentDirectory();
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;

                System.Configuration.Configuration config = PluginLoaderHelper.GetConfigFile(
                    PluginConfigurationHelper.GetAssemblyFilePath(codeBase), 
                    PluginConfigurationHelper.GetConfigFilePath(codeBase));

                bool foundLocation = false;

                try
                {
                    foundLocation = PluginLoaderHelper.PopulateMasterPageLocation(Base.Configs.AppConfig.GetAppConfigSection(config).MasterPageLocationsList, controller, action, area, true, out location);
                }
                catch (Exception e)
                {
                    OrderPageLogger.LogOrderPageException(e);
                }
                finally
                {
                    Directory.SetCurrentDirectory(currentDir);
                }

                return foundLocation;
            }

            return true;
        }

        #endregion
    }
}
