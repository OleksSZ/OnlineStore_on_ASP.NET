using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Persistence.Configurations;

public class CustomerConfig : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("customers");
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(50).IsRequired();
        b.Property(x => x.Email).HasMaxLength(200).IsRequired();
        b.Property(x => x.Address).HasMaxLength(500).IsRequired();
    }
}

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("products");
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Price).HasPrecision(18, 2);
    }
}

public class CourierConfig : IEntityTypeConfiguration<Courier>
{
    public void Configure(EntityTypeBuilder<Courier> b)
    {
        b.ToTable("couriers");
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(50).IsRequired();
    }
}

public class OrderConfig : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("orders");
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.Ignore(x => x.Total); // обчислюване поле, у БД не зберігаємо

        b.HasOne(x => x.Customer).WithMany()
            .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Courier).WithMany()
            .HasForeignKey(x => x.CourierId).OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.Items).WithOne()
            .HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade);
        b.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field); // поле _items
    }
}

public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("order_items");
        b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        b.Ignore(x => x.Total);
        b.HasOne(x => x.Product).WithMany()
            .HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class RouteConfig : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> b)
    {
        b.ToTable("routes");
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasOne(x => x.Courier).WithMany()
            .HasForeignKey(x => x.CourierId).OnDelete(DeleteBehavior.Restrict);

        // RouteId у Order — "тіньова" колонка, у доменній моделі її немає
        b.HasMany(x => x.Orders).WithOne()
            .HasForeignKey("RouteId").IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        b.Navigation(x => x.Orders).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}