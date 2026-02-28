using Reforia.Core.Common;

namespace ReforiaBackend.Dto;

public interface IApiResponse
{
    int StatusCode { get; set; }
    EErrorCode? ErrorCode { get; set; }
    object? Data { get; set; }
}

public class ApiResponse<T> : IApiResponse
{
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public EErrorCode? ErrorCode { get; set; }

    object? IApiResponse.Data 
    { 
        get => Data; 
        set => Data = (T?)value; 
    }
}