# TaskManager — desafio Backend Pleno .NET

Repositório inicial para um exercício técnico de **até 60 minutos**. A API e o ambiente Docker já estão preparados para que o candidato se concentre nas regras de negócio de assinatura.

## Objetivo
Implementar as regras de limite de projetos e tarefas de acordo com o plano de assinatura da empresa.

## Executar
Pré-requisitos: Docker Desktop/Docker Engine com Docker Compose.

Na raiz do repositório:

```bash
docker compose up --build
```

- API: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- Health check: http://localhost:8080/health
- PostgreSQL: `localhost:5432` (database `taskmanager`, usuário `postgres`, senha `postgres`; apenas para desenvolvimento local)

Parar os containers:

```bash
docker compose down
```

Remover também os dados persistidos:

```bash
docker compose down -v
```

O banco é inicializado automaticamente com `EnsureCreated` e dados demonstrativos para facilitar o exercício. Não são necessárias migrações para iniciar.

## Dados de demonstração

Os IDs abaixo são estáveis no banco recém-criado:

| Empresa | Plano | Company ID |
|---|---|---|
| Acme Freemium | Freemium | `11111111-1111-1111-1111-111111111111` |
| Acme Basic | Básico | `22222222-2222-2222-2222-222222222222` |
| Acme Advanced | Avançado | `33333333-3333-3333-3333-333333333333` |

Uma vez criado, o banco mantém os dados no volume Docker. Para restaurar os exemplos, use `docker compose down -v` e execute novamente.

## Endpoints disponíveis

- `GET /api/companies` — lista empresas e planos.
- `GET /api/companies/{companyId}/projects` — lista projetos da empresa.
- `POST /api/companies/{companyId}/projects` — cria projeto (**regra de plano a implementar**).
- `GET /api/projects/{projectId}/tasks` — lista tarefas do projeto.
- `POST /api/projects/{projectId}/tasks` — cria tarefa (**regra de plano a implementar**).

O endpoint de health check confirma que a API está em execução; ele não verifica a disponibilidade do banco.

## Estrutura

```text
src/TaskManager.Api/
  Data/TaskManagerDbContext.cs
  Models/
  Contracts/
  Services/SubscriptionService.cs
  Program.cs
tests/TaskManager.Api.Tests/
Dockerfile
docker-compose.yml
CHALLENGE.md
```

## Observações
- O código inicial compila, mas as operações de criação retornam `501 Not Implemented` até que o candidato implemente as regras.
- O campo `subscriptionType` representa `None`, `Monthly` ou `Annual`. O exercício não exige integração de pagamento.
- Não é necessário implementar autenticação, frontend, mensageria ou deploy cloud durante a sessão.
- .NET 7 está fora do período de suporte oficial; foi mantido para seguir os requisitos da stack. Em projetos novos, prefira uma versão do .NET com suporte vigente.
