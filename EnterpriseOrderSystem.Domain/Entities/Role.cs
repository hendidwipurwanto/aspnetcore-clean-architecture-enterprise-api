using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; private set; }

        private Role() { }

        public Role(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty");

            Name = name;
        }
    }
}
