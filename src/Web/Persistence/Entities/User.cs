using System;
using System.Collections.ObjectModel;

namespace Web.Persistence.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public Collection<Product> Follow { get; set; }
        public DateTime CreatedAt { get; set; }
        
        private User() {}

        public User(string email)
        {
            if (String.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            
            UserId = Guid.NewGuid();
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
    }
}