using NUnit.Framework;
using RestSharp;
using System.Net;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    public class SecurityAndLocalizationTests
    {
        private RestClient _client = null!;

        [SetUp]
        public void Setup()
        {
            bool isCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            string baseUrl = isCi ? "http://127.0.0.1:5123" : "https://localhost:7068";
            var options = new RestClientOptions(baseUrl)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            _client = new RestClient(options);
        }

        [Test]
        public void Home_HasSecurityHeaders()
        {
            var resp = _client.Execute(new RestRequest("/", Method.Get));
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resp.Headers.Any(h => h.Name == "X-Frame-Options" && (h.Value?.ToString() ?? "").Contains("DENY")), Is.True);
            Assert.That(resp.Headers.Any(h => h.Name == "X-Content-Type-Options" && (h.Value?.ToString() ?? "").Contains("nosniff")), Is.True);
            Assert.That(resp.Headers.Any(h => h.Name == "Content-Security-Policy" && (h.Value?.ToString() ?? "").Contains("default-src 'self'")), Is.True);
        }

        [Test]
        public void Products_Details_Localized_RU()
        {
            var req = new RestRequest("/Products/Details/1", Method.Get);
            req.AddHeader("Cookie", ".AspNetCore.Culture=c=ru|uic=ru");
            var resp = _client.Execute(req);
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resp.Content, Does.Contain("Комфорт разработчика"));
        }

        [Test]
        public void CookieBanner_PresentOnHome()
        {
            var resp = _client.Execute(new RestRequest("/", Method.Get));
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resp.Content, Does.Contain("id=\"cookie-consent\""));
        }
    }
}