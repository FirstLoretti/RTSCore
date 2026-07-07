using Microsoft.EntityFrameworkCore;

using RTSCore.Domain.Interfaces;
using RTSCore.Infrastructure.Persistence;

using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite("Data Source=game.db")
);
builder.Services.AddScoped<IUnitRepository, SqlUnitRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();