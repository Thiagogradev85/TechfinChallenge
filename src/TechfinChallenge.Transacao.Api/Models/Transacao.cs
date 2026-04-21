namespace TechfinChallenge.Transacao.Api.Models;

public class TransacaoModel
{
    public string Id { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
}
