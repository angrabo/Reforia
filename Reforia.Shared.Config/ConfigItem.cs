using Reforia.Infrastructure.LiteDb;
using Reforia.Infrastructure.LiteDb.Interfaces;

namespace Reforia.Shared.Config;

public class ConfigItem : IEntity
{
    public Guid Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}