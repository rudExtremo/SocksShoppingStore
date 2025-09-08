using SocksShoppingStore.Models;
using System.Globalization;

namespace SocksShoppingStore.Services
{
    public static class ProductCatalogLocalizer
    {
        // Map: culture -> (productId -> (name, description))
        private static readonly Dictionary<string, Dictionary<int, (string name, string description)>> _map
            = new(StringComparer.OrdinalIgnoreCase)
            {
                ["ru"] = new Dictionary<int, (string, string)>
                {
                    [1] = ("Комфорт разработчика", "Дышащие хлопковые носки для долгих сессий кодинга."),
                    [2] = ("Режим менеджера", "Классические офисные носки с ненавязчивыми полосками."),
                    [3] = ("Страж QA", "Прочные носки, пережившие бесконечные циклы тестов."),
                    [4] = ("Подпись CEO", "Премиальный вид для уверенности на совете директоров."),
                    [5] = ("Фокус аналитика", "Лёгкие носки для дней с глубокой аналитикой."),
                    [6] = ("Палитра дизайнера", "Яркий комфорт под настроение вашего мудборда."),
                    [7] = ("Импульс маркетолога", "Будьте заметны на стендапе в ярких тонах."),
                    [8] = ("Щит безопасности", "Удобные минималистичные носки с усиленной пяткой.")
                }
            };

        public static List<Sock> Localize(List<Sock> items, string? culture)
        {
            if (items.Count == 0) return items;
            var key = Normalize(culture);
            if (!_map.TryGetValue(key, out var dict)) return items;

            foreach (var s in items)
            {
                if (dict.TryGetValue(s.Id, out var t))
                {
                    s.Name = t.name;
                    s.Description = t.description;
                }
            }
            return items;
        }

        public static Sock Localize(Sock item, string? culture)
        {
            var key = Normalize(culture);
            if (_map.TryGetValue(key, out var dict) && dict.TryGetValue(item.Id, out var t))
            {
                item.Name = t.name;
                item.Description = t.description;
            }
            return item;
        }

        private static string Normalize(string? culture)
        {
            if (string.IsNullOrWhiteSpace(culture)) return "en";
            return culture.StartsWith("ru", StringComparison.OrdinalIgnoreCase) ? "ru" : "en";
        }
    }
}

