using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TechfinChallenge.Messaging.Kafka;
using TechfinChallenge.Messaging.RabbitMQ;
using TechfinChallenge.Transacao.Api.Data;
using TechfinChallenge.Transacao.Api.Repositories;
using TechfinChallenge.Transacao.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var jwtSecret = "techfin-secret-key-2026-desafio-seguro";
var clientesApiUrl = "http://localhost:5174/";

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

builder.Services.AddHttpClient<ITransacaoService, TransacaoService>(client =>
{
    client.BaseAddress = new Uri(clientesApiUrl);
});

builder.Services.AddSingleton<ITransacaoRepository, TransacaoRepository>();

var messageBroker = builder.Configuration["MessageBroker"] ?? "RabbitMQ";
if (messageBroker == "Kafka")
    builder.Services.AddKafkaPublisher();
else
    builder.Services.AddRabbitMqPublisher();

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
