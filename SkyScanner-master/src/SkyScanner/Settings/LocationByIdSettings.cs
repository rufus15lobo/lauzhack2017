using SkyScanner.Data;
using System;
using SkyScanner.Settings.Base;

namespace SkyScanner.Settings
{
    public class LocationByIdSettings
    {
        /// <summary>
        /// Initializes a new instance of the LocationAutosuggestSettings with default values
        /// </summary>
        /// <param name="query">Query string to search for</param>
        public LocationByIdSettings(string origin, string destination, string outboundPartialDate)
            : this(origin, destination, outboundPartialDate, Market.Default, Currency.Default, Locale.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LocationAutosuggestSettings with the specified parameters
        /// </summary>
        /// <param name="query">Query string to search for</param>
        /// <param name="queryType">Query type - search by name or ID</param>
        /// <param name="market">Market country</param>
        /// <param name="currency">Selected currency</param>
        /// <param name="locale">Selected language</param>
        public LocationByIdSettings(string origin, string destination, string outboundPartialDate, Market market, Currency currency, Locale locale)
        {
            if (market == null)
            {
                throw new ArgumentNullException(nameof(market));
            }
            if (currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }
            if (locale == null)
            {
                throw new ArgumentNullException(nameof(locale));
            }

            Origin = origin;
            Destination = destination;
            OutboundPartialDate = outboundPartialDate;
            Market = market;
            Currency = currency;
            Locale = locale;
        }

        public Market Market { get; }

        public Currency Currency { get; }

        public Locale Locale { get; }

        public string Destination { get; }
        public string Origin { get; }

        public string OutboundPartialDate { get; }
    }
}
