using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;

namespace Reforia.Core.Modules.Communication.Functions;

public class AddIrcCredentialsFunction : WebFunction<AddIrcCredentialsFunctionBody, AddIrcCredentialsFunctionResponse>
{
    protected override async Task<AddIrcCredentialsFunctionResponse> Handle(AddIrcCredentialsFunctionBody body, IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(config));
        
        await config.Set("IrcUsername", body.Nick);
        await config.Set("IrcPassword", body.Password);

        return new AddIrcCredentialsFunctionResponse()
        {
            Success = true
        };
    }
}