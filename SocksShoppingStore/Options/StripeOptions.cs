namespace SocksShoppingStore.Config
{
    public class StripeOptions
    {
        public string PublishableKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public bool UseTestModeBanner { get; set; } = true;
        public string SuccessReturnPath { get; set; } = "/Checkout/ReturnSuccess";
        public string CancelReturnPath { get; set; } = "/Checkout/PaymentFailed";
    }
}
