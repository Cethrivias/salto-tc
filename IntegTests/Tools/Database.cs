using Api.Libs.Database;
using LinqToDB.Configuration;
using Microsoft.Extensions.Configuration;

namespace IntegTests.Tools {
  public static class Database {
    private static readonly object _lock = new();
    private static LinqToDbConnectionOptions options;

    public static MainDataConnection GetConnection() {
      lock (_lock) {
        if (options is null) {
          var connectionString = Config.GetInstance().GetConnectionString("Main");
          options = new LinqToDbConnectionOptionsBuilder().UseMySqlConnector(connectionString).Build();
        }
      }

      return new MainDataConnection(options);
    }
  }
}