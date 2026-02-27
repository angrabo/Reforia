namespace ReforiaBackend.Dto;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}