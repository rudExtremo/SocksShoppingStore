using NUnit.Framework;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureLabel("package", "SocksShoppingStore.Tests.UI")]
    [Category("UI-Smoke")]
    [Category("Positive")]
    [AllureEpic("Магазин")]
    [AllureSuite("UI Тесты")]
    [AllureFeature("Корзина")]
    [AllureSeverity(SeverityLevel.critical)]
    public class ShoppingCartTests : BaseTest
    {
        [Test]
        [AllureStory("Добавление товара")]
        [AllureDescription("Тест проверяет, что при добавлении товара в корзину счётчик на иконке обновляется.")]
        public void AddToCart_SingleItem_UpdatesCartCounter()
        {
            AllureApi.Step("Шаг 1: Открыть главную страницу", () =>
            {
                HomePage!.Navigate();
            });

            AllureApi.Step("Шаг 2: Проверить, что счётчик корзины равен 0", () =>
            {
                Assert.That(HomePage!.CartItemCountBadge.Text, Is.EqualTo("0"));
            });

            AllureApi.Step("Шаг 3: Добавить первый товар в корзину", () =>
            {
                HomePage!.AddFirstProductToCart();
            });

            AllureApi.Step("Шаг 4: Проверить, что счётчик корзины стал 1", () =>
            {
                Assert.That(HomePage!.CartItemCountBadge.Text, Is.EqualTo("1"));
            });
        }

        [Test]
        [AllureStory("Полный сценарий работы с корзиной")]
        [AllureDescription("Тест проверяет полный цикл: добавление нескольких товаров, проверку суммы и удаление.")]
        [Ignore("Temporarily disabled in CI to stabilize UI test")]
        public void Cart_FullWorkflow_CalculatesTotalCorrectly()
        {
            AllureApi.Step("Шаг 1: Открыть сайт и добавить два одинаковых товара", () =>
            {
                HomePage!.Navigate();
                HomePage.AddFirstProductToCart();
                HomePage.AddFirstProductToCart();
            });

            AllureApi.Step("Шаг 2: Перейти в корзину", () =>
            {
                HomePage!.GoToCart();
            });

            AllureApi.Step("Шаг 3: Проверить количество и итоговую сумму", () =>
            {
                Assert.That(CartPage!.GetFirstItemQuantity(), Is.EqualTo(2));
                decimal expectedSum = 6.40m;
                Assert.That(CartPage.GetTotalSum(), Is.EqualTo(expectedSum).Within(0.01m));
            });

            AllureApi.Step("Шаг 4: Удалить товар и проверить, что корзина пуста", () =>
            {
                CartPage!.DeleteFirstItem();
                Assert.That(CartPage.IsCartEmpty(), Is.True);
            });
        }
    }
}

