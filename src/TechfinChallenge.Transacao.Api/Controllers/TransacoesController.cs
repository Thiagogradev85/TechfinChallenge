using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechfinChallenge.Transacao.Api.DTOs;
using TechfinChallenge.Transacao.Api.Services;

namespace TechfinChallenge.Transacao.Api.Controllers;

[ApiController]
[Route("transacoes")]
[Authorize]
public class TransacoesController : ControllerBase
{
    private readonly TransacaoService _service;

    public TransacoesController(TransacaoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Autorizar([FromBody] TransacaoDto dto)
    {
        var (id, erro) = await _service.AutorizarAsync(dto);

        if (erro != null)
            return Ok(new { status = "NEGADO" });

        return Ok(new { status = "APROVADO", idTransacao = id });
    }
}
