using LinqToDB.Mapping;

namespace Api.Models {
  [Table(Name = "locks")]
  public class Lock {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; init; }
    [Column(Name = "name"), NotNull]
    public string Name { get; init; }
  }
}