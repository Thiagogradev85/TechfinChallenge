using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TechfinChallenge.Auth.Api.DTOs;
using TechfinChallenge.Auth.Api.Models;
using TechfinChallenge.Auth.Api.Repositories;

namespace TechfinChallenge.Auth.Api.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly string _jwtSecret;

    public AuthService(IUsuarioRepository repository, string jwtSecret)
    {
        _repository = repository;
        _jwtSecret = jwtSecret;
    }


    public string? Registrar(UsuarioDto dto)
    {
        if (_repository.BuscarPorEmail(dto.Email) != null)
            return null;

        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            Email = dto.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
        };

        _repository.Criar(usuario);
        return usuario.Id.ToString();
    }

    public string? Login(UsuarioDto dto)
    {
        var usuario = _repository.BuscarPorEmail(dto.Email);

        if (usuario == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            return null;

        return GerarToken(usuario);
    }

    private string GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: new[] { new Claim(ClaimTypes.Email, usuario.Email) },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
