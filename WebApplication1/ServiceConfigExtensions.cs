using Microsoft.Extensions.Configuration;
using System;

namespace WebApplication1
{
    public static class ServiceConfigExtensions
    {
        public static ServiceConfig GetServiceConfig(this IConfiguration configuration)
        {
            if (configuration == null) {
                throw new ArgumentNullException(nameof(configuration));
            }

            var serviceConfig = new ServiceConfig
            {
                ServiceDiscoveryAddress = configuration.GetValue<Uri>("ServiceConfig:serviceDiscoveryAddress"),
                ServiceAddress = configuration.GetValue<Uri>("ServiceConfig:ServiceAddress"),
                ServiceName = configuration.GetValue<string>("ServiceConfig:ServiceName"),
                ServiceId = configuration.GetValue<string>("ServiceConfig:ServiceId")
            };

            return serviceConfig;
        }
    }
}
