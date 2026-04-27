using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TechfinChallenge.Clientes.Api.DTOs;
using TechfinChallenge.Clientes.Api.Services;

namespace TechfinChallenge.Clientes.Api.Controllers;

[ApiController]
[Route("clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _service;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "lista_clientes";

    public ClientesController(IClienteService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    [HttpPost]
    public IActionResult CadastrarCliente([FromBody] ClienteDto dto)
    {
        var result = _service.CadastrarCliente(dto);
        if (!result.IsSuccess)
            return BadRequest(new { status = "ERRO", detalheErro = result.Error });

        _cache.Remove(CacheKey);
        return Ok(new { idCliente = result.Data!.Id, status = "OK" });
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult BuscarCliente(string id)
    {
        var cliente = _service.BuscarPorId(id);
        if (cliente == null)
            return NotFound(new { status = "ERRO", detalheErro = "Cliente não encontrado." });

        return Ok(cliente);
    }

    [HttpGet]
    public IActionResult ListarClientes()
    {
        if (!_cache.TryGetValue(CacheKey, out var clientes))
        {
            clientes = _service.ListarClientes();
            _cache.Set(CacheKey, clientes, TimeSpan.FromMinutes(10));
        }

        return Ok(clientes);
    }

    [HttpPut("{id}")]
    public IActionResult AtualizarCliente(string id, [FromBody] ClienteDto dto)
    {
        var result = _service.AtualizarCliente(id, dto);
        if (!result.IsSuccess)
            return BadRequest(new { status = "ERRO", detalheErro = result.Error });

        _cache.Remove(CacheKey);
        return Ok(new { idCliente = result.Data!.Id, status = "OK" });
    }

    [HttpDelete("{id}")]
    public IActionResult DeletarCliente(string id)
    {
        var result = _service.DeletarCliente(id);
        if (!result.IsSuccess)
            return BadRequest(new { status = "ERRO", detalheErro = result.Error });

        _cache.Remove(CacheKey);
        return Ok(new { status = "OK" });
    }
}
