using System;
using System.IO;

namespace GroceryStoreAPI.Configuration
{
    public static class DatabaseConfiguration
    {
        public static string GetDatabasePath()
        {
            string currDir = Directory.GetCurrentDirectory();
            return Path.Combine(currDir, "database.json");
        }
    }
}
