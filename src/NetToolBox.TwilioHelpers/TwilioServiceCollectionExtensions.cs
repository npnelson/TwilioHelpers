using Microsoft.Extensions.Configuration;
using NetToolBox.TwilioHelpers;
using NetToolBox.TwilioHelpers.Abstractions;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TwilioServiceCollectionExtensions
    {
        /// <summary>
        /// This overload will be deprecated once Azure Functions supports ASPNet style configuration
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddTwilioServices(this IServiceCollection services)
        {
            var optionsRegistered = services.Any(x => x.ServiceType.GenericTypeArguments.Any(x => x == typeof(TwilioSettings)));
            if (!optionsRegistered) throw new InvalidOperationException("You must register options for TwilioSettings");
            services.AddHttpClient();
            services.AddScoped<ITwilioServices, TwilioServices>();
            return services;
        }


        public static IServiceCollection AddTwilioServices(this IServiceCollection services, IConfigurationSection twilioConfigurationSection)
        {
            services.Configure<TwilioSettings>(twilioConfigurationSection);
            services.AddHttpClient();
            services.AddSingleton<ITwilioServices, TwilioServices>();
            return services;
        }
    }
}
