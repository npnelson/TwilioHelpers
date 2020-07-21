using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetToolBox.TwilioHelpers.Abstractions;
using System;
using System.IO;
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
        private readonly ILogger<TwilioServices> _logger;

        public TwilioServices(IHttpClientFactory httpClientFactory, IOptionsMonitor<TwilioSettings> twillioSettingsMonitor, ILogger<TwilioServices> logger)
        {
            _httpClientFactory = httpClientFactory;
            _twillioSettingsMonitor = twillioSettingsMonitor;
            _logger = logger;
        }

        public async Task DeleteRecordingAsync(string pathSid, string pathAccountSid)
        {
            var twilioRestClient = GetTwilioRestClient();
            await RecordingResource.DeleteAsync(pathSid, pathAccountSid, twilioRestClient).ConfigureAwait(false);
            _logger.LogInformation("Deleted Recording PathSid={PathSid} PathAccountSid={PathAccountSid}", pathSid, pathAccountSid);
        }

        public async Task<Stream> GetRecordingWavAsync(Uri recordingUri)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var audioStream = await httpClient.GetStreamAsync(recordingUri).ConfigureAwait(false);
            _logger.LogInformation("Fetched Recording Stream {RecordingUri}", recordingUri);
            return audioStream;
        }

        public async Task SendSMSMessageAsync(string bodyText, string fromPhoneNumber, string toPhoneNumber)
        {
            var twilioRestClient = GetTwilioRestClient();
            await MessageResource.CreateAsync(body: bodyText, from: new PhoneNumber(fromPhoneNumber), to: new PhoneNumber(toPhoneNumber), client: twilioRestClient).ConfigureAwait(false);
            _logger.LogInformation("Sent Text Message {Body} From:{From} To: {To}", bodyText, fromPhoneNumber, toPhoneNumber);
        }



        private ITwilioRestClient GetTwilioRestClient()
        {
            var currentSettings = _twillioSettingsMonitor.CurrentValue;
            var twilioRestClient = new TwilioRestClient(currentSettings.AccountSid, currentSettings.AuthToken, httpClient: new SystemNetHttpClient(_httpClientFactory.CreateClient()));
            return twilioRestClient;
        }
    }
}
