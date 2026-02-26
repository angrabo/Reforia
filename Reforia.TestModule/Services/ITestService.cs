namespace TestModule.Services;

public interface ITestService
{
    Task<List<string>> GetAll();
}