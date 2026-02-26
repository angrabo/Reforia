using System.ComponentModel.DataAnnotations;

namespace Reforia.Core.Common.Config;

public class ConfigItem
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}