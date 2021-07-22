using GroceryStoreAPI.Contracts.Enums;
using GroceryStoreAPI.Contracts.Interfaces;
using GroceryStoreAPI.Contracts.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryStoreAPI.Data
{
    public class CustomerJsonRepository : ICustomerRepository
    {
        private readonly string _filePath;
        private readonly ILogger<CustomerJsonRepository> _logger;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public CustomerJsonRepository(string filePath, ILogger<CustomerJsonRepository> logger)
        {
            _filePath = filePath;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<Customer>>> GetAllCustomersAsync()
        {
            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                var wrapper = JsonSerializer.Deserialize<CustomerWrapper>(json, _jsonOptions);
                return Result<IEnumerable<Customer>>.Success(wrapper.Customers);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reading file {@parameters}", new
                {
                    file = _filePath
                });
                return Result<IEnumerable<Customer>>.Errored(ResultStatus.InternalError, "Error getting customers");
            }
        }

        public async Task<Result> UpdateCustomerAsync(Customer customer)
        {
            return await GetCustomersWrapper(
                async (customers) =>
                {
                    var dbCustomer = customers.FirstOrDefault(c => c.Id == customer.Id);

                    if (dbCustomer != null)
                    {
                        dbCustomer.Name = customer.Name;
                        var result = await WriteFileAsync(customers);

                        return result.Succeeded()
                            ? Result<Customer>.Success()
                            : Result<Customer>.Errored(result.Status, result.ErrorMessage);
                    }

                    return Result<Customer>.Success();
                }
            );

        }

        public async Task<Result> RemoveCustomerAsync(int id)
        {
            return await GetCustomersWrapper(
                async (customers) =>
                {
                    var result = await WriteFileAsync(customers.Where(c => c.Id != id));

                    return result.Succeeded()
                        ? Result<Customer>.Success()
                        : Result<Customer>.Errored(result.Status, result.ErrorMessage);
                }
            );
        }

        private async Task<Result<T>> GetCustomersWrapper<T>(Func<IEnumerable<Customer>, Task<Result<T>>> func)
        {
            var result = await GetAllCustomersAsync();

            if (result.Succeeded())
            {
                return await func(result.Content);
            }

            return Result<T>.Errored(ResultStatus.InternalError, "Not possible to get existing customers, operation aborted");

        }

        public async Task<Result<Customer>> AddCustomerAsync(string name)
        {
            return await GetCustomersWrapper(
               async (customers) => 
               {
                    var newId = customers.OrderByDescending(c => c.Id).FirstOrDefault()?.Id;
                    var customer = new Customer(newId + 1 ?? 1, name);
                    var writeResult = await WriteFileAsync(customers.Concat(new[]{ customer}));

                    return writeResult.Succeeded()
                        ? Result<Customer>.Success(customer)
                        : Result<Customer>.Errored(writeResult.Status, writeResult.ErrorMessage);
                }
            );
        }

        private async Task<Result> WriteFileAsync(IEnumerable<Customer> customers)
        {
            try
            {
                var json = JsonSerializer.Serialize(new CustomerWrapper() { Customers = customers }, _jsonOptions);
                await File.WriteAllTextAsync(_filePath, json);
                return Result<IEnumerable<Customer>>.Success(customers);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error writing file {_filePath}");
                return Result<IEnumerable<Customer>>.Errored(ResultStatus.InternalError, "Error writing to DB");
            }
        }
        
        private class CustomerWrapper
        {
            public IEnumerable<Customer> Customers { get; set; }
        }
    }
}
