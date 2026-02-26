using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Response;

namespace Reforia.Core.Modules.Communication.Functions;

public class GetSettingsFunction : WebFunction<GetSettingsFunctionResponse>
{
    protected override async Task<GetSettingsFunctionResponse> Handle(IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config is null)
            throw new NullReferenceException(nameof(config));
        
        var apiToken = await config.Get(EConfigOptions.ApiToken);
        var shouldHighlightMessages = await config.Get(EConfigOptions.UserHighlight) == "True";
        var language = await config.Get(EConfigOptions.Language) ?? "en";
        var alertOnMention = await config.Get(EConfigOptions.AlertOnMention) == "True";
        var alertOnKeyword = await config.Get(EConfigOptions.AlertOnKeyword) == "True";
        var highlightOnKeyword = await config.Get(EConfigOptions.HighlightOnKeyword) == "True";
        var keywordList = await config.Get(EConfigOptions.KeywordList) ?? "";
        var showBeatmapBanner = await config.Get(EConfigOptions.ShowBeatmapBanner) == "True";
        
        return new GetSettingsFunctionResponse()
        {
            ApiToken = apiToken ?? "",
            IsUserHighlighted = shouldHighlightMessages,
            Language = language,
            AlertOnKeyword = alertOnKeyword,
            AlertOnMention =  alertOnMention,
            HighlightOnKeyword = highlightOnKeyword,
            KeywordList = keywordList.Split(',').Select(k => k.Trim()).Where(k => !string.IsNullOrWhiteSpace(k)).ToList(),
            ShowBeatmapBanner = showBeatmapBanner
        };
    }
}