namespace ReRouteAwareHandler
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Polly;
    using Polly.Contrib.WaitAndRetry;
    using Polly.Timeout;
    using Polly.Wrap;

    public class PollyRetryBreakerHandler : DelegatingHandler
    {
        private readonly AsyncPolicyWrap<HttpResponseMessage> _circuitBreakerPolicy;

        public PollyRetryBreakerHandler(IHttpContextAccessor contextAccessor)
        {
            // Get the values for polly
            var pollyOptions = GetPollyOptionsForContext(contextAccessor.HttpContext);
            _circuitBreakerPolicy = CreatePollyPolicy(pollyOptions);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
            => _circuitBreakerPolicy
                .ExecuteAsync(
                    async ct => await base.SendAsync(request, ct),
                    cancellationToken);

        // Get PollyOptions for current ReRoute - overly simplified for demo
        private static PollyOptions GetPollyOptionsForContext(
            HttpContext httpContext) =>
            httpContext.GetDownstreamReRouteKey() == "foo" ? PollyOptions.Foo : PollyOptions.Default;

        private static AsyncPolicyWrap<HttpResponseMessage> CreatePollyPolicy(PollyOptions pollyOptions)
        {
            var retries = Backoff.ConstantBackoff(
                TimeSpan.FromMilliseconds(pollyOptions.RetryBackOffMs),
                pollyOptions.RetryCount,
                true);

            // Timeout policy
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromMilliseconds(pollyOptions.TimeoutMs));
            
            // Wrap timeout policy with retry
            var retryWithTimeout = Policy
                .Handle<TimeoutRejectedException>()
                .Or<TimeoutException>()
                .OrResult<HttpResponseMessage>(IsTransientError)
                .WaitAndRetryAsync(retries)
                .WrapAsync(timeoutPolicy);

            // Wrap retry with circuit breaker
            var circuitBreaker = Policy
                .Handle<HttpRequestException>()
                .Or<TimeoutRejectedException>()
                .Or<TimeoutException>()
                .OrResult<HttpResponseMessage>(IsTransientError)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: pollyOptions.ExceptionsBeforeBreak,
                    durationOfBreak: TimeSpan.FromMilliseconds(pollyOptions.BreakDurationMs))
                .WrapAsync(retryWithTimeout);

            return circuitBreaker;
        }

        // Any 5xx or 408 response is deemed to be transient  
        private static bool IsTransientError(HttpResponseMessage r)
            => r.StatusCode >= HttpStatusCode.InternalServerError || r.StatusCode == HttpStatusCode.RequestTimeout;
    }
}