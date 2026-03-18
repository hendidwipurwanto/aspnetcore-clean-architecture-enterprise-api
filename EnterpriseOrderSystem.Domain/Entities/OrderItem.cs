using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Domain.Entities
{
    public class OrderItem
    {
        public Guid ProductId { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        private OrderItem() { }

        public OrderItem(Guid productId, decimal price, int quantity)
        {
            ProductId = productId;
            Price = price;
            Quantity = quantity;
        }
    }
}
