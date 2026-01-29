using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinTracking.Shared.Extensions
{
    /// <summary>
    /// Extension methods for string operations
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Checks if string is null or whitespace
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Truncates string to specified length
        /// </summary>
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength
                ? value
                : value.Substring(0, maxLength);
        }

        /// <summary>
        /// Removes extra whitespace from string
        /// </summary>
        public static string RemoveExtraSpaces(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return System.Text.RegularExpressions.Regex.Replace(value.Trim(), @"\s+", " ");
        }
    }
}
