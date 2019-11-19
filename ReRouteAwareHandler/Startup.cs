namespace ReRouteAwareHandler
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Ocelot.DependencyInjection;
    using Ocelot.Middleware;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services) =>
            services
                .AddOcelot()
                .AddPolly();

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseOcelot(configuration =>
            {
                configuration.PreQueryStringBuilderMiddleware = (ctx, next) =>
                {
                    // set ReRouteKey on HttpContext
                    ctx.HttpContext.SetDownstreamReRouteKey(ctx);
                    return next.Invoke();
                };
            }).Wait();
        }
    }
}