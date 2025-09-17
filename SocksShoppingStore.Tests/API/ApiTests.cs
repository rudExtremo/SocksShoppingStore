using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using RestSharp;
using SocksShoppingStore.Models;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("API")]
    [Category("API-Smoke")]
    [Category("Positive")]
    public class ApiTests
    {
        private RestClient? _client;
        private WebApplicationFactory<Program>? _factory;
        private HttpClient? _httpClient;

        [SetUp]
        public void Setup()
        {
            if (TestSettings.UseTestFactory)
            {
                _factory = new WebApplicationFactory<Program>();
                _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
            }
            else
            {
                string baseUrl = TestSettings.BaseUrl;
                var options = new RestClientOptions(baseUrl)
                {
                    RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => TestSettings.IgnoreCertErrors
                };
                _client = new RestClient(options);
            }
        }

        [Test]
        [AllureStory("Products API")]
        [AllureDescription(@"What: Verify /api/products returns 200 and expected number of items.
Steps:
1) Issue GET /api/products (via HttpClient or RestClient).
2) Deserialize response to list of Sock.
Expected: HTTP 200; list is not null; count equals 8.")]
        public void ProductsApi_GetAll_ReturnsOk_AndExpectedCount()
        {
            if (_httpClient != null)
            {
                var response = _httpClient.GetAsync("api/products").GetAwaiter().GetResult();
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var socks = JsonSerializer.Deserialize<List<Sock>>(content!);
                Assert.That(socks, Is.Not.Null);
                Assert.That(socks!.Count, Is.EqualTo(8));
            }
            else
            {
                List<Sock>? socks = null;
                var request = new RestRequest("api/products", Method.Get);
                RestResponse response;

                AllureApi.Step("Step 1: GET /api/products", () =>
                {
                    response = _client!.Execute(request);

                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Content, Is.Not.Null);

                    socks = JsonSerializer.Deserialize<List<Sock>>(response.Content!);
                });

                AllureApi.Step("Step 2: Validate list not null and size", () =>
                {
                    Assert.That(socks, Is.Not.Null);
                    Assert.That(socks!.Count, Is.EqualTo(8));
                });
            }
        }

        [Test]
        [Category("Negative")]
        [AllureStory("Rate Limiting")]
        [AllureDescription(@"What: Validate API rate limiting per free-tier constraints (ApiPerMinute=20).
Steps:
1) Send >20 requests to /api/products within the same minute.
Expected: At least one response has status 429 Too Many Requests.")]
        public void ProductsApi_RateLimit_Enforced_ByPolicy()
        {
            const int attempts = 25; // ApiPerMinute=20 -> exceed window
            var statuses = new List<int>();
            if (_httpClient != null)
            {
                for (int i = 0; i < attempts; i++)
                {
                    var resp = _httpClient.GetAsync("api/products").GetAwaiter().GetResult();
                    statuses.Add((int)resp.StatusCode);
                }
            }
            else
            {
                for (int i = 0; i < attempts; i++)
                {
                    var resp = _client!.Execute(new RestRequest("api/products", Method.Get));
                    statuses.Add((int)resp.StatusCode);
                }
            }
            Assert.That(statuses.Any(s => s == 429), "Expected at least one 429 due to rate limit.");
        }

        [Test]
        [Category("Negative")]
        [AllureStory("Rate Limiting")]
        [AllureDescription(@"What: When client sends Accept: application/json and exceeds API rate limit, the 429 response is JSON.
Steps:
1) Issue > ApiPerMinute requests to /api/products with Accept=application/json.
Expected: Receive 429 Too Many Requests with application/json body containing 'error' and 'message'.")]
        public void ProductsApi_RateLimit_ReturnsJson_WhenAcceptJson()
        {
            const int attempts = 25;
            if (_httpClient != null)
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                for (int i = 0; i < attempts; i++)
                {
                    var resp = _httpClient.GetAsync("api/products").GetAwaiter().GetResult();
                    if ((int)resp.StatusCode == 429)
                    {
                        var ct = resp.Content.Headers.ContentType?.MediaType ?? string.Empty;
                        var body = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        StringAssert.Contains("application/json", ct);
                        StringAssert.Contains("\"error\"", body);
                        StringAssert.Contains("\"message\"", body);
                        return;
                    }
                }
                Assert.Fail("Expected a 429 response but none received");
            }
            else
            {
                var req = new RestRequest("api/products", Method.Get);
                req.AddHeader("Accept", "application/json");
                for (int i = 0; i < attempts; i++)
                {
                    var resp = _client!.Execute(req);
                    if ((int)resp.StatusCode == 429)
                    {
                        StringAssert.Contains("application/json", resp.ContentType ?? string.Empty);
                        StringAssert.Contains("\"error\"", resp.Content ?? string.Empty);
                        StringAssert.Contains("\"message\"", resp.Content ?? string.Empty);
                        return;
                    }
                }
                Assert.Fail("Expected a 429 response but none received");
            }
        }
    }
}
