//-----------------------------------------------------------------------
// <copyright file="PaymentHelper.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Atomia.Billing.Core.Common.PaymentPlugins;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Payment helper
    /// </summary>
    public class PaymentHelper
    {
        /// <summary>
        /// Fills the payment transaction for invoice.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="request">The request.</param>
        /// <param name="paidAmount">The paid amount.</param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <returns>PaymentTransaction object</returns>
        public static PublicPaymentTransaction FillPaymentTransactionForOrder(OrderServiceReferences.AtomiaBillingPublicService.PublicOrder order, HttpRequestBase request, decimal paidAmount, string paymentMethod)
        {
            // Fill transaction
            var paymentTransaction = new PublicPaymentTransaction { GuiPluginName = paymentMethod };
            List<AttributeData> attrData = new List<AttributeData>();
            foreach (var key in GuiPaymentPluginRequestHelper.RequestToCustomAttributes(request))
            {
                attrData.Add(new AttributeData { Name = key.Key, Value = key.Value });
            }
            
            paymentTransaction.Attributes = attrData.ToArray();
            paymentTransaction.CurrencyCode = order.Currency;
            paymentTransaction.TransactionReference = order.Number;
            paymentTransaction.Amount = paidAmount;

            if (paymentTransaction.Amount <= decimal.Zero)
            {
                throw new Exception(String.Format("Total:{0}, paidAmmount:{1}, OrderNumber:{2}", order.Total, paidAmount, order.Number));
            }

            return paymentTransaction;
        }

        /// <summary>
        /// Creates the payment transaction.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="order">The order.</param>
        /// <param name="paidAmount">The paid amount.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Success of creation.</returns>
        public static string CreatePaymentTransaction(Controller controller, OrderServiceReferences.AtomiaBillingPublicService.PublicOrder order, decimal paidAmount, string returnUrl, PublicPaymentTransaction transaction)
        {
            // set the return URL, if redirection outside our application is required
            List<string> tmpList = returnUrl.TrimStart('/').Split('/').ToList();
            var currentUrl = controller.Url.RequestContext.HttpContext.Request.Url;
            string appUrl = currentUrl == null ? controller.HttpContext.Application["OrderApplicationRawURL"].ToString() : string.Format("{0}://{1}/",
                                                  currentUrl.Scheme,
                                                  currentUrl.Authority);
            
            if (appUrl.EndsWith(tmpList[0]))
            {
                tmpList.RemoveAt(0);
            }

            returnUrl = string.Empty;
            for (int i = 0; i < tmpList.Count; i++)
            {
                if (i < tmpList.Count - 1)
                {
                    returnUrl += tmpList[i] + '/';
                }
                else
                {
                    returnUrl += tmpList[i];
                }
            }

            transaction.ReturnUrl = appUrl.TrimEnd(new[] { '/' }) + '/' + returnUrl.TrimStart(new[] { '/' });
            
            PublicPaymentTransaction returnedTransaction;

            var service = GeneralHelper.GetPublicOrderService(controller.HttpContext.ApplicationInstance.Context);
            returnedTransaction = service.MakePayment(transaction);
            
            if (returnedTransaction.Status.ToUpper() == "IN_PROGRESS" && !string.IsNullOrEmpty(returnedTransaction.RedirectUrl))
            {
                return returnedTransaction.RedirectUrl;
            }

            if (returnedTransaction.Status.ToUpper() == "OK")
            {
                return String.Empty;
            }

            return returnedTransaction.Status.ToUpper() == "FRAUD_DETECTED"
                       ? controller.Url.Action("PaymentFailed")
                       : transaction.ReturnUrl;
        }
    }
}
