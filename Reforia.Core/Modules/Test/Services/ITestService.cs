namespace Reforia.Core.Modules.Test.Services;

public interface ITestService
{
    Task<List<string>> GetAll();
}