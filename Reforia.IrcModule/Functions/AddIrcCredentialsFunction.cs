using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Functions.Body;
using Reforia.IrcModule.Functions.Response;
using Reforia.Rpc.Core;
using Reforia.Shared.Config;

namespace Reforia.IrcModule.Functions;

public class AddIrcCredentialsFunction : WebFunction<AddIrcCredentialsFunctionBody, AddIrcCredentialsFunctionResponse>
{
    protected async override Task<AddIrcCredentialsFunctionResponse> Handle(AddIrcCredentialsFunctionBody body, IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(config));
        
        config.Set("IrcUsername", body.Nick);
        config.Set("IrcPassword", body.Password);

        return new AddIrcCredentialsFunctionResponse()
        {
            Success = true
        };
    }
}