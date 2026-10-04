using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    public OrdersController(IOrderService service) => _service = service;

    [HttpGet]
    public async Task<IReadOnlyList<OrderDto>> GetAll() => await _service.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<OrderDto> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("{id:int}/confirm")]
    public async Task<OrderDto> Confirm(int id) => await _service.ConfirmAsync(id);

    [HttpPost("{id:int}/pack")]
    public async Task<OrderDto> Pack(int id) => await _service.PackAsync(id);

    [HttpPost("{id:int}/cancel")]
    public async Task<OrderDto> Cancel(int id) => await _service.CancelAsync(id);
}