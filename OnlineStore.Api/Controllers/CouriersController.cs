using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CouriersController : ControllerBase
{
    private readonly ICourierService _service;
    public CouriersController(ICourierService service) => _service = service;

    [HttpGet]
    public async Task<IReadOnlyList<CourierDto>> GetAll() => await _service.GetAllAsync();

    [HttpGet("available")]
    public async Task<IReadOnlyList<CourierDto>> GetAvailable() => await _service.GetAvailableAsync();

    [HttpGet("{id:int}")]
    public async Task<CourierDto> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<CourierDto>> Create(SaveCourierDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<CourierDto> Update(int id, SaveCourierDto dto) => await _service.UpdateAsync(id, dto);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}