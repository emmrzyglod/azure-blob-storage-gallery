using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Web.Persistence.Entities
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Photo Photo { get; set; }
        public string PhotoId { get; set; }
        public Collection<User> Followers { get; set; }
        public DateTime CreatedAt { get; set; }
        
        private Product() {}

        public Product(string name, string description, decimal price, Photo photo)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (price < 0) throw new ArgumentException(nameof(price) + " have to bigger than 0");
            
            ProductId = Guid.NewGuid();
            Name = name;
            Description = description;
            Price = price;
            Photo = photo;
            CreatedAt = DateTime.UtcNow;
            Followers = new Collection<User>();
        }

        public void AddFollower(User user)
        {
            if (Followers.Any(m => m.UserId == user.UserId)) return;
            Followers.Add(user);
        }
    }
}