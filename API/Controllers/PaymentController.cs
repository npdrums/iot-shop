using System.IO;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using Order = Core.Entities.Orders.Order; // Because of Stripe Order class

namespace API.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        public PaymentController(IPaymentService paymentService,
            IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }


        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<CustomerShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            return await _paymentService.CreateOrUpdatePaymentIntent(cartId);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> stripeWebHook()
        {
            string webHookSecret = _configuration["StripeSettings: WebHookKey"];
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webHookSecret);

            PaymentIntent intent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdateOrderPaymentSucceeded(intent.Id);
                    break;
                case "payment_intent.payment_failed":
                    intent = (PaymentIntent)stripeEvent.Data.Object;
                    order = await _paymentService.UpdateOrderPaymentFailed(intent.Id);
                    break;
            }

            return new EmptyResult();
        }
    }
}
