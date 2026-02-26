using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Contracts;
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
        
        if (!string.IsNullOrEmpty(request.ApiToken)) 
            await configManager.Set(EConfigOptions.ApiToken, request.ApiToken);

        if (!string.IsNullOrEmpty(request.IsUserHighlighted.ToString()))
            await configManager.Set(EConfigOptions.UserHighlight, request.IsUserHighlighted.ToString());
        
        
        return new ChangeSettingsFunctionResponse()
        {
            Success = true
        };
    }
}