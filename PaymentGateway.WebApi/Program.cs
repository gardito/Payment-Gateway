using Cache.Common;
using Cache.InMemory;
using Data.Payment.InMemoryDb;
using Domain.Repositories;
using Domain.Services;
using Payment.Service;
using Service.Banking.CkoBankSimulator;
using Service.Banking.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// IoC container definitions
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBankingThirdPartyService, CkoBankSimulator>();

// Singleton because it is an in-memory db.
// it is not advisable to have singleton db instances in production
builder.Services.AddSingleton<IPaymentRepository, PaymentInMemoryDb>();
builder.Services.AddSingleton<IIdempotencyCachingSystem, InMemoryCache>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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