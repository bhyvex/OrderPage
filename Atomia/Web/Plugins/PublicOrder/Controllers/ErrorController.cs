//-----------------------------------------------------------------------
// <copyright file="ErrorController.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System.Net;
using System.Web.Mvc;
using Atomia.Web.Base.ActionFilters;
using Atomia.Web.Plugin.PublicOrder.Filters;

namespace Atomia.Web.Plugin.PublicOrder.Controllers
{
    /// <summary>
    /// Error Controller class.
    /// </summary>
    [Internationalization(InterfaceImplementer = true)]
    [ResellerDataProvider]
    [CompressResponse]
    [TranslationHelper]
    public class ErrorController : Controller
    {
        /// <summary>
        /// Action for Unknown error.
        /// </summary>
        /// <returns>View for this action.</returns>
        [UrlManagerAttribute]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult Unknown()
        {
            Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        /// <summary>
        /// Action for NotFound Error.
        /// </summary>
        /// <returns>View for this action.</returns>
        [UrlManagerAttribute]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult NotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        /// <summary>
        /// Action for NotFound Error.
        /// </summary>
        /// <returns>View for this action.</returns>
        [UrlManagerAttribute]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult ResourceNotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NoContent;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        /// <summary>
        /// Action for NotFound Error.
        /// </summary>
        /// <returns>View for this action.</returns>
        [UrlManagerAttribute]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult NoExistingDomains()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        /// <summary>
        /// Action for NotFound Error.
        /// </summary>
        /// <returns>View for this action.</returns>
        [UrlManagerAttribute]
        [PluginStuffLoader(PartialItems = true, PluginCssJsFiles = true)]
        public ActionResult NotEnoughPrivileges()
        {
            Response.StatusCode = (int)HttpStatusCode.Forbidden;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }
    }
}
