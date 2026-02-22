using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Contracts;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Exceptions;
using Reforia.Core.Modules.Communication.Functions.Response;

namespace Reforia.Core.Modules.Communication.Functions;

public class GetIrcCredentialsFunction : WebFunction<GetIrcCredentialsFunctionResponse>
{
    protected override async Task<GetIrcCredentialsFunctionResponse> Handle(IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(IConfigService));

        var username = await config.Get(EConfigOptions.IrcUsername);
        var password = await config.Get(EConfigOptions.IrcPassword);

        if (username is null || password is null)
            throw new CommunicationException(EErrorCode.CannotGetIrcCredentials, "Credentials are missing");

        return new GetIrcCredentialsFunctionResponse()
        {
            Nick = username,
            Password = password
        };
    }
}