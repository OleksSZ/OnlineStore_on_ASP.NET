using OnlineStore.Application.Delivery;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IRepository<Customer> _customers;
    private readonly IRepository<Product> _products;
    private readonly IDeliveryCostStrategy _delivery;
    private readonly IUnitOfWork _uow;

    public OrderService(
        IOrderRepository orders,
        IRepository<Customer> customers,
        IRepository<Product> products,
        IDeliveryCostStrategy delivery,
        IUnitOfWork uow)
    {
        _orders = orders;
        _customers = customers;
        _products = products;
        _delivery = delivery;
        _uow = uow;
    }

    private async Task<Order> FindAsync(int id) =>
        await _orders.GetWithDetailsAsync(id) ?? throw new NotFoundException(nameof(Order), id);

    private OrderDto Map(Order o) => o.ToDto(_delivery.Calculate(o));

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync() =>
        (await _orders.GetAllAsync()).Select(Map).ToList();

    public async Task<OrderDto> GetAsync(int id) => Map(await FindAsync(id));

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {
        if (dto.Items is null || dto.Items.Count == 0)
            throw new ArgumentException("Замовлення має містити хоча б одну позицію");

        var customer = await _customers.GetByIdAsync(dto.CustomerId)
            ?? throw new NotFoundException(nameof(Customer), dto.CustomerId);

        var order = new Order(customer);
        foreach (var item in dto.Items)
        {
            var product = await _products.GetByIdAsync(item.ProductId)
                ?? throw new NotFoundException(nameof(Product), item.ProductId);
            order.AddItem(product, item.Quantity);
        }

        await _orders.AddAsync(order);
        await _uow.SaveChangesAsync();
        return Map(order);
    }

    // Загальна схема: знайти -> виконати дію доменної моделі -> зберегти
    private async Task<OrderDto> ChangeAsync(int id, Action<Order> action)
    {
        var order = await FindAsync(id);
        action(order);
        await _uow.SaveChangesAsync();
        return Map(order);
    }

    public Task<OrderDto> ConfirmAsync(int id) => ChangeAsync(id, o => o.Confirm());
    public Task<OrderDto> PackAsync(int id) => ChangeAsync(id, o => o.MarkPacked());
    public Task<OrderDto> CancelAsync(int id) => ChangeAsync(id, o => o.Cancel());
}