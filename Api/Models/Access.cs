using LinqToDB.Mapping;

namespace Api.Models {
  [Table(Name = "access")]
  public class Access {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; init; }
    [Column(Name = "tag_id"), NotNull]
    public int TagId { get; init; }
    [Column(Name = "lock_id"), NotNull]
    public int LockId { get; init; }
  }
}