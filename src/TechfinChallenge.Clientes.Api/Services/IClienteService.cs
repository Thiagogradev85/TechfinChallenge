using TechfinChallenge.Clientes.Api.DTOs;
using TechfinChallenge.Clientes.Api.Models;
using TechfinChallenge.Shared;

namespace TechfinChallenge.Clientes.Api.Services;

public interface IClienteService
{
    Result<Cliente> CadastrarCliente(ClienteDto dto);
    Cliente? BuscarPorId(string id);
    IEnumerable<Cliente> ListarClientes();
    void AtualizarLimite(string id, decimal novoLimite);
}
