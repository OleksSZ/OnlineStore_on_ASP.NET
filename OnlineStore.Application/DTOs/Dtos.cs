namespace OnlineStore.Application.DTOs;

public record CustomerDto(int Id, string Name, string Phone, string Email, string Address);
public record SaveCustomerDto(string Name, string Phone, string Email, string Address);

public record ProductDto(int Id, string Name, decimal Price, int Stock);
public record CreateProductDto(string Name, decimal Price, int Stock);
public record UpdateProductDto(string Name, decimal Price);
public record RestockDto(int Quantity);

public record CourierDto(int Id, string Name, string Phone, bool IsAvailable);
public record SaveCourierDto(string Name, string Phone);