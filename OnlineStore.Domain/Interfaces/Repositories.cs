using OnlineStore.Domain.Common;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Domain.Interfaces;

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Remove(T entity);
}

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithDetailsAsync(int id); // з клієнтом, товарами, кур'єром
}

public interface ICourierRepository : IRepository<Courier>
{
    Task<IReadOnlyList<Courier>> GetAvailableAsync();
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}
public interface IRouteRepository : IRepository<Route>
{
    Task<Route?> GetWithDetailsAsync(int id);
    Task<bool> IsOrderInRouteAsync(int orderId);
}