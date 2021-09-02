using LinqToDB.Mapping;

namespace Api.Models {
  [Table(Name = "users")]
  public class User {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; init; }
    [Column(Name = "username"), NotNull]
    public string Username { get; init; }
    [Column(Name = "password"), NotNull]
    public string Password { get; init; }
    [Column(Name = "tag_id")]
    public int TagId { get; set; }
  }
}