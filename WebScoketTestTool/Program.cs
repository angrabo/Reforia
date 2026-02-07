using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    // Prostą klasą reprezentujemy request i response w JSON
    public class WebRequest
    {
        public string FunctionName { get; set; } = "";
        public string RequestId { get; set; } = "";
        public string Body { get; set; } = "";
    }

    public class WebResponse
    {
        public string RequestId { get; set; } = "";
        public int StatusCode { get; set; }
        public object? Body { get; set; }
        public List<string>? Errors { get; set; }
    }

    static async Task Main(string[] args)
    {
        // Podmień URL na swój hub
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5081/hub")
            .Build();

        await connection.StartAsync();
        Console.WriteLine("Połączono z hubem SignalR!");

        // Tworzymy request
        var request = new WebRequest
        {
            FunctionName = "GetTestFunction",
            RequestId = Guid.NewGuid().ToString(),
            Body = JsonSerializer.Serialize(new { dummy = 0 }) // Body jako JSON string
        };

        // Wywołanie metody Call w hubie
        var response = await connection.InvokeAsync<WebResponse>("Call", request);

        Console.WriteLine("Response:");
        Console.WriteLine(JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }));

        await connection.StopAsync();
    }
}