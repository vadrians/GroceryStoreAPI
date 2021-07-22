using GroceryStoreAPI.Contracts.Interfaces;
using GroceryStoreAPI.Contracts.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Application.Cache
{
    public class CustomerCacheRepository : ICustomerRepository
    {
        private readonly ICustomerRepository _implementation;
        private readonly IMemoryCache _cache;

        public CustomerCacheRepository(ICustomerRepository implementation, IMemoryCache cache)
        {
            _implementation = implementation;
            _cache = cache;
        }

        private string GetCacheKey(string key)
        {
            return $"{nameof(CustomerCacheRepository)}-{key}";
        }

        public async Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync()
        {
            var cachedItem = _cache.Get<IEnumerable<Customer>>(GetCacheKey(nameof(GetAllCustomersAsync)));

            if (cachedItem == null)
            {
                var result = await _implementation.GetAllCustomersAsync();

                if (result.Failed())
                {
                    return Result<IEnumerable<Customer>>.Errored(result.Status, result.ErrorMessage);
                }

                _cache.Set(GetCacheKey(nameof(GetAllCustomersAsync)), result.Content);
                return Result<IEnumerable<Customer>>.Success(result.Content);

            }

            return Result<IEnumerable<Customer>>.Success(cachedItem);
        }

        public async Task<Result> UpdateCustomerAsync(Customer customer)
        {
            var result = await _implementation.UpdateCustomerAsync(customer);
            _cache.Remove(GetCacheKey(nameof(GetAllCustomersAsync)));
            return result;
        }

        public async Task<Result> RemoveCustomerAsync(int id)
        {
            var result = await _implementation.RemoveCustomerAsync(id);
            _cache.Remove(GetCacheKey(nameof(GetAllCustomersAsync)));
            return result;
        }

        public async Task<Result<Customer>> AddCustomerAsync(string name)
        {
            var result = await _implementation.AddCustomerAsync(name);
            _cache.Remove(GetCacheKey(nameof(GetAllCustomersAsync)));
            return result;
        }
    }
}
