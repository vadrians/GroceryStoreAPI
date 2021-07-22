using GroceryStoreAPI.Contracts.Enums;
using GroceryStoreAPI.Contracts.Interfaces;
using GroceryStoreAPI.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Application
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync()
        {
            return await _repository.GetAllCustomersAsync();
        }

        public async Task<Result<Customer>> GetCustomerAsync(int id)
        {
            var result = await _repository.GetAllCustomersAsync();

            if (result.Failed())
            {
                return Result<Customer>.Errored(result.Status, result.ErrorMessage);
            }

            var customer = result.Content.FirstOrDefault(c => c.Id == id);

            return customer == null
                ? Result<Customer>.NotFound("Customer not found")
                : Result<Customer>.Success(customer);
        }

        public async Task<Result> RemoveCustomerAsync(int id)
        {
            return await ValidateCustomerAndRun(id, 
                async () => await _repository.RemoveCustomerAsync(id));
        }

        public async Task<Result<Customer>> AddCustomer(string name)
        {
            var allCustomers = await _repository.GetAllCustomersAsync();

            if (allCustomers.Failed())
            {
                return Result<Customer>.Errored(allCustomers.Status, allCustomers.ErrorMessage);
            }

            if (allCustomers.Content.Any(c => c.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
            {
                return Result<Customer>.Errored(ResultStatus.BadRequest, "Customer already exists");
            }

            var customer = await _repository.AddCustomerAsync(name);

            return customer.Succeeded() 
                ? Result<Customer>.Success(customer.Content)
                : Result<Customer>.Errored(customer.Status, customer.ErrorMessage);
        }

        public async Task<Result> UpdateCustomer(Customer customer)
        {
            return await ValidateCustomerAndRun(customer.Id, 
                async () => await _repository.UpdateCustomerAsync(customer));
        }

        private async Task<Result> ValidateCustomerAndRun(int id, Func<Task<Result>> func)
        {
            var allCustomers = await _repository.GetAllCustomersAsync();

            if (allCustomers.Failed())
            {
                return Result<Customer>.Errored(allCustomers.Status, allCustomers.ErrorMessage);
            }

            if (allCustomers.Content.All(c => c.Id != id))
            {
                return Result<Customer>.NotFound("Customer not found");
            }

            var funcResult = await func();

            return funcResult.Succeeded()
            ? Result<Customer>.Success()
                : Result<Customer>.Errored(funcResult.Status, funcResult.ErrorMessage);
        }
    }
}
