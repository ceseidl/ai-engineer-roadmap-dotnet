using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TasksApi.Tests;

// One test per route, plus the error cases a beginner usually forgets.
public class TasksEndpointsTests
{
    private static HttpClient NewClient() => new WebApplicationFactory<Program>().CreateClient();

    private static async Task<TodoItem> CreateAsync(HttpClient client, string title)
    {
        var response = await client.PostAsJsonAsync("/tasks", new { title });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItem>())!;
    }

    [Fact]
    public async Task Post_creates_task_and_returns_201_with_location()
    {
        using var client = NewClient();

        var response = await client.PostAsJsonAsync("/tasks", new { title = "Study minimal APIs" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var item = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.Equal("Study minimal APIs", item!.Title);
        Assert.False(item.Done);
        Assert.Equal($"/tasks/{item.Id}", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Post_without_title_returns_400()
    {
        using var client = NewClient();

        var response = await client.PostAsJsonAsync("/tasks", new { title = "  " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_all_lists_created_tasks_in_order()
    {
        using var client = NewClient();
        await CreateAsync(client, "First");
        await CreateAsync(client, "Second");

        var items = await client.GetFromJsonAsync<List<TodoItem>>("/tasks");

        Assert.Equal(["First", "Second"], items!.Select(i => i.Title));
    }

    [Fact]
    public async Task Get_by_id_returns_task_or_404()
    {
        using var client = NewClient();
        var created = await CreateAsync(client, "Read about DI");

        var found = await client.GetFromJsonAsync<TodoItem>($"/tasks/{created.Id}");
        var missing = await client.GetAsync("/tasks/999");

        Assert.Equal(created, found);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task Put_updates_title_and_done()
    {
        using var client = NewClient();
        var created = await CreateAsync(client, "Write a test");

        var response = await client.PutAsJsonAsync($"/tasks/{created.Id}", new { title = "Write two tests", done = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.Equal(new TodoItem(created.Id, "Write two tests", true), updated);
    }

    [Fact]
    public async Task Put_on_missing_task_returns_404()
    {
        using var client = NewClient();

        var response = await client.PutAsJsonAsync("/tasks/999", new { title = "Nothing", done = false });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_removes_task_and_second_delete_returns_404()
    {
        using var client = NewClient();
        var created = await CreateAsync(client, "Delete me");

        var first = await client.DeleteAsync($"/tasks/{created.Id}");
        var second = await client.DeleteAsync($"/tasks/{created.Id}");
        var get = await client.GetAsync($"/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}
