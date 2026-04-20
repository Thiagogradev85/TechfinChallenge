# TechfinChallenge

Solução do case técnico para a vaga de Analista de Desenvolvimento de Software na Techfin.

## O que foi pedido

Criar três APIs REST:
- **Auth API** – cadastro de usuário e login com JWT
- **Clientes API** – cadastro e listagem de clientes com cache
- **Transação API** – simulação de autorização de transações

## Como rodar

> Pré-requisito: .NET 10 instalado

```bash
# Clonar o repositório
git clone https://github.com/Thiagogradev85/TechfinChallenge.git
cd TechfinChallenge

# Rodar a API de Auth
cd src/TechfinChallenge.Auth.Api
dotnet run
```

Acesse `https://localhost:{porta}/swagger` para testar os endpoints.

## Stack utilizada

**.NET 10 com ASP.NET Core**
A stack principal que a Techfin usa. Optei por Controllers em vez de Minimal APIs porque é o padrão que a maioria das empresas ainda adota, e o código fica mais organizado e fácil de escalar.

**SQLite in-memory**
O enunciado sugeria banco em memória para facilitar a execução em qualquer ambiente sem precisar instalar nada. O SQLite atende bem esse caso — sobe junto com a API e não precisa de configuração externa.

**Dapper**
Requisito do teste. É um micro-ORM leve que permite escrever as queries SQL diretamente, sem a "mágica" do Entity Framework. Prefiro assim porque fica mais claro o que está sendo executado no banco.

**BCrypt**
Senhas nunca são salvas em texto puro. O BCrypt transforma a senha em um hash irreversível antes de salvar, e na hora do login compara os hashes. É o padrão para isso em .NET.

**JWT (JSON Web Token)**
Após o login, o usuário recebe um token JWT com validade de 1 hora. Esse token é usado nas requisições para as APIs de Clientes e Transação, garantindo que só usuários autenticados consigam acessar os recursos.

**RabbitMQ**
Quando uma transação é aprovada, a API de Transação publica um evento no RabbitMQ e a API de Clientes consome esse evento para atualizar o limite. Isso desacopla as duas APIs — uma não chama a outra diretamente.

**Swagger (Swashbuckle)**
Interface visual para testar os endpoints sem precisar do Postman. O .NET 10 tem uma implementação nativa de OpenAPI, mas ela não gera UI — por isso optei pelo Swashbuckle, que é o mais usado no mercado.

## Estrutura do projeto

```
TechfinChallenge/
├── src/
│   ├── TechfinChallenge.Auth.Api/       # API de autenticação
│   ├── TechfinChallenge.Clientes.Api/   # API de clientes
│   ├── TechfinChallenge.Transacao.Api/  # API de transações
│   └── TechfinChallenge.Shared/         # Configurações compartilhadas (JWT)
└── tests/
    └── TechfinChallenge.Tests/          # Testes unitários com xUnit e Moq
```
