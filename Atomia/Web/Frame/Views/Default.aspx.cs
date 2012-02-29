//-----------------------------------------------------------------------
// <copyright file="Default.aspx.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Dusan Milenkovic </author>
//-----------------------------------------------------------------------

using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Atomia.Web.Frame
{
    /// <summary>
    /// Default view for application.
    /// </summary>
    public partial class _Default : Page
    {
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void Page_Load(object sender, System.EventArgs e)
        {
            // Change the current path so that the Routing handler can correctly interpret
            // the request, then restore the original path so that the OutputCache module
            // can correctly process the response (if caching is enabled).

            string originalPath = Request.Path;
            HttpContext.Current.RewritePath(Request.ApplicationPath, false);
            IHttpHandler httpHandler = new MvcHttpHandler();
            httpHandler.ProcessRequest(HttpContext.Current);
            HttpContext.Current.RewritePath(originalPath, false);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.PreInit"/> event at the beginning of page initialization.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnPreInit(System.EventArgs e)
        {
            // Session["Theme"] = "Default";

            // foreach (Atomia.Web.Base.Configs.Theme theme in Atomia.Web.Base.Configs.AppConfig.Instance.ThemesList)
            // {
            //    if (theme.isActive)
            //    {
            //        Session["Theme"] = theme.Name;
            //        break;
            //    }
            // }
            // base.OnPreInit(e);
        }
    }
}