English | [Português](README.pt-BR.md)

# The .NET AI Engineer Roadmap 2026–27: first exercise

The starting point of an eight-area study roadmap for .NET with AI:
a small ASP.NET Core 10 minimal API for a task list, with one test
per route. The later areas of the roadmap (database and login,
container, layers, AI service on top, production) grow this same API.

Article (pt-BR): https://ceseidl.com.br/artigos/

## Layout

- `src/TasksApi`: the API. `GET /tasks`, `GET /tasks/{id}`,
  `POST /tasks`, `PUT /tasks/{id}` and `DELETE /tasks/{id}`. Tasks
  live in memory.
- `tests/TasksApi.Tests`: 7 xUnit tests with `WebApplicationFactory`,
  including 400 for an empty title and 404 for a missing task.

## Run

```bash
dotnet test
dotnet run --project src/TasksApi
```

Requires the .NET 10 SDK (see `global.json`).

## Key question for area 1

Can you build a small API, with tests, without copying a tutorial
line by line? Try deleting `Program.cs` and writing it again from the
tests.
