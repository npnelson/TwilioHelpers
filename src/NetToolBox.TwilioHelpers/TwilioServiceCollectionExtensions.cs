using Microsoft.Extensions.Configuration;
using NetToolBox.TwilioHelpers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TwilioServiceCollectionExtensions
    {
        public static IServiceCollection AddTwilioServices(this IServiceCollection services, IConfigurationSection twilioConfigurationSection)
        {
            services.Configure<TwilioSettings>(twilioConfigurationSection);
            return services;
        }
    }
}
