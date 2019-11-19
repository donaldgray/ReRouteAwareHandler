namespace ReRouteAwareHandler
{
    /// <summary>
    /// Options for configuring Polly policies
    /// </summary>
    public class PollyOptions
    {
        /// <summary>
        /// Number of exceptions that need to occur before circuit breaker is opened.
        /// </summary>
        public int ExceptionsBeforeBreak { get; private set; }
        
        /// <summary>
        /// Number of milliseconds to open circuit breaker for.
        /// </summary>
        public int BreakDurationMs { get; private set; } 
        
        /// <summary>
        /// Number of automatic retries due to timeouts.
        /// </summary>
        public int RetryCount { get; private set; } 
        
        /// <summary>
        /// Number of milliseconds to wait before retrying.
        /// </summary>
        public int RetryBackOffMs { get; private set; }
        
        /// <summary>
        /// Number of milliseconds to wait before timing out.
        /// </summary>
        public int TimeoutMs { get; private set; }

        public static PollyOptions Default => new PollyOptions
        {
            RetryCount = 2, TimeoutMs = 4000, BreakDurationMs = 1500, ExceptionsBeforeBreak = 5, RetryBackOffMs = 200
        };
        
        public static PollyOptions Foo => new PollyOptions
        {
            RetryCount = 5, TimeoutMs = 15000, BreakDurationMs = 1000, ExceptionsBeforeBreak = 2, RetryBackOffMs = 200
        };
    }
}