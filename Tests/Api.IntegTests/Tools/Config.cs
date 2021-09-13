using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Api.IntegTests.Tools {
  public static class Config {
    private static readonly object Lock = new();
    private static IConfiguration config;

    public static IConfiguration GetInstance() {
      lock (Lock) {
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