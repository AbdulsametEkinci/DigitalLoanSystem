using Microsoft.EntityFrameworkCore;
using DigitalLoanSystem.Infrastructure.Data;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Infrastructure.Repositories;
using DigitalLoanSystem.Infrastructure.Adapters;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Domain.Factories;

var builder = WebApplication.CreateBuilder(args);

// (AppDbContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILoanRepository, LoanRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<ICreditScoreService, MockCreditScoreAdapter>();
builder.Services.AddScoped<IPricingEngineService, MockPricingEngineAdapter>();

// Application Services
builder.Services.AddScoped<ILoanApplicationService, LoanApplicationService>();
builder.Services.AddScoped<ICustomerApplicationService, CustomerApplicationService>();

// Domain Factories
builder.Services.AddSingleton<ILoanStrategyFactory, LoanStrategyFactory>();


builder.Services.AddControllers();
// Swagger
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