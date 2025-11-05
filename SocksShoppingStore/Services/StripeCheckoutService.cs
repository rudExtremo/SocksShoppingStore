using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SocksShoppingStore.Models;
using SocksShoppingStore.Options;
using Stripe;
using Stripe.Checkout;

namespace SocksShoppingStore.Services
{
    public class StripeCheckoutService
    {
        private readonly StripeOptions _options;
        private readonly ILogger<StripeCheckoutService> _logger;

        public StripeCheckoutService(IOptions<StripeOptions> options, ILogger<StripeCheckoutService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<Session> CreateCheckoutSessionAsync(Order draft, HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(_options.SecretKey))
            {
                throw new InvalidOperationException("Stripe SecretKey is not configured.");
            }

            if (!_options.SecretKey.StartsWith("sk_test_", StringComparison.Ordinal))
            {
                // Hard guard to avoid accidental live charges in dev/staging
                throw new InvalidOperationException("Stripe SecretKey must be a test key (sk_test_*) in this environment.");
            }

            var client = new StripeClient(_options.SecretKey);
            var service = new SessionService(client);

            var lineItems = draft.Items.Select(i => new SessionLineItemOptions
            {
                Quantity = i.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = ToCents(i.UnitPrice),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = i.Name
                    }
                }
            }).ToList();

            var host = request.Host.HasValue ? request.Host.Value : "localhost";
            var scheme = request.Scheme ?? "https";
            var baseUrl = $"{scheme}://{host}";
            var successUrl = baseUrl + _options.SuccessReturnPath + "?session_id={CHECKOUT_SESSION_ID}";
            var cancelUrl = baseUrl + _options.CancelReturnPath;

            var createOptions = new SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = lineItems,
                CustomerEmail = string.IsNullOrWhiteSpace(draft.Email) ? null : draft.Email,
                // Stripe will auto-detect locale if not set; we can map culture later
                Locale = null,
            };

            _logger.LogInformation("payment_session_creating: items={Count} total={Total}", draft.Items.Count, draft.Total);
            var session = await service.CreateAsync(createOptions);
            _logger.LogInformation("payment_session_created: id={Id} url={Url}", session.Id, session.Url);
            return session;
        }

        private static long ToCents(decimal amount)
        {
            return (long)Math.Round(amount * 100m, MidpointRounding.AwayFromZero);
        }
    }
}
