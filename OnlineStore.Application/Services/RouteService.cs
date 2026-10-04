using OnlineStore.Application.DTOs;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routes;
    private readonly IOrderRepository _orders;
    private readonly ICourierRepository _couriers;
    private readonly IUnitOfWork _uow;

    public RouteService(
        IRouteRepository routes,
        IOrderRepository orders,
        ICourierRepository couriers,
        IUnitOfWork uow)
    {
        _routes = routes;
        _orders = orders;
        _couriers = couriers;
        _uow = uow;
    }

    private async Task<Route> FindAsync(int id) =>
        await _routes.GetWithDetailsAsync(id) ?? throw new NotFoundException(nameof(Route), id);

    public async Task<IReadOnlyList<RouteDto>> GetAllAsync() =>
        (await _routes.GetAllAsync()).Select(r => r.ToDto()).ToList();

    public async Task<RouteDto> GetAsync(int id) => (await FindAsync(id)).ToDto();

    public async Task<RouteDto> CreateAsync(CreateRouteDto dto)
    {
        var courier = await _couriers.GetByIdAsync(dto.CourierId)
            ?? throw new NotFoundException(nameof(Courier), dto.CourierId);

        var route = new Route(courier);
        await _routes.AddAsync(route);
        await _uow.SaveChangesAsync();
        return route.ToDto();
    }

    public async Task<RouteDto> AddOrderAsync(int routeId, AddOrderToRouteDto dto)
    {
        var route = await FindAsync(routeId);
        var order = await _orders.GetByIdAsync(dto.OrderId)
            ?? throw new NotFoundException(nameof(Order), dto.OrderId);

        if (await _routes.IsOrderInRouteAsync(order.Id))
            throw new InvalidOperationException("Замовлення вже додано до маршруту");

        route.AddOrder(order);
        await _uow.SaveChangesAsync();
        return route.ToDto();
    }

    public async Task<RouteDto> StartAsync(int id)
    {
        var route = await FindAsync(id);
        route.Start();
        await _uow.SaveChangesAsync();
        return route.ToDto();
    }

    public async Task<RouteDto> CompleteAsync(int id)
    {
        var route = await FindAsync(id);
        route.Complete();
        await _uow.SaveChangesAsync();
        return route.ToDto();
    }
}