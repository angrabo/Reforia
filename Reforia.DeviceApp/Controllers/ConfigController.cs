using Microsoft.AspNetCore.Mvc;
using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using ReforiaBackend.Dto.Requests;
using ReforiaBackend.Dto.Responses;

namespace ReforiaBackend.Controllers;

[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _config;

    public ConfigController(IConfigService config)
    {
        _config = config;
    }

    [HttpGet("settings")]
    public async Task<GetSettingsFunctionResponse> GetSettings()
    {
        var apiToken = await _config.Get(EConfigOptions.ApiToken);
        var shouldHighlightMessages = await _config.Get(EConfigOptions.UserHighlight, "False") == "True";
        var language = await _config.Get(EConfigOptions.Language, "en");
        var alertOnMention = await _config.Get(EConfigOptions.AlertOnMention, "True") == "True";
        var alertOnKeyword = await _config.Get(EConfigOptions.AlertOnKeyword, "False") == "True";
        var highlightOnKeyword = await _config.Get(EConfigOptions.HighlightOnKeyword, "False") == "True";
        var keywordList = await _config.Get(EConfigOptions.KeywordList);
        var showBeatmapBanner = await _config.Get(EConfigOptions.ShowBeatmapBanner, "True") == "True";
        
        return new GetSettingsFunctionResponse()
        {
            ApiToken = apiToken,
            IsUserHighlighted = shouldHighlightMessages,
            Language = language,
            AlertOnKeyword = alertOnKeyword,
            AlertOnMention =  alertOnMention,
            HighlightOnKeyword = highlightOnKeyword,
            KeywordList = keywordList.Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrWhiteSpace(k)).ToList(),
            ShowBeatmapBanner = showBeatmapBanner
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
            { EConfigOptions.ShowBeatmapBanner, request.ShowBeatmapBanner.ToString() }
        };
        
        foreach (var (option, value) in updates)
        {
            if (!string.IsNullOrWhiteSpace(value)) 
                await _config.Set(option, value);
        }

        return Ok(new ChangeSettingsResponse { Success = true });
    }
}