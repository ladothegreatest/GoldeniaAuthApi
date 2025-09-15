using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace GoldeniaAuthApi.Services
{
    public interface ISmsService
    {
        Task SendSmsCodeAsync(string phoneNumber, string code);
    }

    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendSmsCodeAsync(string phoneNumber, string code)
        {
            try
            {
                // Initialized Twilio,My trial Account but it has also monthly plan
                var accountSid = "AC8fdc039cdd4c23450307b26bdb9ad665";
                var authToken = "6c344362aab510c21d852f7688f44208";
                var fromNumber = "+12548755286";

                TwilioClient.Init(accountSid, authToken);

                // SMS structure will be like this
                var message = await MessageResource.CreateAsync(
                    body: $"Your Goldenia verification code is: {code}. Valid for 15 minutes.",
                    from: new PhoneNumber(fromNumber),
                    to: new PhoneNumber(phoneNumber)
                );

                Console.WriteLine($"SMS sent successfully. SID: {message.Sid}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send SMS: {ex.Message}");
            }
        }
    }
}
