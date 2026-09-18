using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rnd.Models;

namespace Rnd.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order) + "s");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property(o => o.OrderDate)
                .IsRequired()
                .HasColumnType("TEXT")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(o => o.Amount)
                .IsRequired()
                .HasColumnType("REAL");

            builder.Property(o => o.Description)
                .HasMaxLength(500);

            builder.Property(o => o.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(o => o.Person)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.PersonId)
                .HasDatabaseName("IX_Orders_PersonId");

            builder.HasQueryFilter(o => !o.IsDeleted);
        }
    }
}
