using TechfinChallenge.Shared;

namespace TechfinChallenge.Clientes.Api.Models;

public class Cliente
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public decimal ValorLimite { get; set; }

    public static Result<Cliente> Criar(string nome, string cpf, decimal valorLimite)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result<Cliente>.Failure("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(cpf))
            return Result<Cliente>.Failure("CPF é obrigatório.");

        if (cpf.Length != 11)
            return Result<Cliente>.Failure("CPF deve ter 11 dígitos.");

        if (valorLimite < 0)
            return Result<Cliente>.Failure("Valor de limite não pode ser negativo.");

        return Result<Cliente>.Success(new Cliente
        {
            Id = Guid.NewGuid().ToString(),
            Nome = nome,
            Cpf = cpf,
            ValorLimite = valorLimite
        });
    }
}
