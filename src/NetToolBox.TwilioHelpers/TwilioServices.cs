using Microsoft.Extensions.Options;
using NetToolBox.TwilioHelpers.Abstractions;
using System.Net.Http;
using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NetToolBox.TwilioHelpers
{
    public sealed class TwilioServices : ITwilioServices
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptionsMonitor<TwilioSettings> _twillioSettingsMonitor;

        public TwilioServices(IHttpClientFactory httpClientFactory, IOptionsMonitor<TwilioSettings> twillioSettingsMonitor)
        {
            _httpClientFactory = httpClientFactory;
            _twillioSettingsMonitor = twillioSettingsMonitor;
        }
        public async Task SendSMSMessageAsync(string bodyText, string fromPhoneNumber, string toPhoneNumber)
        {
            var currentSettings = _twillioSettingsMonitor.CurrentValue;
            var twilioRestClient = new TwilioRestClient(currentSettings.AccountSid, currentSettings.AuthToken, httpClient: new SystemNetHttpClient(_httpClientFactory.CreateClient()));
            await MessageResource.CreateAsync(body: bodyText, from: new PhoneNumber(fromPhoneNumber), to: new PhoneNumber(toPhoneNumber), client: twilioRestClient).ConfigureAwait(false);
        }
    }
}
