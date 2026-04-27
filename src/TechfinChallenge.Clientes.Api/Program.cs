using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TechfinChallenge.Clientes.Api.Data;
using TechfinChallenge.Clientes.Api.Messaging;
using TechfinChallenge.Clientes.Api.Repositories;
using TechfinChallenge.Clientes.Api.Services;
using TechfinChallenge.Messaging.Abstractions;
using TechfinChallenge.Messaging.Kafka;
using TechfinChallenge.Messaging.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "Data Source=clientes;Mode=Memory;Cache=Shared";
var jwtSecret = "techfin-secret-key-2026-desafio-seguro";

builder.Services.AddControllers();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(p =>
        p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IClienteRepository, ClienteRepository>();
builder.Services.AddSingleton<IClienteService, ClienteService>();
builder.Services.AddSingleton<ITransacaoEventHandler, TransacaoEventHandler>();

var messageBroker = builder.Configuration["MessageBroker"] ?? "RabbitMQ";
if (messageBroker == "Kafka")
    builder.Services.AddKafkaConsumer();
else
    builder.Services.AddRabbitMqConsumer();

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
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
