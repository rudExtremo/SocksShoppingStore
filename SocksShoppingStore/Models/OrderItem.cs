using System.ComponentModel.DataAnnotations;

namespace SocksShoppingStore.Models
{
    public class OrderItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;
    }
}

