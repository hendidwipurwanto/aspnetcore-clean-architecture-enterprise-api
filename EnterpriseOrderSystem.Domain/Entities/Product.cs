using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        private Product() { }

        public Product(string name, decimal price, int stock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name is required");

            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero");

            if (stock < 0)
                throw new ArgumentException("Stock cannot be negative");

            Name = name;
            Price = price;
            Stock = stock;
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice <= 0)
                throw new ArgumentException("Invalid price");

            Price = newPrice;
            SetUpdated();
        }

        public void ReduceStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Invalid quantity");

            if (Stock < quantity)
                throw new InvalidOperationException("Insufficient stock");

            Stock -= quantity;
            SetUpdated();
        }
    }
}
