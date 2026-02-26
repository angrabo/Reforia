using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Contracts;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;

namespace Reforia.Core.Modules.Communication.Functions;

public class SetIrcCredentialsFunction : WebFunction<SetIrcCredentialsFunctionBody, SetIrcCredentialsFunctionResponse>
{
    protected override async Task<SetIrcCredentialsFunctionResponse> Handle(SetIrcCredentialsFunctionBody body, IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(config));
        
        await config.Set(EConfigOptions.IrcUsername, body.Nick);
        await config.Set(EConfigOptions.IrcPassword, body.Password);

        return new SetIrcCredentialsFunctionResponse()
        {
            Success = true
        };
    }
}