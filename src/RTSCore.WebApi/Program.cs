using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Common.Behaviors;
using RTSCore.Infrastructure.Persistence;

using Scalar.AspNetCore;

using FluentValidation;
using RTSCore.WebApi.Common;
using RTSCore.Infrastructure.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RTSCore.Domain.ValueObjects;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using RTSCore.Application.Campaign.Lifecycle;
using RTSCore.Domain.ValueObjects.Configurations;
using RTSCore.Domain.Services.Combat;
using RTSCore.Domain.Entities.Campaign;
using RTSCore.Domain.Entities.Common;
using RTSCore.Domain.Entities.Identity;
using RTSCore.Application.Common;
using RTSCore.Application.Common.Validation;
using Microsoft.Extensions.Options;
using RTSCore.WebApi.Configurations;
using RTSCore.Application.AI.Diplomacy;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        theme: AnsiConsoleTheme.Code
    )
    .CreateLogger();
builder.Host.UseSerilog();

builder.Configuration.AddJsonFile("gameconfigurations.json");

builder.Services.AddSingleton<IValidator<GameConfigurations>, GameConfigurationsValidator>();
builder.Services.AddSingleton<IValidateOptions<GameConfigurations>, FluentOptionsValidator<GameConfigurations>>();

builder.Services.AddOptions<GameConfigurations>()
    .Bind(builder.Configuration.GetSection("GameConfigurations"))
    .ValidateOnStart();

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameConfigurations>>().Value.Units);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameConfigurations>>().Value.AutoBattle);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameConfigurations>>().Value.Buildings);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameConfigurations>>().Value.Cities);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameConfigurations>>().Value.Diplomacy);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameConfigurations>>().Value.Factions);
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<GameConfigurations>>().Value.Movement);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=game.db"));

builder.Services.AddScoped<IUnitRepository, SqlUnitRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<IBuildingRepository, SqlBuildingRepository>();
builder.Services.AddScoped<IFactionRepository, SqlFactionRepository>();
builder.Services.AddScoped<ICityRepository, SqlCityRepository>();
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, SqlRefreshTokenRepository>();
builder.Services.AddScoped<IArmyRepository, SqlArmyRepository>();
builder.Services.AddScoped<AiDiplomat>();

//builder.Services.AddSingleton(Array.Empty<FactionPreset>());
//builder.Services.AddSingleton(GameBalance.Buildings.GetAllTemplates);
builder.Services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IAutoBattleCalculator, AutoBattleCalculator>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException($"Секция конфигурации {nameof(JwtSettings)} не найдена");

var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMediatR(cfg =>
{
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));

    cfg.RegisterServicesFromAssembly(typeof(StartCampaignCommand).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(StartCampaignCommand).Assembly);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }