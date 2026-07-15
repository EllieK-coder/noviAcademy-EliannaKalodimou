using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using NLog.Extensions.Logging;
using NoviCode;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(container =>
{
    container.RegisterModule(new ApplicationModule());
    container.RegisterType<Dispatcher>().As<IDispatcher>();
});



var builder = WebApplication.CreateBuilder(args);

// Logging via NLog (same nlog.config layout as the Console app).
builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");

// One AppDbContext per request (scoped) — the EF Core repositories depend on it.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(DbConnection.ConnectionString));

builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<IWalletRepository, EfWalletRepository>();

// Single-instance in-memory cache. The Application talks only to the ICache port;
// the IMemoryCache-backed implementation (MemoryCacheStore) lives in Infrastructure,
// so caching stays a swappable detail.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, MemoryCacheStore>();

	options.UseSqlServer(DbConnection.ConnectionString));

builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<IWalletRepository, EfWalletRepository>();

// Single-instance in-memory cache (Day 6). Redis would replace this behind a load balancer.
// The Application talks only to the ICache port; the IMemoryCache-backed implementation
// (MemoryCacheStore) lives in Infrastructure, so caching stays a swappable detail.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICache, MemoryCacheStore>();

// The services own the caching (read-through + write-through) and reach the DB via the repositories.

// Accept/emit enums (e.g. Currency) as their string names, not numbers.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

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

	app.UseSwagger();
	app.UseSwaggerUI();
	app.MapGet("/", () => Results.Redirect("/swagger")); // root → Swagger UI
}

app.MapControllers();

app.Run();
