using System.Reflection;
using Authentication.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApiIdentity.Data;
using WebApiIdentity.Domain;
using WebApiIdentity.Domain.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddIdentityAuthentication<AppDbContext, User, Role>(builder.Configuration);

var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddMediatR(assembly);
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IPipelineBehavior<,>));
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.TryAddSingleton<CurrentTimeFunc>(() => DateTimeOffset.UtcNow);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
