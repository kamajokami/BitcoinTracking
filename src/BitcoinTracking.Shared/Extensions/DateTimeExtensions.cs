using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinTracking.Shared.Extensions
{
    /// <summary>
    /// Extension methods for DateTime operations
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts DateTime to Czech format string (dd.MM.yyyy HH:mm:ss)
        /// </summary>
        public static string ToCzechFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        /// <summary>
        /// Converts DateTime to short Czech format (dd.MM.yyyy)
        /// </summary>
        public static string ToCzechDateOnly(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }

        /// <summary>
        /// Checks if DateTime is today
        /// </summary>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }
    }
}
