//-----------------------------------------------------------------------
// <copyright file="PluginFileLoaderImplementer.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Atomia.Web.Base.Configs;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Base.Interfaces;
using Elmah;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Plugin File Loader Implementer.
    /// </summary>
    public class PluginFileLoaderImplementer : IPluginFileLoader
    {
        #region IPluginFileLoader Members

        /// <summary>
        /// Fetches the CSS file names.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <returns>
        /// The list of CssFile appConfig configuration items
        /// </returns>
        public System.Collections.Generic.List<Atomia.Web.Base.Configs.CssFile> FetchCssFileNames(System.Web.Routing.RouteData routeData)
        {
            List<CssFile> listToReturn = new List<CssFile>();

            string currentDir = Directory.GetCurrentDirectory();
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;

            System.Configuration.Configuration config = PluginLoaderHelper.GetConfigFile(
                PluginConfigurationHelper.GetAssemblyFilePath(codeBase), 
                PluginConfigurationHelper.GetConfigFilePath(codeBase));

            try
            {
                listToReturn.AddRange(AppConfig.GetAppConfigSection(config).CssFileList.Cast<CssFile>().Where(cssFile => ((String.IsNullOrEmpty(cssFile.Area) || !routeData.DataTokens.ContainsKey("area") || cssFile.Area == routeData.DataTokens["area"].ToString()) && (String.IsNullOrEmpty(cssFile.Controller) || !routeData.Values.ContainsKey("controller") || cssFile.Controller == routeData.Values["controller"].ToString())) && (String.IsNullOrEmpty(cssFile.Action) || !routeData.Values.ContainsKey("action") || cssFile.Action == routeData.Values["action"].ToString())));
            }
            catch (Exception e)
            {
                OrderPageLogger.LogOrderPageException(e);
            }

            Directory.SetCurrentDirectory(currentDir);

            return listToReturn;
        }

        /// <summary>
        /// Fetches the javascript file names.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <returns>
        /// The list of JavscriptFile appConfig configuration items
        /// </returns>
        public System.Collections.Generic.List<Atomia.Web.Base.Configs.JavscriptFile> FetchJavascriptFileNames(System.Web.Routing.RouteData routeData)
        {
            List<JavscriptFile> listToReturn = new List<JavscriptFile>();

            string currentDir = Directory.GetCurrentDirectory();
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;

            System.Configuration.Configuration config = PluginLoaderHelper.GetConfigFile(
                PluginConfigurationHelper.GetAssemblyFilePath(codeBase), 
                PluginConfigurationHelper.GetConfigFilePath(codeBase));

            try
            {
                listToReturn.AddRange(AppConfig.GetAppConfigSection(config).JavascriptFilesList.Cast<JavscriptFile>().Where(javascriptFile => ((String.IsNullOrEmpty(javascriptFile.Area) || !routeData.DataTokens.ContainsKey("area") || javascriptFile.Area == routeData.DataTokens["area"].ToString()) && (String.IsNullOrEmpty(javascriptFile.Controller) || !routeData.Values.ContainsKey("controller") || javascriptFile.Controller == routeData.Values["controller"].ToString())) && (String.IsNullOrEmpty(javascriptFile.Action) || !routeData.Values.ContainsKey("action") || javascriptFile.Action == routeData.Values["action"].ToString())));
            }
            catch (Exception e)
            {
               OrderPageLogger.LogOrderPageException(e);
            }

            Directory.SetCurrentDirectory(currentDir);

            return listToReturn;
        }

        #endregion
    }
}
