using Microsoft.AspNetCore.Mvc;
using TechfinChallenge.Auth.Api.DTOs;
using TechfinChallenge.Auth.Api.Services;

namespace TechfinChallenge.Auth.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] UsuarioDto dto)
    {
        var id = _authService.Registrar(dto);

        if (id == null)
            return BadRequest(new { status = "ERRO", detalheErro = "Email já cadastrado." });

        return Ok(new { id, status = "OK" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UsuarioDto dto)
    {
        var token = _authService.Login(dto);

        if (token == null)
            return Unauthorized(new { status = "ERRO", detalheErro = "Email ou senha inválidos." });

        return Ok(new { token });
    }
}
