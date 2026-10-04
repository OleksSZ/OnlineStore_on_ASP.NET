using OnlineStore.Application.DTOs;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Interfaces;

namespace OnlineStore.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IRepository<Customer> _repo;
    private readonly IUnitOfWork _uow;

    public CustomerService(IRepository<Customer> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    private async Task<Customer> FindAsync(int id) =>
        await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Customer), id);

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(c => c.ToDto()).ToList();

    public async Task<CustomerDto> GetAsync(int id) => (await FindAsync(id)).ToDto();

    public async Task<CustomerDto> CreateAsync(SaveCustomerDto dto)
    {
        var customer = new Customer(dto.Name, dto.Phone, dto.Email, dto.Address);
        await _repo.AddAsync(customer);
        await _uow.SaveChangesAsync();
        return customer.ToDto();
    }

    public async Task<CustomerDto> UpdateAsync(int id, SaveCustomerDto dto)
    {
        var customer = await FindAsync(id);
        customer.Update(dto.Name, dto.Phone, dto.Email, dto.Address);
        await _uow.SaveChangesAsync();
        return customer.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        _repo.Remove(await FindAsync(id));
        await _uow.SaveChangesAsync();
    }
}

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repo;
    private readonly IUnitOfWork _uow;

    public ProductService(IRepository<Product> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    private async Task<Product> FindAsync(int id) =>
        await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Product), id);

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(p => p.ToDto()).ToList();

    public async Task<ProductDto> GetAsync(int id) => (await FindAsync(id)).ToDto();

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product(dto.Name, dto.Price, dto.Stock);
        await _repo.AddAsync(product);
        await _uow.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await FindAsync(id);
        product.Update(dto.Name, dto.Price);
        await _uow.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task<ProductDto> RestockAsync(int id, RestockDto dto)
    {
        var product = await FindAsync(id);
        product.Restock(dto.Quantity);
        await _uow.SaveChangesAsync();
        return product.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        _repo.Remove(await FindAsync(id));
        await _uow.SaveChangesAsync();
    }
}

public class CourierService : ICourierService
{
    private readonly ICourierRepository _repo;
    private readonly IUnitOfWork _uow;

    public CourierService(ICourierRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    private async Task<Courier> FindAsync(int id) =>
        await _repo.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Courier), id);

    public async Task<IReadOnlyList<CourierDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(c => c.ToDto()).ToList();

    public async Task<IReadOnlyList<CourierDto>> GetAvailableAsync() =>
        (await _repo.GetAvailableAsync()).Select(c => c.ToDto()).ToList();

    public async Task<CourierDto> GetAsync(int id) => (await FindAsync(id)).ToDto();

    public async Task<CourierDto> CreateAsync(SaveCourierDto dto)
    {
        var courier = new Courier(dto.Name, dto.Phone);
        await _repo.AddAsync(courier);
        await _uow.SaveChangesAsync();
        return courier.ToDto();
    }

    public async Task<CourierDto> UpdateAsync(int id, SaveCourierDto dto)
    {
        var courier = await FindAsync(id);
        courier.Update(dto.Name, dto.Phone);
        await _uow.SaveChangesAsync();
        return courier.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        _repo.Remove(await FindAsync(id));
        await _uow.SaveChangesAsync();
    }
}