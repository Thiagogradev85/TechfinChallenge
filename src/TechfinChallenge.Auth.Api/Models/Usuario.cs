namespace TechfinChallenge.Auth.Api.Models;

public class Usuario
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
}
