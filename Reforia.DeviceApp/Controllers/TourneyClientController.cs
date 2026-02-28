using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Reforia.Core.Common;
using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using ReforiaBackend.Dto;
using ReforiaBackend.Dto.Requests.TourneyClient;

namespace ReforiaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourneyClientController : ControllerBase
{
    private readonly IConfigService                   _config;

    public TourneyClientController(IConfigService config)
    {
        _config = config;
    }

    [HttpPost("open")]
    public async Task<IActionResult> OpenClient([FromBody] OpenClientRequest request)
    {
        var folder = await _config.Get(EConfigOptions.OsuTourneyClientPathFolder);

        if (string.IsNullOrWhiteSpace(folder))
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = 400,
                Error = "Osu! Tourney Client path is not set.",
                ErrorCode = EErrorCode.InvalidTourneyClientPath
            });

        var osuClientPath = Path.Combine(folder, "osu!.exe");
        var osuTourneyConfigPath = Path.Combine(folder, "tournament.cfg");

        if (!System.IO.File.Exists(osuClientPath))
            return BadRequest(new ApiResponse<object>
            {
                StatusCode = 400,
                Error = "Osu! Tournament Client files not found.",
                ErrorCode = EErrorCode.InvalidTourneyClientPath
            });

        if (!System.IO.File.Exists(osuTourneyConfigPath))
            await System.IO.File.WriteAllTextAsync(osuTourneyConfigPath, "");
        
        var updates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Acronym", request.Acronym },
            { "TeamSize", request.TeamSize.ToString() }
        };

        var lines = await System.IO.File.ReadAllLinesAsync(osuTourneyConfigPath);
        for (var i = 0; i < lines.Length; i++)
        {
            var parts = lines[i].Split('=', 2);
                
            if (parts.Length != 2) 
                continue;
                
            var key = parts[0].Trim();
                
            if (!updates.TryGetValue(key, out var newValue))
                continue;
                
            lines[i] = $"{key} = {newValue}";
            updates.Remove(key);
        }

        var finalLines = lines.ToList();
            
        finalLines.AddRange(updates.Select(remaining => $"{remaining.Key} = {remaining.Value}"));

        await System.IO.File.WriteAllLinesAsync(osuTourneyConfigPath, finalLines);

        Process.Start(new ProcessStartInfo
        {
            FileName = osuClientPath,
            WorkingDirectory = folder,
            UseShellExecute = true
        });

        return Ok("Client configuration updated and starting...");
    }
}