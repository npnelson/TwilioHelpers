using Microsoft.AspNetCore.Http;

namespace NetToolBox.TwilioHelpers.AspNetCore.Abstractions
{
    public interface ITwilioSignatureValidator
    {
        bool ValidateRequest(HttpRequest request);
    }
}
