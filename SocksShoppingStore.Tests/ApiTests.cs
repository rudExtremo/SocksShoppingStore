using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using RestSharp;
using SocksShoppingStore.Models;
using System.Net;
using System.Text.Json;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("Магазин")]
    [AllureSuite("API Тесты")]
    public class ApiTests
    {
        private RestClient? _client;

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
        [AllureStory("Каталог товаров")]
        [AllureDescription("Тест проверяет, что API возвращает корректный список товаров.")]
        public void GetAllProducts_ReturnsOkStatusAndCorrectNumberOfItems()
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