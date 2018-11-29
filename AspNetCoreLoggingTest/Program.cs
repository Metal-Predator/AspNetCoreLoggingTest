using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;

namespace AspNetCoreLoggingTest
{
    internal static class Program
    {
        public static void Main(string[] args) =>
            CreateWebHostBuilder(args).Build().Run();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder<Startup>(args).UseNLog();
    }
}