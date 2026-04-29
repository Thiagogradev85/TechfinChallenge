# TechfinChallenge

Case técnico para a vaga de Analista de Desenvolvimento de Software na Techfin.

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (para o frontend)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (para PostgreSQL, Kafka e RabbitMQ)

## Como subir tudo com um comando

**Pré-requisito:** Docker Desktop precisa estar aberto (ícone na bandeja do sistema).

Na raiz do projeto:

```bash
npm install        # apenas na primeira vez
npm run dev        # verifica Docker → build → 3 APIs + frontend
```

O comando `npm run dev` automaticamente:
1. Verifica se o Docker Desktop está rodando (para se não estiver)
2. Sobe os containers se ainda não estiverem no ar (pula se já estiverem)
3. Compila o projeto .NET
4. Sobe as 3 APIs em paralelo
5. Sobe o frontend React

Acesse o sistema em: **http://localhost:5173**

> Precisa subir só os containers sem o restante? Use `docker-compose up -d` na raiz.

## Acessos

| Serviço        | URL                        | Usuário / Senha                    |
|----------------|----------------------------|------------------------------------|
| Frontend       | http://localhost:5173      | Crie via tela de cadastro          |
| Auth API       | http://localhost:5141      | —                                  |
| Clientes API   | http://localhost:5174      | —                                  |
| Transacao API  | http://localhost:5081      | —                                  |
| Kafka UI       | http://localhost:8080      | —                                  |
| pgAdmin        | http://localhost:5050      | admin@techfin.com / admin123       |

## Docker — gerenciamento manual

Caso queira controlar os containers separadamente:

```bash
# Subir todos os containers
docker-compose up -d

# Subir apenas o banco de dados
docker-compose up -d postgres pgadmin

# Ver status dos containers
docker ps

# Parar tudo
docker-compose down

# Parar e apagar os dados do banco (reset total)
docker-compose down -v
```

## Como conectar ao banco pelo pgAdmin

1. Acesse http://localhost:5050
2. Login: `admin@techfin.com` / Senha: `admin123`
3. Clique em **Add New Server**
4. Aba **General** → Name: `TechfinLocal`
5. Aba **Connection**:
   - Host: `postgres` (nome do container, não `localhost`)
   - Port: `5432`
   - Database: `techfin`
   - Username: `techfin`
   - Password: `techfin123`
6. Clique em **Save**

## Brokers de mensageria

O sistema suporta **RabbitMQ** e **Kafka**. Para trocar o broker, use a tela **Configurações** no frontend — não é necessário editar arquivos.

> Após trocar o broker é necessário reiniciar as APIs (`Ctrl+C` e `npm run dev` novamente).

## Testes

```bash
dotnet test tests/TechfinChallenge.Tests
```

## Fluxo básico pelo frontend

1. Crie um usuário na tela de **Login** (aba "Criar conta")
2. Faça login
3. Acesse **Clientes** → cadastre um cliente com limite
4. Acesse **Transações** → autorize uma transação
5. Verifique que o limite do cliente foi debitado

## Estrutura do projeto

```
TechfinChallenge/
├── docker-compose.yml
├── package.json                          # npm run dev — inicia tudo
├── frontend/                             # React 18 + Vite + Tailwind CSS
└── src/
    ├── TechfinChallenge.Auth.Api/
    ├── TechfinChallenge.Clientes.Api/
    ├── TechfinChallenge.Transacao.Api/
    ├── TechfinChallenge.Messaging.Abstractions/
    ├── TechfinChallenge.Messaging.RabbitMQ/
    ├── TechfinChallenge.Messaging.Kafka/
    └── TechfinChallenge.Shared/
```
