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
        private readonly bool _isProduction;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptionsMonitor<TwilioSettings> _twillioSettingsMonitor;
        private readonly ILogger<TwilioServices> _logger;
        private readonly PhoneNumbers.PhoneNumberUtil _phoneNumberUtil;

        public TwilioServices(IHttpClientFactory httpClientFactory, IOptionsMonitor<TwilioSettings> twillioSettingsMonitor, ILogger<TwilioServices> logger, string environmentName)
        {
            _isProduction = environmentName.Equals("Production", StringComparison.OrdinalIgnoreCase);
            _httpClientFactory = httpClientFactory;
            _twillioSettingsMonitor = twillioSettingsMonitor;
            _logger = logger;
            _phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
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
            string finalPhoneNumber;
            if (_isProduction)
            {
                finalPhoneNumber = toPhoneNumber;
            }
            else
            {
                finalPhoneNumber = _twillioSettingsMonitor.CurrentValue.NonProductionSmsDestination;
            }
            if (string.IsNullOrWhiteSpace(finalPhoneNumber))
            {
                if (_isProduction)
                {
                    _logger.LogError("We are running in a production environment but the 'to' phone number is null or empty. The call to the Twilio SMS service will be suppressed and execution will continue without throwing an exception, but this likely indicates a data integrity issue that should be investigated. The body text was {BodyText}", bodyText);
                }
                else
                {
                    _logger.LogWarning("We are not running in a production environment and the TwilioSettings.NonProductionSMSDestination phone number is not set. The call to the Twilio SMS service will be suppressed.");
                }

                return;
            }
            else
            {
                var fromPhoneNumberProto = _phoneNumberUtil.Parse(fromPhoneNumber, "US");
                var e164From = _phoneNumberUtil.Format(fromPhoneNumberProto, PhoneNumbers.PhoneNumberFormat.E164);
                var finalPhoneNumberProto = _phoneNumberUtil.Parse(finalPhoneNumber, "US");
                var e164final = _phoneNumberUtil.Format(finalPhoneNumberProto, PhoneNumbers.PhoneNumberFormat.E164);

                await MessageResource.CreateAsync(body: bodyText, from: new PhoneNumber(e164From), to: new PhoneNumber(e164final), client: twilioRestClient).ConfigureAwait(false);

                if (_isProduction)
                {
                    _logger.LogInformation("Sent Text Message {Body} From:{From} To: {To}", bodyText, fromPhoneNumber, toPhoneNumber);
                }
                else
                {
                    _logger.LogInformation("Sent Text Message {Body} From:{From} To: {To} but redirected to {FinalPhoneNumber} because we are not running in production", bodyText, fromPhoneNumber, toPhoneNumber, finalPhoneNumber);
                }
            }
        }



        private ITwilioRestClient GetTwilioRestClient()
        {
            var currentSettings = _twillioSettingsMonitor.CurrentValue;
            var twilioRestClient = new TwilioRestClient(currentSettings.AccountSid, currentSettings.AuthToken, httpClient: new SystemNetHttpClient(_httpClientFactory.CreateClient()));
            return twilioRestClient;
        }
    }
}
