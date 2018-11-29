using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreLoggingTest.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreLoggingTest.Middlewares
{
    internal sealed class ClientContextMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var clientContext = context.RequestServices.GetService<ClientContext>();

            var correlationId = GetCorrelationId(context.Request);
            clientContext.CorrelationId = correlationId;

            context.Response.Headers.Add(SharedRequestHeaders.CorrelationId, correlationId);

            return next(context);
        }

        private static string GetCorrelationId(HttpRequest request)
        {
            return GetRequestHeaderOrDefault(request, SharedRequestHeaders.CorrelationId) ??
                   Guid.NewGuid().ToString().ToUpper();
        }

        private static string GetRequestHeaderOrDefault(HttpRequest request, string header)
        {
            return request.Headers[header].FirstOrDefault();
        }
    }
}