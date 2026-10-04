using OnlineStore.Application.DTOs;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Application;

internal static class Mappings
{
    public static CustomerDto ToDto(this Customer c) => new(c.Id, c.Name, c.Phone, c.Email, c.Address);
    public static ProductDto ToDto(this Product p) => new(p.Id, p.Name, p.Price, p.Stock);
    public static CourierDto ToDto(this Courier c) => new(c.Id, c.Name, c.Phone, c.IsAvailable);

    public static OrderDto ToDto(this Order o, decimal deliveryCost) => new(
        o.Id, o.CustomerId, o.Customer.Name,
        o.CourierId, o.Courier?.Name,
        o.Status.ToString(), o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice, i.Total)).ToList(),
        o.Total, deliveryCost);

    public static RouteDto ToDto(this Route r) => new(
        r.Id, r.CourierId, r.Courier.Name,
        r.Status.ToString(), r.CreatedAt,
        r.Orders.Select(o => o.Id).ToList());
}