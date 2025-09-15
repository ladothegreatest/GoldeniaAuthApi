using GoldeniaAuthApi.Models;

namespace GoldeniaAuthApi.Data
{
    public static class DataStore
    {
        //This is the temporary memory I used for the api, later it can be integrated with databases
        public static List<EmailVerification> EmailVerifications { get; set; } = new();
        public static List<KycDocument> KycDocuments { get; set; } = new();
        public static List<SmsVerification> SmsVerifications { get; set; } = new();
    }
}
