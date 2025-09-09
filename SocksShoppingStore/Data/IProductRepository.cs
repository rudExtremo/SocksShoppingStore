using SocksShoppingStore.Models;

namespace SocksShoppingStore.Data
{
    public interface IProductRepository
    {
        List<Sock> GetAllSocks();
        Sock? GetSockById(int id);
        // Future admin features
        void Add(Sock item);
        bool Update(Sock item);
        bool Delete(int id);
    }
}

