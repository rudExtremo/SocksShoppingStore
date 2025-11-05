using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocksShoppingStore.Config;
using SocksShoppingStore.Services;
using Stripe;
using Stripe.Checkout;

namespace SocksShoppingStore.Controllers
{
    [ApiController]
    [Route("webhooks/stripe")]
    public class StripeWebhookController : ControllerBase
    {
        private readonly StripeOptions _options;
        private readonly ILogger<StripeWebhookController> _logger;
        private readonly PaymentSessionStore _sessionStore;

        public StripeWebhookController(IOptions<StripeOptions> options, ILogger<StripeWebhookController> logger, PaymentSessionStore sessionStore)
        {
            _options = options.Value;
            _logger = logger;
            _sessionStore = sessionStore;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            // Read raw body
            using var reader = new StreamReader(HttpContext.Request.Body);
            var json = await reader.ReadToEndAsync();

            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
            {
                _logger.LogWarning("stripe_webhook_no_secret_configured");
                return Unauthorized();
            }

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, signature, _options.WebhookSecret);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "stripe_webhook_signature_verification_failed");
                return BadRequest();
            }

            switch (stripeEvent.Type)
            {
                case Events.CheckoutSessionCompleted:
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session != null)
                    {
                        _sessionStore.MarkCompleted(session.Id);
                        _logger.LogInformation("stripe_webhook_checkout_session_completed: session={Id} payment_status={Status}", session.Id, session.PaymentStatus);
                    }
                    break;
                }
                case Events.CheckoutSessionExpired:
                {
                    var session = stripeEvent.Data.Object as Session;
                    _logger.LogInformation("stripe_webhook_checkout_session_expired: session={Id}", session?.Id);
                    break;
                }
                default:
                    _logger.LogDebug("stripe_webhook_unhandled_event: type={Type}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
    }
}
