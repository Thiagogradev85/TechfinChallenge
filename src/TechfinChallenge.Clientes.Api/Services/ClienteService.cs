using TechfinChallenge.Clientes.Api.DTOs;
using TechfinChallenge.Clientes.Api.Models;
using TechfinChallenge.Clientes.Api.Repositories;

namespace TechfinChallenge.Clientes.Api.Services;

public class ClienteService
{
    private readonly ClienteRepository _repository;

    public ClienteService(ClienteRepository repository)
    {
        _repository = repository;
    }

    public (string? id, string? erro) CadastrarCliente(ClienteDto dto)
    {
        if (dto.ValorLimite < 0)
            return (null, "Valor de limite não pode ser negativo.");

        if (_repository.BuscarPorCpf(dto.Cpf) != null)
            return (null, "CPF já cadastrado.");

        var cliente = new Cliente
        {
            Id = Guid.NewGuid().ToString(),
            Nome = dto.Nome,
            Cpf = dto.Cpf,
            ValorLimite = dto.ValorLimite
        };

        _repository.Criar(cliente);
        return (cliente.Id, null);
    }

    public IEnumerable<Cliente> ListarClientes()
    {
        return _repository.ListarTodos();
    }

    public void AtualizarLimite(string id, decimal novoLimite)
    {
        _repository.AtualizarLimite(id, novoLimite);
    }
}
