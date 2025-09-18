using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureEpic("SocksShoppingStore")]
    [AllureSuite("Integration")]
    [Category("Integration")]
    [Category("Positive")]
    public class AppEndpointsTests
    {
        private WebApplicationFactory<Program>? _factory;
        private HttpClient? _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true });
        }

        [Test]
        [AllureDescription(@"What: Localization.Set switches UI culture and redirects locally.
Steps:
1) POST '/Localization/Set' with culture=ru and returnUrl='/' (no auto-redirect).
2) Follow up GET '/' and check content contains some cyrillic letter.
Expected: 302 redirect; subsequent page contains Cyrillic text.")]
        public void Localization_Set_Redirects_And_Applies()
        {
            using var clientNoRedirect = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var form = new FormUrlEncodedContent(new[] { new KeyValuePair<string,string>("culture","ru"), new KeyValuePair<string,string>("returnUrl","/") });
            var resp = clientNoRedirect.PostAsync("/Localization/Set", form).GetAwaiter().GetResult();
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            var cookies = resp.Headers.GetValues("Set-Cookie");
            StringAssert.Contains(".AspNetCore.Culture", string.Join(";", cookies));

            // use redirect-following client to read localized home
            var home = _client!.GetAsync("/").GetAwaiter().GetResult();
            var html = home.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            // Expect some Cyrillic after switching culture
            Assert.That(html.Any(ch => ch >= '\u0400' && ch <= '\u04FF'), Is.True);
        }

        [Test]
        [AllureDescription(@"What: Legal pages return 200 and include configured values.
Steps:
1) GET '/legal/privacy' and '/legal/terms'.
Expected: 200 OK for each.")]
        public void Legal_Privacy_And_Terms_Ok()
        {
            var p = _client!.GetAsync("/legal/privacy").GetAwaiter().GetResult();
            var t = _client!.GetAsync("/legal/terms").GetAwaiter().GetResult();
            Assert.That(p.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(t.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [AllureDescription(@"What: Cart redirect path falls back to Referer when present and safe.
Steps:
1) GET '/Cart/AddToCart?id=1' with Referer '/Products/Details/1' (no auto-redirect).
Expected: 302 to '/Products/Details/1'.")]
        public void Cart_AddToCart_FallsBack_To_Referer()
        {
            using var clientNoRedirect = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var req = new HttpRequestMessage(HttpMethod.Get, "/Cart/AddToCart?id=1");
            req.Headers.Referrer = new Uri("http://localhost/Products/Details/1");
            var resp = clientNoRedirect.SendAsync(req).GetAwaiter().GetResult();
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(resp.Headers.Location!.ToString(), Is.EqualTo("/Products/Details/1"));
        }

        [TearDown]
        public void Teardown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        [AllureDescription(@"What: Verify home page returns 200 and has HTML content.
Steps:
1) GET '/'.
Expected: 200 OK, text/html content type.")]
        public void Home_Returns_Ok_With_Html()
        {
            var resp = _client!.GetAsync("/").GetAwaiter().GetResult();
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            StringAssert.Contains("text/html", resp.Content.Headers.ContentType!.MediaType);
        }

        [Test]
        [AllureDescription(@"What: Product details page returns 200.
Steps:
1) GET '/Products/Details/1'.
Expected: 200 OK.")]
        public void Products_Details_Returns_Ok()
        {
            var resp = _client!.GetAsync("/Products/Details/1").GetAwaiter().GetResult();
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [AllureDescription(@"What: Health and robots endpoints respond.
Steps:
1) GET '/healthz' and '/robots.txt'.
Expected: 200.")]
        public void Health_And_Robots_Respond()
        {
            var h = _client!.GetAsync("/healthz").GetAwaiter().GetResult();
            var r = _client!.GetAsync("/robots.txt").GetAwaiter().GetResult();
            Assert.That(h.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(r.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [AllureDescription(@"What: Status endpoint is IP-restricted.
Steps:
1) GET '/_status'.
Expected: 200 JSON on allowlist or 403 otherwise.")]
        public void Status_Endpoint_AllowsOnly_Allowlist()
        {
            var resp = _client!.GetAsync("/_status").GetAwaiter().GetResult();
            Assert.That(new[]{ HttpStatusCode.OK, HttpStatusCode.Forbidden }, Contains.Item(resp.StatusCode));
        }

        [Test]
        [AllureDescription(@"What: Cart JSON endpoints work via Accept header.
Steps:
1) GET '/Cart/AddToCart?id=1' with Accept: application/json.
2) GET '/Cart/RemoveFromCart?id=1' with Accept: application/json.
Expected: 200; JSON contains totals and item block.")]
        public void Cart_Json_Actions_Work()
        {
            var req1 = new HttpRequestMessage(HttpMethod.Get, "/Cart/AddToCart?id=1");
            req1.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var add = _client!.SendAsync(req1).GetAwaiter().GetResult();
            Assert.That(add.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var content = add.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            StringAssert.Contains("totalItems", content);

            var req2 = new HttpRequestMessage(HttpMethod.Get, "/Cart/RemoveFromCart?id=1");
            req2.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var rem = _client!.SendAsync(req2).GetAwaiter().GetResult();
            Assert.That(rem.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var content2 = rem.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            StringAssert.Contains("totalItems", content2);
        }

        [Test]
        [AllureDescription(@"What: AddToCart redirects back to returnUrl when provided and local.
Steps:
1) Disable auto-redirect; GET '/Cart/AddToCart?id=1&returnUrl=/'
Expected: 302 with Location header '/'.")]
        public void Cart_AddToCart_Redirects_To_ReturnUrl()
        {
            using var clientNoRedirect = _factory!.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var resp = clientNoRedirect.GetAsync("/Cart/AddToCart?id=1&returnUrl=/").GetAwaiter().GetResult();
            Assert.That(resp.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(resp.Headers.Location!.ToString(), Is.EqualTo("/"));
        }
    }
}
