using OnlineStore.Domain.Common;

namespace OnlineStore.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; } = null!;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { }

    public Product(string name, decimal price, int stock)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Guard.Positive(price, nameof(price));
        if (stock < 0) throw new ArgumentException("Залишок не може бути від'ємним");
        Price = price;
        Stock = stock;
    }

    internal void Reserve(int qty)
    {
        if (qty > Stock) throw new InvalidOperationException($"Недостатньо товару '{Name}' на складі");
        Stock -= qty;
    }

    internal void Release(int qty) => Stock += qty;

    public void Restock(int qty)
    {
        if (qty <= 0) throw new ArgumentException("Кількість має бути більше 0");
        Stock += qty;
    }
    public void Update(string name, decimal price)
    {
        Name = Guard.NotEmpty(name, nameof(name));
        Guard.Positive(price, nameof(price));
        Price = price;
    }
}