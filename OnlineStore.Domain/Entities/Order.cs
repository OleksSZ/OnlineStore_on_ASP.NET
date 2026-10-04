using OnlineStore.Domain.Common;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderItem> _items = new();

    private static readonly Dictionary<OrderStatus, OrderStatus[]> Transitions = new()
    {
        [OrderStatus.New]        = [OrderStatus.Confirmed, OrderStatus.Cancelled],
        [OrderStatus.Confirmed]  = [OrderStatus.Packed, OrderStatus.Cancelled],
        [OrderStatus.Packed]     = [OrderStatus.InDelivery, OrderStatus.Cancelled],
        [OrderStatus.InDelivery] = [OrderStatus.Delivered],
        [OrderStatus.Delivered]  = [],
        [OrderStatus.Cancelled]  = []
    };

    public int CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    public int? CourierId { get; private set; }
    public Courier? Courier { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.New;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public IReadOnlyList<OrderItem> Items => _items;
    public decimal Total => _items.Sum(i => i.Total);

    private Order() { }

    public Order(Customer customer)
    {
        Customer = customer;
        CustomerId = customer.Id;
    }

    public void AddItem(Product product, int quantity)
    {
        if (Status != OrderStatus.New)
            throw new InvalidOperationException("Змінювати склад можна лише в новому замовленні");
        _items.Add(new OrderItem(product, quantity));
    }

    public void Confirm()
    {
        if (!_items.Any()) throw new InvalidOperationException("Замовлення порожнє");
        ChangeStatus(OrderStatus.Confirmed);
        foreach (var i in _items) i.Product.Reserve(i.Quantity);
    }

    public void MarkPacked() => ChangeStatus(OrderStatus.Packed);

    // викликається лише з Route.Start(); зайнятістю кур'єра керує маршрут
    internal void StartDelivery(Courier courier)
    {
        ChangeStatus(OrderStatus.InDelivery);
        Courier = courier;
        CourierId = courier.Id;
    }

    public void MarkDelivered() => ChangeStatus(OrderStatus.Delivered);

    public void Cancel()
    {
        var wasReserved = Status is OrderStatus.Confirmed or OrderStatus.Packed;
        ChangeStatus(OrderStatus.Cancelled);
        if (wasReserved)
            foreach (var i in _items) i.Product.Release(i.Quantity);
    }

    private void ChangeStatus(OrderStatus next)
    {
        if (!Transitions[Status].Contains(next))
            throw new InvalidOperationException($"Перехід {Status} → {next} неможливий");
        Status = next;
    }
}