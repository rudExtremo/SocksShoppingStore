namespace SocksShoppingStore.Models
{
    public class CartItem
    {
        public Sock Sock { get; set; } = new();
        public int Quantity { get; set; }
    }
}