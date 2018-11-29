using System.Threading.Tasks;
using AspNetCoreLoggingTest.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace AspNetCoreLoggingTest.Middlewares
{
    internal sealed class NLogMappingContextInitializationMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var clientContext = context.RequestServices.GetService<ClientContext>();
            MappedDiagnosticsLogicalContext.Set(SharedRequestHeaders.CorrelationId, clientContext.CorrelationId);

            return next(context);
        }
    }
}