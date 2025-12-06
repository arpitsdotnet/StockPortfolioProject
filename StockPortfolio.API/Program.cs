using Microsoft.EntityFrameworkCore;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;
using StockPortfolio.Core.Services.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient();

builder.Services.AddScoped<IStockApiClient, AlphaVantageApiClientService>();

builder.Services.AddTransient<SymbolSearchHandler>();
builder.Services.AddTransient<TimeSeriesIntradayHandler>();
builder.Services.AddTransient<TimeSeriesDailyHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
