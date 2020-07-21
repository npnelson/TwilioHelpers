using System;
using System.IO;
using System.Threading.Tasks;

namespace NetToolBox.TwilioHelpers.Abstractions
{
    public interface ITwilioServices
    {
        Task SendSMSMessageAsync(string bodyText, string fromPhoneNumber, string toPhoneNumber);
        Task<Stream> GetRecordingWavAsync(Uri recordingUri);
        Task DeleteRecordingAsync(string pathSid, string pathAccountSid);

    }
}
