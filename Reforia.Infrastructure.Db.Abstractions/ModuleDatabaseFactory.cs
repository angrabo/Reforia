using System.Collections.Concurrent;
using LiteDB;
using Reforia.Infrastructure.LiteDb.Interfaces;

namespace Reforia.Infrastructure.LiteDb;

public class ModuleDatabaseFactory : IModuleDatabaseFactory
{
    private readonly ConcurrentDictionary<string, ILiteDatabase> _databases = new();
    private const    string                                      DbFolder   = "Data";

    public ILiteDatabase GetDatabase(string moduleName)
    {
        return _databases.GetOrAdd(moduleName, name =>
        {
            var folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbFolder);
            var filePath = Path.Combine(folderPath, $"{name}.db");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return new LiteDatabase(filePath);
        });
    }
}