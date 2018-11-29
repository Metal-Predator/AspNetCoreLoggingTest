using System;
using AspNetCoreLoggingTest.Infrastructure;
using AspNetCoreLoggingTest.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;

namespace AspNetCoreLoggingTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<ClientContextMiddleware>();
            services.AddSingleton<NLogMappingContextInitializationMiddleware>();

            services.AddScoped<ClientContext>();

            if (UseCustomLogging())
            {
                services.AddLogging(lb => lb.AddFilter("Microsoft.AspNetCore.Hosting.Internal.WebHost", ll => false));
                services.AddSingleton<CustomLoggingMiddleware>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IConfiguration config)
        {
            ConfigureNLog(config);

            app.UseMiddleware<ClientContextMiddleware>();
            app.UseMiddleware<NLogMappingContextInitializationMiddleware>();

            if (UseCustomLogging())
            {
                // Should be before MVC but after context and MDLC initialization
                app.UseMiddleware<CustomLoggingMiddleware>();
            }

            app.UseMvc();
        }

        private static void ConfigureNLog(IConfiguration config)
        {
            var baseDir = config.GetValue<string>("LogsBaseDir");

            LogManager.Configuration.Variables["LogsDir"] = baseDir;
            LogManager.Configuration.Variables["ComponentName"] = typeof(Startup).Assembly.GetName().Name;

            LogManager.ReconfigExistingLoggers();
        }

        private static string GetEnvName()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }

        private static bool UseCustomLogging()
        {
            return string.Equals(GetEnvName(), "CUSTOM_LOGGING", StringComparison.OrdinalIgnoreCase);
        }
    }
}