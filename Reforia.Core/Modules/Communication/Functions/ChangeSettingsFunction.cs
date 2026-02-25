using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;

namespace Reforia.Core.Modules.Communication.Functions;

public class ChangeSettingsFunction : WebFunction<ChangeSettingsFunctionBody, ChangeSettingsFunctionResponse>
{
    protected override async Task<ChangeSettingsFunctionResponse> Handle(ChangeSettingsFunctionBody request,
                                                                         IServiceProvider provider)
    {
        var configManager = provider.GetService<IConfigService>();
        if (configManager == null)
            throw new NullReferenceException(nameof(IConfigService));
        
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
            {
                await configManager.Set(option, value);
            }
        }
        
        return new ChangeSettingsFunctionResponse()
        {
            Success = true
        };
    }
}