using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechfinChallenge.Auth.Api.Data;
using TechfinChallenge.Auth.Api.Repositories;
using TechfinChallenge.Auth.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Source=techfin;Mode=Memory;Cache=Shared";
var jwtSecret = "techfin-secret-key-2024-muito-segura";

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UsuarioRepository>();
builder.Services.AddSingleton(_ => new AuthService(
    new UsuarioRepository(), jwtSecret));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

DatabaseInitializer.Initialize(connectionString);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
