using Auth;
using Auth.Abstractions;
using Auth.Authentication.Extensions;
using Auth.Authentication.TokenStorage;
using Auth.Data;
using Auth.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddIdentityAuthentication<AppDbContext, User, Role>(builder.Configuration);

builder.Services.AddSingleton<CurrentTimeFunc>(() => DateTimeOffset.UtcNow);

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
