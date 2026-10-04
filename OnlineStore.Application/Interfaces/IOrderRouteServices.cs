using OnlineStore.Application.DTOs;

namespace OnlineStore.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAllAsync();
    Task<OrderDto> GetAsync(int id);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto> ConfirmAsync(int id);
    Task<OrderDto> PackAsync(int id);
    Task<OrderDto> CancelAsync(int id);
}

public interface IRouteService
{
    Task<IReadOnlyList<RouteDto>> GetAllAsync();
    Task<RouteDto> GetAsync(int id);
    Task<RouteDto> CreateAsync(CreateRouteDto dto);
    Task<RouteDto> AddOrderAsync(int routeId, AddOrderToRouteDto dto);
    Task<RouteDto> StartAsync(int id);
    Task<RouteDto> CompleteAsync(int id);
}