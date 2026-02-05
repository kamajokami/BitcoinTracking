

namespace BitcoinTracking.Web.Configuration
{
    /// <summary>
    /// Configuration settings for REST API communication
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Base URL of BitcoinTracking REST API
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// HTTP timeout in seconds for API calls
        /// </summary>
        public int TimeoutSeconds { get; set; }
    }
}
