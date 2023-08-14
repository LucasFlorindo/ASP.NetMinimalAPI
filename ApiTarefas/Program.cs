using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("TasksDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello world!");

app.MapGet("quotes", async () => await new HttpClient().GetStringAsync("https://ron-swanson-quotes.herokuapp.com/v2/quotes"));

app.MapGet("/tasks", async (AppDbContext db) =>
{
    return await db.Tasks.ToListAsync();
});

app.MapPost("/tasks", async (Task task, AppDbContext db) =>
{
    db.Tasks.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/tasks/{task.Id}", task);
});

app.Run();


class Tasks
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool IsConcluded { get; set; }


}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Task> Tasks => Set<Task>();
}



