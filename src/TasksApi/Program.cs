using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<TaskStore>();

var app = builder.Build();

var tasks = app.MapGroup("/tasks");

tasks.MapGet("/", (TaskStore store) => store.All());

tasks.MapGet("/{id:int}", (int id, TaskStore store) =>
    store.Find(id) is { } item ? Results.Ok(item) : Results.NotFound());

tasks.MapPost("/", (NewTask input, TaskStore store) =>
{
    if (string.IsNullOrWhiteSpace(input.Title))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = ["Title is required."]
        });

    var item = store.Add(input.Title.Trim());
    return Results.Created($"/tasks/{item.Id}", item);
});

tasks.MapPut("/{id:int}", (int id, UpdateTask input, TaskStore store) =>
{
    if (string.IsNullOrWhiteSpace(input.Title))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = ["Title is required."]
        });

    return store.Update(id, input.Title.Trim(), input.Done) is { } item
        ? Results.Ok(item)
        : Results.NotFound();
});

tasks.MapDelete("/{id:int}", (int id, TaskStore store) =>
    store.Remove(id) ? Results.NoContent() : Results.NotFound());

app.Run();

public record TodoItem(int Id, string Title, bool Done);
public record NewTask(string? Title);
public record UpdateTask(string? Title, bool Done);

// In-memory store. Area 2 of the roadmap swaps it for a real database.
public class TaskStore
{
    private readonly ConcurrentDictionary<int, TodoItem> _items = new();
    private int _nextId;

    public IEnumerable<TodoItem> All() => _items.Values.OrderBy(i => i.Id);

    public TodoItem? Find(int id) => _items.TryGetValue(id, out var item) ? item : null;

    public TodoItem Add(string title)
    {
        var item = new TodoItem(Interlocked.Increment(ref _nextId), title, false);
        _items[item.Id] = item;
        return item;
    }

    public TodoItem? Update(int id, string title, bool done)
    {
        if (!_items.ContainsKey(id)) return null;
        var item = new TodoItem(id, title, done);
        _items[id] = item;
        return item;
    }

    public bool Remove(int id) => _items.TryRemove(id, out _);
}

public partial class Program;
