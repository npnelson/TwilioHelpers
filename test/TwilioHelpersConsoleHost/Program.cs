using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetToolBox.TwilioHelpers.Abstractions;
using System;
using System.Threading.Tasks;

namespace TwilioHelpersConsoleHost
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

            var sp = new ServiceCollection();
            sp.AddTwilioServices(Configuration.GetSection("TwilioSettings"));
            var services = sp.BuildServiceProvider();
            var twilio = services.GetRequiredService<ITwilioServices>();
            await twilio.SendSMSMessageAsync("TestText", "", "");
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
