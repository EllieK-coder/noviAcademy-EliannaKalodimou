using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using NoviCode;
using WorldRank.Application.Interfaces;
using WorldRank.Application.Strategies;
using WorldRank.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Logging via NLog (same nlog.config layout as the Console app).
builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");

builder.Services.AddDbContext<WorldRankDbContext>(options => {
    options.UseSqlServer("Server=localhost;Database=WorldRank;Integrated Security=true; TrustServerCertificate=true");
});

//services/Repositories
builder.Services.AddScoped<IPlayerRepository, DBPlayerRepository>();
builder.Services.AddScoped<IWalletRepository, DBWalletRepository>();

//strategy
builder.Services.AddSingleton<IFundsStrategy, AddFundsStrategy>();
builder.Services.AddSingleton<IFundsStrategy, SubtractFundsStrategy>();
builder.Services.AddSingleton<IFundsStrategy, ForceSubtractFundsStrategy>();

// Accept/emit enums (e.g. Currency) as their string names, not numbers.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Swagger / OpenAPI — interactive API docs at /swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Serve the Swagger JSON and UI in Development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger")); // root → Swagger UI
}

app.MapControllers();

app.Run();
