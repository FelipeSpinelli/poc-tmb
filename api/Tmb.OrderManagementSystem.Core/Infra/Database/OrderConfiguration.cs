using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tmb.OrderManagementSystem.Core.Domain;

namespace Tmb.OrderManagementSystem.Core.Infra.Database;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("pedidos");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .HasMaxLength(36);

        builder.Property(o => o.Customer)
            .HasColumnName("cliente")
            .IsRequired()
            .HasMaxLength(Order.Constraints.CUSTOMER_MAX_LENGTH);

        builder.Property(o => o.Product)
            .HasColumnName("produto")
            .IsRequired()
            .HasMaxLength(Order.Constraints.PRODUCT_MAX_LENGTH);

        builder.Property(o => o.Price)
            .HasColumnName("valor")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Status)
            .HasColumnName("status")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.CreationDate)
            .HasColumnName("data_criacao")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Ignore(o => o.IsValid);
        builder.Ignore(o => o.Notifications);
    }
}