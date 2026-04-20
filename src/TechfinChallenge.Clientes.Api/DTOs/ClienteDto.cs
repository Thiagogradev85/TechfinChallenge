namespace TechfinChallenge.Clientes.Api.DTOs;

public class ClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal ValorLimite { get; set; }
}
