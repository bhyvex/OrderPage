//-----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Dusan Milenkovic </author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Reflection;
using System.Web;
using Atomia.Web.Base.Configs;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Base.InterfaceConfigs;
using Elmah;

namespace Atomia.Web.Frame
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// The MvcApplication class
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MvcApplication"/> class.
        /// </summary>
        public MvcApplication()
        {
            this.HandlerClass = null;

            try
            {
                if (AtomiaWebBaseInterfaceConfig.Instance != null && AtomiaWebBaseInterfaceConfig.Instance.InterfaceList != null)
                {
                    GlobalEventDefaultHandler configHandler = AtomiaWebBaseInterfaceConfig.Instance.InterfaceList.GlobalEventDefaultHandler;

                    if (configHandler != null)
                    {
                        Type t = Type.GetType(string.Format("{0}.{1}, {2}", configHandler.ClassNamespace, configHandler.ClassName, configHandler.ClassAssembly));
                        if (t != null)
                        {
                            this.HandlerClass = (GlobalEventsDefaultHandler)Activator.CreateInstance(t);
                        }
                        else
                        {
                            Assembly pluginAssembly = GetPluginAssembly(configHandler);

                            if (pluginAssembly != null)
                            {
                                foreach (Type pluginAssemblyType in pluginAssembly.GetTypes().Where(pluginAssemblyType => pluginAssemblyType.Namespace == configHandler.ClassNamespace && pluginAssemblyType.Name == configHandler.ClassName))
                                {
                                    this.HandlerClass = (GlobalEventsDefaultHandler)Activator.CreateInstance(pluginAssemblyType);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OrderPageLogger.LogOrderPageException(e);
            }

            if (this.HandlerClass == null)
            {
                this.HandlerClass = new GlobalEventsDefaultHandler();
            }
        }

        /// <summary>
        /// Gets or sets the handler class.
        /// </summary>
        /// <value>The handler class.</value>
        private GlobalEventsDefaultHandler HandlerClass { get; set; }

        /// <summary>
        /// Handles the Start event of the Session control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Session_Start(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Session_Start(sender, e);
            }
        }

        /// <summary>
        /// Handles the Start event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Start(object sender, EventArgs e)
        {
            if (this.HandlerClass == null)
            {
                return;
            }

            this.HandlerClass.Application_Start(sender, e);
        }

        /// <summary>
        /// Handles the PreRequestHandlerExecute event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_PreRequestHandlerExecute(sender, e);
            }
        }

        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_BeginRequest(sender, e);
            }
        }

        /// <summary>
        /// Handles the Error event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_Error(sender, e);
            }
        }

        /// <summary>
        /// Handles the Init event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Init(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_Init(sender, e);
            }
        }

        /// <summary>
        /// Handles the Disposed event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_Disposed(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_Disposed(sender, e);
            }
        }

        /// <summary>
        /// Handles the End event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_End(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_End(sender, e);
            }
        }

        /// <summary>
        /// Handles the EndRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_EndRequest(sender, e);
            }
        }

        /// <summary>
        /// Handles the PostRequestHandlerExecute event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_PostRequestHandlerExecute(sender, e);
            }
        }

        /// <summary>
        /// Handles the PreSendRequestHeaders event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_PreSendRequestHeaders(sender, e);
            }
        }

        /// <summary>
        /// Handles the PreSendContent event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_PreSendContent(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_PreSendContent(sender, e);
            }
        }

        /// <summary>
        /// Handles the AcquireRequestState event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_AcquireRequestState(sender, e);
            }
        }

        /// <summary>
        /// Handles the ReleaseRequestState event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_ReleaseRequestState(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_ReleaseRequestState(sender, e);
            }
        }

        /// <summary>
        /// Handles the ResolveRequestCache event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_ResolveRequestCache(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_ResolveRequestCache(sender, e);
            }
        }

        /// <summary>
        /// Handles the UpdateRequestCache event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_UpdateRequestCache(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_UpdateRequestCache(sender, e);
            }
        }

        /// <summary>
        /// Handles the AuthenticateRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_AuthenticateRequest(sender, e);
            }
        }

        /// <summary>
        /// Handles the AuthorizeRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Application_AuthorizeRequest(sender, e);
            }
        }

        /// <summary>
        /// Handles the End event of the Session control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Session_End(object sender, EventArgs e)
        {
            if (this.HandlerClass != null)
            {
                this.HandlerClass.Session_End(sender, e);
            }
        }

        /// <summary>
        /// Gets the plugin assembly.
        /// </summary>
        /// <param name="configHandler">The config handler.</param>
        /// <returns>The found plugin assembly.</returns>
        private static Assembly GetPluginAssembly(GlobalEventDefaultHandler configHandler)
        {
            if (HttpContext.Current != null && HttpContext.Current.Server != null)
            {
                GlobalEventsDefaultHandler globalEvents = new GlobalEventsDefaultHandler();

                string folderPath = HttpContext.Current.Server.MapPath("~/" + AppConfig.Instance.WebFramePlugins.Path.Trim(new[] { '/', '\\', ' ' }));

                string binFolderPath = HttpContext.Current.Server.MapPath("~/bin");

                globalEvents.PluginAssemblyLoader(folderPath, binFolderPath);
            }

            Assembly pluginAssembly = null;

            if (AppDomain.CurrentDomain.GetAssemblies().Any(asmbl => asmbl.FullName == configHandler.ClassAssembly))
            {
                pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().First(asmbl => asmbl.FullName == configHandler.ClassAssembly);
            }
            else if (!configHandler.ClassAssembly.Contains("Version") && !configHandler.ClassAssembly.Contains("Culture") && !configHandler.ClassAssembly.Contains("PublicKeyToken") && AppDomain.CurrentDomain.GetAssemblies().Any(asmbl => !String.IsNullOrEmpty(asmbl.FullName) && asmbl.FullName.StartsWith(configHandler.ClassAssembly)))
            {
                pluginAssembly = AppDomain.CurrentDomain.GetAssemblies().First(asmbl => !String.IsNullOrEmpty(asmbl.FullName) && asmbl.FullName.StartsWith(configHandler.ClassAssembly));
            }

            return pluginAssembly;
        }
    }
}