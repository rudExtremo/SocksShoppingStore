using SocksShoppingStore.Models;

namespace SocksShoppingStore.Data
{
    // Adapter over existing static ProductRepository to enable DI gradually
    public class LegacyProductRepository : IProductRepository
    {
        public List<Sock> GetAllSocks() => ProductRepository.GetAllSocks();
        public Sock? GetSockById(int id) => ProductRepository.GetSockById(id);
        public void Add(Sock item)
        {
            var all = ProductRepository.GetAllSocks();
            if (item.Id == 0)
            {
                item.Id = (all.Count == 0 ? 1 : all.Max(s => s.Id) + 1);
            }
            all.Add(item);
        }
        public bool Update(Sock item)
        {
            var all = ProductRepository.GetAllSocks();
            var idx = all.FindIndex(s => s.Id == item.Id);
            if (idx < 0) return false;
            all[idx] = item;
            return true;
        }
        public bool Delete(int id)
        {
            var all = ProductRepository.GetAllSocks();
            return all.RemoveAll(s => s.Id == id) > 0;
        }
    }
}

