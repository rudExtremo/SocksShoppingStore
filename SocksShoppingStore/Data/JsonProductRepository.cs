using System.Text.Json;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Data
{
    public class JsonProductRepository : IProductRepository
    {
        private readonly string _filePath;
        private readonly object _gate = new object();

        private class Storage
        {
            public int NextId { get; set; } = 1;
            public List<Sock> Socks { get; set; } = new();
        }

        public JsonProductRepository(string filePath)
        {
            _filePath = filePath;
            EnsureFile();
        }

        public List<Sock> GetAllSocks()
        {
            var st = Read();
            return st.Socks.ToList();
        }

        public Sock? GetSockById(int id)
        {
            var st = Read();
            return st.Socks.FirstOrDefault(s => s.Id == id);
        }

        public void Add(Sock item)
        {
            lock (_gate)
            {
                var st = Read();
                if (item.Id == 0)
                {
                    item.Id = st.NextId++;
                }
                else if (st.Socks.Any(s => s.Id == item.Id))
                {
                    // align next id above any provided explicit ids
                    st.NextId = Math.Max(st.NextId, item.Id + 1);
                }
                st.Socks.Add(Clone(item));
                Write(st);
            }
        }

        public bool Update(Sock item)
        {
            lock (_gate)
            {
                var st = Read();
                var idx = st.Socks.FindIndex(s => s.Id == item.Id);
                if (idx < 0) return false;
                st.Socks[idx] = Clone(item);
                Write(st);
                return true;
            }
        }

        public bool Delete(int id)
        {
            lock (_gate)
            {
                var st = Read();
                var removed = st.Socks.RemoveAll(s => s.Id == id) > 0;
                if (removed) Write(st);
                return removed;
            }
        }

        private void EnsureFile()
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            // Seed file if it doesn't exist or is empty
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
            {
                // seed from legacy in-memory repo
                var seed = new Storage
                {
                    Socks = ProductRepository.GetAllSocks().Select(Clone).ToList(),
                    NextId = (ProductRepository.GetAllSocks().Count == 0 ? 1 : ProductRepository.GetAllSocks().Max(s => s.Id) + 1)
                };
                Write(seed);
            }
        }

        private Storage Read()
        {
            using var fs = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return JsonSerializer.Deserialize<Storage>(fs, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new Storage();
        }

        private void Write(Storage st)
        {
            using var fs = File.Open(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            var opts = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.Serialize(fs, st, opts);
        }

        private static Sock Clone(Sock s) => new Sock
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Price = s.Price,
            ImageUrl = s.ImageUrl
        };
    }
}

