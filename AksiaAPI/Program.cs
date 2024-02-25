using AksiaAPI.Models.Business;
using AksiaAPI.Repositories;
using AksiaAPI.Repositories.Interfaces;
using AksiaAPI.Services;
using AksiaAPI.Services.Interfaces;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AksiaDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddProblemDetails(setup => {
    setup.IncludeExceptionDetails = (ctx, env) => false;
    setup.Map<BusinessException>(exception => new ProblemDetails
    {
        Detail = exception.Detail,
        Status = exception.Status,
        Type = exception.Type,
    });
});
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
app.UseProblemDetails();
app.MapControllers();
app.Run();
