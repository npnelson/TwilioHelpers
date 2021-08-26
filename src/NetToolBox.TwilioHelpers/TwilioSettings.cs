namespace NetToolBox.TwilioHelpers
{
    public sealed class TwilioSettings
    {
        public string AccountSid { get; set; } = null!;

        public string AuthToken { get; set; } = null!;

        public string NonProductionSmsDestination { get; set; } = null!;
    }
}
