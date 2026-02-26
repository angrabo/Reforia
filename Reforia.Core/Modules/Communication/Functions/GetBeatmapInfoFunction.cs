using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Common.Dto;
using Reforia.Core.Modules.Communication.Contracts;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Exceptions;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;

namespace Reforia.Core.Modules.Communication.Functions;

public class GetBeatmapInfoFunction : WebFunction<GetBeatmapInfoFunctionBody, GetBeatmapInfoFunctionResponse>
{
    protected override async Task<GetBeatmapInfoFunctionResponse> Handle(GetBeatmapInfoFunctionBody request, IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(config));

        var httpClient = provider.GetRequiredService<HttpClient>();

        var url = $"https://osu.ppy.sh/api/get_beatmaps?b={request.BeatmapId}&k={await config.Get(EConfigOptions.ApiToken)}";

        var response = await httpClient.GetAsync(url);

        var responseBody = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
            throw new CommunicationException(EErrorCode.CannotGetApiData, $"Failed to get beatmap info. Status code: [{response.StatusCode}] {responseBody}");
        
        var beatmapsResponse = JsonSerializer.Deserialize<List<BeatmapDto>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var beatmap = beatmapsResponse?.FirstOrDefault();
        
        if (beatmap == null)
            throw new CommunicationException(EErrorCode.CannotGetApiData, "Beatmap not found in API response.");

        return new GetBeatmapInfoFunctionResponse(beatmap);
    }
}