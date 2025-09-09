namespace SocksShoppingStore.Models
{
    public class CatalogViewModel
    {
        public List<Sock> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Query { get; set; }
        public string? Sort { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 1;
    }
}

