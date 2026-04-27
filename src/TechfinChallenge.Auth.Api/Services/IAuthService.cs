using TechfinChallenge.Auth.Api.DTOs;

namespace TechfinChallenge.Auth.Api.Services;

public interface IAuthService
{
    string? Registrar(UsuarioDto dto);
    string? Login(UsuarioDto dto);
}
