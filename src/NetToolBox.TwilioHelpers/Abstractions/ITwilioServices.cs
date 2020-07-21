using System;
using System.IO;
using System.Threading.Tasks;

namespace NetToolBox.TwilioHelpers.Abstractions
{
    public interface ITwilioServices
    {
        Task SendSMSMessageAsync(string bodyText, string fromPhoneNumber, string toPhoneNumber);
        Task<Stream> GetRecordingWav(Uri recordingUri);
        Task DeleteRecording(string pathSid, string pathAccountSid);

    }
}
