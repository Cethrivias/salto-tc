using System;
using LinqToDB.Mapping;

namespace Domain.Models {
  [Table(Name = "users_access_logs")]
  public class UserAccessLog {
    [PrimaryKey, Identity, Column(Name = "id")]
    public int Id { get; set; }
    [Column(Name = "user_id")]
    public int UserId { get; set; }
    [Column(Name = "lock_id")]
    public int LockId { get; set; }
    [Column(Name = "created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
  }
}