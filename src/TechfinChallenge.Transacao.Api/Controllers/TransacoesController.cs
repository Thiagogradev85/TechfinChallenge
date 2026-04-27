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
    private readonly ITransacaoService _service;

    public TransacoesController(ITransacaoService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Autorizar([FromBody] TransacaoDto dto)
    {
        var result = await _service.AutorizarAsync(dto);

        if (!result.IsSuccess)
            return Ok(new { status = "NEGADO" });

        return Ok(new { status = "APROVADO", idTransacao = result.Data!.Id });
    }
}
