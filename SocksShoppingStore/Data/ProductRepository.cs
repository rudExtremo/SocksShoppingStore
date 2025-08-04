using SocksShoppingStore.Models;

namespace SocksShoppingStore.Data
{
    public static class ProductRepository
    {
        private static readonly List<Sock> _socks = new List<Sock>
        {
            new Sock { Id = 1, Name = "Носки 'Программист'", Description = "Для долгих ночей кодинга", Price = 3.50m, ImageUrl = "/images/coder_socks.png" },
            new Sock { Id = 2, Name = "Носки 'Менеджер'", Description = "Чтобы уверенно шагать по офису", Price = 4.50m, ImageUrl = "/images/manager_socks.png" },
            new Sock { Id = 3, Name = "Носки 'Тестировщик'", Description = "В них легко найти любой баг", Price = 3.99m, ImageUrl = "/images/qa_socks.png" },
            new Sock { Id = 4, Name = "Носки 'CEO'", Description = "Для самых ярких решений.", Price = 5.00m, ImageUrl = "/images/CEO.png" },
            new Sock { Id = 5, Name = "Носки 'Аналитик'", Description = "Чтобы видеть все точки над 'i'.", Price = 3.20m, ImageUrl = "/images/analitic.png" },
            new Sock { Id = 6, Name = "Носки 'Дизайнер'", Description = "Креативность до кончиков пальцев.", Price = 4.10m, ImageUrl = "/images/designer.png" },
            new Sock { Id = 7, Name = "Носки 'Маркетолог'", Description = "Свежие идеи для вашего продвижения.", Price = 3.80m, ImageUrl = "/images/market.png" },
            new Sock { Id = 8, Name = "Носки 'Секьюрити'", Description = "Классика, которая всегда прикрывает.", Price = 2.50m, ImageUrl = "/images/security.png" }
        };

        public static List<Sock> GetAllSocks() => _socks;

        public static Sock? GetSockById(int id) => _socks.FirstOrDefault(s => s.Id == id);
    }
}