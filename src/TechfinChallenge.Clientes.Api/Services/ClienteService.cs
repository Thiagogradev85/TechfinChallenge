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

    public Result<Cliente> AtualizarCliente(string id, ClienteDto dto)
    {
        var existente = _repository.BuscarPorId(id);
        if (existente == null)
            return Result<Cliente>.Failure("Cliente não encontrado.");

        var clienteComCpf = _repository.BuscarPorCpf(dto.Cpf);
        if (clienteComCpf != null && clienteComCpf.Id != id)
            return Result<Cliente>.Failure("CPF já cadastrado por outro cliente.");

        var result = Cliente.Criar(dto.Nome, dto.Cpf, dto.ValorLimite);
        if (!result.IsSuccess)
            return result;

        result.Data!.Id = id;
        _repository.Atualizar(id, result.Data!);
        return result;
    }

    public Result<Cliente> DeletarCliente(string id)
    {
        var existente = _repository.BuscarPorId(id);
        if (existente == null)
            return Result<Cliente>.Failure("Cliente não encontrado.");

        _repository.Deletar(id);
        return Result<Cliente>.Success(existente);
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
