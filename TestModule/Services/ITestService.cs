namespace ReforiaBackend.Services;

public interface ITestService
{
    Task<List<string>> GetAll();
}