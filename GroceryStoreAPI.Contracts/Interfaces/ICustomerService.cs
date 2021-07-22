using GroceryStoreAPI.Contracts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Contracts.Interfaces
{
    public interface ICustomerService
    {
        Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync();
        Task<Result<Customer>> GetCustomerAsync(int id);
        Task<Result> RemoveCustomerAsync(int id);
        Task<Result<Customer>> AddCustomer(string name);
        Task<Result> UpdateCustomer(Customer customer);
    }
}
