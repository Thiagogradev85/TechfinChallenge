namespace TechfinChallenge.Clientes.Api.Models;

public class Cliente
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal ValorLimite { get; set; }
}
