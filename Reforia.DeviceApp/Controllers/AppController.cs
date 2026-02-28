using Microsoft.AspNetCore.Mvc;
using ReforiaBackend.Dto.Requests.App;

namespace ReforiaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppController : ControllerBase
{
    private readonly ILogger<AppController> _logger;

    public AppController(ILogger<AppController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult SaveLog([FromBody] SaveLogRequest request)
    {
        if (!Enum.TryParse<LogLevel>(request.Level, true, out var logLevel))
        {
            _logger.LogWarning("Unknown log level {Level}: {Message}", request.Level, request.Message);
            return Ok();
        }

        _logger.Log(logLevel, request.Message);

        return Ok();
    }
    
    [HttpGet]
    public IActionResult GetStatus()
    {
        _logger.LogInformation("Status check received from device app");
        return Ok(new { Status = "OK", Timestamp = DateTime.UtcNow });
    }
}