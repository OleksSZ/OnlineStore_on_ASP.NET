using Microsoft.EntityFrameworkCore;
using OnlineStore.Api.Middleware;
using OnlineStore.Application;
using OnlineStore.Infrastructure;
using OnlineStore.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default не задано");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();