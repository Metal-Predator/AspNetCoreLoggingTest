using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AspNetCoreLoggingTest.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AspNetCoreLoggingTest.Middlewares
{
    internal sealed class CustomLoggingMiddleware : IMiddleware
    {
        private readonly ILogger<CustomLoggingMiddleware> _logger;

        public CustomLoggingMiddleware(ILogger<CustomLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var url = context.Request.GetDisplayUrl();
            _logger.LogInformation($"Request started [{context.Request.Method}] \"{url}\"");

            var sw = Stopwatch.StartNew();

            try
            {
                await next(context);
                sw.Stop();

                _logger.LogInformation($"Request [{context.Request.Method}] \"{url}\" completed successfully in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex) // Unhandled exception (No MVC exception filters)
            {
                _logger.LogError($"Unhandled exception occured: {ex}");
                WriteErrorResponse(context.Response, ex);
            }
        }

        private static void WriteErrorResponse(HttpResponse response, Exception ex)
        {
            response.StatusCode = 500;
            var error = new UnhandledError { Error = ex.ToString() };

            using (var writer = new StreamWriter(response.Body))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = JsonSerializer.CreateDefault();
                serializer.Serialize(jsonWriter, error);
            }
        }
    }
}