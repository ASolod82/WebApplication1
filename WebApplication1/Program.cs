using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Winton.Extensions.Configuration.Consul;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            //webHost.RunAsService();
            webHost.Run();


            //Start AppContext.BaseDirectory


//            Thread.Sleep(2000);
        }



        //static CancellationToken _cancellationToken = new CancellationToken();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => {
                config.AddConsul("Cloud/Core", default, options =>
                {
                    options.ConsulConfigurationOptions =
                                         cco => { cco.Address = new Uri("http://localhost:8500"); };
                    options.Optional = true;
                    options.ReloadOnChange = true;
                    options.OnLoadException = exceptioncontext => { exceptioncontext.Ignore = false; };

                });

            })
                .UseStartup<Startup>() 
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information))    
            ;
    }
}
