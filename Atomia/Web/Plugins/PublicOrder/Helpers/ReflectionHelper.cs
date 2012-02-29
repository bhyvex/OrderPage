//-----------------------------------------------------------------------
// <copyright file="ReflectionHelper.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elmah;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Reflection Helper.
    /// </summary>
    public class ReflectionHelper : Controller
    {
        /// <summary>
        /// Gets the client specific resource.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="resourceKey">The resource key.</param>
        /// <returns>Resource value.</returns>
        public string GetClientSpecificResource(Controller controller, string resourceKey)
        {
            try
            {
                string pluginNamespace = "Atomia.Web.Plugin.";
                string theme = controller.Session["Theme"].ToString();
                string methodName = "GetClientSpecificResource";
                List<object> methodParameters = new List<object> { controller, resourceKey };
                string defaultPluginNamespace = "Atomia.Web.Plugin.Default";

                return (string)ReflectMethod(pluginNamespace, theme, methodName, methodParameters, defaultPluginNamespace);
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
                throw;
            }
        }

        /// <summary>
        /// Reflects the method for specified client (theme).
        /// </summary>
        /// <param name="pluginNamespace">The plugin namespace.</param>
        /// <param name="theme">The theme.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="methodParameters">The method parameters.</param>
        /// <param name="defaultPluginNamespace">The default plugin namespace.</param>
        /// <returns>Action result.</returns>
        private static object ReflectMethod(string pluginNamespace, string theme, string methodName, List<object> methodParameters, string defaultPluginNamespace)
        {
            // Get plugin assembly for specific client containing method for check
            System.Reflection.Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().ToList().Find(a => a.FullName.Contains(pluginNamespace + theme)) ??
                                                  AppDomain.CurrentDomain.GetAssemblies().ToList().Find(a => a.FullName.Contains(defaultPluginNamespace));

            foreach (Type t in assembly.GetTypes())
            {
                var typeInstance = assembly.CreateInstance(t.FullName);
                var methodInfo = t.GetMethod(methodName);

                if (methodInfo != null && typeInstance != null)
                {
                    return (object)methodInfo.Invoke(typeInstance, methodParameters.ToArray());
                }

                // If method does not exist in client plugin, use default
                if (assembly.FullName != defaultPluginNamespace)
                {
                    assembly = AppDomain.CurrentDomain.GetAssemblies().ToList().Find(a => a.FullName.Contains(defaultPluginNamespace));
                    foreach (Type td in assembly.GetTypes())
                    {
                        typeInstance = assembly.CreateInstance(td.FullName);
                        methodInfo = td.GetMethod(methodName);

                        if (methodInfo != null && typeInstance != null)
                        {
                            return (object)methodInfo.Invoke(typeInstance, methodParameters.ToArray());
                        }
                    }
                }

                throw new NullReferenceException(String.Format("Assembly:{0}, Method:{1}", pluginNamespace + theme, methodName));
            }

            return null;
        }
    }
}
