using TechfinChallenge.Clientes.Api.Models;

namespace TechfinChallenge.Clientes.Api.Repositories;

public interface IClienteRepository
{
    Cliente? BuscarPorCpf(string cpf);
    Cliente? BuscarPorId(string id);
    void Criar(Cliente cliente);
    IEnumerable<Cliente> ListarTodos();
    void AtualizarLimite(string id, decimal novoLimite);
}
