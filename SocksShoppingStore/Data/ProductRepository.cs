using SocksShoppingStore.Models;

namespace SocksShoppingStore.Data
{
    public static class ProductRepository
    {
        private static readonly List<Sock> _socks = new List<Sock>
        {
            new Sock { Id = 1, Name = "Coder's Comfort", Description = "Breathable cotton crew socks for long coding sessions.", Price = 3.50m, ImageUrl = "/images/coder_socks.png" },
            new Sock { Id = 2, Name = "Manager Mode", Description = "Classic office socks with subtle stripes.", Price = 4.50m, ImageUrl = "/images/manager_socks.png" },
            new Sock { Id = 3, Name = "QA Guardian", Description = "Durable socks that survive endless test cycles.", Price = 3.99m, ImageUrl = "/images/qa_socks.png" },
            new Sock { Id = 4, Name = "CEO Signature", Description = "Premium look for boardroom confidence.", Price = 5.00m, ImageUrl = "/images/CEO.png" },
            new Sock { Id = 5, Name = "Analyst Focus", Description = "Lightweight socks for data-deep days.", Price = 3.20m, ImageUrl = "/images/analitic.png" },
            new Sock { Id = 6, Name = "Designer Palette", Description = "Colorful comfort to match your moodboard.", Price = 4.10m, ImageUrl = "/images/designer.png" },
            new Sock { Id = 7, Name = "Marketer Boost", Description = "Stand out at the stand-up with vibrant tones.", Price = 3.80m, ImageUrl = "/images/market.png" },
            new Sock { Id = 8, Name = "Security Shield", Description = "Comfy, minimal socks with reinforced heel.", Price = 2.50m, ImageUrl = "/images/security.png" }
        };

        public static List<Sock> GetAllSocks() => _socks;

        public static Sock? GetSockById(int id) => _socks.FirstOrDefault(s => s.Id == id);
    }
}

