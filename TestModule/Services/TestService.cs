namespace ReforiaBackend.Services;

public class TestService : ITestService
{
    
    
    public Task<List<string>> GetAll()
    {
        return Task.FromResult(new List<string>
        {
            "Player1",
            "Player2"
        });
    }
}