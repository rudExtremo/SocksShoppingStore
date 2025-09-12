using NUnit.Framework;
using Allure.NUnit;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Net;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
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
            _client = new RestClient(options, configureSerialization: s => s.UseSystemTextJson());
        }

        [Test]
        public void Home_HasSecurityHeaders()
        {
            var resp = _client.Execute(new RestRequest("/", Method.Get));
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var headers = resp.Headers ?? Array.Empty<HeaderParameter>();
            Assert.That(headers.Any(h => h.Name == "X-Frame-Options" && (h.Value?.ToString() ?? "").Contains("DENY")), Is.True);
            Assert.That(headers.Any(h => h.Name == "X-Content-Type-Options" && (h.Value?.ToString() ?? "").Contains("nosniff")), Is.True);
            Assert.That(headers.Any(h => h.Name == "Content-Security-Policy" && (h.Value?.ToString() ?? "").Contains("default-src 'self'")), Is.True);
        }

        [Test]
        public void Products_Details_Localized_RU()
        {
            var req = new RestRequest("/Products/Details/1", Method.Get);
            req.AddHeader("Cookie", ".AspNetCore.Culture=c=ru|uic=ru");
            var resp = _client.Execute(req);
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resp.Content ?? string.Empty, Does.Contain("Комфорт разработчика"));
        }

        [Test]
        public void CookieBanner_PresentOnHome()
        {
            var resp = _client.Execute(new RestRequest("/", Method.Get));
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(resp.Content ?? string.Empty, Does.Contain("id=\"cookie-consent\""));
        }

        [Test]
        public void Home_CspContainsNonce()
        {
            var resp = _client.Execute(new RestRequest("/", Method.Get));
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var headers = resp.Headers ?? Array.Empty<HeaderParameter>();
            var csp = headers.FirstOrDefault(h => h.Name == "Content-Security-Policy")?.Value?.ToString() ?? string.Empty;
            Assert.That(csp, Does.Contain("script-src 'self' 'nonce-"));
        }
    }
}

