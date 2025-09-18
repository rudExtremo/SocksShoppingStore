using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Integration")]
    [Category("Integration")]
    [Category("Security")]
    public class SecurityAndLocalizationTests
    {
        private RestClient? _rsClient;
        private HttpClient? _httpClient;
        private WebApplicationFactory<Program>? _factory;

        [SetUp]
        public void Setup()
        {
            if (TestSettings.UseTestFactory)
            {
                _factory = new WebApplicationFactory<Program>();
                _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = true
                });
            }
            else
            {
                string baseUrl = TestSettings.BaseUrl;
                var options = new RestClientOptions(baseUrl)
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => TestSettings.IgnoreCertErrors
                };
                _rsClient = new RestClient(options, configureSerialization: s => s.UseSystemTextJson());
            }
        }

        [Test]
        [AllureDescription(@"What: Verify home page responds with core security headers.
Steps:
1) GET '/'.
Expected: X-Frame-Options, X-Content-Type-Options, and Content-Security-Policy headers are present.")]
        public void Home_Has_SecurityHeaders()
        {
            if (_httpClient != null)
            {
                var resp = _httpClient.GetAsync("/").GetAwaiter().GetResult();
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                var headers = resp.Headers;
                Assert.That(headers.Contains("X-Frame-Options"), Is.True);
                Assert.That(headers.Contains("X-Content-Type-Options"), Is.True);
                Assert.That(headers.Contains("Content-Security-Policy"), Is.True);
            }
            else
            {
                var resp = _rsClient!.Execute(new RestRequest("/", Method.Get));
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                var headers = resp.Headers ?? Array.Empty<HeaderParameter>();
                Assert.That(headers.Any(h => h.Name == "X-Frame-Options" && (h.Value?.ToString() ?? "").Contains("DENY")), Is.True);
                Assert.That(headers.Any(h => h.Name == "X-Content-Type-Options" && (h.Value?.ToString() ?? "").Contains("nosniff")), Is.True);
                Assert.That(headers.Any(h => h.Name == "Content-Security-Policy" && (h.Value?.ToString() ?? "").Contains("default-src 'self'")), Is.True);
            }
        }

        [Test]
        [AllureDescription(@"What: Ensure RU culture cookie affects product details localization.
Steps:
1) GET '/Products/Details/1' with '.AspNetCore.Culture=c=ru|uic=ru'.
Expected: The page contains Russian localized strings.")]
        public void Products_Details_Localized_RU()
        {
            if (_httpClient != null)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "/Products/Details/1");
                request.Headers.Add("Cookie", ".AspNetCore.Culture=c=ru|uic=ru");
                var resp = _httpClient.SendAsync(request).GetAwaiter().GetResult();
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                var content = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.That(content, Does.Contain("Комфорт разработчика"));
            }
            else
            {
                var req = new RestRequest("/Products/Details/1", Method.Get);
                req.AddHeader("Cookie", ".AspNetCore.Culture=c=ru|uic=ru");
                var resp = _rsClient!.Execute(req);
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resp.Content ?? string.Empty, Does.Contain("Комфорт разработчика"));
            }
        }

        [Test]
        [AllureDescription(@"What: Check presence of cookie consent banner on Home.
Steps:
1) GET '/'.
Expected: HTML contains element with id 'cookie-consent'.")]
        public void Home_Has_CookieBanner()
        {
            if (_httpClient != null)
            {
                var resp = _httpClient.GetAsync("/").GetAwaiter().GetResult();
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                var content = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.That(content, Does.Contain("id=\"cookie-consent\""));
            }
            else
            {
                var resp = _rsClient!.Execute(new RestRequest("/", Method.Get));
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resp.Content ?? string.Empty, Does.Contain("id=\"cookie-consent\""));
            }
        }

        [Test]
        [AllureDescription(@"What: Verify CSP header contains a per-request nonce on Home.
Steps:
1) GET '/'.
Expected: 'Content-Security-Policy' header exists and contains ""script-src 'self' 'nonce-...'"".")]
        public void Home_Csp_ContainsNonce()
        {
            if (_httpClient != null)
            {
                var resp = _httpClient.GetAsync("/").GetAwaiter().GetResult();
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(resp.Headers.Contains("Content-Security-Policy"), Is.True);
            }
            else
            {
                var resp = _rsClient!.Execute(new RestRequest("/", Method.Get));
                Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                var headers = resp.Headers ?? Array.Empty<HeaderParameter>();
                var csp = headers.FirstOrDefault(h => h.Name == "Content-Security-Policy")?.Value?.ToString() ?? string.Empty;
                Assert.That(csp, Does.Contain("script-src 'self' 'nonce-"));
            }
        }
    }
}
