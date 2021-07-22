using GroceryStoreAPI.Contracts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Contracts.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync();
        Task<Result> UpdateCustomerAsync(Customer customer);
        Task<Result> RemoveCustomerAsync(int id);
        Task<Result<Customer>> AddCustomerAsync(string name);
    }
}
