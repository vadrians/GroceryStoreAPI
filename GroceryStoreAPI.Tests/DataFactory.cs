using GroceryStoreAPI.Contracts.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace GroceryStoreAPI.Tests
{
    public static class DataFactory
    {
        public static Result<IEnumerable<Customer>> GetTestCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer(1, "Victor"),
                new Customer(2, "Adrian"),
            };

            return  Result<IEnumerable<Customer>>.Success(customers);
        }

    }
}
