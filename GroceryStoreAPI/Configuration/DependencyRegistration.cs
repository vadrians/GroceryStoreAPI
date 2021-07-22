using GroceryStoreAPI.Application;
using GroceryStoreAPI.Application.Cache;
using GroceryStoreAPI.Contracts.Interfaces;
using GroceryStoreAPI.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GroceryStoreAPI.Configuration
{
    public static class DependencyRegistration
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddScoped<ICustomerRepository>(x =>
                new CustomerCacheRepository(
                    new CustomerJsonRepository(DatabaseConfiguration.GetDatabasePath(),
                        x.GetRequiredService<ILogger<CustomerJsonRepository>>()),
                    x.GetRequiredService<IMemoryCache>()));

            services.AddScoped<ICustomerService, CustomerService>();
        }
    }
}
