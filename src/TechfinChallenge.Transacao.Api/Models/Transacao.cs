using TechfinChallenge.Shared;

namespace TechfinChallenge.Transacao.Api.Models;

public class TransacaoModel
{
    public string Id { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;

    public string Tipo { get; set; } = string.Empty;

    public static Result<TransacaoModel> Criar(string clienteId, decimal valor, string tipo)
    {
        if (string.IsNullOrWhiteSpace(clienteId))
            return Result<TransacaoModel>.Failure("ClienteId é obrigatório.");

        if (valor <= 0)
            return Result<TransacaoModel>.Failure("Valor deve ser maior que zero.");

        if (tipo != "debito" && tipo != "credito")
            return Result<TransacaoModel>.Failure("Tipo deve ser 'debito' ou 'credito'.");

        return Result<TransacaoModel>.Success(new TransacaoModel
        {
            Id = Guid.NewGuid().ToString(),
            ClienteId = clienteId,
            Valor = valor,
            Status = "aprovado",
            Tipo = tipo
        });
    }
}
