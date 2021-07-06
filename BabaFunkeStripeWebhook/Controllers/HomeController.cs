using BabaFunkeStripeWebhook.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BabaFunkeStripeWebhook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public HomeController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Test the StripeWebhook");
        }

        /*
          * Stripe webhook custom endpoint
          */
        [HttpPost("stripewebhook")]
        public async Task<IActionResult> OnCustomerPaymentMade()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            //TODO Security 
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Then define and call a method to handle the successful payment intent.
                    await HandlePaymentIntentSucceeded(paymentIntent);
                }
                else
                {
                    // Unexpected event type
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
        {
            var toEmail = paymentIntent.ReceiptEmail;
            var ccEmail = "hello@babafunke.com";
            var emailBody = $"Ndewo,\n\nCustomerEmail: {toEmail}\n\nOnce again, thanks for ordering our online private class with Aunty Chi! " +
                $"This form is to help us better prepare for an exciting journey with you by identifying your preferred time slot among others.See link below.\n\n" +
                $"https://forms.gle/2HPTEV3qmoBUH9999";

            await _emailService.SendEmail(emailBody, toEmail, ccEmail);
        }

    }
}
