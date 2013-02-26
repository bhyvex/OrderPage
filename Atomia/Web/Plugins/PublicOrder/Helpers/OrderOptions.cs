//-----------------------------------------------------------------------
// <copyright file="OrderOptions.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Payment options.
    /// <remarks>
    /// If new needed just add another public static field with proper value.
    /// </remarks>
    /// </summary>
    public class OrderOptions
    {
        /// <summary>
        /// New domain option.
        /// </summary>
        public static readonly OrderOptions New = new OrderOptions("new");

        /// <summary>
        /// Own domain option.
        /// </summary>
        public static readonly OrderOptions Own = new OrderOptions("own");

        /// <summary>
        /// Subdomain option.
        /// </summary>
        public static readonly OrderOptions Sub = new OrderOptions("sub");

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderOptions"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected OrderOptions(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the option value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Value;
        }

        /// <summary>
        /// Gets the order option.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Order option.</returns>
        public static OrderOptions GetOrderOption(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            OrderOptions result = null;
            switch (value.ToLowerInvariant())
            {
                case "new":
                    {
                        result = New;
                        break;
                    }

                case "own":
                    {
                        result = Own;
                        break;
                    }

                case "sub":
                    {
                        result = Sub;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            return result;
        }
    }
}
