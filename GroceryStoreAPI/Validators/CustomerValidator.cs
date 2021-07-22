using GroceryStoreAPI.Contracts.Models;
using System;

namespace GroceryStoreAPI.Validators
{
    public static class CustomerValidator
    {

        public static void Validate(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid Id");
            }
        }

        public static void Validate(Customer customer)
        {
            Validate(customer.Id);
            Validate(customer.Name);
        }

        public static void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name can't be empty");
            }
        }
    }
}
