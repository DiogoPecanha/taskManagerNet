# Desafio técnico — TaskManager | Backend Pleno .NET | 60 minutos

## Contexto
O TaskManager é uma API SaaS para gerenciamento de projetos e tarefas. Cada empresa possui um plano de assinatura que define os limites de uso.

## Tarefa obrigatória
Implemente as regras de assinatura nos endpoints de criação de projetos e tarefas.

### Planos e limites

| Regra | Freemium | Básico | Avançado |
|---|---:|---:|---:|
| Projetos por empresa | 1 | Até 10 | Ilimitados |
| Tarefas por projeto | Até 20 | Ilimitadas | Ilimitadas |
| Tipo de assinatura | `None` | `Monthly` ou `Annual` | `Monthly` ou `Annual` |

### Comportamento esperado
1. Ao criar um projeto, identificar o plano da empresa e rejeitar a criação se o limite de projetos tiver sido atingido.
2. Ao criar uma tarefa, identificar a empresa/plano por meio do projeto e rejeitar a criação se uma conta Freemium já tiver 20 tarefas nesse projeto.
3. Tratar empresa ou projeto inexistente com resposta HTTP adequada.
4. Retornar `201 Created` em caso de sucesso e uma resposta clara de conflito/limite excedido (por exemplo, `409 Conflict`) quando a regra de assinatura impedir a operação.
5. Manter a regra de negócio fora do endpoint/handler sempre que possível.

### Se sobrar tempo
- Adicionar testes unitários para um cenário permitido e um cenário bloqueado.
- Melhorar o tratamento de erros usando `ProblemDetails` ou exceção de domínio específica.
- Explicar como faria a atualização segura de plano e a integração com pagamento em uma evolução futura.

## Escopo e avaliação
Não é necessário implementar CRUD completo, autenticação, integração de pagamento, AWS, Kafka, frontend ou uma arquitetura distribuída.

| Critério | Peso |
|---|---:|
| Correção das regras de assinatura | 40% |
| Clareza e organização do código C# | 20% |
| Contratos HTTP e tratamento de erros | 15% |
| Testes | 15% |
| Explicação de decisões e trade-offs | 10% |

A sessão dura no máximo **60 minutos**. Priorize uma solução funcional e explique o que ficaria pendente com mais tempo.
