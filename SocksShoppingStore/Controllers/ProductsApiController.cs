using Microsoft.AspNetCore.Mvc;
using SocksShoppingStore.Data;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Controllers
{
    [Route("api/products")] // Базовый адрес для всех методов в этом контроллере
    [ApiController]       // Атрибут, который включает специальные возможности для API
    public class ProductsApiController : ControllerBase
    {
        [HttpGet] // Этот метод будет отвечать на GET-запросы
        public ActionResult<IEnumerable<Sock>> GetAllProducts()
        {
            // Получаем все товары из нашего репозитория
            var socks = ProductRepository.GetAllSocks();
            // Возвращаем их с HTTP-статусом 200 OK
            return Ok(socks);
        }
    }
}
