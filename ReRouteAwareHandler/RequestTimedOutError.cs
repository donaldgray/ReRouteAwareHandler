namespace ReRouteAwareHandler
{
    using System;
    using Ocelot.Errors;

    public class RequestTimedOutError : Error
    {
        public RequestTimedOutError(Exception exception)
            : base($"Timeout making http request, exception: {exception}", OcelotErrorCode.RequestTimedOutError)
        {
        }
    }
}