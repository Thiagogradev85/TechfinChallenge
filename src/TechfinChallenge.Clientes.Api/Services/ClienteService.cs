using TechfinChallenge.Clientes.Api.DTOs;
using TechfinChallenge.Clientes.Api.Models;
using TechfinChallenge.Clientes.Api.Repositories;
using TechfinChallenge.Shared;

namespace TechfinChallenge.Clientes.Api.Services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _repository;

    public ClienteService(IClienteRepository repository)
    {
        _repository = repository;
    }

    public Result<Cliente> CadastrarCliente(ClienteDto dto)
    {
        if (_repository.BuscarPorCpf(dto.Cpf) != null)
            return Result<Cliente>.Failure("CPF já cadastrado.");

        var result = Cliente.Criar(dto.Nome, dto.Cpf, dto.ValorLimite);
        if (!result.IsSuccess)
            return result;

        _repository.Criar(result.Data!);
        return result;
    }

    public IEnumerable<Cliente> ListarClientes()
    {
        return _repository.ListarTodos();
    }

    public void AtualizarLimite(string id, decimal novoLimite)
    {
        _repository.AtualizarLimite(id, novoLimite);
    }

    public Cliente? BuscarPorId(string id)
    {
        return _repository.BuscarPorId(id);
    }
}
