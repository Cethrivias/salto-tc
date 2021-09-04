using Api.Models;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Api.Libs.Database {
  public class MainDataConnection : DataConnection {
    public MainDataConnection(LinqToDbConnectionOptions<MainDataConnection> options) : base(options) { }

    public ITable<User> Users => GetTable<User>();
    public ITable<Tag> Tags => GetTable<Tag>();
    public ITable<Lock> Locks => GetTable<Lock>();
    public ITable<Access> Access => GetTable<Access>();
    public ITable<UserAccessLog> UsersAccessLogs => GetTable<UserAccessLog>();
  }
}