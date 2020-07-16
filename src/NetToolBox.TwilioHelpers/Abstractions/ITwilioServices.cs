using System.Threading.Tasks;

namespace NetToolBox.TwilioHelpers.Abstractions
{
    public interface ITwilioServices
    {
        Task SendSMSMessageAsync(string bodyText, string fromPhoneNumber, string toPhoneNumber);
    }
}
