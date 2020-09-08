using System;

namespace Web.Persistence.Entities
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string Path { get; set; }
        public DateTime CreatedAt { get; set; }
        
        private Photo() {}

        public Photo(string path)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            
            Path = path;
            CreatedAt = DateTime.UtcNow;
        }
    }
}