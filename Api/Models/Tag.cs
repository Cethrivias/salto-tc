using LinqToDB.Mapping;

namespace Api.Models
{
  [Table(Name = "tags")]
  public class Tag
  {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; init; }
    [Column(Name = "name"), NotNull]
    public string Name { get; init; }
  }
}