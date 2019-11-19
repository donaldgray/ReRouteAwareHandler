namespace ReRouteAwareHandler
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Ocelot.DependencyInjection;
    using Ocelot.Errors;
    using Polly.CircuitBreaker;
    using Polly.Timeout;

    public static class OcelotBuilderExtensions
    {
        /// <summary>
        /// Add <see cref="PollyRetryBreakerHandler"/> to Ocelot. 
        /// </summary>
        public static IOcelotBuilder AddPolly(this IOcelotBuilder ocelotBuilder)
        {
            var errorMapping = new Dictionary<Type, Func<Exception, Error>>
            {
                {typeof(TaskCanceledException), e => new RequestTimedOutError(e)},
                {typeof(TimeoutRejectedException), e => new RequestTimedOutError(e)},
                {typeof(BrokenCircuitException), e => new RequestTimedOutError(e)}
            };
            
            ocelotBuilder
                .AddDelegatingHandler<PollyRetryBreakerHandler>()
                .Services.AddSingleton(errorMapping);

            return ocelotBuilder;
        }
    }
}