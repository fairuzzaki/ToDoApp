using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ToDoApp.Data;
using ToDoApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Todo"));

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS middleware
app.UseCors();

// Initialize the database with default data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    AddTestData(context);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// To Initiate default todo on startup
void AddTestData(DataContext context)
{
    // Look for any TodoItems already in the database.
    if (context.Todo.Any())
    {
        return;   // DB has been seeded
    }

    context.Todo.AddRange(
        new ToDo
        {
            Description = "Learn ASP.NET Core",
            //TargetDate = new DateOnly(2024, 7, 30),
            IsDone = false
        },
        new ToDo
        {
            Description = "Learn ReactJs",
            //TargetDate = new DateOnly(2024, 8, 30),
            IsDone = false
        },
        new ToDo
        {
            Description = "Learn Java",
            //TargetDate = new DateOnly(2024, 9, 30),
            IsDone = false
        }
    );
    context.SaveChanges();
}
