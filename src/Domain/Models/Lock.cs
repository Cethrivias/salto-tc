using LinqToDB.Mapping;

namespace Domain.Models {
  [Table(Name = "locks")]
  public class Lock {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; set; }
    [Column(Name = "name"), NotNull]
    public string Name { get; set; }
  }
}