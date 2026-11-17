[English](README.md) | Português

# O Roteiro do Engenheiro de IA em .NET 2026–27: primeiro exercício

O ponto de partida de um roteiro de estudo em oito áreas para .NET
com IA: uma minimal API pequena em ASP.NET Core 10 para uma lista de
tarefas, com um teste por rota. As próximas áreas do roteiro (banco e
login, contêiner, camadas, serviço de IA por cima, produção) fazem
crescer esta mesma API.

Artigo: https://ceseidl.com.br/artigos/

## Estrutura

- `src/TasksApi`: a API. `GET /tasks`, `GET /tasks/{id}`,
  `POST /tasks`, `PUT /tasks/{id}` e `DELETE /tasks/{id}`. As tarefas
  ficam em memória.
- `tests/TasksApi.Tests`: 7 testes xUnit com `WebApplicationFactory`,
  incluindo 400 para título vazio e 404 para tarefa inexistente.

## Como rodar

```bash
dotnet test
dotnet run --project src/TasksApi
```

Precisa do SDK do .NET 10 (ver `global.json`).

## Pergunta-chave da área 1

Dá para subir uma API pequena, com teste, sem copiar um tutorial
linha por linha? Experimente apagar o `Program.cs` e escrevê-lo de
novo a partir dos testes.
