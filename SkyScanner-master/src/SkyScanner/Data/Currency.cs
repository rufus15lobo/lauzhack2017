﻿// Copyright (c) 2015-2016 Tamas Vajk. All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SkyScanner.Data
{
    /// <summary>
    /// Currency supported by SkyScanner
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Default currency - British Pound
        /// </summary>
        public static readonly Currency Default = new Currency
        {
            Code = "GBP",
            Symbol = "£",
            ThousandsSeparator = ",",
            DecimalSeparator = ".",
            SymbolOnLeft = true,
            SpaceBetweenAmountAndSymbol = false,
            RoundingCoefficient = 0,
            DecimalDigits = 2
        };
        
        /// <summary>
        /// Code of the currency - such as GBP, USD
        /// </summary>
        public string Code { get; internal set; }
        internal string Symbol { get; set; }
        internal string ThousandsSeparator { get; set; }
        internal string DecimalSeparator { get; set; }
        internal bool SymbolOnLeft { get; set; }
        internal bool SpaceBetweenAmountAndSymbol { get; set; }
        internal int RoundingCoefficient { get; set; }
        internal int DecimalDigits { get; set; }
        public override string ToString()
        {
            return Code;
        }
        /// <summary>
        /// Returns the pretty printed value
        /// </summary>
        /// <param name="value">The sum to pretty print</param>
        /// <returns></returns>
        public string FormatValue(decimal value)
        {
            var nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = DecimalSeparator,
                NumberGroupSeparator = ThousandsSeparator,
                NumberDecimalDigits = DecimalDigits
            };

            var v = value.ToString("N", nfi);
            var space = SpaceBetweenAmountAndSymbol ? " " : "";

            IEnumerable<string> parts = new[] { v, space, Symbol };

            if (SymbolOnLeft)
            {
                parts = parts.Reverse();
            }

            return string.Join("", parts);
        }

        /// <summary>
        /// Instantiates a new currency with the provided currency code.
        /// </summary>
        /// <param name="code">The currency code. E.g. EUR, GBP, CHF.</param>
        /// <returns>The currency for the provided code</returns>
        public static Currency FromCode(string code)
        {
            return new Currency() { Code = code };
        }
    }
}