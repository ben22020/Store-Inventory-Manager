using Microsoft.AspNetCore.Mvc.Formatters;
using Project1.DB;

//string connectionString = await File.ReadAllTextAsync("C:/Users/bbruc/Revature/Ben-H/ben-db-connection-string.txt");

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("PROJECT1-DB-Connection");
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDBCommands>(sp => new DBInteraction(connectionString, sp.GetRequiredService<ILogger<DBInteraction>>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

