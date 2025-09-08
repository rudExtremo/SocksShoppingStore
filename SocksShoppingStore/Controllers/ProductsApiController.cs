using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SocksShoppingStore.Data;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Controllers
{
    [Route("api/products")]
    [ApiController]
    [EnableRateLimiting("api")]
    public class ProductsApiController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<Sock>> GetAllProducts()
        {
            var socks = ProductRepository.GetAllSocks();
            return Ok(socks);
        }
    }
}

