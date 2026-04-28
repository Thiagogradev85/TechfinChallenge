using Microsoft.AspNetCore.Mvc;
using TechfinChallenge.Clientes.Api.Services;

namespace TechfinChallenge.Clientes.Api.Controllers;

[ApiController]
[Route("tests")]
public class TestsController : ControllerBase
{
    private readonly TestRunnerService _runner;

    public TestsController(TestRunnerService runner)
    {
        _runner = runner;
    }

    [HttpPost("run")]
    public async Task<IActionResult> RunAll()
    {
        var (results, log) = await _runner.RunAsync();
        return Ok(new { results, log });
    }

    [HttpPost("run/{nome}")]
    public async Task<IActionResult> RunOne(string nome)
    {
        var (results, log) = await _runner.RunAsync(nome);
        return Ok(new { results, log });
    }
}
