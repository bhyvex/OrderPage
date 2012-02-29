//-----------------------------------------------------------------------
// <copyright file="PartialsProviderImplementer.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Base.Interfaces;
using Elmah;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Partial Provider Implementer.
    /// </summary>
    public class PartialsProviderImplementer : IPartialsProvider
    {
        #region IPartialsProvider Members

        /// <summary>
        /// Fetches the page partial items(widgets).
        /// </summary>
        /// <param name="controller">The plugin controller.</param>
        /// <param name="action">The plugin action(view)</param>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <returns>Object containing page partial items</returns>
        public object FetchPagePartialItems(string controller, string action, System.Web.Routing.RouteData routeData)
        {
            List<PartialItems> partialsList = new List<PartialItems>();

            if (String.IsNullOrEmpty(controller) || String.IsNullOrEmpty(action))
            {
                return partialsList;
            }

            if (!PluginLoaderHelper.PopulatePartialsList(Base.Configs.AppConfig.Instance.PartialsList, controller, action, routeData.DataTokens.ContainsKey("area") ? routeData.DataTokens["area"].ToString() : String.Empty, false, partialsList, null))
            {
                string currentDir = Directory.GetCurrentDirectory();
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;

                System.Configuration.Configuration config = PluginLoaderHelper.GetConfigFile(
                    PluginConfigurationHelper.GetAssemblyFilePath(codeBase), 
                    PluginConfigurationHelper.GetConfigFilePath(codeBase));

                try
                {
                    PluginLoaderHelper.PopulatePartialsList(Base.Configs.AppConfig.GetAppConfigSection(config).PartialsList, controller, action, routeData.DataTokens.ContainsKey("area") ? routeData.DataTokens["area"].ToString() : String.Empty, true, partialsList, null);
                }
                catch (Exception e)
                {
                    OrderPageLogger.LogOrderPageException(e);
                }

                Directory.SetCurrentDirectory(currentDir);
            }

            return partialsList;
        }

        /// <summary>
        /// Fetches the plugin partial items(widgets).
        /// </summary>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <returns>Object containing plugin partial items</returns>
        public object FetchPluginPartialItems(System.Web.Routing.RouteData routeData)
        {
            return null;
        }

        /// <summary>
        /// Fetches the partial items for the given page in the given placeholder(container).
        /// </summary>
        /// <param name="controller">The plugin controller.</param>
        /// <param name="action">The plugin action(view).</param>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <param name="container">The placeholder(container).</param>
        /// <returns>
        /// Object containing page partial items in the given placeholder (container)
        /// </returns>
        public object FetchPageContainerPartialItems(string controller, string action, System.Web.Routing.RouteData routeData, string container)
        {
            List<PartialItems> partialsList = new List<PartialItems>();

            if (String.IsNullOrEmpty(controller) || String.IsNullOrEmpty(action))
            {
                return partialsList;
            }

            if (!PluginLoaderHelper.PopulatePartialsList(Base.Configs.AppConfig.Instance.PartialsList, controller, action, routeData.DataTokens.ContainsKey("area") ? routeData.DataTokens["area"].ToString() : String.Empty, false, partialsList, container))
            {
                string currentDir = Directory.GetCurrentDirectory();
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;

                System.Configuration.Configuration config = PluginLoaderHelper.GetConfigFile(
                    PluginConfigurationHelper.GetAssemblyFilePath(codeBase), 
                    PluginConfigurationHelper.GetConfigFilePath(codeBase));

                try
                {
                    PluginLoaderHelper.PopulatePartialsList(Base.Configs.AppConfig.GetAppConfigSection(config).PartialsList, controller, action, routeData.DataTokens.ContainsKey("area") ? routeData.DataTokens["area"].ToString() : String.Empty, true, partialsList, container);
                }
                catch (Exception e)
                {
                   OrderPageLogger.LogOrderPageException(e);
                }

                Directory.SetCurrentDirectory(currentDir);
            }

            return partialsList;
        }

        /// <summary>
        /// Fetches the partial items for the given placeholder(container).
        /// </summary>
        /// <param name="routeData">The route data from the current executing context.</param>
        /// <param name="container">The placeholder(container).</param>
        /// <returns>
        /// Object containing plugin partial items for the given placeholder (container)
        /// </returns>
        public object FetchPluginContainerPartialItems(System.Web.Routing.RouteData routeData, string container)
        {
            return null;
        }

        #endregion
    }
}
