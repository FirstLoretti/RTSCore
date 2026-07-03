using Microsoft.EntityFrameworkCore;

using RTSCore.Infrastructure.Persistence;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlite("Data Source=rts.db")
);

var app = builder.Build();
app.MapControllers();

app.Run();