using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NetToolBox.TwilioHelpers.AspNetCore.Abstractions;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

//adapted from https://raw.githubusercontent.com/twilio/twilio-aspnet/master/src/Twilio.AspNet.Core/RequestValidationHelper.cs
namespace NetToolBox.TwilioHelpers.AspNetCore
{
    public class TwilioSignatureValidator : ITwilioSignatureValidator
    {
        private readonly IOptionsMonitor<TwilioSettings> _settingsMonitor;

        public TwilioSignatureValidator(IOptionsMonitor<TwilioSettings> settingsMonitor)
        {
            _settingsMonitor = settingsMonitor;
        }
        public bool ValidateRequest(HttpRequest request)
        {
            //we fire up a new HMACSHA1 each time just in case the options change out from underneath us
            //if we were really worried about performance or allocations, we could store a HMACSHA1 and then swap it out with a new one if we detected the options changing

            // validate request
            // http://www.twilio.com/docs/security-reliability/security
            // Take the full URL of the request, from the protocol (http...) through the end of the query string (everything after the ?)
            string fullUrl = $"{request.Scheme}://{(request.IsHttps ? request.Host.Host : request.Host.ToUriComponent())}{request.Path}{request.QueryString}";
            var value = new StringBuilder();
            value.Append(fullUrl);
            // If the request is a POST, take all of the POST parameters and sort them alphabetically.
            if (request.Method == "POST")
            {
                // Iterate through that sorted list of POST parameters, and append the variable name and value (with no delimiters) to the end of the URL string
                var sortedKeys = request.Form.Keys.OrderBy(k => k, StringComparer.Ordinal).ToList();
                foreach (var key in sortedKeys)
                {
                    value.Append(key);
                    value.Append(request.Form[key]);
                }
            }

            // Sign the resulting value with HMAC-SHA1 using your AuthToken as the key (remember, your AuthToken's case matters!).
            var sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(_settingsMonitor.CurrentValue.AuthToken));
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(value.ToString()));

            // Base64 encode the hash
            var encoded = Convert.ToBase64String(hash);

            // Compare your hash to ours, submitted in the X-Twilio-Signature header. If they match, then you're good to go.
            var sig = request.Headers["X-Twilio-Signature"];

            return sig == encoded;
        }
    }
}
