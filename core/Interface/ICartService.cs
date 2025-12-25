using System;
using core.Entities;

namespace core.Interface;

public interface ICartService
{
    Task<ShoppingCart> GetCartAsync(string key);
    Task<ShoppingCart> SetCartAsync(ShoppingCart cart);
    Task<bool> DeleteCartAsync(string key);

}
