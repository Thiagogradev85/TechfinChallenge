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
    private readonly ClienteService _service;
    private readonly IMemoryCache _cache;

    public ClientesController(ClienteService service, IMemoryCache cache)
    {
        _service = service;
        _cache = cache;
    }

    [HttpPost]
    public IActionResult CadastrarCliente([FromBody] ClienteDto dto)
    {
        var (id, erro) = _service.CadastrarCliente(dto);

        if (erro != null)
            return BadRequest(new { status = "ERRO", detalheErro = erro });

        return Ok(new { idCliente = id, status = "OK" });
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
        var cacheKey = "lista_clientes";

        if (!_cache.TryGetValue(cacheKey, out var clientes))
        {
            clientes = _service.ListarClientes();
            _cache.Set(cacheKey, clientes, TimeSpan.FromMinutes(10));
        }

        return Ok(clientes);
    }
}
