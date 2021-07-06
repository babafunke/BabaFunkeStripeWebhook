using System.Threading.Tasks;

namespace BabaFunkeStripeWebhook.Services
{
    public interface IEmailService
    {
        Task SendEmail(string body, string to, string cc);
    }
}
