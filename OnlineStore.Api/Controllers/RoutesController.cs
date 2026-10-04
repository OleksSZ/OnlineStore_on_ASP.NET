using Microsoft.AspNetCore.Mvc;
using OnlineStore.Application.DTOs;
using OnlineStore.Application.Interfaces;

namespace OnlineStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _service;
    public RoutesController(IRouteService service) => _service = service;

    [HttpGet]
    public async Task<IReadOnlyList<RouteDto>> GetAll() => await _service.GetAllAsync();

    [HttpGet("{id:int}")]
    public async Task<RouteDto> Get(int id) => await _service.GetAsync(id);

    [HttpPost]
    public async Task<ActionResult<RouteDto>> Create(CreateRouteDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPost("{id:int}/orders")]
    public async Task<RouteDto> AddOrder(int id, AddOrderToRouteDto dto) => await _service.AddOrderAsync(id, dto);

    [HttpPost("{id:int}/start")]
    public async Task<RouteDto> Start(int id) => await _service.StartAsync(id);

    [HttpPost("{id:int}/complete")]
    public async Task<RouteDto> Complete(int id) => await _service.CompleteAsync(id);
}