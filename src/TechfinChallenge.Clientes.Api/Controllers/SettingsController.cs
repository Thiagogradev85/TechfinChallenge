using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace TechfinChallenge.Clientes.Api.Controllers;

[ApiController]
[Route("settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public SettingsController(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { messageBroker = _configuration["MessageBroker"] ?? "RabbitMQ" });
    }

    [HttpPost("broker")]
    public IActionResult SetBroker([FromBody] SetBrokerRequest request)
    {
        if (request.Broker != "RabbitMQ" && request.Broker != "Kafka")
            return BadRequest(new { status = "ERRO", detalheErro = "Broker inválido. Use 'RabbitMQ' ou 'Kafka'." });

        var path = Path.Combine(_env.ContentRootPath, "appsettings.json");
        var json = System.IO.File.ReadAllText(path);
        var node = JsonNode.Parse(json)!;
        node["MessageBroker"] = request.Broker;
        System.IO.File.WriteAllText(path, node.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        return Ok(new { messageBroker = request.Broker, status = "OK" });
    }
}

public record SetBrokerRequest(string Broker);
