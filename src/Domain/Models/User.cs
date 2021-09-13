using LinqToDB.Mapping;

namespace Domain.Models {
  [Table(Name = "users")]
  public class User {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; set; }
    [Column(Name = "username"), NotNull]
    public string Username { get; set; }
    [Column(Name = "password"), NotNull]
    public string Password { get; set; }
    [Column(Name = "tag_id")]
    public int? TagId { get; set; }
  }
}