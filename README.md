# TechfinChallenge

Case técnico para a vaga de Analista de Desenvolvimento de Software na Techfin.

## Sobre o projeto

Três APIs REST que simulam um fluxo de autorização de transações financeiras: autenticação com JWT, cadastro de clientes e processamento de transações com comunicação assíncrona via RabbitMQ.

## Pré-requisitos

- .NET 10
- RabbitMQ rodando localmente na porta 5672

Para subir o RabbitMQ com o serviço do Windows:
```
net start RabbitMQ
```

## Como rodar

Cada API roda em um terminal separado, a partir da raiz do projeto:

```
dotnet run --project src/TechfinChallenge.Auth.Api
```
```
dotnet run --project src/TechfinChallenge.Clientes.Api
```
```
dotnet run --project src/TechfinChallenge.Transacao.Api
```

As portas são definidas automaticamente. O Swagger de cada API fica em `/swagger`.

## Fluxo de uso

1. Criar um usuário em `POST /auth/register`
2. Fazer login em `POST /auth/login` e copiar o token
3. Colar o token no botão **Authorize** do Swagger
4. Cadastrar um cliente em `POST /clientes`
5. Autorizar uma transação em `POST /transacoes` informando o id do cliente e o valor

Quando uma transação é aprovada, a API de Transação publica na fila do RabbitMQ e a API de Clientes consome essa mensagem para debitar o valor do limite automaticamente.

## Rodando os testes

```
dotnet test tests/TechfinChallenge.Tests
```

## Estrutura

```
TechfinChallenge/
├── src/
│   ├── TechfinChallenge.Auth.Api/
│   ├── TechfinChallenge.Clientes.Api/
│   └── TechfinChallenge.Transacao.Api/
└── tests/
    └── TechfinChallenge.Tests/
```

## Decisões técnicas

**SQLite in-memory** — banco sobe junto com a API, sem necessidade de instalar nada externo. Usei uma conexão estática com `Cache=Shared` para manter os dados enquanto a aplicação está rodando.

**Dapper** - Pedido no teste

**JWT compartilhado** — as três APIs usam o mesmo secret, então o token gerado na Auth é válido nas outras duas sem nenhuma chamada adicional.

**RabbitMQ para mensageria**  - Pedido no teste

**Swashbuckle 6.9.0** — o .NET 10 tem suporte nativo a OpenAPI mas não gera interface visual. O Swashbuckle resolve isso e é o mais adotado no mercado.
