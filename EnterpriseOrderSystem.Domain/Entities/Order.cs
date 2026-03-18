using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items;

        public decimal TotalAmount { get; private set; }

        private Order() { }

        public Order(Guid userId)
        {
            UserId = userId;
        }

        public void AddItem(Product product, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Invalid quantity");

            product.ReduceStock(quantity);

            var item = new OrderItem(product.Id, product.Price, quantity);

            _items.Add(item);

            RecalculateTotal();
        }

        private void RecalculateTotal()
        {
            TotalAmount = _items.Sum(x => x.Price * x.Quantity);
        }
    }
}
