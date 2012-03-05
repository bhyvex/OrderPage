//-----------------------------------------------------------------------
// <copyright file="CountriesHelper.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Atomia.Web.Plugin.PublicOrder.Configurations;

namespace Atomia.Web.Plugin.PublicOrder.Helpers
{
    /// <summary>
    /// Helper Class.
    /// </summary>
    public class CountriesHelper
    {
        /// <summary>
        /// Gets the default country code from config.
        /// </summary>
        /// <returns>Default code.</returns>
        public static string GetDefaultCountryCodeFromConfig()
        {
            PublicOrderConfigurationSection opcs = LocalConfigurationHelper.GetLocalConfigurationSection();

            return opcs.DefaultCountry.Code;
        }

        /// <summary>
        /// Gets the default currency code from config.
        /// </summary>
        /// <remarks>
        /// If no code is set to default, currency code is determined by country code.
        /// </remarks>
        /// <param name="countryCode">The country code.</param>
        /// <returns>Default currency code.</returns>
        public static string GetDefaultCurrencyCodeFromConfig(string countryCode)
        {
            string currencyCode = string.Empty;
            PublicOrderConfigurationSection opcs = LocalConfigurationHelper.GetLocalConfigurationSection();
            CountryItem defaultCountryItem = opcs.CountriesList.Cast<CountryItem>().ToList().Find(x => x.Default);
            if (defaultCountryItem == null && !string.IsNullOrEmpty(countryCode))
            {
                defaultCountryItem = opcs.CountriesList.Cast<CountryItem>().ToList().Find(x => x.Code.EndsWith(countryCode));
            }

            if (defaultCountryItem != null)
            {
                currencyCode = defaultCountryItem.Currency;
            }

            return currencyCode;
        }

        /// <summary>
        /// Gets the supported countries select list.
        /// </summary>
        /// <param name="countries">The countries.</param>
        /// <returns>Collection of countries.</returns>
        public static List<SelectListItem> GetSupportedCountriesSelectList(List<OrderServiceReferences.AtomiaBillingPublicService.Country> countries)
        {
            List<SelectListItem> countryList = countries.Select(country => new SelectListItem { Value = country.Code, Text = country.Name }).ToList();

            countryList.Sort((a, b) => a.Text.CompareTo(b.Text));

            return countryList;
        }

        /// <summary>
        /// Gets the EU country codes.
        /// </summary>
        /// <param name="countries">The countries.</param>
        /// <returns>Collection of country codes.</returns>
        public static List<string> GetEUCountryCodes(List<OrderServiceReferences.AtomiaBillingPublicService.Country> countries)
        {
            return (from country in countries where country.Tag == "EU" select country.Code).ToList();
        }
    }
}
