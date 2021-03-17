using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IdentityServerCustomized
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .CreateBootstrapLogger();

            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, services, configuration) =>
                {
                    var imageVersion = Assembly.GetAssembly(typeof(Program))
                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                        .InformationalVersion;

                    configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.WithProperty("AppType", Assembly.GetCallingAssembly().GetName().Name)
                    .Enrich.WithProperty("Env", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                    .Enrich.WithProperty("Version", imageVersion)
                    .Enrich.WithProperty("Module", "IdentityServer")
                    .Enrich.FromLogContext();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
