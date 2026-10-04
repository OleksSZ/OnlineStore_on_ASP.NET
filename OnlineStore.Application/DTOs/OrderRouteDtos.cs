namespace OnlineStore.Application.DTOs;

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Total);

public record OrderDto(
    int Id, int CustomerId, string CustomerName,
    int? CourierId, string? CourierName,
    string Status, DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items,
    decimal Total, decimal DeliveryCost);

public record CreateOrderItemDto(int ProductId, int Quantity);
public record CreateOrderDto(int CustomerId, List<CreateOrderItemDto> Items);

public record RouteDto(
    int Id, int CourierId, string CourierName,
    string Status, DateTime CreatedAt,
    IReadOnlyList<int> OrderIds);

public record CreateRouteDto(int CourierId);
public record AddOrderToRouteDto(int OrderId);