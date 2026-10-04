using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service) => _service = service;

    [HttpGet]
    public async Task<IReadOnlyList<CustomerDto>> GetAll() => await _service.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<CustomerDto> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> Create(SaveCustomerDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<CustomerDto> Update(int id, SaveCustomerDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}