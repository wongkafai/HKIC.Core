using System;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace HKIC.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EnvironmentName.Development)
            {
                CreateWebHostBuilder(args).Build().Run();
                return;
            }
            var host = new WebHostBuilder()
               .UseKestrel(options =>
               {
                   options.Listen(IPAddress.Any, 8080);
                   options.Limits.MaxConcurrentUpgradedConnections = 10 * 1024;
                   options.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(100));
                   options.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(100));
               })
               .UseContentRoot(Directory.GetCurrentDirectory())
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   var env = hostingContext.HostingEnvironment;

                   config.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                         .AddJsonFile($"config/appsettings.json", optional: true, reloadOnChange: true)
                         .AddJsonFile($"secret/appsettings.json", optional: true, reloadOnChange: true);

                   if (env.IsDevelopment())
                   {
                       var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                       if (appAssembly != null)
                       {
                           config.AddUserSecrets(appAssembly, optional: true);
                       }
                   }

                   config.AddEnvironmentVariables();

                   if (args != null)
                   {
                       config.AddCommandLine(args);
                   }
               })
               .ConfigureLogging((hostingContext, logging) =>
               {
                    //logging.UseConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                   logging.AddDebug();
               })
               //.UseIISIntegration()
               .UseDefaultServiceProvider((context, options) =>
               {
                   options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
               })
               .ConfigureServices(services =>
               {
                   services.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>();
               })
               .UseStartup<Startup>()
               .Build();


            host.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
