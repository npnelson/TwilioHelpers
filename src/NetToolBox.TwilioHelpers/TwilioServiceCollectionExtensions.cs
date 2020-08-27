using Microsoft.Extensions.Configuration;
using NetToolBox.TwilioHelpers;
using NetToolBox.TwilioHelpers.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TwilioServiceCollectionExtensions
    {
        public static IServiceCollection AddTwilioServices(this IServiceCollection services, IConfigurationSection twilioConfigurationSection)
        {
            services.Configure<TwilioSettings>(twilioConfigurationSection);
            services.AddHttpClient();
            services.AddSingleton<ITwilioServices, TwilioServices>();
            return services;
        }
    }
}