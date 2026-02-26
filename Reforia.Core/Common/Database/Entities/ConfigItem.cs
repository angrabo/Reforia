using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reforia.Core.Common.Database.Entities;


[Table("Config")]
public class ConfigItem
{
    [Key]
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}