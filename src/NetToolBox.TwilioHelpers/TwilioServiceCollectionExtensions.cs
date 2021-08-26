using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetToolBox.TwilioHelpers;
using NetToolBox.TwilioHelpers.Abstractions;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TwilioServiceCollectionExtensions
    {
        public static IServiceCollection AddTwilioServices(this IServiceCollection services, IConfigurationSection twilioConfigurationSection, string environmentName)
        {
            services.Configure<TwilioSettings>(twilioConfigurationSection);
            services.AddHttpClient();
            services.AddSingleton<ITwilioServices, TwilioServices>(x => new TwilioServices(x.GetRequiredService<IHttpClientFactory>(), x.GetRequiredService<IOptionsMonitor<TwilioSettings>>(), x.GetRequiredService<ILogger<TwilioServices>>(), environmentName));
            return services;
        }
    }
}