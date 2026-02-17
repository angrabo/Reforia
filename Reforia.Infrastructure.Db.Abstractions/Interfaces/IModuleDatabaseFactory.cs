using LiteDB;

namespace Reforia.Infrastructure.LiteDb.Interfaces;

public interface IModuleDatabaseFactory
{
    ILiteDatabase GetDatabase(string moduleName);
}