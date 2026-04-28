using System.Diagnostics;
using System.Xml.Linq;

namespace TechfinChallenge.Clientes.Api.Services;

public record TestResult(
    string Name,
    string ClassName,
    string Outcome,
    double DurationMs,
    string? ErrorMessage
);

public class TestRunnerService
{
    private readonly string _testProjectPath;

    public TestRunnerService()
    {
        var root = Directory.GetCurrentDirectory();
        while (!Directory.Exists(Path.Combine(root, "tests")) && Path.GetDirectoryName(root) is string parent)
            root = parent;
        _testProjectPath = Path.Combine(root, "tests", "TechfinChallenge.Tests");
    }

    public async Task<(List<TestResult> Results, string Log)> RunAsync(string? filtro = null)
    {
        var trxPath = Path.Combine(Path.GetTempPath(), $"results_{Guid.NewGuid()}.trx");

        var args = $"test \"{_testProjectPath}\" --logger \"trx;LogFileName={trxPath}\"";
        if (!string.IsNullOrEmpty(filtro))
            args += $" --filter \"Name={filtro}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        var log = $"WorkDir: {_testProjectPath}\nExists: {Directory.Exists(_testProjectPath)}\nExit: {process.ExitCode}\nOUT: {stdout}\nERR: {stderr}";
        return (ParseTrx(trxPath), log);
    }

    private static List<TestResult> ParseTrx(string trxPath)
    {
        if (!File.Exists(trxPath)) return [];

        var xml = XDocument.Load(trxPath);
        XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        return xml.Descendants(ns + "UnitTestResult")
            .Select(r =>
            {
                var name = r.Attribute("testName")?.Value ?? "";
                var outcome = r.Attribute("outcome")?.Value ?? "Unknown";
                var durationStr = r.Attribute("duration")?.Value ?? "00:00:00";
                var error = r.Descendants(ns + "Message").FirstOrDefault()?.Value;

                var duration = TimeSpan.TryParse(durationStr, out var ts) ? ts.TotalMilliseconds : 0;
                var parts = name.Split('.');
                var className = parts.Length > 1 ? parts[^2] : name;
                var methodName = parts.Last();

                return new TestResult(methodName, className, outcome, duration, error);
            })
            .ToList();
    }
}
