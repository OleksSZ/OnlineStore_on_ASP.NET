using OnlineStore.Domain.Common;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Domain.Entities;

public class Route : Entity
{
    private readonly List<Order> _orders = new();

    public int CourierId { get; private set; }
    public Courier Courier { get; private set; } = null!;
    public RouteStatus Status { get; private set; } = RouteStatus.Planned;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public IReadOnlyList<Order> Orders => _orders;

    private Route() { }

    public Route(Courier courier)
    {
        Courier = courier;
        CourierId = courier.Id;
    }

    public void AddOrder(Order order)
    {
        if (Status != RouteStatus.Planned)
            throw new InvalidOperationException("Маршрут уже розпочато");
        if (order.Status != OrderStatus.Packed)
            throw new InvalidOperationException("У маршрут можна додати лише зібране замовлення");
        _orders.Add(order);
    }

    public void Start()
    {
        if (Status != RouteStatus.Planned)
            throw new InvalidOperationException("Маршрут уже розпочато");
        if (!_orders.Any())
            throw new InvalidOperationException("Маршрут порожній");

        Courier.MarkBusy(); // один раз на весь маршрут
        foreach (var o in _orders) o.StartDelivery(Courier);
        Status = RouteStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != RouteStatus.InProgress)
            throw new InvalidOperationException("Маршрут не розпочато");

        foreach (var o in _orders.Where(o => o.Status == OrderStatus.InDelivery))
            o.MarkDelivered();
        Courier.MarkFree();
        Status = RouteStatus.Completed;
    }
}