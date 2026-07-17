using MediatR;

using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Common.Behaviors;
using RTSCore.Domain.Interfaces;
using RTSCore.Infrastructure.Persistence;
using RTSCore.Application.Units.Commands;

using Scalar.AspNetCore;

using FluentValidation;
using RTSCore.WebApi.Common;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite("Data Source=game.db")
);

builder.Services.AddScoped<IUnitRepository, SqlUnitRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddExceptionHandler<GlobalExeptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AddExperienceCommand).Assembly)
);
builder.Services.AddValidatorsFromAssembly(typeof(AddExperienceCommand).Assembly);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();
app.MapControllers();

app.Run();

public partial class Program { }