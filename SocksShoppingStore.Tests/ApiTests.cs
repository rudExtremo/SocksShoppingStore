using NUnit.Framework;
using RestSharp;
using SocksShoppingStore.Models;
using System.Net;
using System.Text.Json;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    public class ApiTests
    {
        private RestClient _client;

        [SetUp]
        public void Setup()
        {
            // Определяем, запущены ли мы в среде CI
            bool isCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            string baseUrl = isCi ? "http://127.0.0.1:5123" : "https://localhost:7068";

            // Для локального запуска с самодельным сертификатом нужно его игнорировать
            var options = new RestClientOptions(baseUrl)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            _client = new RestClient(options);
        }

        [Test]
        public void GetAllProducts_ReturnsOkStatusAndCorrectNumberOfItems()
        {
            // 1. Создаем запрос к нашему API
            var request = new RestRequest("api/products", Method.Get);

            // 2. Выполняем запрос
            var response = _client.Execute(request);

            // 3. Проверяем, что сервер ответил успешно (код 200 OK)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Статус-код ответа неверный");
            Assert.That(response.Content, Is.Not.Null, "Тело ответа не должно быть пустым");

            // 4. Десериализуем JSON-ответ в список объектов Sock
            var socks = JsonSerializer.Deserialize<List<Sock>>(response.Content!);

            // 5. Проверяем, что количество товаров в ответе правильное (у нас их 8)
            Assert.That(socks, Is.Not.Null);
            Assert.That(socks.Count, Is.EqualTo(8), "Количество товаров в API-ответе неверное");
        }
    }
}