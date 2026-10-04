using Microsoft.Extensions.DependencyInjection;
using OnlineStore.Application.Delivery;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Services;

namespace OnlineStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICourierService, CourierService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IRouteService, RouteService>();

        // Активная стратегия доставки: чтобы сменить, замени тип на FlatRateDeliveryCost
        services.AddSingleton<IDeliveryCostStrategy, FreeOverThresholdDeliveryCost>();
        return services;
    }
}