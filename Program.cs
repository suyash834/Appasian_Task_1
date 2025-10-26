using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS for your Vercel frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins(
                "https://appasian-frontend-task1-1ds1.vercel.app",
                "http://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    );
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend"); 

// In-memory task list
var tasks = new List<TaskItem>();

app.MapGet("/", () => "Hello World!");
app.MapGet("/tasks", () => Results.Ok(tasks));

app.MapPost("/tasks", (TaskItem task) =>
{
    task.Id = Guid.NewGuid();
    tasks.Add(task);
    return Results.Created($"/tasks/{task.Id}", task);
});

app.MapPut("/tasks/{id}", (Guid id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();
    task.IsCompleted = !task.IsCompleted;
    return Results.NoContent();
});

app.MapDelete("/tasks/{id}", (Guid id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();
    tasks.Remove(task);
    return Results.NoContent();
});

app.Run();
