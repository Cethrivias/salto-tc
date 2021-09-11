using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace IntegTests.Tools {
  public static class Config {
    private static readonly object _lock = new();
    private static IConfiguration config;

    public static IConfiguration GetInstance() {
      lock (_lock) {
        if (config is null) {
          config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();
        }
      }

      return config;
    }
  }
}