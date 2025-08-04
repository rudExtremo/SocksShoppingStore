using NUnit.Framework;

namespace SocksShoppingStore.Tests
{
    [TestFixture]
    public class ShoppingCartTests : BaseTest // Наследуемся от нашего базового класса
    {
        [Test]
        public void AddToCart_SingleItem_UpdatesCartCounter()
        {
            // 1. Открываем сайт
            HomePage!.Navigate();

            // 2. Проверяем, что счетчик корзины равен "0"
            Assert.That(HomePage.CartItemCountBadge.Text, Is.EqualTo("0"), "Изначально счетчик корзины должен быть 0");

            // 3. Добавляем первый товар в корзину
            HomePage.AddFirstProductToCart();

            // 4. Ждем немного, чтобы счетчик успел обновиться (на реальных проектах используют явные ожидания)
            Thread.Sleep(500);

            // 5. Проверяем, что счетчик корзины стал "1"
            Assert.That(HomePage.CartItemCountBadge.Text, Is.EqualTo("1"), "Счетчик корзины не обновился после добавления товара");
        }

        [Test]
        public void Cart_FullWorkflow_CalculatesTotalCorrectly()
        {
            // 1. Открываем сайт и добавляем первый товар
            HomePage!.Navigate();
            HomePage.AddFirstProductToCart();
            Thread.Sleep(500); // Пауза для обновления

            // 2. Добавляем этот же товар еще раз
            HomePage.AddFirstProductToCart();
            Thread.Sleep(500); // Пауза для обновления

            // 3. Переходим в корзину
            HomePage.GoToCart();

            // 4. Проверяем количество первого товара (должно быть 2)
            Assert.That(CartPage!.GetFirstItemQuantity(), Is.EqualTo(2), "Количество товара в корзине неверное");

            // 5. Проверяем, что итоговая сумма верна (2 * 3.50€ = 7.00€)
            // Цена первого товара "Носки 'Программист'" - 3.50
            decimal expectedSum = 7.00m;
            Assert.That(CartPage.GetTotalSum(), Is.EqualTo(expectedSum), "Итоговая сумма в корзине рассчитана неверно");

            // 6. Удаляем товар и проверяем, что корзина пуста
            CartPage.DeleteFirstItem();
            Assert.That(CartPage.IsCartEmpty(), Is.True, "Корзина не пуста после удаления товара");
        }
    }
}