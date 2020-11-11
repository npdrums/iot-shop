using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ShoppingCartController : BaseApiController
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        public ShoppingCartController(IShoppingCartRepository shoppingCartRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerShoppingCart>> GetShoppingCartById(string id)
        {
            var shoppingCart = await _shoppingCartRepository.GetShoppingCartAsync(id);
            return Ok(shoppingCart ?? new CustomerShoppingCart(id));
        }

        [HttpPost]
        public async Task<ActionResult<CustomerShoppingCart>> UpdateShoppingCart(CustomerShoppingCart shoppingCart)
        {
            var updatedShoppingCart = await _shoppingCartRepository.UpdateShoppingCartAsync(shoppingCart);
            return Ok(updatedShoppingCart);
        }

        [HttpDelete]
        public async Task DeleteShoppingCart(string id)
        {
            await _shoppingCartRepository.DeleteShoppingCartAsync(id);
        }
    }
}