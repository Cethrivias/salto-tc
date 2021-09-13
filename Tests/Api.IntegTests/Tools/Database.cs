using Infrastructure.Libs.Database;
using LinqToDB.Configuration;
using Microsoft.Extensions.Configuration;

namespace Api.IntegTests.Tools {
  public static class Database {
    private static readonly object Lock = new();
    private static LinqToDbConnectionOptions options;

    public static MainDataConnection GetConnection() {
      lock (Lock) {
        if (options is null) {
          var connectionString = Config.GetInstance().GetConnectionString("Main");
          options = new LinqToDbConnectionOptionsBuilder().UseMySqlConnector(connectionString).Build();
        }
      }

      return new MainDataConnection(options);
    }
  }
}