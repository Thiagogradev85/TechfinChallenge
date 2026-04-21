# TechfinChallenge

Case técnico para a vaga de Analista de Desenvolvimento de Software na Techfin.

## Pré-requisitos

.NET 10
RabbitMQ rodando localmente na porta 5672

## Como rodar

Abrir um terminal para cada API, a partir da raiz do projeto:

dotnet run --project src/TechfinChallenge.Auth.Api
dotnet run --project src/TechfinChallenge.Clientes.Api
dotnet run --project src/TechfinChallenge.Transacao.Api


## Testes

dotnet test tests/TechfinChallenge.Tests

## Fluxo básico

Criar usuário em `POST /auth/register`
Fazer login em `POST /auth/login` e copiar o token
Usar o token no Authorize do Swagger
Cadastrar cliente em `POST /clientes`
Autorizar transação em `POST /transacoes`
Verificar débito do limite em `GET /clientes/{id}`

## Estrutura

TechfinChallenge/
├── src/
│   ├── TechfinChallenge.Auth.Api/
│   ├── TechfinChallenge.Clientes.Api/
│   └── TechfinChallenge.Transacao.Api/
└── tests/
    └── TechfinChallenge.Tests/

