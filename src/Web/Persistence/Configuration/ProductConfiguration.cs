using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Persistence.Entities;

namespace Web.Persistence.Configuration
{
    public class ProductConfiguration: IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(s => s.ProductId);
            
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(16,8)");
            builder.Property(p => p.Price).IsRequired();
        }
    }
}