namespace ReRouteAwareHandler
{
    using Microsoft.AspNetCore.Http;
    using Ocelot.Middleware;

    public static class HttpContextExtensions
    {
        private const string ReRouteItemKey = "ReRouteKey";
        
        /// <summary>
        /// Add key of current ReRoute into HttpContext.Items.
        /// </summary>
        public static void SetDownstreamReRouteKey(this HttpContext httpContext, DownstreamContext downstreamContext)
        {
            if (downstreamContext?.DownstreamReRoute == null) return;

            httpContext.Items[ReRouteItemKey] = downstreamContext.DownstreamReRoute.Key;
        }

        /// <summary>
        /// Get key of current ReRoute from HttpContext.
        /// </summary>
        public static string GetDownstreamReRouteKey(this HttpContext httpContext) 
            => httpContext.Items.TryGetValue(ReRouteItemKey, out var value)
                ? value.ToString()
                : null;
    }
}