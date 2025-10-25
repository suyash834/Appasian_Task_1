using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS for the frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseCors("AllowAll");

// In-memory task list
var tasks = new List<TaskItem>();

// app.MapGet("/", () => "Hello World!");
// GET /tasks - list all tasks
app.MapGet("/tasks", () => Results.Ok(tasks));

// POST /tasks - add a new task
app.MapPost("/tasks", (TaskItem task) =>
{
    task.Id = Guid.NewGuid();
    tasks.Add(task);
    return Results.Created($"/tasks/{task.Id}", task);
});

// PUT /tasks/{id} - toggle completion status
app.MapPut("/tasks/{id}", (Guid id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();
    task.IsCompleted = !task.IsCompleted;
    return Results.NoContent();
});

// DELETE /tasks/{id} - delete a task
app.MapDelete("/tasks/{id}", (Guid id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();
    tasks.Remove(task);
    return Results.NoContent();
});

app.Run();
