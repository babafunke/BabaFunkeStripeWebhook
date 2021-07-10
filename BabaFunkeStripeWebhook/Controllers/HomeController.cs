using BabaFunkeStripeWebhook.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using System.IO;
using System.Threading.Tasks;

namespace BabaFunkeStripeWebhook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IEmailService emailService, ILogger<HomeController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Test the StripeWebhook");
        }

        /// <summary>
        /// This is the main endpoint that is provided to Stripe
        /// It contains variables exposed by Stripe which include events, customer email etc.
        /// </summary>
        /// <returns></returns>
        [HttpPost("stripewebhook")]
        public async Task<IActionResult> OnCustomerPaymentMade()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                //PaymentIntentSucceeded is automatically triggered when a payment is sucessful i.e. customer has been successfully charged
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    // Call a custom method to handle the successful payment intent. 
                    await HandlePaymentIntentSucceeded(paymentIntent);
                }
                else
                {
                    _logger.LogInformation($"Error! {stripeEvent.Type}");
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// This is the custom method that indicates what the webhook does when the payment is successful
        /// In my case, I retrieve the customer's email and send an email to the address
        /// </summary>
        /// <param name="paymentIntent">Stripe instance that includes the payment details such as customer email, amount paid etc.</param>
        /// <returns></returns>
        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            var toEmail = paymentIntent.ReceiptEmail;
            var ccEmail = "hello@babafunke.com";
            var emailBody = $"Ndewo,\n\nCustomerEmail: {toEmail}\n\nOnce again, thanks for ordering our online class with Aunty Chi! " +
                $"This form is to help us better prepare for an exciting journey with you by identifying your preferred time slot among others.See link below.\n\n" +
                $"https://auntychiigboclass.geniigames.app/";

            await _emailService.SendEmail(emailBody, toEmail, ccEmail);
        }

    }
}