using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseOrderSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }

        public Guid RoleId { get; private set; }
        public Role Role { get; private set; }

        private User() { }

        public User(string email, string passwordHash, Guid roleId)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Invalid password");

            PasswordHash = newPasswordHash;
            SetUpdated();
        }
    }
}
