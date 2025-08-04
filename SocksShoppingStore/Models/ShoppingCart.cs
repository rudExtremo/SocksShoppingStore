namespace SocksShoppingStore.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new();

        public void AddItem(Sock sock)
        {
            var item = Items.FirstOrDefault(i => i.Sock.Id == sock.Id);
            if (item == null)
            {
                Items.Add(new CartItem { Sock = sock, Quantity = 1 });
            }
            else
            {
                item.Quantity++;
            }
        }

        public void RemoveItem(int sockId)
        {
            var item = Items.FirstOrDefault(i => i.Sock.Id == sockId);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    Items.Remove(item);
                }
            }
        }

        public void DeleteItem(int sockId)
        {
            Items.RemoveAll(i => i.Sock.Id == sockId);
        }

        public decimal GetTotalSum()
        {
            return Items.Sum(i => i.Sock.Price * i.Quantity);
        }

        public int GetTotalItems()
        {
            return Items.Sum(i => i.Quantity);
        }
    }
}