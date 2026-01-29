namespace BitcoinTracking.Shared.Constants
{
    /// <summary>
    /// Application-wide constants
    /// Constants shared across projects.
    /// Iinfrastructure without domain logic.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// Database
        /// </summary>
        public static class Database
        {
            public const string DefaultConnectionStringName = "DefaultConnection";
        }

        /// <summary>
        /// Logging
        /// </summary>
        public static class Logging
        {
            public const string LogFileName = "bitcoin-tracking-{Date}.log";
            public const string LogDirectory = "Logs";
        }

        /// <summary>
        /// Validation
        /// </summary>
        public static class Validation
        {
            public const int MaxNoteLength = 500;
            public const int MinNoteLength = 1;
        }
    }
}
