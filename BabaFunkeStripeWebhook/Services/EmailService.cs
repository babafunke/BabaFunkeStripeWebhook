using FluentEmail.Core;
using System;
using System.Threading.Tasks;

namespace BabaFunkeStripeWebhook.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task SendEmail(string body, string to, string cc)
        {
            try
            {
                var email = await _fluentEmail
                    .To(to)
                    .CC(cc)
                    .Subject($"Identify preferences for your classes")
                    .Body(body)
                    .SendAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
