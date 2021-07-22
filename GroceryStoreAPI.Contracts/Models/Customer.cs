namespace GroceryStoreAPI.Contracts.Models
{
    public class Customer
    {
        public Customer()
        {
            
        }

        public Customer(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
