using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Persistence.Entities;

namespace Web.Persistence.Configuration
{
    public class PhotoConfiguration: IEntityTypeConfiguration<Photo>
    {
        public void Configure(EntityTypeBuilder<Photo> builder)
        {
            builder.HasKey(s => s.PhotoId);
            
            builder.Property(p => p.CreatedAt).IsRequired();
        }
    }
}