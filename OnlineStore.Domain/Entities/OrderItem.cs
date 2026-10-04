using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities;

public class OrderItem : Entity
{
    public int ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; } // ціна на момент замовлення

    private OrderItem() { }

    internal OrderItem(Product product, int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Кількість має бути більше 0");
        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        UnitPrice = product.Price;
    }

    public decimal Total => UnitPrice * Quantity;
}