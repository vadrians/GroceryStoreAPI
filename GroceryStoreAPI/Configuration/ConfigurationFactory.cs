using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace GroceryStoreAPI.Configuration
{
    public class ConfigurationFactory
    {
        public static IConfigurationRoot GetConfigurationRoot()
        {
            return GetConfigurationBuilder()
                .Build();
        }

        public static IConfigurationBuilder GetConfigurationBuilder()
        {
            var aspnetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string currDir = Directory.GetCurrentDirectory();
            return new ConfigurationBuilder()
                .SetBasePath(currDir)
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{aspnetCoreEnvironment}.json", true);
        }
    }
}
