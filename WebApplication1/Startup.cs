using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Winton.Extensions.Configuration.Consul;

namespace WebApplication1
{
    public class Startup
    {
        private IApplicationLifetime _appLifetime;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {

            Configuration = new ConfigurationBuilder().AddConfiguration(configuration)
                .AddConsul("site1/appsettings.json", default, options =>
          {
              options.ConsulConfigurationOptions =
                                   cco => { cco.Address = new Uri("http://localhost:8500"); };
              options.Optional = true;
              options.ReloadOnChange = true;
              options.OnLoadException = exceptioncontext => { exceptioncontext.Ignore = false; };

          }).Build();

            ChangeToken.OnChange(Configuration.GetReloadToken, OnChanged);

            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            //builder.AddEnvironmentVariables("SECURE_");
            //Configuration = builder.Build();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IConfiguration>(Configuration);
            _computedConfigurationHash = ComputeConfigurationHash();
            //var q =  Configuration["SECURE_APP"] as string;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            _appLifetime = appLifetime;
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        private void OnChanged()
        {
            if (_computedConfigurationHash != null && !ComputeConfigurationHash().SequenceEqual(_computedConfigurationHash))
            {
                _appLifetime?.StopApplication();
            }
        }

        private byte[] _computedConfigurationHash;

        private byte[] ComputeConfigurationHash()
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, new Dictionary<string, string>(Configuration.AsEnumerable(), default));
                return SHA1.Create().ComputeHash(ms.ToArray());
            }
        }

        private void OnStopped()
        {
            new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = @"C:\Program Files\dotnet\dotnet.exe",
                    Arguments = @"exec C:\Users\VinT\source\repos\WebApplication1\WebApplication1\bin\Debug\netcoreapp2.1\WebApplication1.dll"
                }
            }.Start();


            //new Process()
            //{
            //    StartInfo = new ProcessStartInfo()
            //    {
            //        UseShellExecute = true,
            //        FileName = @"C:\Windows\System32\sc.exe",
            //        Arguments = @"start atest"
            //    }
            //}.Start();

        }
    }
}
