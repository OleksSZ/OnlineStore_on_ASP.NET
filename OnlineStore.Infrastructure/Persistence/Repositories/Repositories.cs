using Microsoft.EntityFrameworkCore;
using OnlineStore.Domain.Common;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly AppDbContext Db;
    public Repository(AppDbContext db) => Db = db;

    public virtual async Task<T?> GetByIdAsync(int id) => await Db.Set<T>().FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync() =>
        await Db.Set<T>().AsNoTracking().ToListAsync();

    public async Task AddAsync(T entity) => await Db.Set<T>().AddAsync(entity);
    public void Remove(T entity) => Db.Set<T>().Remove(entity);
}

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext db) : base(db) { }

    private IQueryable<Order> WithDetails() => Db.Orders
        .Include(o => o.Customer)
        .Include(o => o.Courier)
        .Include(o => o.Items).ThenInclude(i => i.Product);

    public Task<Order?> GetWithDetailsAsync(int id) =>
        WithDetails().FirstOrDefaultAsync(o => o.Id == id);

    public override async Task<IReadOnlyList<Order>> GetAllAsync() =>
        await WithDetails().AsNoTracking().ToListAsync();
}

public class CourierRepository : Repository<Courier>, ICourierRepository
{
    public CourierRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Courier>> GetAvailableAsync() =>
        await Db.Couriers.AsNoTracking().Where(c => c.IsAvailable).ToListAsync();
}

public class RouteRepository : Repository<Route>, IRouteRepository
{
    public RouteRepository(AppDbContext db) : base(db) { }

    public Task<Route?> GetWithDetailsAsync(int id) => Db.Routes
        .Include(r => r.Courier)
        .Include(r => r.Orders).ThenInclude(o => o.Items).ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(r => r.Id == id);

    // базовый GetAllAsync не подтягивает кур'єра и заказы, поэтому переопределяем
    public override async Task<IReadOnlyList<Route>> GetAllAsync() =>
        await Db.Routes.Include(r => r.Courier).Include(r => r.Orders)
            .AsNoTracking().ToListAsync();

    public Task<bool> IsOrderInRouteAsync(int orderId) =>
        Db.Routes.AnyAsync(r => r.Orders.Any(o => o.Id == orderId));
}