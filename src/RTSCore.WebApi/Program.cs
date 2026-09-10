using Microsoft.EntityFrameworkCore;

using RTSCore.Application.Common.Behaviors;
using RTSCore.Domain.Interfaces;
using RTSCore.Infrastructure.Persistence;

using Scalar.AspNetCore;

using FluentValidation;
using RTSCore.WebApi.Common;
using RTSCore.Domain.ValueObjects.Presets;

using RTSCore.Domain.Services;
using RTSCore.Application.Campaign.Commands;
using RTSCore.Application.Campaign.Services.Diplomacy;
using RTSCore.Application.Common.Settings;
using RTSCore.Domain.Interfaces.Authentication;
using RTSCore.Infrastructure.Authentication;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=game.db"));

builder.Services.AddScoped<IUnitRepository, SqlUnitRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<IBuildingRepository, SqlBuildingRepository>();
builder.Services.AddScoped<IFactionRepository, SqlFactionRepository>();
builder.Services.AddScoped<ICityRepository, SqlCityRepository>();
builder.Services.AddScoped<IUserRepository, SqlUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, SqlRefreshTokenRepository>();
builder.Services.AddScoped<DiplomacyAi>();
builder.Services.AddSingleton(Array.Empty<FactionPreset>());
builder.Services.AddSingleton(GameBalance.Buildings.GetAllTemplates);
builder.Services.AddSingleton(GameBalance.Units.GetAllTemplates);
builder.Services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException($"Секция конфигурации {nameof(JwtSettings)} не найдена");

var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
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

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }