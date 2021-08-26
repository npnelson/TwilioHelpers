using Microsoft.Extensions.Configuration;
using NetToolBox.TwilioHelpers.AspNetCore;
using NetToolBox.TwilioHelpers.AspNetCore.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TwilioAspNetCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddTwilioAspNetCoreServices(this IServiceCollection services, IConfigurationSection twilioConfigurationSection, string environmentName)
        {
            services.AddTwilioServices(twilioConfigurationSection, environmentName);
            services.AddSingleton<ITwilioSignatureValidator, TwilioSignatureValidator>();
            return services;
        }
    }
}
