using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Data
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IDatabase _db;
        public ShoppingCartRepository(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<bool> DeleteShoppingCartAsync(string shoppingCartId)
        {
            return await _db.KeyDeleteAsync(shoppingCartId);
        }

        public async Task<CustomerShoppingCart> GetShoppingCartAsync(string shoppingCartId)
        {
            Console.WriteLine("HERE:" + shoppingCartId);
            var data = await _db.StringGetAsync(shoppingCartId);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerShoppingCart>(data);
        }

        public async Task<CustomerShoppingCart> UpdateShoppingCartAsync(CustomerShoppingCart shoppingCart)
        {
            var created = await _db.StringSetAsync(shoppingCart.Id,
                                JsonSerializer.Serialize(shoppingCart), TimeSpan.FromDays(3));
            if (!created) return null;
            return await GetShoppingCartAsync(shoppingCart.Id);
        }
    }
}