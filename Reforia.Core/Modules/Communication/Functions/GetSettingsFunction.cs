using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Contracts;
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
        
        return new GetSettingsFunctionResponse()
        {
            ApiToken = apiToken ?? "",
            IsUserHighlighted = shouldHighlightMessages
        };
    }
}