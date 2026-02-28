using Microsoft.AspNetCore.Mvc;
using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Functions.Response;
using ReforiaBackend.Dto.Requests;
using ReforiaBackend.Dto.Responses;
using ReforiaBackend.Dto.Responses.Config;

namespace ReforiaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _config;

    public ConfigController(IConfigService config)
    {
        _config = config;
    }

    [HttpGet("settings")]
    public async Task<GetSettingsResponse> GetSettings()
    {
        var apiToken = await _config.Get(EConfigOptions.ApiToken);
        var shouldHighlightMessages = await _config.Get(EConfigOptions.UserHighlight, "False") == "True";
        var language = await _config.Get(EConfigOptions.Language, "en");
        var alertOnMention = await _config.Get(EConfigOptions.AlertOnMention, "True") == "True";
        var alertOnKeyword = await _config.Get(EConfigOptions.AlertOnKeyword, "False") == "True";
        var highlightOnKeyword = await _config.Get(EConfigOptions.HighlightOnKeyword, "False") == "True";
        var keywordList = await _config.Get(EConfigOptions.KeywordList);
        var showBeatmapBanner = await _config.Get(EConfigOptions.ShowBeatmapBanner, "True") == "True";
        var defaultStartValue = await _config.Get(EConfigOptions.DefaultStartValue, "10");
        var defaultTimerValue = await _config.Get(EConfigOptions.DefaultTimerValue, "120");
        var osuTourneyClientPathFolder = await _config.Get(EConfigOptions.OsuTourneyClientPathFolder, string.Empty);
        
        return new GetSettingsResponse()
        {
            ApiToken = apiToken,
            IsUserHighlighted = shouldHighlightMessages,
            Language = language,
            AlertOnKeyword = alertOnKeyword,
            AlertOnMention =  alertOnMention,
            HighlightOnKeyword = highlightOnKeyword,
            KeywordList = keywordList.Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrWhiteSpace(k)).ToList(),
            ShowBeatmapBanner = showBeatmapBanner,
            DefaultStartValue = defaultStartValue,
            DefaultTimerValue = defaultTimerValue,
            OsuTourneyClientPathFolder = osuTourneyClientPathFolder
        };
    }
    
    [HttpPost("settings")]
    public async Task<IActionResult> ChangeSettings([FromBody] ChangeSettingsRequest request)
    {
        var updates = new Dictionary<EConfigOptions, string?>
        {
            { EConfigOptions.ApiToken, request.ApiToken },
            { EConfigOptions.Language, request.Language },
            { EConfigOptions.UserHighlight, request.IsUserHighlighted.ToString() },
            { EConfigOptions.AlertOnMention, request.AlertOnMention.ToString() },
            { EConfigOptions.AlertOnKeyword, request.AlertOnKeyword.ToString() },
            { EConfigOptions.HighlightOnKeyword, request.HighlightOnKeyword.ToString() },
            { EConfigOptions.KeywordList, string.Join(',', request.KeywordList) },
            { EConfigOptions.ShowBeatmapBanner, request.ShowBeatmapBanner.ToString() },
            { EConfigOptions.DefaultStartValue, request.DefaultStartValue },
            { EConfigOptions.DefaultTimerValue, request.DefaultTimerValue },
            { EConfigOptions.OsuTourneyClientPathFolder, request.OsuTourneyClientPathFolder }
        };
        
        foreach (var (option, value) in updates)
        {
            if (!string.IsNullOrWhiteSpace(value)) 
                await _config.Set(option, value);
        }

        return Ok(new ChangeSettingsResponse { Success = true });
    }

    [HttpGet("credentials")]
    public async Task<IActionResult> GetCredentials()
    {
        var username = await _config.Get(EConfigOptions.IrcUsername);
        var password = await _config.Get(EConfigOptions.IrcPassword);

        if (string.IsNullOrWhiteSpace(username)|| string.IsNullOrWhiteSpace(password))
            return BadRequest("IRC credentials are not set");

        return Ok(new GetIrcCredentialsResponse()
        {
            Nick = username,
            Password = password
        });
    }

    [HttpPost("credentials")]
    public async Task<IActionResult> SetCredentials([FromBody] SetIrcCredentialsRequest request)
    {
        await _config.Set(EConfigOptions.IrcUsername, request.Nick);
        await _config.Set(EConfigOptions.IrcPassword, request.Password);
        
        return Ok(new SetIrcCredentialsResponse { Success = true });
    }
}