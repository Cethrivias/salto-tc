using LinqToDB.Mapping;

namespace Api.Models
{
  [Table(Name = "tags")]
  public class Tag
  {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; set; }
    [Column(Name = "name"), NotNull]
    public string Name { get; set; }
  }
}