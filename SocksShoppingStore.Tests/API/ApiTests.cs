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
    [AllureEpic("Магазин")]
    [AllureSuite("API Тесты")]
    [Category("API-Smoke")]
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
        [AllureStory("Каталог товаров")]
        [AllureDescription("Тест проверяет, что API возвращает корректный список товаров.")]
        public void GetAllProducts_ReturnsOkStatusAndCorrectNumberOfItems()
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

                AllureApi.Step("Шаг 1: Отправить GET-запрос на /api/products", () =>
                {
                    response = _client!.Execute(request);

                    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(response.Content, Is.Not.Null);

                    socks = JsonSerializer.Deserialize<List<Sock>>(response.Content!);
                });

                AllureApi.Step("Шаг 2: Проверить количество товаров в ответе", () =>
                {
                    Assert.That(socks, Is.Not.Null);
                    Assert.That(socks!.Count, Is.EqualTo(8));
                });
            }
        }
    }
}

