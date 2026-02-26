using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Functions.Response;
using Reforia.Rpc.Core;
using Reforia.Shared.Config;

namespace Reforia.IrcModule.Functions;

public class GetIrcCredentialsFunction : WebFunction<GetIrcCredentialsFunctionResponse>
{
    protected async override Task<GetIrcCredentialsFunctionResponse> Handle(IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(IConfigService));

        var username = config.Get("IrcUsername");
        var password = config.Get("IrcPassword");

        if (username is null || password is null)
            throw new Exception("Credentials are missing");

        return new GetIrcCredentialsFunctionResponse()
        {
            Nick = username,
            Password = password
        };
    }
}