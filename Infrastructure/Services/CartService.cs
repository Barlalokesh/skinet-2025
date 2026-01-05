using System;
using System.Text.Json;
using core.Entities;
using core.Interface;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class CartService(IConnectionMultiplexer redis) : ICartService
{
    private readonly IDatabase _database = redis.GetDatabase();

    // Delete cart
    public async Task<bool> DeleteCartAsync(string id)
    {
        return await _database.KeyDeleteAsync(GetRedisKey(id));
    }

    // Get cart
    public async Task<ShoppingCart?> GetCartAsync(string id)
    {
        var data = await _database.StringGetAsync(GetRedisKey(id));
        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(data!);
    }

    // Set cart
    public async Task<ShoppingCart?> SetCartAsync(ShoppingCart cart)
    {
        var key = GetRedisKey(cart.Id); // ALWAYS use this
        var created = await _database.StringSetAsync(key, JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        if (!created) return null;

        return await GetCartAsync(cart.Id);
    }

    // Helper: consistent Redis key
    private RedisKey GetRedisKey(string id) => $"cart-{id}";
}
