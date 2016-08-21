using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using Atomia.Web.Base.ActionTrail;
using Atomia.Web.Base.AuditLog;

namespace Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail
{
    /// <summary>
    /// The helper for logging exceptions in the Order Page.
    /// </summary>
    public class OrderPageLogger
    {
        /// <summary>
        /// Logs the order page exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public static void LogOrderPageException(Exception ex)
        {
            var shortMessage = string.Format("Atomia Order Page threw an exception.\r\n {0}", ex.Message + "\r\n" + ex.StackTrace);

            LogOrderPageException(ex, shortMessage);
        }

        /// <summary>
        /// Logs the order page exception.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="shortMessage">The short message.</param>
        public static void LogOrderPageException(Exception ex, string shortMessage)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            if (HttpContext.Current.Application["ActionTrailPowerSwitch"] != null)
            {
                bool actionTrailPowerSwitch;
                if (Boolean.TryParse(HttpContext.Current.Application["ActionTrailPowerSwitch"].ToString().ToLower(), out actionTrailPowerSwitch))
                {
                    if (actionTrailPowerSwitch)
                    {
                        string accountId = null;

                        var user = Thread.CurrentPrincipal.Identity.IsAuthenticated ? Thread.CurrentPrincipal.Identity.Name : null;

                        WebBaseLogger.CreateActionTrailExceptionLog(ex, "Atomia Order Page", accountId, user, shortMessage, new List<string> { "Atomia Order Page Exceptions" });
                    }
                }
            }

            if (HttpContext.Current.Application["ElmahPowerSwitch"] == null)
            {
                return;
            }

            bool elmahPowerSwitch;
            if (!Boolean.TryParse(HttpContext.Current.Application["ElmahPowerSwitch"].ToString().ToLower(), out elmahPowerSwitch))
            {
                return;
            }

            if (elmahPowerSwitch)
            {
                WebBaseLogger.CreateElmahExceptionLog(ex);
            }
        }

        /// <summary>
        /// Logs the audit.
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="message">The message.</param>
        /// <param name="accountId">The account identifier.</param>
        /// <param name="username">The username.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="details">The details.</param>
        public static void LogAudit(
            string actionType,
            string message,
            string accountId,
            string username,
            string objectId,
            Dictionary<string, object> details)
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            if (HttpContext.Current.Application["AuditLogPowerSwitch"] != null)
            {
                bool auditLogPowerSwitch;
                if (bool.TryParse(HttpContext.Current.Application["AuditLogPowerSwitch"].ToString().ToLower(), out auditLogPowerSwitch))
                {
                    if (auditLogPowerSwitch)
                    {
                        WebBaseAuditLogger.CreateAuditLog("Atomia Order Page", actionType, message, accountId, username, objectId, details);
                    }
                }
            }
        }
    }
}