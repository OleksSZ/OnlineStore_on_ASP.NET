using OnlineStore.Application.DTOs;

namespace OnlineStore.Application.Interfaces;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync();
    Task<CustomerDto> GetAsync(int id);
    Task<CustomerDto> CreateAsync(SaveCustomerDto dto);
    Task<CustomerDto> UpdateAsync(int id, SaveCustomerDto dto);
    Task DeleteAsync(int id);
}

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync();
    Task<ProductDto> GetAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
    Task<ProductDto> RestockAsync(int id, RestockDto dto);
    Task DeleteAsync(int id);
}

public interface ICourierService
{
    Task<IReadOnlyList<CourierDto>> GetAllAsync();
    Task<IReadOnlyList<CourierDto>> GetAvailableAsync();
    Task<CourierDto> GetAsync(int id);
    Task<CourierDto> CreateAsync(SaveCourierDto dto);
    Task<CourierDto> UpdateAsync(int id, SaveCourierDto dto);
    Task DeleteAsync(int id);
}