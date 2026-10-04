using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<IReadOnlyList<ProductDto>> GetAll() => await _service.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<ProductDto> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ProductDto> Update(int id, UpdateProductDto dto) => await _service.UpdateAsync(id, dto);

    [HttpPatch("{id:int}/restock")]
    public async Task<ProductDto> Restock(int id, RestockDto dto) => await _service.RestockAsync(id, dto);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}